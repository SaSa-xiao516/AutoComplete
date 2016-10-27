using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Xml;
using System.Configuration;
using AutoComplete.Comm;
using AutoComplete.ACChange;
using System.Data.Common;
using System.Threading;
using System.Text.RegularExpressions;

namespace AutoComplete
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_BatchCompare_Click(object sender, EventArgs e)
        {
            try
            {
                //不支持压缩包
                if (Directory.Exists(this.textBox1.Text) == false)
                {
                    MessageBox.Show(this.textBox1.Text + " Not Exist!");
                    return;
                }
                if (Directory.Exists(this.textBox2.Text) == false)
                {
                    MessageBox.Show(this.textBox2.Text + " Not Exist!");
                    return;
                }
                if (Directory.Exists(this.textBox3.Text) == false)
                {
                    MessageBox.Show(this.textBox3.Text + " Not Exist!");
                    return;
                }
                if (this.textBox1.Text.Substring(this.textBox1.Text.Length - 1, 1) != "\\" || this.textBox2.Text.Substring(this.textBox2.Text.Length - 1, 1) != "\\" || this.textBox3.Text.Substring(this.textBox3.Text.Length - 1, 1) != "\\")
                {
                    MessageBox.Show("Folder should end with \"\\\"");
                    return;
                }
                string[] keyArray = this.textBox4.Text.Split(',');
                string[] ignoreArray = this.textBox5.Text.Split(',');

                AutoCompleteTestBase.MultiThreadCompare(this.textBox1.Text, this.textBox2.Text, this.textBox3.Text, keyArray, ignoreArray);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_CompareACAndRT_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(this.textBox1.Text) == false)
            {
                MessageBox.Show(this.textBox1.Text + " Not Exist!");
                return;
            }
            if (Directory.Exists(this.textBox2.Text) == false)
            {
                MessageBox.Show(this.textBox2.Text + " Not Exist!");
                return;
            }
            if (Directory.Exists(this.textBox3.Text) == false)
            {
                MessageBox.Show(this.textBox3.Text + " Not Exist!");
                return;
            }
            if (this.textBox1.Text.Substring(this.textBox1.Text.Length - 1, 1) != "\\" || this.textBox2.Text.Substring(this.textBox2.Text.Length - 1, 1) != "\\" || this.textBox3.Text.Substring(this.textBox3.Text.Length - 1, 1) != "\\")
            {
                MessageBox.Show("Folder should end with \"\\\"");
                return;
            }
            string[] keyArray = this.textBox4.Text.Split(',');
            string[] ignoreArray = this.textBox5.Text.Split(',');
            AutoCompleteTestBase.compareFileByKey(this.textBox1.Text, this.textBox2.Text, this.textBox3.Text, keyArray, ignoreArray);
        }

        private void btn_CompareACAndRTSingleFile_Click(object sender, EventArgs e)
        {
            //仅仅比对Symbol，SecurityType，ExchangeId
            //输入：要比对的AC文件 + RT文件

        }

        private void VerifyACAsExpChange_Click(object sender, EventArgs e)
        {

        }
    }
}
