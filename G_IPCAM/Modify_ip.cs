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
    public partial class Modify_ip : Form
    {
        public Modify_ip()
        {
            InitializeComponent();
            //tb_model.Text.ToString() = md_name;
            Form1 obj = new Form1();
            //obj.textBoxMsg = tb_model.Text;
            tb_model.Text = obj.textBoxMsg ;
        }

       

    }
}
