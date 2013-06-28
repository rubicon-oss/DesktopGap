<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Security.aspx.cs" Inherits="WebHostWebApplication.Security" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
  <a href="Compatibility.aspx">Reload</a><br/>
  <iframe src="XSS.aspx" width="800" height="600">
 
  </iframe>
</asp:Content>
