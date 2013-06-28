<%@ Page Language="C#" MasterPageFile="Main.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
<script>
  var guidservice = null;
    var windowservice = null;

    var windowHandle = null;
    var modalHandle = null;
    var tabHandle = null;
    var backgroundHandle = null;

    function toggleBackground(handle) {
      if (handle != null && !handle.closed) {
        if (handle.document.body.style.backgroundColor != "black")
          handle.document.body.style.backgroundColor = "black";
        else {
          handle.document.body.style.backgroundColor = "steelblue";
        }

      }
    }

    function openWindow(url) {
      var id = windowservice.PrepareNewWindow(url, "popup", "active");
      windowHandle = window.open(url, id, "");
    }


    function openModalWindow(url) {
      var id = windowservice.PrepareNewWindow(url, "popup", "modal");
      modalHandle = window.open(url, id, "height=500,width=230,fullscreen=no,resizable=no,left=100");
        setTimeout(function() {
            windowservice.ShowModal(id);
        }, 0);
    }

    function openTab(url) {
      var id = windowservice.PrepareNewWindow(url, "tab", "active");
      tabHandle = window.open(url, id, "");
    }

    function openBackgroundTab(url) {
      var id = windowservice.PrepareNewWindow(url, "tab", "background");
      backgroundHandle = window.open(url, id, "");
    }

    function DesktopGap_DocumentRegistered() {
      guidservice = window.external.GetService(window.documentHandle, "GuidService");
      windowservice = window.external.GetService(window.documentHandle, "WindowManagerService");
    }

  </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
  <div style="background-color: lightskyblue; padding: 10px; border-radius: 5px;">
    <a href="#" onclick="openWindow('DragAndDrop.aspx')">Open a new Window</a> | <a href="#" onclick="toggleBackground(windowHandle)">Toggle Background</a><br />
    <a href="#" onclick="openModalWindow('DragAndDrop.aspx')">Open a modal Window</a>
    <br />
    <a href="#" onclick="openTab('DragAndDrop.aspx')">Open a new Tab</a> | <a href="#" onclick="toggleBackground(tabHandle)">Toggle Background</a><br />
    <a href="#" onclick="openBackgroundTab('DragAndDrop.aspx')">Open a new Tab in background</a> | <a href="#" onclick="toggleBackground(backgroundHandle)">Toggle Background</a><br />
  </div>
  <div style="background-color: tomato; padding: 10px; border-radius: 5px;">
    <a href="#" onclick="openWindow('Tick.aspx')">Open a new Window</a>
    <br />
    <a href="#" onclick="openModalWindow('Tick.aspx')">Open a modal Window</a>

  </div>
    <div style="background-color: burlywood; padding: 10px; border-radius: 5px;">
    <a href="#" onclick="openWindow('Tick.aspx')">Open a new Window</a>
    <br />
    <a href="#" onclick="openModalWindow('Tick.aspx')">Open a modal Window</a>

  </div>
</asp:Content>
