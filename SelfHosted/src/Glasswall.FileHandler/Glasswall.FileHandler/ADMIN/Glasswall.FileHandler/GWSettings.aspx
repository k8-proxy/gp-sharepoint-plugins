<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GWSettings.aspx.cs" Inherits="Glasswall.FileHandler.Layouts.Glasswall.FileHandler.GWSettings" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <style type="text/css">
        .btnRight {
           float:right;
        }
        .txtAPIinput{
            min-width:200px;
            width:100%;
            float:right;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="width: 80%;">
        <tr>
            <td>
                <asp:Label ID="lblAPIUrl" runat="server" Text="Label">Application REST API Url</asp:Label></td>
            <td>
                <asp:TextBox ID="txt_APIUrl" runat="server" CssClass="txtAPIinput"></asp:TextBox></td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblAPIKey" runat="server">Application Secret Key</asp:Label></td>
            <td>
                <asp:TextBox ID="txt_APIKey" runat="server" CssClass="txtAPIinput"></asp:TextBox></td>
        </tr>
        <tr>
            <td colspan="2"><asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btnRight" OnClick="btnSave_Click" ToolTip="Click here to save the settings" /></td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lbl_status" runat="server" Text="Label" Visible="false" ForeColor="#003399" Font-Italic="True"></asp:Label></td>
        </tr>
    </table>
    <br />    
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Glasswall- Central Administration
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Glasswall Filehandler Settings
</asp:Content>
