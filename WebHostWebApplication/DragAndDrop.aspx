<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DragAndDrop.aspx.cs"
  Inherits="WebHostWebApplication.DragAndDrop" MasterPageFile="Main.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="head">
  <script>
    function DesktopGap_DocumentRegistered() {
      InitializeDesktopGap();

      var dropZone = document.getElementById("realDropTarget");

      OnDragOver(dropZone,
          function (args) {
            if (args.Names.length > 2) {

              this.setAttribute("dg_dropcondition", "1");
              this.style.background = "lightgreen";
            }
            else {
              this.removeAttribute("dg_dropcondition");
            }

          });

      OnDrop(dropZone,
          function (args) {
            this.style.background = "lightblue";
          });

      OnDragLeave(dropZone,
       function (args) {
         this.style.background = "orange";
       });

      var dropZone2 = document.getElementById("fakeDropTarget");

      OnDragOver(dropZone2,
          function (args) {
            if (args.Names.length == 1) {
              this.setAttribute("dg_dropcondition", "1");
              this.style.background = "lightgreen";
            }
            else {
              this.removeAttribute("dg_dropcondition");
            }
          });

      OnDrop(dropZone2,
         function (args) {
           this.style.background = "lightblue";
         });

      OnDragLeave(dropZone2,
         function (args) {
           this.style.background = "yellow";
         });

      document.getElementById("status").style.background = "green";
    }
    /*var dt    = e.dataTransfer;
      var files = dt.files;
      */
  </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
  <span id="status" style="background-color: red; width: 10px; height: 10px; border-radius: 5px; float: right;"></span>
  <h2>Drag & Drop</h2>

  <div dg_droptarget="true" id="fakeDropTarget" style="padding-left: 3px; background-color: yellow; width: 400px; height: 100px; border-radius: 5px">
    <h3>1 ITEM MAX</h3>
  </div>

  <div dg_droptarget="true" id="realDropTarget" style="padding-left: 3px; background-color: orange; width: 400px; height: 100px; border-radius: 5px">
    <h3>3 ITEMS MIN</h3>
  </div>

  <div id="noDropTarget" style="padding-left: 3px; background-color: blue; width: 400px; height: 100px; border-radius: 5px">
    <h3>NO ITEMS</h3>
  </div>
</asp:Content>
