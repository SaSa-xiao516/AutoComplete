namespace AutoComplete
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_BatchCompare = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btn_CompareACAndRTSingleFile = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_BatchCompare
            // 
            this.btn_BatchCompare.Location = new System.Drawing.Point(12, 289);
            this.btn_BatchCompare.Name = "btn_BatchCompare";
            this.btn_BatchCompare.Size = new System.Drawing.Size(112, 38);
            this.btn_BatchCompare.TabIndex = 0;
            this.btn_BatchCompare.Text = "BathCompareAC";
            this.btn_BatchCompare.UseVisualStyleBackColor = true;
            this.btn_BatchCompare.Click += new System.EventHandler(this.btn_BatchCompare_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.textBox4);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(546, 257);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BathCompareCSV";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 241);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(149, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Ignore global_fund_list.csv file";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 213);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Ignore Array";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Key Array";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 128);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "ResultPath";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "FolderB";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "FolderA";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(83, 210);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(428, 20);
            this.textBox5.TabIndex = 4;
            this.textBox5.Text = "RecordId,MarketCapital,AverageVolume,NetAssets";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(83, 168);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(428, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.Text = "rtSymbol,Symbol,rtExchangeId,ExchangeId,rtSecurityType,SecurityType";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(83, 125);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(428, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = "D:\\workfile\\AutoComplete\\QA\\10-22\\Result\\";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(83, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(428, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.Text = "D:\\workfile\\AutoComplete\\QA\\10-22\\ACOld\\AutocompleteUniverse\\";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(83, 37);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(428, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "D:\\workfile\\AutoComplete\\QA\\10-22\\ACNew\\AutocompleteUniverse\\";
            // 
            // btn_CompareACAndRTSingleFile
            // 
            this.btn_CompareACAndRTSingleFile.Location = new System.Drawing.Point(152, 289);
            this.btn_CompareACAndRTSingleFile.Name = "btn_CompareACAndRTSingleFile";
            this.btn_CompareACAndRTSingleFile.Size = new System.Drawing.Size(112, 38);
            this.btn_CompareACAndRTSingleFile.TabIndex = 2;
            this.btn_CompareACAndRTSingleFile.Text = "CompareACAndRT  SingleFile";
            this.btn_CompareACAndRTSingleFile.UseVisualStyleBackColor = true;
            this.btn_CompareACAndRTSingleFile.Click += new System.EventHandler(this.btn_CompareACAndRTSingleFile_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(286, 289);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(112, 38);
            this.button3.TabIndex = 3;
            this.button3.Text = "CompareACAndRT  BatchFile";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(424, 289);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(134, 38);
            this.button4.TabIndex = 4;
            this.button4.Text = "VerifyACAsExpChange";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.VerifyACAsExpChange_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(12, 349);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(546, 154);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CompareACandRT";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 559);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btn_CompareACAndRTSingleFile);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_BatchCompare);
            this.Name = "Form1";
            this.Text = "AutoComplete";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_BatchCompare;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_CompareACAndRTSingleFile;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
    }
}

