using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Web;


namespace SpeedTestGUI
{
    public partial class Form1 : Form
    {
        public int timeLeft;
        private static  DateTime startTime;
        private static  DateTime endTime;

        double getSpeed(string url)
        {
            string fileName = "testFile";

            WebClient wc = new WebClient();

            startTime = System.DateTime.Now;
            try { wc.DownloadFile(url, fileName); }
            catch { return -1d; }
            endTime = System.DateTime.Now;
            wc.Dispose();


            TimeSpan time = (endTime - startTime);
            FileInfo info = new FileInfo(fileName);
            double sizeInBytes = (double)info.Length;
            double totalTime = time.TotalSeconds;

            double speed = (sizeInBytes / totalTime) / 1000;
            return speed;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter sr = new StreamWriter("data.csv", true);
            sr.WriteLine("DateTime, Speed (Kbit/s)");
            sr.Close();

            timeLeft = 0;
            updateTimer.Enabled = true;
            updateTimer.Start();
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft == 0)
            {
                toolStripStatusLabel1.Text = "Updating... [note: this application may be unresponsive for 10-20 secs.]";
                Application.DoEvents();
                double speed = getSpeed("http://support.easystreet.com/easydsl/15MBtestfile.bin");
                //double speed = getSpeed("http://support.easystreet.com/easydsl/testfile.bin");
                if (speed != -1)
                {
                    string stat = System.DateTime.Now.ToString() + ", " + Convert.ToString(speed);
                    textBox1.Text += stat + " Kbit/s \r\n";
                    StreamWriter sr = new StreamWriter("data.csv", true);
                    sr.WriteLine(stat);
                    sr.Close();
                    timeLeft = Convert.ToInt32(textBox2.Text) * 60;
                }
            }
            else
            {
                timeLeft--;
            }
            toolStripStatusLabel1.Text = "Next update in: " + Convert.ToString(timeLeft) + " seconds.";
            
        }

    }
}
