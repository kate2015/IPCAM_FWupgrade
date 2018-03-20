using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace G_IPCAM
{
    public partial class Form1 : Form
    {
        public int deviceNum = 0;
        private int _broadTimes = 5;
        private int _supportDeviceNum = 0;
        private Broadcast _broadcastObject = new Broadcast();
        public String[] hostIps;
        public int supportDeviceNum = 100;
        public int supportNetInfNum = 100;
        public int count = 1;
        public int sum = 0;
        public String[] ipaddr;
        public String[] mac;
        public String[] WebCam;
        public String hostIp;
        public dynamic stuff;

        private int _broadcastPort = 4950;
        private bool _shouldStop;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void broadcast_fun()
        {
            int itfNum = 0;

            //System.Text.StringBuilder ipAddressList = new System.Text.StringBuilder();
            //ipAddressList.Clear();

            foreach (System.Net.NetworkInformation.NetworkInterface nic
                in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (System.Net.NetworkInformation.IPAddressInformation ipInfo
                    in nic.GetIPProperties().UnicastAddresses)
                {
                    if (System.Net.IPAddress.IsLoopback(ipInfo.Address) == false
                        && ipInfo.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        Console.WriteLine("ip = {0}", ipInfo.Address.ToString());
                        if (String.Compare(ipInfo.Address.ToString(), 0, "192.168.5.", 0, 10, true) == 0)
                        {
                            _broadcastObject.hostIp = (String)ipInfo.Address.ToString();
                            Console.WriteLine("nita ip = {0}", _broadcastObject.hostIp);
                            if (itfNum < _broadcastObject.supportNetInfNum)
                            {
                                _broadcastObject.hostIps[itfNum] = (String)ipInfo.Address.ToString();
                            }
                            itfNum++;
                        }

                    }
                }
            }
            Console.WriteLine("itfNum = {0}", itfNum);
            for (int i = 0; i < _broadTimes; i++)
                _broadcastObject.broadcast_getacipc();
            return;
        }

        public void receive_broadcast_thread()
        {

            bool findSameMac = false;
            int i = 0;

            ipaddr = new String[supportDeviceNum];
            mac = new String[supportDeviceNum];
            WebCam = new String[supportDeviceNum];

            for (i = 0; i < supportDeviceNum; i++)
            {
                ipaddr[i] = null;
                mac[i] = null;
                WebCam[i] = null;
            }

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _broadcastPort);

            Socket newsock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            newsock.Bind(ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)(sender);
            String macAddr;

            try {
                while (!_shouldStop)
                {
                    byte[] data = new byte[1024];
                    try
                    {
                        int recv = newsock.ReceiveFrom(data, ref Remote);
                        if (recv == 0)
                            continue;

                        //dynamic stuff = JObject.Parse(Encoding.UTF8.GetString(data, 0, recv));
                        stuff = JObject.Parse(Encoding.UTF8.GetString(data, 0, recv));

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
                                    Console.WriteLine("nitaa{0} ipaddr = {1}", i, (String)stuff.ip);

                                    dataGridView1.BeginInvoke((MethodInvoker)delegate () { dataGridView1.Rows.Add(i, (String)stuff.ip); });

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
            }
            catch (ThreadAbortException exception)
            {
                Console.WriteLine("Exception message:{0}", exception.Message);
                Thread.ResetAbort();
            }
            
            Console.WriteLine("broadcaster thread: terminating.");
            
        }

        void Stop_receive_broadcast_thread()
        {
            _shouldStop = true;
        }

        public void button1_Click(object sender, EventArgs e)
        {
            Thread _recvBroadcastThread = new Thread(receive_broadcast_thread);

            _supportDeviceNum = _broadcastObject.supportDeviceNum;
            _broadcastObject.hostIps = new String[_broadcastObject.supportNetInfNum];//for multi interface case
            _recvBroadcastThread.Start();

            broadcast_fun();

            DialogResult r = MessageBox.Show("Trying to connect...");
            if (r == DialogResult.OK)
            {
                // Close the receive_broadcast_thread.
                //MessageBox.Show(ipaddr[0].ToString());
                //MessageBox.Show(ipaddr[1].ToString());
                Stop_receive_broadcast_thread();

            }

        }

        /*private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            const string message =
                "Are you sure that you would like to close the form?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Exclamation);

            // If the no button was pressed ...
            if (result == DialogResult.No)
            {
                // cancel the closure of the form.
                e.Cancel = true;
            }
        }*/

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PLease Select IPcamera first, then try again ");

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PLease Select IPcamera first, then try again ");
        }
    }
}
