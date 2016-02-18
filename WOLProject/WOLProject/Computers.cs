using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WOLProject
{
   
    public class Computers
    {
        public List<Computer> computersList;

        public Computers()
        {
            computersList = new List<Computer>();
        }
    }

    public class Computer
    {
        public int Num { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public bool IsOn { get; set; }
    }
}
