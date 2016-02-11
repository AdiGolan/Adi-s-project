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
        int indexHost = -1;

        public MainForm()
        {
            InitializeComponent();
             CreatePythonEngine();
            //this.WindowState = FormWindowState.Maximized;
        }

        private void CreatePythonEngine()
        {
            Process pythonProcess = new Process();
            pythonProcess.StartInfo.FileName = @"C:\Python26\python.exe";
            pythonProcess.StartInfo.Arguments = @"Python\PythonServer.py";
            pythonProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            pythonProcess.Start();

            pythonSession = new PythonSession(this);
        }
 
        //private void NetScan()
        //{
        //    NetworkScan.GetIpNetTable(this, "");
        //}

        public void AddComputer(int i, string [] conn, bool online)
        {
            this.Invoke((MethodInvoker)delegate
            {
                string[] fields = { i.ToString(), conn[0], conn[1], "on" };
                ListViewItem item = new ListViewItem(fields, 0);
                listViewHosts.Items.Add(item);
            });

        }

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

     }
}
