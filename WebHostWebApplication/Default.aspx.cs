using System;
using System.Globalization;
using System.Web;
using System.Web.UI;

namespace WebHostWebApplication
{
  public partial class _Default : Page
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      // Create a new HttpCookie.
      var myHttpCookie = new HttpCookie ("LastVisit", DateTime.Now.ToString (CultureInfo.InvariantCulture));
      var myHttpOnlyCookie = new HttpCookie ("secret-sauce", "ketchup") { HttpOnly = true };

      Response.AppendCookie (myHttpCookie);
      Response.AppendCookie (myHttpOnlyCookie);

      Response.Write (Request.UserAgent ?? "user agent null");
      Response.Write ("<br><br>");
    }
  }
}