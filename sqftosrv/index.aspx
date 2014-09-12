<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="sqftosrv.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:FileUpload ID="fuSQF" runat="server" />
        <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click" />
        <br /><br />
        <asp:Panel ID="pnlLoading" Visible="false" runat="server">
            <asp:Image ID="imgLoading" ImageUrl="~/images/loading-anim.gif" AlternateText="loading..." runat="server"/>
        </asp:Panel>
        <asp:Panel ID="pnlDownload" Visible="false" runat="server">
            <asp:HyperLink ID="btnDownload" runat="server" Text="Download" NavigateUrl="#"></asp:HyperLink>
        </asp:Panel>
        <br />
        <asp:Label runat="server" ID="lblStatus" Text="waiting for file"></asp:Label>
    </div>        
    </form>
</body>
</html>
