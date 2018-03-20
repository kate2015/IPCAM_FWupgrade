using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.Enabled = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(httprequest_timer);

        }

        public void httprequest_timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            camera_info();
            
        }

        void camera_info()
        {
            string HWresp_st = null;
            String[] getSysInfoUri = new String[_supportDeviceNum];
            HttpWebResponse netResp;
            Stream sys_stream;

            for (int i = 0; i < _supportDeviceNum; i++)
            {
                if (ipaddr[i] != null  ) // &&_alreadyDecideUpgradeStatus[i] == false
                {
                    getSysInfoUri[i] = string.Format("http://{0}/cgi-bin/system.cgi",ipaddr[i]);
                    netResp = GethttpRequest(getSysInfoUri[i]);
                    
                    if (null != netResp)
                    {
                        HWresp_st = create_stream(netResp);
                        sys_stream = GenerateStreamFromString(HWresp_st);
                        f_sys_xml(sys_stream, i);
                    }

                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private HttpWebResponse GethttpRequest(String S)
        {

            HttpWebResponse response = null;
            HttpWebRequest request = null;
            String login = "admin";
            String passwd = "2100";

            try
            {
                request = (HttpWebRequest)HttpWebRequest.Create(S);
                NetworkCredential nc = new NetworkCredential(login, passwd);
                request.Credentials = nc;
                request.Method = "GET";
                request.Timeout = 12000;    // time out value = 1 s.
                request.KeepAlive = false;
                request.Proxy = null;
                response = (HttpWebResponse)request.GetResponse();

            }
            catch (Exception ex)
            {
                MessageBox.Show("error{0}", ex.Message);
            }

            return (response != null ? response : null);
        }

        private string create_stream(HttpWebResponse HWResp)
        {
            string StrRsp = null;
            if (null != HWResp)
            {
                Stream receiveStream = HWResp.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                StrRsp = readStream.ReadToEnd(); // Read the xxx.cgi xml 

                receiveStream.Close();
                readStream.Close();

                HWResp.Close();
                HWResp = null;
            }
            return StrRsp;
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public void f_sys_xml(Stream S, int devIdx)
        {

            XmlTextReader xmlReader = new XmlTextReader(S);
            string md_name = null;
            string fwVer = null;
            DataGridViewRow row = dataGridView1.Rows[devIdx];

            while (xmlReader.Read())
            {
                switch (xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        //listBox1.Items.Add("<" + xmlReader.Name + ">");

                        switch (xmlReader.Name)
                        {
                            case "System":
                                xmlReader.Read();
                                if (xmlReader.Name.Equals("ModelName"))
                                {
                                    //xmlReader.Read();
                                    //textBox_model.Text = xmlReader.Value;
                                    //modelname = xmlReader.Value;
                                }
                                break;
                            case "SkuName":
                                if (xmlReader.Name.Equals("SkuName"))
                                {
                                    xmlReader.Read();
                                    //md_type = xmlReader.Name;
                                    switch (xmlReader.Value)
                                    {
                                        case "CA-NF21-N":
                                            md_name = "CA-NF21-N";
                                            break;
                                        case "CA-NF21-W":
                                            md_name = "CA-NF21-W";
                                            break;
                                        case "CA-NF21-WI":
                                            md_name = "CA-NF21-WI";
                                            break;
                                    }
                                    //md_name = dataGridView1.Rows[devIdx].Cells[1] ;
                                    //dataGridView1.BeginInvoke((MethodInvoker)delegate () { dataGridView1.Rows[devIdx].Cells[1], (string) md_name; });
                                    row.Cells[2].Value = md_name;
                                }
                                break;
                            case "PartNumber":
                                xmlReader.Read();
                                //TextBox_partnum.Text = xmlReader.Value;
                                //partnumber = xmlReader.Value;
                                break;
                            case "PCBASerialNumber":
                                xmlReader.Read();
                                // textBox_pcba.Text = xmlReader.Value;
                                //pcbanumber = xmlReader.Value;
                                break;
                            case "SysSerialNumber":
                                xmlReader.Read();
                                //tb_serial.Text = xmlReader.Value;
                                //sysserialnum = xmlReader.Value;
                                break;
                            case "FWVersion":
                                xmlReader.Read();
                                //tb_fwversion.Text = xmlReader.Value;
                                row.Cells[4].Value = xmlReader.Value;
                                break;
                            case "LoadDefault":
                                xmlReader.Read();
                                //loaddefault = xmlReader.Value;
                                break;
                            case "MacAddress":
                                xmlReader.Read();
                                //this.textBox_macaddress.Nam = xmlReader.Value;
                                row.Cells[3].Value = xmlReader.Value;
                                break;
                            case "Token":
                                xmlReader.Read();
                                //_token = xmlReader.Value;
                                break;
                        }
                        break;
                }
            }
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
            
            Int32 selectedCellCount = dataGridView1.GetCellCount(DataGridViewElementStates.Selected);



            if (selectedCellCount > 0)
            {
                if (dataGridView1.AreAllCellsSelected(true))
                {
                    MessageBox.Show("All cells are selected", "Selected Cells");
                }
                else
                {
                    System.Text.StringBuilder sb =
                        new System.Text.StringBuilder();

                    for (int i = 0;
                        i < selectedCellCount; i++)
                    {
                        sb.Append("Row: ");
                        sb.Append(dataGridView1.SelectedCells[i].RowIndex
                            .ToString());
                        sb.Append(", Column: ");
                        sb.Append(dataGridView1.SelectedCells[i].ColumnIndex
                            .ToString());
                        sb.Append(Environment.NewLine);
                    }

                    sb.Append("Total: " + selectedCellCount.ToString());
                    MessageBox.Show(sb.ToString(), "Selected Cells");
                }
            }
            else {
                MessageBox.Show("PLease Select IPcamera first, then try again ");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PLease Select IPcamera first, then try again ");
        }
    }
}
