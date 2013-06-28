<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>DesktopGap - Sleep</title>
  <script language="javascript">

    function DesktopGap_AssignID(guid) {
      window.documentHandle = guid;
    }


    function DesktopGap_RetrieveID() {
      return window.documentHandle;
    }

    function lieDown() {
      var sleeperService = window.external.GetService(window.documentHandle, "SleepService");
      sleeperService.Sleep(10000);
    }

  </script>
</head>
<body>
  <div id="target" style="font-size: 5em; background-color: lightblue; padding: 10px; border-radius: 5px; font-family: monospace"></div>
  <div style="padding: 10px; float: right">
    <button onclick="lieDown()">Lie down for a bit</button>
  </div>
</body>
</html>
