using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using System.Xml;
using System.IO;

namespace Test1
{
    public partial class _Default : System.Web.UI.Page
    {
        public static class globals
        {
            public static int index = 0;
            public static XmlNodeList files;
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private bool testDownload(string url)
        {
            //WebClient wc = new WebClient();
            //try
            {
                
                //wc.CancelAsync();
            }
            //catch
            {
                //wc.Dispose();
                //return false;
            }
            //wc.Dispose();
            return true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //mainDiv.InnerHtml += "<h2>test!</h2>";
            mainDiv.InnerHtml = "";
            globals.index = 0;

            string url = HttpUtility.UrlPathEncode("http://www.seeqpod.com/api/music/anonSearchPaginationFlash?s=0&n=50&q=" + TextBox1.Text);
            HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;

            HttpWebResponse resp = null;
            try
            {
                resp = req.GetResponse() as HttpWebResponse;
                // Get the status code.
                // if 200  = "OK"
                //    400  = "Bad Request"
                //    401  = "Unauthorized"
                //    404  = "Not Found"
                //    500  = "Server Error"
                int iStatCode = (int)resp.StatusCode;
                string sStatus = iStatCode.ToString();
                mainDiv.InnerHtml += "Status Code: " + sStatus + "<br>"; //Debuging connectivity
            }
            catch (WebException err)
            {
                mainDiv.InnerHtml += "WebException raised!<br>";
                mainDiv.InnerHtml += err.Message + "<br>";
                mainDiv.InnerHtml += err.Status + "<br>";
                return;
            }

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(resp.GetResponseStream());

            globals.files = xDoc.GetElementsByTagName("location");
            int i = 0;
            string prec = "";
            foreach (XmlNode node in globals.files)
            {
                if (i == globals.index)
                    prec = "Playing >>    ";
                else
                    prec = i.ToString() + ":    ";
                if(testDownload(node.InnerText))
                mainDiv.InnerHtml += prec + "<a href=" + node.InnerText + ">" + HttpUtility.UrlDecode(node.InnerText) + "</a><br><br>";
                i++;
            }

            if (globals.files.Count == 0)
            {
                mainDiv.InnerHtml += "NO FILES FOUND... sorry";
            }
            else
            {
                MediaPlayer1.MediaSource = globals.files[0].InnerText;
            }
            
        }

        protected void buttonNext_Click(object sender, EventArgs e)
        {
            globals.index++;
            MediaPlayer1.MediaSource = globals.files[globals.index].InnerText;
            MediaPlayer1.AutoLoad = true;
            MediaPlayer1.AutoPlay = true;

            mainDiv.InnerHtml = "";
            int i = 0;
            string prec = "";
            foreach (XmlNode node in globals.files)
            {
                if (i == globals.index)
                    prec = "Playing >>    ";
                else
                    prec = i.ToString() + ":    ";

                mainDiv.InnerHtml += prec + "<a href=" + node.InnerText + ">" + HttpUtility.UrlDecode(node.InnerText) + "</a><br><br>";
                i++;
            }

            if (globals.files.Count == 0)
            {
                mainDiv.InnerHtml += "NO FILES FOUND... sorry";
            }
        }
    }
}
