using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace G_IPCAM
{
   
    public partial class Try_connect : Form
    {
        public Form1 _form1 = new Form1();
        bool form = true;

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

            Thread closeformtimer = new Thread(new ThreadStart(CloseAfter5Sce));
            closeformtimer.Start();
        }

        private void CloseAfter5Sce()
        {

            Form1 IForm = new Form1();
            //GIC-749 close form in 60sec
            Thread.Sleep(15000);

            if (form == true) {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Close();
                    
                });
            }

        }

        private void Try_connect_FormClosing(object sender, FormClosingEventArgs e)
        {


            //Close receive_broadcast thread while close Tryconnect button....
            _form1.Stop_receive_broadcast_thread();

            (this.Owner as Form1).Enabled = true;
            form = false;
        }

    }
}
