using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace WOLProject
{
    public static class WakeOnLan
    {

        /// <summary>
        /// Test a MACAddress string.
        /// hello
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static bool Test(string macAddress)
        {
            var valid_chars = "0123456789ABCDEFabcdef";

            if (string.IsNullOrEmpty(macAddress)) return false;
            if (macAddress.Length != 12) return false;

            foreach (var c in macAddress)
            {
                if (valid_chars.IndexOf(c) < 0)
                   return false;
            }
            return true;
        }

        /// <summary>
        /// Parse a MACAddress string into a byte array.
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        public static byte[] Parse(string macAddress)
        {
            byte[] mac = new byte[6];
            
            for (var i = 0; i < 6; i++)
            {
                var t = macAddress.Substring((i * 2), 2);
                mac[i] = Convert.ToByte(t, 16);
            }
            return mac;
        }

        public static void WakeUp(string macAddress)
        {
            macAddress = macAddress.Replace(":", "");
            if (!Test(macAddress))
                throw new ArgumentException(
                    "Invalid MACAddress string.",
                    "macAddress",
                    null);

            byte[] mac = Parse(macAddress);
            // WOL 'magic' packet is sent over UDP.
            using (UdpClient client = new UdpClient())
            {

                // Send to: 255.255.255.0:40000 over UDP.
                client.Connect(IPAddress.Broadcast, 0);

                // Two parts to a 'magic' packet:
                //     First is 0xFFFFFFFFFFFF,
                //     Second is 16 * MACAddress.
                byte[] packet = new byte[17 * 6];

                // Set to: 0xFFFFFFFFFFFF.
                for (int i = 0; i < 6; i++)
                {
                    packet[i] = 0xFF;
                }

                // Set to: 16 * MACAddress
                for (int i = 1; i <= 16; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        packet[i * 6 + j] = mac[j];
                    }
                }

                // Send WOL 'magic' packet.
                client.Send(packet, packet.Length);
            }
        }
    }
}
