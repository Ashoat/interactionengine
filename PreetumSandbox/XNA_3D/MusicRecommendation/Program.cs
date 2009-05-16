using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Web;


namespace MusicRecommendation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Artist Name: ");
            string artist = Console.ReadLine();
            Console.WriteLine("");
            Console.WriteLine("Retrieving data: " + artist);

            //string url = "http://ws.audioscrobbler.com/2.0/?method=artist.getsimilar&artist=Death%20Cab%20for%20Cutie&api_key=b25b959554ed76058ac220b7b2e0a026";
            string url = "http://ws.audioscrobbler.com/2.0/?method=artist.getsimilar&artist="+artist+"&api_key=b25b959554ed76058ac220b7b2e0a026";


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
                Console.WriteLine("Status Code: {0}", sStatus); //Debuging connectivity
                Console.WriteLine("");
            }
            catch (WebException e)
            {
                Console.WriteLine("WebException raised!");
                Console.WriteLine("{0}", e.Message);
                Console.WriteLine("{0}", e.Status);

                Console.WriteLine("");
                Console.WriteLine("Press your keyboard to exit.");
                while (Console.ReadKey(true) == null) { }
                return;
            }

            //StreamReader sr = new StreamReader(resp.GetResponseStream());
            //string result = sr.ReadToEnd();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(resp.GetResponseStream());

            XmlNodeList names = xDoc.GetElementsByTagName("name");
            //Console.WriteLine(names[0].InnerText);
            foreach (XmlNode node in names)
            {
                Console.WriteLine(node.InnerText);
            }

            Console.WriteLine("");
            Console.WriteLine("Done. Press your keyboard, man!");
            while (Console.ReadKey(true) == null) { }
        }
    }
}
