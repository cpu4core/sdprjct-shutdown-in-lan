using System;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using System.Net.Sockets;

//using System.Collections;
//using System.Collections.Generic;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic;
//using System.Net.NetworkInformation;
//using System.Reflection;

namespace shutdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string ipPrefix, ipCom, comname;
        TcpClient clientReceive;
        TcpListener Listeners = new TcpListener(52000);
        TcpClient Myclient;

        public string ScanComputers(string ipFinders)
        {
            string scanIP = ipFinders;
            IPAddress myScanIP = IPAddress.Parse(scanIP);
            IPHostEntry myScanHost = null;
            string[] arr = new string[2];
            try
            {
                myScanHost = Dns.GetHostByAddress(myScanIP);
                if (myScanHost != null)
                {
                    arr[0] = myScanHost.HostName;
                    arr[1] = scanIP;
                    //computerList.Add(arr);
                    //Console.Write(myScanHost.HostName.ToString() + "\t");
                    //Console.WriteLine(scanIP.ToString());
                    //return scanIP.ToString() +  " | "  + myScanHost.HostName.ToString() + Environment.NewLine;
                    return myScanHost.HostName.ToString();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private void ReceiveAndSend(object state)
        {
            string message = string.Empty;
            while (true)
            {
                if (Listeners.Pending() == true)
                {
                    Myclient = Listeners.AcceptTcpClient();
                    StreamReader reader = new StreamReader(Myclient.GetStream());
                    while (reader.Peek() > -1)
                    {
                        message += Convert.ToChar(reader.Read()).ToString();
                    }
                    if (message == "-s -t 0")
                    {
                        System.Diagnostics.Process.Start(@"C:\Windows\System32\shutdown.exe", message);
                    }
                }

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            this.Listeners.Start();
            System.Threading.ThreadPool.QueueUserWorkItem(ReceiveAndSend);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            ipPrefix = textBox1.Text.Trim();
            backgroundWorker1.RunWorkerAsync();
        }


        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string message = "ต้องการ Shut Down เครื่อง " + dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString() + " ใช่หรือไม่ ??";
            string title = Application.ProductName;
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons, MessageBoxIcon.Question);

            if(result == DialogResult.Yes && dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString() != "")
            {
                try
                {
                    clientReceive = new TcpClient(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), 52000);
                    StreamWriter writer = new StreamWriter(clientReceive.GetStream());
                    writer.Write("-s -t 0");
                    writer.Flush();
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int ipMachine = 1; ipMachine < 17; ipMachine++)
            {
                //if (ipMachine == 7 || ipMachine == 16)
                //{
                    ipCom = Convert.ToString(ipMachine);
                    comname = ScanComputers(ipPrefix + "." + ipCom);
                    if (comname != "")
                    {
                        dataGridView1.Rows.Add();
                        int rowindex = dataGridView1.Rows.Count - 2;
                        dataGridView1.Rows[rowindex].Cells[0].Value = ipPrefix + "." + ipCom;
                        dataGridView1.Rows[rowindex].Cells[1].Value = comname;
                        dataGridView1.Rows[rowindex].Cells[2].Value = "SHUT DOWN";
                    }
                //}
            }
            MessageBox.Show("find all");
        }
    }
}
