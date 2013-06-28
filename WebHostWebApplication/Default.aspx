<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="WebHostWebApplication._Default" MasterPageFile="Main.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
  <script type="text/javascript">
    mistypedVariable = 17;
    function onLoad() {
      //  alert(parseInt(window.document.documentMode));

    }

    function imageLoaded() {
      document.getElementById("status").style.background = "green";
    }

    window.onbeforeunload = function () {
      return "o rly?";
    };
  </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
  <span id="status" style="background-color: red; width: 10px; height: 10px; border-radius: 5px; float: right;"></span>

  <div style="width: 100%; padding: 10px">
    <img src="http://i.s-microsoft.com/global/ImageStore/PublishingImages/logos/hp/logo-lg-1x.png" alt=":( Could not load Image" onload="imageLoaded()" />

  </div>

  <span style="float: left">

    <h2>Drag & Drop</h2>
    <a href="DragAndDrop.aspx">Click here.</a>

    <h2>Window Management</h2>
    <a href="WindowManagement.aspx">Click here.</a>

    <h2>Frames</h2>
    <a href="Frames.aspx">Click Here.</a>

    <h2>iFrames</h2>
    <a href="iFrames.aspx">Click Here.</a>

    <h2>Sleep</h2>
    <a href="Sleep.aspx">Click Here.</a>

    <h2>Tick</h2>
    <a href="Tick.aspx">Click Here.</a>

  </span>
  <span style="float: right; padding-right: 30px;">
    <asp:Image runat="server" ImageUrl="rainbow-dash.png" Width="400" />
  </span>

<%--  <div>
    <video width="320" height="240" autoplay="true">
      <source src="/video.mp4" type="video/mp4" >
    </video>
  </div>--%>
</asp:Content>
