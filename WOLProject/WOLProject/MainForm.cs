using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace WOLProject
{
    public partial class MainForm : Form
    {
        PythonSession pythonSession;
        LocalDb localDb;
        int indexHost = -1;

        public MainForm()
        {
            InitializeComponent();
            CreatePythonEngine();
            
            //this.WindowState = FormWindowState.Maximized;
        }

        private void CreatePythonEngine()
        {
            localDb = new LocalDb("db-data.txt");
            foreach (var computer in localDb.LocalDbData.Computers)
            {
                computer.IsOn = false;
                computer.IpAddress = null;
            }

            Process pythonProcess = new Process();
            pythonProcess.StartInfo.FileName = System.Configuration.ConfigurationManager.AppSettings["PythonApp"];
            pythonProcess.StartInfo.Arguments = @"Python\PythonServer.py";
            pythonProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            pythonProcess.Start();

            pythonSession = new PythonSession(this, localDb);
        }

        //private void NetScan()
        //{
        //    NetworkScan.GetIpNetTable(this, "");
        //}

        private void listViewOnline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHosts.SelectedIndices.Count > 0)
            {
                listViewHosts.ContextMenuStrip = contextMenuStrip1;
                indexHost = listViewHosts.SelectedIndices[0];
            }
            else
                listViewHosts.ContextMenuStrip = null;

        }

        private void wakeOnLaneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mac = listViewHosts.Items[indexHost].SubItems[2].Text;
            WakeOnLan.WakeUp(mac);
        }
        public void BuildComputerList()
        {
            this.Invoke((MethodInvoker)delegate
            {
                listViewHosts.Items.Clear();
                foreach (var computer in localDb.LocalDbData.Computers)
                {
                    listViewHosts.Items.Add(new ListViewItem(new string[] {
                    computer.Num.ToString(),
                    computer.IpAddress,
                    computer.MacAddress,
                    computer.IsOn ? "On" : "Off" }, 1));
                }
            });
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            BuildComputerList();
        }

    }
}
