namespace WonderfulTool
{
    partial class mesEditor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairParaTXT = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairVáriosArquivosComOMapaAtual = new System.Windows.Forms.ToolStripMenuItem();
            this.inserirVáriosArquivosComOMapaAtual = new System.Windows.Forms.ToolStripMenuItem();
            this.tXTParaMESToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertNGCWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.insertPS2WonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.insertAnotherWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.extrairParaTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAnotherWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.extractWonderfulLifeJAP = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAnotherWonderfulLifeJAP = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridText = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OriginalText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.edited = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBoxCodification = new System.Windows.Forms.ComboBox();
            this.comboBoxlimit = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.extractMESWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.extractMESAnotherWonderfulLifeUSA = new System.Windows.Forms.ToolStripMenuItem();
            this.txtEditorDetalhe = new System.Windows.Forms.TextBox();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.panelPreviewContainer = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridText)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelPreviewContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.editMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1202, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenu,
            this.saveMenu,
            this.saveAsMenu});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(61, 20);
            this.fileMenu.Text = "Arquivo";
            // 
            // openMenu
            // 
            this.openMenu.Name = "openMenu";
            this.openMenu.Size = new System.Drawing.Size(148, 22);
            this.openMenu.Text = "Abrir";
            this.openMenu.Click += new System.EventHandler(this.openMenu_Click);
            // 
            // saveMenu
            // 
            this.saveMenu.Name = "saveMenu";
            this.saveMenu.Size = new System.Drawing.Size(148, 22);
            this.saveMenu.Text = "Salvar";
            this.saveMenu.Click += new System.EventHandler(this.saveMenu_Click);
            // 
            // saveAsMenu
            // 
            this.saveAsMenu.Name = "saveAsMenu";
            this.saveAsMenu.Size = new System.Drawing.Size(148, 22);
            this.saveAsMenu.Text = "Salvar como...";
            this.saveAsMenu.Click += new System.EventHandler(this.saveAsMenu_Click);
            // 
            // editMenu
            // 
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extrairParaTXT,
            this.extrairVáriosArquivosComOMapaAtual,
            this.inserirVáriosArquivosComOMapaAtual,
            this.tXTParaMESToolStripMenuItem,
            this.extrairParaTXTToolStripMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.Size = new System.Drawing.Size(71, 20);
            this.editMenu.Text = "Converter";
            // 
            // extrairParaTXT
            // 
            this.extrairParaTXT.Name = "extrairParaTXT";
            this.extrairParaTXT.Size = new System.Drawing.Size(275, 22);
            this.extrairParaTXT.Text = "Arquivo atual para TXT";
            this.extrairParaTXT.Click += new System.EventHandler(this.extrairParaTXT_Click);
            // 
            // extrairVáriosArquivosComOMapaAtual
            // 
            this.extrairVáriosArquivosComOMapaAtual.Name = "extrairVáriosArquivosComOMapaAtual";
            this.extrairVáriosArquivosComOMapaAtual.Size = new System.Drawing.Size(275, 22);
            this.extrairVáriosArquivosComOMapaAtual.Text = "Vários MES para TXT com a fonte atual";
            this.extrairVáriosArquivosComOMapaAtual.Click += new System.EventHandler(this.extrairVáriosArquivosComOMapaAtual_Click);
            // 
            // inserirVáriosArquivosComOMapaAtual
            // 
            this.inserirVáriosArquivosComOMapaAtual.Name = "inserirVáriosArquivosComOMapaAtual";
            this.inserirVáriosArquivosComOMapaAtual.Size = new System.Drawing.Size(275, 22);
            this.inserirVáriosArquivosComOMapaAtual.Text = "Vários TXT para MES com a fonte atual";
            this.inserirVáriosArquivosComOMapaAtual.Click += new System.EventHandler(this.inserirVáriosArquivosComOMapaAtual_Click);
            // 
            // tXTParaMESToolStripMenuItem
            // 
            this.tXTParaMESToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertNGCWonderfulLifeUSA,
            this.insertPS2WonderfulLifeUSA,
            this.insertAnotherWonderfulLifeUSA});
            this.tXTParaMESToolStripMenuItem.Name = "tXTParaMESToolStripMenuItem";
            this.tXTParaMESToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.tXTParaMESToolStripMenuItem.Text = "TXT para MES";
            // 
            // insertNGCWonderfulLifeUSA
            // 
            this.insertNGCWonderfulLifeUSA.Name = "insertNGCWonderfulLifeUSA";
            this.insertNGCWonderfulLifeUSA.Size = new System.Drawing.Size(267, 22);
            this.insertNGCWonderfulLifeUSA.Text = "NGC - A Wonderful Life - USA";
            this.insertNGCWonderfulLifeUSA.Click += new System.EventHandler(this.insertNGCWonderfulLifeUSA_Click);
            // 
            // insertPS2WonderfulLifeUSA
            // 
            this.insertPS2WonderfulLifeUSA.Name = "insertPS2WonderfulLifeUSA";
            this.insertPS2WonderfulLifeUSA.Size = new System.Drawing.Size(267, 22);
            this.insertPS2WonderfulLifeUSA.Text = "PS2 - A Wonderful Life - USA";
            this.insertPS2WonderfulLifeUSA.Click += new System.EventHandler(this.insertPS2WonderfulLifeUSA_Click);
            // 
            // insertAnotherWonderfulLifeUSA
            // 
            this.insertAnotherWonderfulLifeUSA.Name = "insertAnotherWonderfulLifeUSA";
            this.insertAnotherWonderfulLifeUSA.Size = new System.Drawing.Size(267, 22);
            this.insertAnotherWonderfulLifeUSA.Text = "NGC - Another Wonderful Life - USA";
            this.insertAnotherWonderfulLifeUSA.Click += new System.EventHandler(this.insertAnotherWonderfulLifeUSA_Click);
            // 
            // extrairParaTXTToolStripMenuItem
            // 
            this.extrairParaTXTToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractWonderfulLifeUSA,
            this.extractAnotherWonderfulLifeUSA,
            this.extractWonderfulLifeJAP,
            this.extractAnotherWonderfulLifeJAP});
            this.extrairParaTXTToolStripMenuItem.Name = "extrairParaTXTToolStripMenuItem";
            this.extrairParaTXTToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            this.extrairParaTXTToolStripMenuItem.Text = "MES para TXT";
            // 
            // extractWonderfulLifeUSA
            // 
            this.extractWonderfulLifeUSA.Name = "extractWonderfulLifeUSA";
            this.extractWonderfulLifeUSA.Size = new System.Drawing.Size(231, 22);
            this.extractWonderfulLifeUSA.Text = "A Wonderful Life - USA";
            this.extractWonderfulLifeUSA.Click += new System.EventHandler(this.extractWonderfulLifeUSA_Click);
            // 
            // extractAnotherWonderfulLifeUSA
            // 
            this.extractAnotherWonderfulLifeUSA.Name = "extractAnotherWonderfulLifeUSA";
            this.extractAnotherWonderfulLifeUSA.Size = new System.Drawing.Size(231, 22);
            this.extractAnotherWonderfulLifeUSA.Text = "Another Wonderful Life - USA";
            this.extractAnotherWonderfulLifeUSA.Click += new System.EventHandler(this.extractAnotherWonderfulLifeUSA_Click);
            // 
            // extractWonderfulLifeJAP
            // 
            this.extractWonderfulLifeJAP.Name = "extractWonderfulLifeJAP";
            this.extractWonderfulLifeJAP.Size = new System.Drawing.Size(231, 22);
            this.extractWonderfulLifeJAP.Text = "A Wonderful Life - JAP";
            this.extractWonderfulLifeJAP.Click += new System.EventHandler(this.extractWonderfulLifeJAP_Click);
            // 
            // extractAnotherWonderfulLifeJAP
            // 
            this.extractAnotherWonderfulLifeJAP.Name = "extractAnotherWonderfulLifeJAP";
            this.extractAnotherWonderfulLifeJAP.Size = new System.Drawing.Size(231, 22);
            this.extractAnotherWonderfulLifeJAP.Text = "Another Wonderful Life - JAP";
            this.extractAnotherWonderfulLifeJAP.Click += new System.EventHandler(this.extractAnotherWonderfulLifeJAP_Click);
            // 
            // dataGridText
            // 
            this.dataGridText.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridText.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.OriginalText,
            this.edited,
            this.status});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridText.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridText.Location = new System.Drawing.Point(0, 52);
            this.dataGridText.Name = "dataGridText";
            this.dataGridText.Size = new System.Drawing.Size(1202, 345);
            this.dataGridText.TabIndex = 1;
            // 
            // ID
            // 
            this.ID.HeaderText = "Message";
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            // 
            // OriginalText
            // 
            this.OriginalText.HeaderText = "Texto Original";
            this.OriginalText.Name = "OriginalText";
            this.OriginalText.ReadOnly = true;
            // 
            // edited
            // 
            this.edited.HeaderText = "Editado";
            this.edited.Name = "edited";
            // 
            // status
            // 
            this.status.HeaderText = "Estado";
            this.status.Name = "status";
            // 
            // comboBoxCodification
            // 
            this.comboBoxCodification.FormattingEnabled = true;
            this.comboBoxCodification.Location = new System.Drawing.Point(474, 25);
            this.comboBoxCodification.Name = "comboBoxCodification";
            this.comboBoxCodification.Size = new System.Drawing.Size(363, 21);
            this.comboBoxCodification.TabIndex = 2;
            this.comboBoxCodification.SelectedIndexChanged += new System.EventHandler(this.comboBoxCodification_SelectedIndexChanged);
            // 
            // comboBoxlimit
            // 
            this.comboBoxlimit.FormattingEnabled = true;
            this.comboBoxlimit.Items.AddRange(new object[] {
            "21",
            "42"});
            this.comboBoxlimit.Location = new System.Drawing.Point(1147, 27);
            this.comboBoxlimit.Name = "comboBoxlimit";
            this.comboBoxlimit.Size = new System.Drawing.Size(43, 21);
            this.comboBoxlimit.TabIndex = 3;
            this.comboBoxlimit.SelectedIndexChanged += new System.EventHandler(this.comboBoxlimit_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(987, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Máximo de caracteres por linha";
            // 
            // extractMESWonderfulLifeUSA
            // 
            this.extractMESWonderfulLifeUSA.Name = "extractMESWonderfulLifeUSA";
            this.extractMESWonderfulLifeUSA.Size = new System.Drawing.Size(231, 22);
            this.extractMESWonderfulLifeUSA.Text = "A Wonderful Life - USA";
            // 
            // extractMESAnotherWonderfulLifeUSA
            // 
            this.extractMESAnotherWonderfulLifeUSA.Name = "extractMESAnotherWonderfulLifeUSA";
            this.extractMESAnotherWonderfulLifeUSA.Size = new System.Drawing.Size(231, 22);
            this.extractMESAnotherWonderfulLifeUSA.Text = "Another Wonderful Life - USA";
            // 
            // txtEditorDetalhe
            // 
            this.txtEditorDetalhe.Location = new System.Drawing.Point(0, 403);
            this.txtEditorDetalhe.Multiline = true;
            this.txtEditorDetalhe.Name = "txtEditorDetalhe";
            this.txtEditorDetalhe.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtEditorDetalhe.Size = new System.Drawing.Size(492, 151);
            this.txtEditorDetalhe.TabIndex = 5;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxPreview.Dock = System.Windows.Forms.DockStyle.None;
            this.pictureBoxPreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(704, 151);
            this.pictureBoxPreview.TabIndex = 6;
            this.pictureBoxPreview.TabStop = false;
            // 
            // panelPreviewContainer
            // 
            this.panelPreviewContainer.AutoScroll = true;
            this.panelPreviewContainer.Controls.Add(this.pictureBoxPreview);
            this.panelPreviewContainer.Location = new System.Drawing.Point(498, 403);
            this.panelPreviewContainer.Name = "panelPreviewContainer";
            this.panelPreviewContainer.Size = new System.Drawing.Size(704, 151);
            this.panelPreviewContainer.TabIndex = 7;
            // 
            // mesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 566);
            this.Controls.Add(this.panelPreviewContainer);
            this.Controls.Add(this.txtEditorDetalhe);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxlimit);
            this.Controls.Add(this.comboBoxCodification);
            this.Controls.Add(this.dataGridText);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "mesEditor";
            this.Text = "Mes Editor By Angel333119";
            this.Load += new System.EventHandler(this.mesEditor_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridText)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelPreviewContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem openMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenu;
        private System.Windows.Forms.ToolStripMenuItem saveAsMenu;
        private System.Windows.Forms.DataGridView dataGridText;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ComboBox comboBoxCodification;
        private System.Windows.Forms.ComboBox comboBoxlimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn OriginalText;
        private System.Windows.Forms.DataGridViewTextBoxColumn edited;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem extrairParaTXT;
        private System.Windows.Forms.ToolStripMenuItem extractMESWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem extractMESAnotherWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem extrairParaTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem extractAnotherWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem extractWonderfulLifeJAP;
        private System.Windows.Forms.ToolStripMenuItem extractAnotherWonderfulLifeJAP;
        private System.Windows.Forms.ToolStripMenuItem tXTParaMESToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertNGCWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem insertPS2WonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem insertAnotherWonderfulLifeUSA;
        private System.Windows.Forms.ToolStripMenuItem extrairVáriosArquivosComOMapaAtual;
        private System.Windows.Forms.ToolStripMenuItem inserirVáriosArquivosComOMapaAtual;
        private System.Windows.Forms.TextBox txtEditorDetalhe;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Panel panelPreviewContainer;
    }
}

