<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="XSS.aspx.cs" Inherits="WebHostWebApplication.XSS" MasterPageFile="Main.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script>
    function load(url) {
      window.location = url;
    }

    function loadNav(url) {
      window.navigate(url);
    }

  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" runat="server">
  <div style="background-color: cadetblue; border-radius: 5px; padding: 5px;">
    <button onclick="load('http://www.microsoft.com')">Load www.microsoft.com</button>
    <button onclick="loadNav('http://www.microsoft.com')">Navigate www.microsoft.com</button>
    <button onclick="load(String.fromCharCode(104, 116, 116, 112, 58, 47, 47, 119, 119, 119, 46, 109, 105, 99, 114, 111, 115, 111, 102, 116, 46, 99, 111, 109))">Load obfuscated www.microsoft.com</button>
    <button onclick="loadNav(String.fromCharCode(104, 116, 116, 112, 58, 47, 47, 119, 119, 119, 46, 109, 105, 99, 114, 111, 115, 111, 102, 116, 46, 99, 111, 109))">Navigate obfuscated www.microsoft.com</button>
  </div>
</asp:Content>
