using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace G_IPCAM
{
   
    public partial class Try_connect : Form
    {
        public Form1 _form1 = new Form1();
        

        public Try_connect()
        {
            InitializeComponent();
            
        }

        private void Try_connect_Load(object sender, EventArgs e)
        {

            Application.EnableVisualStyles();

            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnimationSpeed = 100;

            (this.Owner as Form1).Enabled = false; //Disable Form1 button.
        }

        private void Try_connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1 IForm = new Form1();


            //Close receive_broadcast thread while close Tryconnect button....
            _form1.Stop_receive_broadcast_thread();

            (this.Owner as Form1).Enabled = true;

        }
    }
}
