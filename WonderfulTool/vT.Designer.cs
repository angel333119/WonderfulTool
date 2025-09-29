namespace WonderfulTool
{
    partial class vT
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
            this.zoomLevel = new System.Windows.Forms.Label();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.Resolucao = new System.Windows.Forms.Label();
            this.paleta = new System.Windows.Forms.Label();
            this.Enderecotextura = new System.Windows.Forms.Label();
            this.Paletteminus = new System.Windows.Forms.Button();
            this.Paletteplus = new System.Windows.Forms.Button();
            this.palettenumber = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonAbrirArquivos = new System.Windows.Forms.Button();
            this.comboBoxImages = new System.Windows.Forms.ComboBox();
            this.comboBoxBinFiles = new System.Windows.Forms.ComboBox();
            this.pictureBoxDisplay = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // zoomLevel
            // 
            this.zoomLevel.AutoSize = true;
            this.zoomLevel.Location = new System.Drawing.Point(132, 350);
            this.zoomLevel.Name = "zoomLevel";
            this.zoomLevel.Size = new System.Drawing.Size(34, 13);
            this.zoomLevel.TabIndex = 40;
            this.zoomLevel.Text = "Zoom";
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(41, 359);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(75, 23);
            this.btnZoomOut.TabIndex = 39;
            this.btnZoomOut.Text = "Zoom -";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(41, 330);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(75, 23);
            this.btnZoomIn.TabIndex = 38;
            this.btnZoomIn.Text = "Zoom +";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            // 
            // Resolucao
            // 
            this.Resolucao.AutoSize = true;
            this.Resolucao.Location = new System.Drawing.Point(60, 197);
            this.Resolucao.Name = "Resolucao";
            this.Resolucao.Size = new System.Drawing.Size(60, 13);
            this.Resolucao.TabIndex = 37;
            this.Resolucao.Text = "Resolution:";
            // 
            // paleta
            // 
            this.paleta.AutoSize = true;
            this.paleta.Location = new System.Drawing.Point(60, 172);
            this.paleta.Name = "paleta";
            this.paleta.Size = new System.Drawing.Size(77, 13);
            this.paleta.TabIndex = 36;
            this.paleta.Text = "Palette adress:";
            // 
            // Enderecotextura
            // 
            this.Enderecotextura.AutoSize = true;
            this.Enderecotextura.Location = new System.Drawing.Point(60, 145);
            this.Enderecotextura.Name = "Enderecotextura";
            this.Enderecotextura.Size = new System.Drawing.Size(80, 13);
            this.Enderecotextura.TabIndex = 35;
            this.Enderecotextura.Text = "Texture adress:";
            // 
            // Paletteminus
            // 
            this.Paletteminus.Enabled = false;
            this.Paletteminus.Location = new System.Drawing.Point(41, 255);
            this.Paletteminus.Name = "Paletteminus";
            this.Paletteminus.Size = new System.Drawing.Size(75, 23);
            this.Paletteminus.TabIndex = 34;
            this.Paletteminus.Text = "Palette -";
            this.Paletteminus.UseVisualStyleBackColor = true;
            // 
            // Paletteplus
            // 
            this.Paletteplus.Enabled = false;
            this.Paletteplus.Location = new System.Drawing.Point(41, 226);
            this.Paletteplus.Name = "Paletteplus";
            this.Paletteplus.Size = new System.Drawing.Size(75, 23);
            this.Paletteplus.TabIndex = 33;
            this.Paletteplus.Text = "Palette +";
            this.Paletteplus.UseVisualStyleBackColor = true;
            // 
            // palettenumber
            // 
            this.palettenumber.AutoSize = true;
            this.palettenumber.Location = new System.Drawing.Point(122, 245);
            this.palettenumber.Name = "palettenumber";
            this.palettenumber.Size = new System.Drawing.Size(40, 13);
            this.palettenumber.TabIndex = 32;
            this.palettenumber.Text = "Palette";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Select a image";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Select a BIN file";
            // 
            // buttonAbrirArquivos
            // 
            this.buttonAbrirArquivos.Location = new System.Drawing.Point(101, 33);
            this.buttonAbrirArquivos.Name = "buttonAbrirArquivos";
            this.buttonAbrirArquivos.Size = new System.Drawing.Size(149, 23);
            this.buttonAbrirArquivos.TabIndex = 29;
            this.buttonAbrirArquivos.Text = "Open File";
            this.buttonAbrirArquivos.UseVisualStyleBackColor = true;
            this.buttonAbrirArquivos.Click += new System.EventHandler(this.buttonAbrirArquivos_Click);
            // 
            // comboBoxImages
            // 
            this.comboBoxImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxImages.Enabled = false;
            this.comboBoxImages.FormattingEnabled = true;
            this.comboBoxImages.Location = new System.Drawing.Point(125, 107);
            this.comboBoxImages.Name = "comboBoxImages";
            this.comboBoxImages.Size = new System.Drawing.Size(149, 21);
            this.comboBoxImages.TabIndex = 28;
            // 
            // comboBoxBinFiles
            // 
            this.comboBoxBinFiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBinFiles.Enabled = false;
            this.comboBoxBinFiles.FormattingEnabled = true;
            this.comboBoxBinFiles.Location = new System.Drawing.Point(125, 72);
            this.comboBoxBinFiles.Name = "comboBoxBinFiles";
            this.comboBoxBinFiles.Size = new System.Drawing.Size(149, 21);
            this.comboBoxBinFiles.TabIndex = 27;
            // 
            // pictureBoxDisplay
            // 
            this.pictureBoxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxDisplay.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxDisplay.Location = new System.Drawing.Point(284, 19);
            this.pictureBoxDisplay.Name = "pictureBoxDisplay";
            this.pictureBoxDisplay.Size = new System.Drawing.Size(603, 545);
            this.pictureBoxDisplay.TabIndex = 26;
            this.pictureBoxDisplay.TabStop = false;
            // 
            // vT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(899, 582);
            this.Controls.Add(this.zoomLevel);
            this.Controls.Add(this.btnZoomOut);
            this.Controls.Add(this.btnZoomIn);
            this.Controls.Add(this.Resolucao);
            this.Controls.Add(this.paleta);
            this.Controls.Add(this.Enderecotextura);
            this.Controls.Add(this.Paletteminus);
            this.Controls.Add(this.Paletteplus);
            this.Controls.Add(this.palettenumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAbrirArquivos);
            this.Controls.Add(this.comboBoxImages);
            this.Controls.Add(this.comboBoxBinFiles);
            this.Controls.Add(this.pictureBoxDisplay);
            this.Name = "vT";
            this.Text = "Visualizador de Texturas - By Angel333119";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label zoomLevel;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Label Resolucao;
        private System.Windows.Forms.Label paleta;
        private System.Windows.Forms.Label Enderecotextura;
        private System.Windows.Forms.Button Paletteminus;
        private System.Windows.Forms.Button Paletteplus;
        private System.Windows.Forms.Label palettenumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAbrirArquivos;
        private System.Windows.Forms.ComboBox comboBoxImages;
        private System.Windows.Forms.ComboBox comboBoxBinFiles;
        private System.Windows.Forms.PictureBox pictureBoxDisplay;
    }
}