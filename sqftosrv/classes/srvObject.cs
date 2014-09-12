using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sqftosrv.classes
{
    public class srvObject
    {
        public int id { get; set; }
        public string a2Classname { get; set; }
        public string pos { get; set; }
        public string dir = "0";

        public List<string> WriteObject()
        {
            List<string> returnValue = new List<string>();
            
            // write createV command;

            // write setDir command;

            // write setVector command;

            // write allowDamage command;


            return returnValue;
        }

    }
}