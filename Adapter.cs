using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

namespace WifiDawg
{
    public class Adapter
    {
        public string _mac { get; set; }
        public string _name { get; set; }
        public List<string> _ipv4addresses { get; set; }
        public bool _status { get; set; }
        public DateTime _lastAvailable { get; set; }

        //CONSTRUCTOR
        public Adapter(string mac, string name, bool status, List<string> addr) 
        {
            _ipv4addresses = new List<string>();
            _mac = mac;
            _name = name;
            _lastAvailable = DateTime.Now;
            _status = status;

            foreach (string s in addr) 
            {
                _ipv4addresses.Add(s);
            }

                     
            //Task task = Task.Run(() => { EnableMonitoring(); });

        }
        //METHODS
   

        
    }

    

}
