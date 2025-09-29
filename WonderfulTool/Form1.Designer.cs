namespace WonderfulTool
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.optimizedCLZCompact = new System.Windows.Forms.Button();
            this.CLZExtract = new System.Windows.Forms.Button();
            this.CLZCompact = new System.Windows.Forms.Button();
            this.TextureViewer = new System.Windows.Forms.Button();
            this.texEditor = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(289, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Extrair";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(289, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Inserir";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(301, 81);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DAT - AFS";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.optimizedCLZCompact);
            this.groupBox2.Controls.Add(this.CLZExtract);
            this.groupBox2.Controls.Add(this.CLZCompact);
            this.groupBox2.Location = new System.Drawing.Point(12, 99);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(301, 107);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CLZ";
            // 
            // optimizedCLZCompact
            // 
            this.optimizedCLZCompact.Location = new System.Drawing.Point(6, 77);
            this.optimizedCLZCompact.Name = "optimizedCLZCompact";
            this.optimizedCLZCompact.Size = new System.Drawing.Size(289, 23);
            this.optimizedCLZCompact.TabIndex = 2;
            this.optimizedCLZCompact.Text = "Compactar Otimizado";
            this.optimizedCLZCompact.UseVisualStyleBackColor = true;
            this.optimizedCLZCompact.Click += new System.EventHandler(this.optimizedCLZCompact_Click);
            // 
            // CLZExtract
            // 
            this.CLZExtract.Location = new System.Drawing.Point(6, 19);
            this.CLZExtract.Name = "CLZExtract";
            this.CLZExtract.Size = new System.Drawing.Size(289, 23);
            this.CLZExtract.TabIndex = 0;
            this.CLZExtract.Text = "Extrair";
            this.CLZExtract.UseVisualStyleBackColor = true;
            this.CLZExtract.Click += new System.EventHandler(this.CLZExtract_Click);
            // 
            // CLZCompact
            // 
            this.CLZCompact.Location = new System.Drawing.Point(6, 48);
            this.CLZCompact.Name = "CLZCompact";
            this.CLZCompact.Size = new System.Drawing.Size(289, 23);
            this.CLZCompact.TabIndex = 1;
            this.CLZCompact.Text = "Compactar";
            this.CLZCompact.UseVisualStyleBackColor = true;
            this.CLZCompact.Click += new System.EventHandler(this.CLZCompact_Click);
            // 
            // TextureViewer
            // 
            this.TextureViewer.Location = new System.Drawing.Point(165, 214);
            this.TextureViewer.Name = "TextureViewer";
            this.TextureViewer.Size = new System.Drawing.Size(142, 23);
            this.TextureViewer.TabIndex = 4;
            this.TextureViewer.Text = "Visualizador de Texturas";
            this.TextureViewer.UseVisualStyleBackColor = true;
            this.TextureViewer.Click += new System.EventHandler(this.TextureViewer_Click);
            // 
            // texEditor
            // 
            this.texEditor.Location = new System.Drawing.Point(18, 214);
            this.texEditor.Name = "texEditor";
            this.texEditor.Size = new System.Drawing.Size(142, 23);
            this.texEditor.TabIndex = 5;
            this.texEditor.Text = "Editor de Texto";
            this.texEditor.UseVisualStyleBackColor = true;
            this.texEditor.Click += new System.EventHandler(this.texEditor_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 249);
            this.Controls.Add(this.texEditor);
            this.Controls.Add(this.TextureViewer);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "by Angel333119";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button CLZExtract;
        private System.Windows.Forms.Button CLZCompact;
        private System.Windows.Forms.Button optimizedCLZCompact;
        private System.Windows.Forms.Button TextureViewer;
        private System.Windows.Forms.Button texEditor;
    }
}

