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
    public partial class Try_bar : Form
    {
        private Thread demoThread = null;

        public Try_bar()
        {
            InitializeComponent();
        }

        public void Try_bar_Load(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnimationSpeed = 100;

            //this.progressBar1.Value = 0;

            this.demoThread = new Thread(new ThreadStart(this.Try_bar_close));
            demoThread.Start();
        }

        private void Try_bar_close() {
            Thread.Sleep(5000);

           

            //string a = IForm.GetValue();


            this.Invoke((MethodInvoker)delegate
            {
                // close the form on the forms thread
                this.Close();
            });
        }
        private void Try_bar_FormClosing(object sender, FormClosingEventArgs e)
        {
            //await Delay(500);
            //System.Threading.Tasks.Task.Delay(15000).ContinueWith(_ => { this.Close(); });
        }
    }
}
