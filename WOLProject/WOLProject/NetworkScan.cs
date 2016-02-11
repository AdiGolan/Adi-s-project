using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace WOLProject
{
    class NetworkScan
    { 
        public static void GetIpNetTable(MainForm mainForm, string filter)
        {
            //create a new NetworkBrowser object, and get the
            //list of network computers it found, and add each
            //entry to the combo box on this form
            try
            {
                NetworkBrowser nb = new NetworkBrowser();
                ArrayList ar = nb.getNetworkComputers();
                foreach (string pc_name in ar)
                {
                    if (mainForm.InvokeRequired)
                    {
                        mainForm.Invoke((MethodInvoker)delegate
                        {
                            mainForm.AddComputer(pc_name, true);
                        });
                    }
                    //try
                    //{
                    //    if (pc_name.Contains(filter))
                    //    {
                    //        bool alreadyIn = false;
                           
                    //        if (mainForm.InvokeRequired)
                    //        {
                    //            mainForm.Invoke((MethodInvoker)delegate
                    //            {
                    //                string txt;
                    //                foreach (ListViewItem listViewItem in mainForm.listViewComps.Items)
                    //                {
                    //                    txt = listViewItem.Text.Split(' ')[0];
                    //                    if ( pc_name.IndexOf(txt, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    //                    {
                    //                        alreadyIn = true;
                    //                    }
                    //                }
                    //            });
                    //        }
                    //        else
                    //        {
                    //            string txt;
                    //            foreach (ListViewItem listViewItem in mainForm.listViewComps.Items)
                    //            {
                    //                txt = listViewItem.Text.Split(' ')[0];
                    //                if (pc_name.IndexOf(txt, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                    //                {
                    //                    alreadyIn = true;
                    //                }
                    //            }
                    //        }

                    //        if (!alreadyIn)
                    //        mainForm.AddNewIP(pc_name, 0);
                           
                   //     }
                   // }
                    //catch (Exception ex)  { continue;  }
                }
            }
            catch (Exception ex)
            {
               
            }

        }

    }
}
