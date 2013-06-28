<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>DesktopGap - Tick</title>
  <script language="javascript">
    var ticker = null;
    var guidservice;
    var windowservice;
    var modalHandle;

    function DesktopGap_AssignID(guid) {
      window.documentHandle = guid;
    }

    function DesktopGap_RetrieveID() {
      return window.documentHandle;
    }

    function displayTime() {
      var d = new Date();
      var t = d.toLocaleTimeString();
      document.getElementById("target").innerHTML = t;
    }

    function startTicking() {
      if(ticker == null)
        ticker = setInterval(displayTime, 1000);
    }

    function stopTicking() {
      if (ticker != null)
        clearInterval(ticker);
      ticker = null;
    }

    function DesktopGap_DocumentRegistered() {
      guidservice = window.external.GetService(window.documentHandle, "GuidService");
      windowservice = window.external.GetService(window.documentHandle, "WindowManagerService");
    }


  </script>
</head>
<body>
  <div id="target" style="font-size: 5em; background-color: lightblue; padding: 10px; border-radius: 5px; font-family: monospace"></div>
  <div style="padding: 10px; float: right">
    <button onclick="stopTicking()">Stop tickin'!</button>
    <button onclick="startTicking()">Start tickin'!</button>
  </div>
</body>
</html>
