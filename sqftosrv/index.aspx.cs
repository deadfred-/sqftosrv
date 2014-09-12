using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text.RegularExpressions;
using sqftosrv.classes;


namespace sqftosrv
{
    public partial class index : System.Web.UI.Page
    {
        public Boolean goodFile = false;
        public srvObject currObject = new srvObject();
        public Guid fileGuid;


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            pnlDownload.Visible = false;
            pnlLoading.Visible = false;

            if (fuSQF.HasFile)
            {
                try
                {
                    string fileName = Path.GetFileName(fuSQF.FileName);

                    if (fileName.Contains(".sqf"))
                    {
                        fileGuid = Guid.NewGuid();

                        fuSQF.SaveAs(Server.MapPath("~/") + fileGuid.ToString() + fileName);
                        lblStatus.Text = "Upload Status: Processing File";
                        goodFile = true;

                        // process file
                        pnlLoading.Visible = true;
                        
                        List<srvObject> serverObjectsParsed = ConvertSQFToSrvFormat(fileName);

                        // Create File and link to that.
                        SaveFileForDownload(serverObjectsParsed);

                        pnlDownload.Visible = true;
                        btnDownload.NavigateUrl = Server.MapPath("~/") + fileGuid.ToString() + ".txt";


                        pnlLoading.Visible = false;
                        // delte temp file
                        File.Delete(Server.MapPath("~/") + fileGuid.ToString() + fileName);

                        lblStatus.Text = "We're Done";
                    }
                    else
                    {
                        lblStatus.Text = "Upload Status: Failed!  Make sure this file is an SQF!";
                        goodFile = false;
                    }

                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Upload Status: File could not be uploaded.  Make sure it is an SQF";
                }
            }
        }

        private void SaveFileForDownload(List<srvObject> inObjects)
        {
            StreamWriter sw = new StreamWriter(Server.MapPath("~/") + fileGuid.ToString() + ".txt");

            foreach (srvObject record in inObjects)
            {
                sw.WriteLine("//Object " + record.id);
                sw.WriteLine("if (true) then {");
                String createVehLine = string.Format("\t_this = createVehicle [\"{0}\", {1}, [], 0, \"CAN_COLLIDE\"];",record.a2Classname,record.pos);                
                sw.WriteLine(createVehLine);
                sw.WriteLine("\t_this setDir\t" + record.dir + ";");
                sw.WriteLine("\t_this setPos\t" + record.pos + ";");
                sw.WriteLine("\t_this setVariable [\"ObjectUID\", \"GeneratedByDotNetScript\", true];");
                sw.WriteLine("\t_this addEventHandler [\"HandleDamage\", {false}];");
                sw.WriteLine("\t_this allowDamage false;");
                sw.WriteLine("\t_this setvectorup [0,0,1];");
                sw.WriteLine("};");
                sw.WriteLine("");
            }
            sw.Close();

        }


        private List<srvObject> ConvertSQFToSrvFormat(string fileName)
        {
            List<srvObject> returnValue = new List<srvObject>(); 
            StreamReader sr = new StreamReader(Server.MapPath("~/") + fileGuid.ToString() + fileName);
            string line;

            Boolean parsingObject = false;
            List<srvObject> srvObjects = new List<srvObject>();
            
            int id = 0;
            

            while ((line = sr.ReadLine()) != null)
            {
                try
                {
                    // convert to windows line endings
                    line = line.Replace("\r\n", "\n");

                    // match each line and create a server object based upon it

                    // find a new line
                    Match NewObj = Regex.Match(line, @"(?<newObj>objNull)");
                    if (NewObj.Success)
                    {
                        parsingObject = true;
                        currObject = new srvObject();
                    }
                    if (parsingObject)
                    {
                        //(?<ClassName>\"\w+\").*(?<Position>\[[0-9]+\.[0-9]+,\s*[0-9]+\.[0-9]+(,\s*.*[0-9])*])
                        Match ClassPos = Regex.Match(line, "(?<ClassName>\\\"\\w+\\\").*(?<Position>\\[[0-9]+\\.[0-9]+,\\s*[0-9]+\\.[0-9]+(,\\s*.*[0-9])*])");
                        // grab classes
                        if (ClassPos.Success)
                        {
                            currObject.a2Classname = ClassPos.Groups["ClassName"].Value.Trim('"');
                            currObject.pos = ClassPos.Groups["Position"].Value;
                        }


                        Match Dir = Regex.Match(line, @"setDir(?<Dir>.*);");
                        // grab dirs
                        if (Dir.Success)
                        {
                            currObject.dir = Dir.Groups["Dir"].Value.Trim();
                        }

                    }

                    Match endObj = Regex.Match(line, @"(?<endObj>^\};)", RegexOptions.Multiline);
                    if (endObj.Success)
                    {
                        parsingObject = false;
                        currObject.id = id;
                        returnValue.Add(currObject);
                        id++;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                

            }
            sr.Close();
            return returnValue;
        }
    }
}