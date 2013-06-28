<%@ Page Language="C#" MasterPageFile="Main.Master"%>

<asp:Content runat="server" ContentPlaceHolderID="head">
  <script type="text/javascript">
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
      var id = desktopgap.preparePopUp(url);
      windowHandle = window.open(url, id, "");
    }


    function openModalWindow(url) {
      var id = desktopgap.prepareModalPopUp(url);
      modalHandle = window.open(url, id, "height=500,width=230,fullscreen=no,resizable=no,left=100");
    }

    function openTab(url) {
      var id = desktopgap.prepareTab(url);
      tabHandle = window.open(url, id, "");
    }

    function openBackgroundTab(url) {
      var id = desktopgap.prepareBackgroundTab(url);
      backgroundHandle = window.open(url, id, "");
    }

    function DesktopGap_DocumentRegistered() {
      InitializeDesktopGap();
      
      var dropZone = document.getElementById("dropTarget");
      
      OnDragOver(dropZone,
          function (args) {
            if (args.preventDefault) args.preventDefault();
            if(args.dataTransfer && args.dataTransfer.dropEffect) args.dataTransfer.dropEffect = 'copy';
            this.setAttribute("dg_dropcondition", "1");
            this.style.background = "lightgreen";
            return false;
          });
          
      OnDrop(dropZone,
          function (args) {
             if (args.preventDefault) args.preventDefault();
            this.style.background = "lightblue";
          });      
    }

  </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="body">
  <div dg_droptarget="true" 
       id="dropTarget" 
       style="padding-left:3px; background-color: yellow; width: 100px; height: 100px; border-radius: 5px" >
  </div>
</asp:Content>