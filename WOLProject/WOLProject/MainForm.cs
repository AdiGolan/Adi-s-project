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
using System.Collections;

namespace WOLProject
{
    public partial class MainForm : Form
    {
        PythonSession pythonSession;
        Computers computers;
        int indexHost = -1;

        public MainForm()
        {
            InitializeComponent();
            computers = new Computers();
            CreatePythonEngine();
        }

        private void CreatePythonEngine()
        {

            Process pythonProcess = new Process();
            pythonProcess.StartInfo.FileName = @"C:\Python26\python.exe";  //System.Configuration.ConfigurationManager.AppSettings["PythonApp"];
            pythonProcess.StartInfo.Arguments = @"Python\NetworkScan.py";
            pythonProcess.StartInfo.WorkingDirectory = Application.StartupPath;
            pythonProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pythonProcess.Start();
            pythonSession = new PythonSession(this, computers);
            timer1.Enabled = true;
        }


        private void listViewOnline_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewHosts.SelectedIndices.Count > 0)
            {
                indexHost = listViewHosts.SelectedIndices[0];
                string status = listViewHosts.Items[indexHost].SubItems[3].Text;
                if (status == "On")
                {
                    wakeOnLaneToolStripMenuItem.Visible = false;
                    shutdownToolStripMenuItem.Visible = true;
                }
                else
                {
                    wakeOnLaneToolStripMenuItem.Visible = true;
                    shutdownToolStripMenuItem.Visible = false;
                }
                listViewHosts.ContextMenuStrip = contextMenuStrip1;

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

                DataTable computersFromDB = DataBaseHandler.ExecuteDataTable("Select * From Computers");
                List<Computer> computersListFromDB = new List<Computer>();
                int i = 1;
                foreach (DataRow row in computersFromDB.Rows)
                {
                    computersListFromDB.Add(new Computer()
                    {
                        IpAddress = row["IP"].ToString(),
                        IsOn = false,
                        MacAddress = row["MAC"].ToString(),
                        Num = i++,
                    });
                }

                foreach (Computer comp in computers.computersList)
                {
                    Computer compDB = InDB(computersListFromDB, comp.MacAddress);
                    if (compDB == null)
                    {
                        DataBaseHandler.ExecuteNonQuery("Insert Into Computers (IP, MAC) Values ('" + comp.IpAddress + "','" + comp.MacAddress + "')");
                        computersListFromDB.Add(comp);
                    }
                    else
                        compDB.IsOn = true;
                }
                int n = 0, f=0 , a=0;
                foreach (Computer computer in computersListFromDB)
                {
                    listViewHosts.Items.Add(new ListViewItem(new string[] {
                        computer.Num.ToString(),
                        computer.IpAddress,
                        computer.MacAddress,
                        computer.IsOn ? "On" : "Off" }, computer.IsOn ? 0 : 1));
                    a++;
                    if (computer.IsOn)
                    {
                        n++;
                    }
                    else
                    {
                        f++;
                    }
                }
                string all = a.ToString();
                string on = n.ToString();
                string off = f.ToString();
                allcomputers.Text = all;
                oncomputers.Text = on;
                offcomputers.Text = off;
            });

        }

        private Computer InDB(List<Computer> computersListFromDB, string mac)
        {
            foreach (Computer comp in computersListFromDB)
            {
                if (comp.MacAddress.Equals(mac))
                    return comp;
            }
            return null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
           CreatePythonEngine();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreatePythonEngine();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void computersoff_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
        
        }

    }
}

