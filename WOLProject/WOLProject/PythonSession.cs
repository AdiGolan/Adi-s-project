using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WOLProject
{
    class PythonSession
    {
        Socket listener;
        const int PORT = 12345;
        Thread listenerThread;
        private MainForm mainForm;
        private Computers computers;

        public PythonSession(MainForm mainForm, Computers computers)
        {
            this.computers = computers;
            this.mainForm = mainForm;
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerThread = new Thread(new ThreadStart(Run));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        public void Run()
        {
            byte[] buffer = new byte[20000];

            listener.Bind(new IPEndPoint(IPAddress.Any, PORT));
            listener.Listen(1);
            Socket pythonSocket = listener.Accept();
            listener.Close();

            while (true)
            {
                int count = pythonSocket.Receive(buffer);
                string data = Encoding.ASCII.GetString(buffer).Substring(0, count);
                string[] fields = data.Split('#');
                switch (fields[0]) // Operation
                {
                    case "ARP":
                        for (int i = 1; i < fields.Length; i++)
                            if (!String.IsNullOrEmpty(fields[i]))
                            {
                                var fieldData = fields[i].Split('@');
                                var macAddress = fieldData[1];
                                var ipAddress = fieldData[0];
                                // Add or update computer in localDb
                                computers.computersList.Add(new Computer()
                                {
                                    IpAddress = ipAddress,
                                    IsOn = true,
                                    MacAddress = macAddress,
                                    Num = i,
                                });
                            }
                        this.mainForm.BuildComputerList();
                        break;
                }
            }
        }
    }
}
