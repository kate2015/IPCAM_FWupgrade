using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace G_IPCAM
{
    public partial class Modify_ip : Form
    {
        private string string1;
        private string Ip;
        public string Ipaddress, newIp;
        private Form1 form1;

        public Modify_ip(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;
        }

        public string String1
        {
            set
            {
                string1 = value;
            }
            get
            {
                return newIp;
            }
           
        }

        public void SetValue()
        {
            this.tb_model.Text = string1; //Pass Form1 Model type to Form Midify_ip 
            this.tb_ipaddress.Text = Ip; //Pass Form1 datagrid view  ipaddr to Form Modify_ip
        }

        public string String2
        {
            set
            {
                Ip = value;
            }
        }

        public string GetValue()
        {
            newIp = this.tb_ipaddress.Text;
            return newIp;
        }

        public void button_accept_Click(object sender, EventArgs e)
        {
            
            Ipaddress = Ip;
            newIp = this.tb_ipaddress.Text;
            string dlc_content = "<?xml version =\"1.0\" encoding=\"utf-8\"?><network><Ipv4Address>" + newIp + "</Ipv4Address></network>";
            //string post_data = "content=" + Uri.EscapeDataString(dlc_content);
            string url = "http://" + Ipaddress + "/cgi-bin/network.cgi";
            
            HttpWebResponse HWResp;

            try
            {
                HWResp = PosthttpRequest(dlc_content, url);
            }
            catch (Exception ex)
            {
                MessageBox.Show("error" + ex.ToString());
            }

            this.Close();

            //close all form.
            //Environment.Exit(Environment.ExitCode); 


            Try_bar TryForm = new Try_bar();
            TryForm.Show();
            
            //GIC-750 update Form1 datagridview ipaddress at the same time
            form1.dataGridView1.SelectedRows[0].Cells[1].Value = newIp;

        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Modify_ip_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private HttpWebResponse PosthttpRequest(String dlc_content, String url)
        {
            HttpWebResponse response = null;
            string login, passwd;
            login = "admin";
            passwd = "2100";


            try
            {
                //create a request
                byte[] buffer = Encoding.ASCII.GetBytes(string.Concat("content=", dlc_content));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                // Set some reasonable limits on resources used by this request
                NetworkCredential nc = new NetworkCredential(login, passwd);
                request.Method = "POST";
                request.Credentials = nc;
                request.Proxy = null;
                //request.MaximumAutomaticRedirections = 4;
                //request.MaximumResponseHeadersLength = 4;

                request.ContentType = "applicaton/xml";
                request.Timeout = 8000;
                //request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.ContentLength = buffer.Length;
                request.KeepAlive = false;

                Stream stream = request.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();


                response = (HttpWebResponse)request.GetResponse();

                //remember to close. otherwise. request.Getresponse() will timeout;
                response = null;
                request.Abort();
                request = null;
            }
            catch (WebException ex)
            {
                MessageBox.Show("error{0}", ex.ToString());
            }

            return (response != null ? response : null);
            //return responsedata;
        }
    }
}
