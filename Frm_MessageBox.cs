using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParsicAutoUpdater
{
    public partial class Frm_MessageBox : Form
    {
        string Message = "";
        string Title = "";
        public Frm_MessageBox()
        {
            InitializeComponent();
        }
        public Frm_MessageBox(string _Message, string _Title)
        {
            InitializeComponent();
            Message = _Message;
            Title = _Title;
        }

        private void Frm_MessageBox_Load(object sender, EventArgs e)
        {
            Txt_Message.Text = Message;
            this.Text = Title;
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
