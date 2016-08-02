namespace WindowsFormsApplication1
{
    partial class savetable
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.selectink = new System.Windows.Forms.CheckBox();
            this.DeleteBotton = new System.Windows.Forms.Button();
            this.PrintScene = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SaveChair = new System.Windows.Forms.Button();
            this.PrintSideChair = new System.Windows.Forms.Button();
            this.PlSitOnSdCh = new System.Windows.Forms.Button();
            this.ChatingScene = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.Complex_Object = new System.Windows.Forms.Button();
            this.Print_Complex_objects = new System.Windows.Forms.Button();
            this.complex_plmd = new System.Windows.Forms.Button();
            this.complex_Table = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(2, 290);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(71, 37);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // selectink
            // 
            this.selectink.AutoSize = true;
            this.selectink.Location = new System.Drawing.Point(247, 12);
            this.selectink.Name = "selectink";
            this.selectink.Size = new System.Drawing.Size(74, 17);
            this.selectink.TabIndex = 1;
            this.selectink.Text = "Select Ink";
            this.selectink.UseVisualStyleBackColor = true;
            this.selectink.CheckedChanged += new System.EventHandler(this.selectink_CheckedChanged);
            // 
            // DeleteBotton
            // 
            this.DeleteBotton.Location = new System.Drawing.Point(2, 482);
            this.DeleteBotton.Name = "DeleteBotton";
            this.DeleteBotton.Size = new System.Drawing.Size(42, 19);
            this.DeleteBotton.TabIndex = 3;
            this.DeleteBotton.Text = "DeleteBotton";
            this.DeleteBotton.UseVisualStyleBackColor = true;
            this.DeleteBotton.Click += new System.EventHandler(this.DeleteBotton_Click_1);
            // 
            // PrintScene
            // 
            this.PrintScene.Location = new System.Drawing.Point(2, 328);
            this.PrintScene.Name = "PrintScene";
            this.PrintScene.Size = new System.Drawing.Size(80, 28);
            this.PrintScene.TabIndex = 8;
            this.PrintScene.Text = "PrintScene";
            this.PrintScene.UseVisualStyleBackColor = true;
            this.PrintScene.Click += new System.EventHandler(this.PrintScene_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 449);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 27);
            this.button1.TabIndex = 9;
            this.button1.Text = "PrintFrntChair";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(1, 424);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 19);
            this.button2.TabIndex = 10;
            this.button2.Text = "SavePlmd";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(-1, 507);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(71, 30);
            this.button3.TabIndex = 11;
            this.button3.Text = "PrintPlmd";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // SaveChair
            // 
            this.SaveChair.Location = new System.Drawing.Point(2, 394);
            this.SaveChair.Name = "SaveChair";
            this.SaveChair.Size = new System.Drawing.Size(78, 24);
            this.SaveChair.TabIndex = 12;
            this.SaveChair.Text = "SaveChair";
            this.SaveChair.UseVisualStyleBackColor = true;
            this.SaveChair.Click += new System.EventHandler(this.SaveChair_Click);
            // 
            // PrintSideChair
            // 
            this.PrintSideChair.Location = new System.Drawing.Point(1, 543);
            this.PrintSideChair.Name = "PrintSideChair";
            this.PrintSideChair.Size = new System.Drawing.Size(74, 28);
            this.PrintSideChair.TabIndex = 13;
            this.PrintSideChair.Text = "PrintSdChair";
            this.PrintSideChair.UseVisualStyleBackColor = true;
            this.PrintSideChair.Click += new System.EventHandler(this.PrintSideChair_Click);
            // 
            // PlSitOnSdCh
            // 
            this.PlSitOnSdCh.Location = new System.Drawing.Point(2, 362);
            this.PlSitOnSdCh.Name = "PlSitOnSdCh";
            this.PlSitOnSdCh.Size = new System.Drawing.Size(71, 26);
            this.PlSitOnSdCh.TabIndex = 14;
            this.PlSitOnSdCh.Text = "PlsitonSdCh";
            this.PlSitOnSdCh.UseVisualStyleBackColor = true;
            this.PlSitOnSdCh.Click += new System.EventHandler(this.PlSitOnSdCh_Click);
            // 
            // ChatingScene
            // 
            this.ChatingScene.Location = new System.Drawing.Point(2, 188);
            this.ChatingScene.Name = "ChatingScene";
            this.ChatingScene.Size = new System.Drawing.Size(68, 30);
            this.ChatingScene.TabIndex = 16;
            this.ChatingScene.Text = "ChatingScene";
            this.ChatingScene.UseVisualStyleBackColor = true;
            this.ChatingScene.Click += new System.EventHandler(this.ChatingScene_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(-1, 214);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(80, 39);
            this.textBox2.TabIndex = 18;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1, 162);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(86, 25);
            this.button4.TabIndex = 19;
            this.button4.Text = "save table";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // Complex_Object
            // 
            this.Complex_Object.Location = new System.Drawing.Point(1, 109);
            this.Complex_Object.Name = "Complex_Object";
            this.Complex_Object.Size = new System.Drawing.Size(78, 47);
            this.Complex_Object.TabIndex = 20;
            this.Complex_Object.Text = "Complex Chair ";
            this.Complex_Object.UseVisualStyleBackColor = true;
            this.Complex_Object.Click += new System.EventHandler(this.Complex_Object_Click);
            // 
            // Print_Complex_objects
            // 
            this.Print_Complex_objects.Location = new System.Drawing.Point(2, 52);
            this.Print_Complex_objects.Name = "Print_Complex_objects";
            this.Print_Complex_objects.Size = new System.Drawing.Size(89, 51);
            this.Print_Complex_objects.TabIndex = 21;
            this.Print_Complex_objects.Text = "Print Complex Object";
            this.Print_Complex_objects.UseVisualStyleBackColor = true;
            this.Print_Complex_objects.Click += new System.EventHandler(this.Print_Complex_objects_Click);
            // 
            // complex_plmd
            // 
            this.complex_plmd.Location = new System.Drawing.Point(3, 1);
            this.complex_plmd.Name = "complex_plmd";
            this.complex_plmd.Size = new System.Drawing.Size(77, 45);
            this.complex_plmd.TabIndex = 22;
            this.complex_plmd.Text = "complex people ";
            this.complex_plmd.UseVisualStyleBackColor = true;
            this.complex_plmd.Click += new System.EventHandler(this.complex_plmd_Click);
            // 
            // complex_Table
            // 
            this.complex_Table.Location = new System.Drawing.Point(1, 254);
            this.complex_Table.Name = "complex_Table";
            this.complex_Table.Size = new System.Drawing.Size(91, 39);
            this.complex_Table.TabIndex = 23;
            this.complex_Table.Text = "complex_table";
            this.complex_Table.UseVisualStyleBackColor = true;
            this.complex_Table.Click += new System.EventHandler(this.complex_Table_Click);
            // 
            // savetable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 608);
            this.Controls.Add(this.complex_Table);
            this.Controls.Add(this.complex_plmd);
            this.Controls.Add(this.Print_Complex_objects);
            this.Controls.Add(this.Complex_Object);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ChatingScene);
            this.Controls.Add(this.PlSitOnSdCh);
            this.Controls.Add(this.PrintSideChair);
            this.Controls.Add(this.SaveChair);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PrintScene);
            this.Controls.Add(this.DeleteBotton);
            this.Controls.Add(this.selectink);
            this.Controls.Add(this.textBox1);
            this.Name = "savetable";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox selectink;
        private System.Windows.Forms.Button DeleteBotton;
        private System.Windows.Forms.Button PrintScene;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button SaveChair;
        private System.Windows.Forms.Button PrintSideChair;
        private System.Windows.Forms.Button PlSitOnSdCh;
        private System.Windows.Forms.Button ChatingScene;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button Complex_Object;
        private System.Windows.Forms.Button Print_Complex_objects;
        private System.Windows.Forms.Button complex_plmd;
        private System.Windows.Forms.Button complex_Table;
    }
}

