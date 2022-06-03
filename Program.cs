using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace WifiDawg
{
    public class Core
    {

        //private static string[][] _adapter;
        public static NetworkInterface[] adapters;
        private static List<Adapter> NICs = new List<Adapter>();
        private static Stopwatch stopWatch = new Stopwatch();
        public static void Main()
        {
            Logfile.Init();
            Startup();
                                   
            Logfile.Write("Monitoring " + NICs.Count + " network devices.");
                                    
            Logfile.Write("Listening for address changes and availability change. Press any key to exit.");
                        
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(ConnectionChangedCallback);
            
            Console.ReadLine();
        }


        private static void ConnectionChangedCallback(object sender, EventArgs e)
        {
            UpdateAdapterList();
            DateTime now = DateTime.Now;
            Logfile.Write("!! Network Connection change detected !! Invoked @ " + now.ToString()); 
            
            // Find out which nic has changed. 
            // If state was up, and now down : start a timer. 
            // If the IP changes - check . 
            // Update the NIC data
            
            // BACKLOG: This code does not support addition or removal of adapters at runtime - that could be implemented. 

            foreach (NetworkInterface adapter in adapters)
            {
                foreach (Adapter nic in NICs) 
                {
                    //wich adapter is this?

                    //Console.WriteLine(adapter.Name + nic._name);
                 
                    //Console.WriteLine(adapter.GetPhysicalAddress().ToString() + nic._mac);
                    
                    if (adapter.GetPhysicalAddress().ToString() == nic._mac) // it is the correct adapter
                    {
                        //has adapter state changed? 

                        //IS DOWN was UP
                        if ( (adapter.OperationalStatus == OperationalStatus.Down ) && (nic._status == true) ) 
                        {
                            //adapter was up, but now its down! 
                            nic._status = false;
                                                        
                            TimeSpan timeSpan = now - nic._lastAvailable;
                            nic._lastAvailable = now;

                            Logfile.Write("Network Adapter: " + nic._name + " is DOWN! Uptime was : " + timeSpan.ToString());

                        }

                        //IS UP was DOWN
                        if ((adapter.OperationalStatus == OperationalStatus.Up) && (nic._status == false))
                        {
                            //Update the object with correct / new data. 
                            nic._status = true;
                            int oldNumberOfIps = nic._ipv4addresses.Count;
                            nic._ipv4addresses = GetNICIPv4s(adapter);

                            TimeSpan timeSpan = now - nic._lastAvailable;

                            Logfile.Write("Network Adapter : " + nic._name + " is UP! Total downtime was : " + timeSpan.ToString());
                            Logfile.Write("Adapter had " + oldNumberOfIps + " IPv4 addresses assigned. Now it has " + nic._ipv4addresses.Count + ". These are:");
                            foreach(string ip in nic._ipv4addresses) 
                            {
                                Logfile.Write(ip);
                            }
                            
                        }
                    }
                
                }       

                
            }

        }

        public static void UpdateAdapterList() 
        {
            
                 
            adapters = NetworkInterface.GetAllNetworkInterfaces();
        }

        static void Startup()        
        {
            // Get all Network Adapters - write to logfile.
            // Get all IPs - write to logfile. 
            // Get status - Write to logfile. 
            
            adapters = NetworkInterface.GetAllNetworkInterfaces();
            
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            string hostName = Dns.GetHostName();

                        
            Logfile.Write("Network Adapters in " + hostName);
            Logfile.Write("----------------------------------------------------------------------");
            foreach (NetworkInterface n in adapters)
            {

                if ((n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || n.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {

                    //add a new object to the array of nics 

                    string macaddress = n.GetPhysicalAddress().ToString();
                    string name = n.Name;
                    List<string> ips = new List<string>();
                    bool status = false; 
                    //adpater is either an ethernet adapter or a wifi adapter - AND is UP 
                    
                    

                    if (n.OperationalStatus == OperationalStatus.Up) 
                    {
                        status = true;
                        ips = GetNICIPv4s(n);

                        if (ips.Count != 0) 
                        {
                            Logfile.Write("Adapter Name: " + n.Name + ". Has " + ips.Count() + " IPv4 addresses assigned. These are: ");
                            foreach (string ip in ips) 
                            {
                                Logfile.Write(ip);
                            }
                        }
                        else if (ips.Count == 0)
                        {
                            Logfile.Write("Adapter Name: " + n.Name + ". Has no IPv4 addresses assigned.");                           
                        }
                        
                    }

                    if (n.OperationalStatus == OperationalStatus.Down) 
                    {
                        Logfile.Write("Adapter Name: " + n.Name + " is currently down or not connected");
                        status = false;
                    }

                    NICs.Add(new Adapter(macaddress, name, status, ips));



                    Logfile.Write("");

                } 
                


            }

        }
        private static List<string> GetNICIPv4s(NetworkInterface networkInterface)
        {
            List<string> ips = new List<string>();

            foreach (UnicastIPAddressInformation ip in networkInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    string thisIP = ip.Address.ToString();
                    if (thisIP != null)
                    {
                        ips.Add(thisIP);
                    }

                }
            }
            return ips;
        }
    
    
    
    
    
    
    
    
    
    
    
    
    
    } // end of class
 }//end of namespace

