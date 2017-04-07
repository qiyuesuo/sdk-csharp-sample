using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDKTest
{
    public partial class ParamerDlg : Form
    {
 
        private ListView listView;

        public ParamerDlg(ListView listView)
        {
            this.listView = listView;
            InitializeComponent();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(KeyText.Text)) {
                MessageBox.Show("缺少参数名称");
                return;
            }

            if (String.IsNullOrEmpty(ValueText.Text)) {
                MessageBox.Show("缺少参数值");
                return;
            }



            string[] values = { KeyText.Text, ValueText.Text };
            ListViewItem iteam = new ListViewItem(values);
            listView.Items.Add(iteam);
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
