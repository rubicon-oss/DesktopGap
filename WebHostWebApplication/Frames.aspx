<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>DesktopGap - Frames</title>
</head>
<frameset rows="*,30%">
  <frameset cols="30%,*">
    <frame src="Default.aspx">
    <frame src="DragAndDrop.aspx">
  </frameset>
  <frame src="DragAndDropIFrame.aspx"/>
</frameset>
</html>
