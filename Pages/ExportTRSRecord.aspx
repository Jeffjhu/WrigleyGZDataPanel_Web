<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportTRSRecord.aspx.cs" Inherits="WrigleyDataPanel.Pages.ExportTRSRecord" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<title>Export TRS Record</title>
<link href="/Common/css/layout.datein.css" rel="stylesheet" />
</head>
<body>
    <form id="frmExport" runat="server">
    <div style="width:640px; height:50px;margin-top:10px;margin-left:4px;">
        <span class="commonText">Line:</span>
        <asp:DropDownList ID="selLine" runat="server" CssClass='rosDrop LineSelect'>
            <asp:ListItem Selected="True">SHT1</asp:ListItem>
            <asp:ListItem>SHT2</asp:ListItem>
            <asp:ListItem>SHT3</asp:ListItem>
            <asp:ListItem>WA1</asp:ListItem>
            <asp:ListItem>WA2</asp:ListItem>
            <asp:ListItem>WA3</asp:ListItem>
            <asp:ListItem>WA4</asp:ListItem>
            <asp:ListItem>WA5</asp:ListItem>
            <asp:ListItem>WA12</asp:ListItem>
            <asp:ListItem>WA13</asp:ListItem>
            <asp:ListItem>WA14</asp:ListItem>
            <asp:ListItem>WA15</asp:ListItem>
            <asp:ListItem>WA16</asp:ListItem>
            <asp:ListItem>WA17</asp:ListItem>
            <asp:ListItem>WA18</asp:ListItem>
            <asp:ListItem>WA19</asp:ListItem>
        </asp:DropDownList>

        <span class="commonText">Day From:</span>
        <asp:TextBox ID="dayFrom" runat="server" class="txtDate"></asp:TextBox>
        <span class="commonText">Day To:</span>
        <asp:TextBox ID="dayTo" runat="server" CssClass="txtDate"></asp:TextBox>       
        <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="btnExport" OnClick="btnExport_Click"/>
    </div>
    <div style="margin-left:4px;">
     <span class="commonText">请选择产线、开始日期和结束日期。</span>
    </div>
    </form>
</body>
</html>
