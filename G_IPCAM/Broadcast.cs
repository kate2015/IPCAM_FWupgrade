using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace G_IPCAM
{
    class Broadcast
    {

        public int deviceNum = 0;
        public int supportDeviceNum = 100;
        public int supportNetInfNum = 100;
        //public String[] ipaddr;
        //public String[] mac;
        public String hostIp;
        public String[] hostIps;
        private int _broadcastPort = 4950;

        public void broadcast_getacipc()
        {
            broadcast_msg_str("{\"bcst\":true}", _broadcastPort);
        }
        /*public void receive_broadcast_thread()
        {
            bool findSameMac = false;
            int i = 0;
            

            ipaddr = new String[supportDeviceNum];
            mac = new String[supportDeviceNum];

            for (i = 0; i < supportDeviceNum; i++)
            {
                ipaddr[i] = null;
                mac[i] = null;
            }

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _broadcastPort);

            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            newsock.Bind(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            String macAddr;
            while (!_shouldStop)
            {
                byte[] data = new byte[1024];
                try
                {
                    int recv = newsock.ReceiveFrom(data, ref Remote);
                    if (recv == 0)
                        continue;
                    
                    dynamic stuff = JObject.Parse(Encoding.UTF8.GetString(data, 0, recv));
                        
                    findSameMac = false;
                    
                    //check stuff.macstuff.mac is exist or not
                    for (i = 0; i < supportDeviceNum; i++)
                    {
                        if (mac[i] != null)
                        {
                            if (stuff.mac == null)
                                break;
                            macAddr = (String)stuff.mac;
                            if (mac[i].Equals(macAddr) == true)
                            {
                                findSameMac = true;
                                break;
                            }
                        }
                    }

                    for (i = 0; i < supportDeviceNum; i++)
                    {
                        if (mac[i] == null)
                        {
                            if (findSameMac == false)
                            {
                                if (stuff.mac == null || stuff.ip == null)
                                    break;
                                mac[i] = (String)stuff.mac;
                                ipaddr[i] = (String)stuff.ip;
                                Console.WriteLine("mac = {0}", (String)stuff.mac);
                                Console.WriteLine("nita ipaddr = {0}", (String)stuff.ip);
                                Console.WriteLine("test{0}", ipaddr[i]);
                                //dataGridView1.BeginInvoke((MethodInvoker)delegate () { dataGridView1.Rows.Add(i, (String)stuff.ip); });
                                deviceNum++;
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error:{0}", ex.ToString(), "\n");
                    //_exceptionHandleObject.write_exception_log(ex.ToString());
                    System.Environment.Exit(-12);
                }
            }
            Console.WriteLine("broadcaster thread: terminating.");
        }*/
        public void RequestStop()
        {
            _shouldStop = true;
        }
        public void broadcast_msg_str(string message, int port)
        {
            Console.WriteLine("broadcaster:{0}", message);
            broadcast_msg_byte(Encoding.ASCII.GetBytes(message), port);
        }

        private void broadcast_msg_byte(byte[] message, int port)
        {
            int i = 0;
            for (i = 0; i < supportNetInfNum; i++)
            {
                if (hostIps[i] == null)
                    break;
                Console.WriteLine("host ip = {0}", hostIps[i]);
                using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    sock.EnableBroadcast = true;
                    sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

                    var iep = new IPEndPoint(IPAddress.Broadcast, port);

                    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(hostIps[i]), port);
                    sock.Bind(ipep);

                    sock.SendTo(message, iep);
                }
            }
        }

        private volatile bool _shouldStop;
    }
}
