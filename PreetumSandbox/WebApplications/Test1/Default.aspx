<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Test1._Default" %>

<%@ Register assembly="System.Web.Silverlight" namespace="System.Web.UI.SilverlightControls" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        #form1
        {
            height: 200px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server" Text="Enter artist, song, or both:"></asp:Label>
        &nbsp;
    
        <asp:TextBox ID="TextBox1" runat="server" Width="129px"></asp:TextBox>
        &nbsp;
        <asp:Button ID="Button1" runat="server" Text="Button" onclick="Button1_Click" />
    
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
    
    </div>
    <br />
    <asp:MediaPlayer ID="MediaPlayer1" runat="server" Height="62px" 
        MediaSkinSource="~/AudioGray.xaml" Width="468px" AutoPlay="True" 
        ScaleMode="None" Volume="1"></asp:MediaPlayer>
        <br /><br />
    <asp:Button ID="buttonNext" runat="server" Text=">>" 
        onclick="buttonNext_Click" />
    </form>
    
    <div id="mainDiv" runat="server">
    </div>
    
</body>
</html>
