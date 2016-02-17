using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WOLProject
{
    
    public class LocalDb
    {
        private string _dbFile;
        private XmlSerializer _xmlSer;
        public LocalDbData LocalDbData { get; set; }
        public LocalDb(string dbFile)
        {
            _xmlSer = new XmlSerializer(typeof(LocalDbData));
            _dbFile = dbFile;
            // Load from disk _dbFile
            if (File.Exists(_dbFile))
            {
                using (var fileStream = File.OpenRead(_dbFile))
                {
                    LocalDbData = _xmlSer.Deserialize(fileStream) as LocalDbData;
                }
            }
            else
            {
                LocalDbData = new LocalDbData();
                LocalDbData.Computers.Add(new Computer
                {
                    IpAddress ="1.1.1.1",
                    IsOn = false,
                    MacAddress = "11:22:33:44:55:66",
                    Num = 1,
                });
            }
        }

        public void Save()
        {
            if (File.Exists(_dbFile))
            {
                File.Delete(_dbFile);
            }
            // save to disk to _dbFile
            using (var fileStream = File.OpenWrite(_dbFile))
            {
                _xmlSer.Serialize(fileStream, LocalDbData);
            }
        }
    }

    public class LocalDbData
    {
        public LocalDbData()
        {
            Computers = new List<Computer>();
        }
        public List<Computer> Computers { get; set; }
    }

    public class Computer
    {
        public int Num { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
        public bool IsOn { get; set; }
    }
}
