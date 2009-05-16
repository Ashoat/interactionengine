using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Web;

namespace SpeedTest
{
    class Program
    {
        static double getSpeed(string url)
        {
            string fileName = "testFile";
            DateTime startTime;
            DateTime endTime;
            WebClient wc = new WebClient();

            startTime = System.DateTime.Now;
            wc.DownloadFile(url, fileName);
            endTime = System.DateTime.Now;
            wc.Dispose();

            TimeSpan time = (endTime - startTime);
            FileInfo info = new FileInfo(fileName);
            double sizeInBytes = (double)info.Length;
            double totalTime = time.TotalSeconds;

            double speed = (sizeInBytes / totalTime) / 1000;
            return speed;
        }
        static void Main(string[] args)
        {
            double speed = getSpeed("http://support.easystreet.com/easydsl/15MBtestfile.bin");
            Console.WriteLine(speed + " Kbit/s");
        }
    }
}
