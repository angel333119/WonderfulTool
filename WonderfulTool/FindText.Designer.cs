namespace WonderfulTool
{
    partial class FindText
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
            this.btnFindNext1 = new System.Windows.Forms.Button();
            this.txtFind1 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFind = new System.Windows.Forms.TabPage();
            this.chkWholeWord1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkWrapAround1 = new System.Windows.Forms.CheckBox();
            this.chkMatchCase1 = new System.Windows.Forms.CheckBox();
            this.tabReplace = new System.Windows.Forms.TabPage();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.btnReplace = new System.Windows.Forms.Button();
            this.chkWholeWord2 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkWrapAround2 = new System.Windows.Forms.CheckBox();
            this.chkMatchCase2 = new System.Windows.Forms.CheckBox();
            this.txtFind2 = new System.Windows.Forms.TextBox();
            this.btnFindNext2 = new System.Windows.Forms.Button();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.tabFind.SuspendLayout();
            this.tabReplace.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFindNext1
            // 
            this.btnFindNext1.Location = new System.Drawing.Point(377, 30);
            this.btnFindNext1.Name = "btnFindNext1";
            this.btnFindNext1.Size = new System.Drawing.Size(99, 25);
            this.btnFindNext1.TabIndex = 0;
            this.btnFindNext1.Text = "Procurar próximo";
            this.btnFindNext1.UseVisualStyleBackColor = true;
            this.btnFindNext1.Click += new System.EventHandler(this.btnFindNext1_Click);
            // 
            // txtFind1
            // 
            this.txtFind1.Location = new System.Drawing.Point(68, 33);
            this.txtFind1.Name = "txtFind1";
            this.txtFind1.Size = new System.Drawing.Size(303, 20);
            this.txtFind1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabFind);
            this.tabControl1.Controls.Add(this.tabReplace);
            this.tabControl1.Location = new System.Drawing.Point(-2, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(490, 252);
            this.tabControl1.TabIndex = 2;
            // 
            // tabFind
            // 
            this.tabFind.Controls.Add(this.chkWholeWord1);
            this.tabFind.Controls.Add(this.label1);
            this.tabFind.Controls.Add(this.chkWrapAround1);
            this.tabFind.Controls.Add(this.chkMatchCase1);
            this.tabFind.Controls.Add(this.txtFind1);
            this.tabFind.Controls.Add(this.btnFindNext1);
            this.tabFind.Location = new System.Drawing.Point(4, 22);
            this.tabFind.Name = "tabFind";
            this.tabFind.Padding = new System.Windows.Forms.Padding(3);
            this.tabFind.Size = new System.Drawing.Size(482, 226);
            this.tabFind.TabIndex = 0;
            this.tabFind.Text = "Procurar";
            this.tabFind.UseVisualStyleBackColor = true;
            // 
            // chkWholeWord1
            // 
            this.chkWholeWord1.AutoSize = true;
            this.chkWholeWord1.Location = new System.Drawing.Point(10, 125);
            this.chkWholeWord1.Name = "chkWholeWord1";
            this.chkWholeWord1.Size = new System.Drawing.Size(135, 17);
            this.chkWholeWord1.TabIndex = 7;
            this.chkWholeWord1.Text = "Coincidir palavra inteira";
            this.chkWholeWord1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Localizar:";
            // 
            // chkWrapAround1
            // 
            this.chkWrapAround1.AutoSize = true;
            this.chkWrapAround1.Location = new System.Drawing.Point(10, 175);
            this.chkWrapAround1.Name = "chkWrapAround1";
            this.chkWrapAround1.Size = new System.Drawing.Size(106, 17);
            this.chkWrapAround1.TabIndex = 5;
            this.chkWrapAround1.Text = "Pesquisa circular";
            this.chkWrapAround1.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase1
            // 
            this.chkMatchCase1.AutoSize = true;
            this.chkMatchCase1.Location = new System.Drawing.Point(10, 150);
            this.chkMatchCase1.Name = "chkMatchCase1";
            this.chkMatchCase1.Size = new System.Drawing.Size(189, 17);
            this.chkMatchCase1.TabIndex = 4;
            this.chkMatchCase1.Text = "Diferenciar maiúsculas/minúsculas";
            this.chkMatchCase1.UseVisualStyleBackColor = true;
            // 
            // tabReplace
            // 
            this.tabReplace.Controls.Add(this.btnReplaceAll);
            this.tabReplace.Controls.Add(this.btnReplace);
            this.tabReplace.Controls.Add(this.chkWholeWord2);
            this.tabReplace.Controls.Add(this.label3);
            this.tabReplace.Controls.Add(this.label2);
            this.tabReplace.Controls.Add(this.chkWrapAround2);
            this.tabReplace.Controls.Add(this.chkMatchCase2);
            this.tabReplace.Controls.Add(this.txtFind2);
            this.tabReplace.Controls.Add(this.btnFindNext2);
            this.tabReplace.Controls.Add(this.txtReplace);
            this.tabReplace.Location = new System.Drawing.Point(4, 22);
            this.tabReplace.Name = "tabReplace";
            this.tabReplace.Padding = new System.Windows.Forms.Padding(3);
            this.tabReplace.Size = new System.Drawing.Size(482, 226);
            this.tabReplace.TabIndex = 1;
            this.tabReplace.Text = "Substituir";
            this.tabReplace.UseVisualStyleBackColor = true;
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(377, 90);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(98, 25);
            this.btnReplaceAll.TabIndex = 9;
            this.btnReplaceAll.Text = "Substituir tudo";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(377, 60);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(98, 25);
            this.btnReplace.TabIndex = 8;
            this.btnReplace.Text = "Substituir";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // chkWholeWord2
            // 
            this.chkWholeWord2.AutoSize = true;
            this.chkWholeWord2.Location = new System.Drawing.Point(10, 125);
            this.chkWholeWord2.Name = "chkWholeWord2";
            this.chkWholeWord2.Size = new System.Drawing.Size(135, 17);
            this.chkWholeWord2.TabIndex = 7;
            this.chkWholeWord2.Text = "Coincidir palavra inteira";
            this.chkWholeWord2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Substituir:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Localizar:";
            // 
            // chkWrapAround2
            // 
            this.chkWrapAround2.AutoSize = true;
            this.chkWrapAround2.Location = new System.Drawing.Point(10, 175);
            this.chkWrapAround2.Name = "chkWrapAround2";
            this.chkWrapAround2.Size = new System.Drawing.Size(106, 17);
            this.chkWrapAround2.TabIndex = 4;
            this.chkWrapAround2.Text = "Pesquisa circular";
            this.chkWrapAround2.UseVisualStyleBackColor = true;
            // 
            // chkMatchCase2
            // 
            this.chkMatchCase2.AutoSize = true;
            this.chkMatchCase2.Location = new System.Drawing.Point(10, 150);
            this.chkMatchCase2.Name = "chkMatchCase2";
            this.chkMatchCase2.Size = new System.Drawing.Size(189, 17);
            this.chkMatchCase2.TabIndex = 3;
            this.chkMatchCase2.Text = "Diferenciar maiúsculas/minúsculas";
            this.chkMatchCase2.UseVisualStyleBackColor = true;
            // 
            // txtFind2
            // 
            this.txtFind2.Location = new System.Drawing.Point(68, 33);
            this.txtFind2.Name = "txtFind2";
            this.txtFind2.Size = new System.Drawing.Size(303, 20);
            this.txtFind2.TabIndex = 2;
            // 
            // btnFindNext2
            // 
            this.btnFindNext2.Location = new System.Drawing.Point(377, 30);
            this.btnFindNext2.Name = "btnFindNext2";
            this.btnFindNext2.Size = new System.Drawing.Size(98, 25);
            this.btnFindNext2.TabIndex = 1;
            this.btnFindNext2.Text = "Procurar próximo";
            this.btnFindNext2.UseVisualStyleBackColor = true;
            this.btnFindNext2.Click += new System.EventHandler(this.btnFindNext2_Click);
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(68, 63);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(303, 20);
            this.txtReplace.TabIndex = 0;
            // 
            // FindText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 255);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.Name = "FindText";
            this.Text = "Localizar";
            this.tabControl1.ResumeLayout(false);
            this.tabFind.ResumeLayout(false);
            this.tabFind.PerformLayout();
            this.tabReplace.ResumeLayout(false);
            this.tabReplace.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFindNext1;
        private System.Windows.Forms.TextBox txtFind1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabFind;
        private System.Windows.Forms.TabPage tabReplace;
        private System.Windows.Forms.TextBox txtFind2;
        private System.Windows.Forms.Button btnFindNext2;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.CheckBox chkWrapAround1;
        private System.Windows.Forms.CheckBox chkMatchCase1;
        private System.Windows.Forms.CheckBox chkWrapAround2;
        private System.Windows.Forms.CheckBox chkMatchCase2;
        private System.Windows.Forms.CheckBox chkWholeWord1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkWholeWord2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnReplaceAll;
    }
}