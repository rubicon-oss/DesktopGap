<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DragAndDrop.aspx.cs" Inherits="WebHostWebApplication.DragAndDrop" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>DesktopGap - Drag &amp; Drop</title>
  <script>

    function DesktopGap() {
      this._registered_events = {};
    }

    DesktopGap.prototype.register = function (guid, e, f) {
      this._registered_events[guid] = { "func": f, "element": e };

    };

    DesktopGap.prototype.registerForDragAndDrop = function (element, dragover, dragdrop, dragenter, dragleave) {

      var guidservice = window.external.GetService(window.documentHandle, "GuidService");

      var guid = guidservice.CreateGuid();
      this.register(guid, element, dragover);
      window.external.addEventListener(window.documentHandle, "DragOver", "DesktopGap_EventDispatch", "DragAndDrop",
          { DocumentHandle: window.documentHandle, EventID: guid, Criteria: { elementID: element.id } });

      guid = guidservice.CreateGuid();
      this.register(guid, element, dragdrop);
      window.external.addEventListener(window.documentHandle, "DragDrop", "DesktopGap_EventDispatch", "DragAndDrop",
          { DocumentHandle: window.documentHandle, EventID: guid, Criteria: { elementID: element.id } });

      guid = guidservice.CreateGuid();
      this.register(guid, element, dragleave);
      window.external.addEventListener(window.documentHandle, "DragLeave", "DesktopGap_EventDispatch", "DragAndDrop",
          { DocumentHandle: window.documentHandle, EventID: guid, Criteria: { elementID: element.id } });
    };


    function DesktopGap_AssignID(guid) {
      window.documentHandle = guid;
    }


    function DesktopGap_RetrieveID() {
      return window.documentHandle;
    }

    var desktopgap = new DesktopGap();

    function DesktopGap_EventDispatch(args) {
      var a = JSON.parse(args);

      var evt = desktopgap._registered_events[a.EventID];
      var func = evt.func;
      var element = evt.element;
      func.call(element, a);
    }


    function DesktopGap_DocumentRegistered() {
      var dropZone = document.getElementById("realDropTarget");
      desktopgap.registerForDragAndDrop(dropZone,
          function (args) {
            if (args.Names.length > 2) {
              this.setAttribute("dg_dropcondition", "1");
              this.style.background = "lightgreen";
            } else this.setAttribute("dg_dropcondition", "0");
          },
          function (args) {
            this.style.background = "lightblue";
          },
          function (args) { },
          function (args) {
            this.style.background = "orange";
          });

      var dropZone2 = document.getElementById("fakeDropTarget");
      desktopgap.registerForDragAndDrop(dropZone2,
          function (args) {

            if (args.Names.length == 1) {
              this.setAttribute("dg_dropcondition", "1");
              this.style.background = "lightgreen";
            } else this.setAttribute("dg_dropcondition", "0");
          },
          function (args) {
            this.style.background = "lightblue";
          },
          function (args) { },
          function (args) {
            this.style.background = "yellow";
          });
      document.getElementById("status").style.background = "green";
    }
  </script>


</head>
<body>
  <span id="status" style="background-color: red; width: 10px; height: 10px; border-radius: 5px; float: right;"></span>
  <h2>Drag & Drop</h2>

  <div dg_droptarget="true" id="fakeDropTarget" style="padding-left:3px; background-color: yellow; width: 400px; height: 100px; border-radius: 5px">
    <h3>1 ITEM MAX</h3>
  </div>

  <div dg_droptarget="true" id="realDropTarget" style="padding-left:3px; background-color: orange; width: 400px; height: 100px; border-radius: 5px">
    <h3>3 ITEMS MIN</h3>
  </div>

  <div id="noDropTarget" style="padding-left:3px; background-color: blue; width: 400px; height: 100px; border-radius: 5px">
    <h3>NO ITEMS</h3>
  </div>
</body>
</html>
