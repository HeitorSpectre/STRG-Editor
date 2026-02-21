namespace STRGeditor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarComoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exportarParaTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarDeTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.fecharArquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSearch = new System.Windows.Forms.ToolStrip();
            this.toolStripLabelSearch = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxSearch = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonClearSearch = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPrevResult = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonNextResult = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelResults = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparatorSearch = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listBoxArquivos = new System.Windows.Forms.ListBox();
            this.listBoxLinguagens = new System.Windows.Forms.ListBox();
            this.panelDataGrid = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.text = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vScrollBarTexts = new System.Windows.Forms.VScrollBar();
            this.labelTextPosition = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.toolStripSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelDataGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem,
            this.sobreToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.salvarTodosToolStripMenuItem,
            this.salvarToolStripMenuItem,
            this.salvarComoToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exportarParaTXTToolStripMenuItem,
            this.exportarTodosToolStripMenuItem,
            this.importarDeTXTToolStripMenuItem,
            this.toolStripMenuItem3,
            this.fecharArquivoToolStripMenuItem,
            this.fecharTodosToolStripMenuItem});
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.arquivoToolStripMenuItem.Text = "File";
            this.arquivoToolStripMenuItem.Click += new System.EventHandler(this.arquivoToolStripMenuItem_Click);
            // 
            // abrirToolStripMenuItem
            // 
            this.abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            this.abrirToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.abrirToolStripMenuItem.Text = "Open File(s)...";
            this.abrirToolStripMenuItem.Click += new System.EventHandler(this.abrirToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.saveToolStripMenuItem.Text = "Save File";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // salvarTodosToolStripMenuItem
            // 
            this.salvarTodosToolStripMenuItem.Name = "salvarTodosToolStripMenuItem";
            this.salvarTodosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salvarTodosToolStripMenuItem.Text = "Save All Files";
            this.salvarTodosToolStripMenuItem.Click += new System.EventHandler(this.salvarTodosToolStripMenuItem_Click);
            // 
            // salvarToolStripMenuItem
            // 
            this.salvarToolStripMenuItem.Name = "salvarToolStripMenuItem";
            this.salvarToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salvarToolStripMenuItem.Text = "Save File As...";
            this.salvarToolStripMenuItem.Click += new System.EventHandler(this.salvarToolStripMenuItem_Click);
            // 
            // salvarComoToolStripMenuItem
            // 
            this.salvarComoToolStripMenuItem.Name = "salvarComoToolStripMenuItem";
            this.salvarComoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.salvarComoToolStripMenuItem.Text = "Save Compressed File...";
            this.salvarComoToolStripMenuItem.Click += new System.EventHandler(this.salvarComoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // exportarParaTXTToolStripMenuItem
            // 
            this.exportarParaTXTToolStripMenuItem.Name = "exportarParaTXTToolStripMenuItem";
            this.exportarParaTXTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportarParaTXTToolStripMenuItem.Text = "Export to TXT";
            this.exportarParaTXTToolStripMenuItem.Click += new System.EventHandler(this.exportarParaTXTToolStripMenuItem_Click);
            // 
            // exportarTodosToolStripMenuItem
            // 
            this.exportarTodosToolStripMenuItem.Name = "exportarTodosToolStripMenuItem";
            this.exportarTodosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exportarTodosToolStripMenuItem.Text = "Export All Files to TXT";
            this.exportarTodosToolStripMenuItem.Click += new System.EventHandler(this.exportarTodosToolStripMenuItem_Click);
            // 
            // importarDeTXTToolStripMenuItem
            // 
            this.importarDeTXTToolStripMenuItem.Name = "importarDeTXTToolStripMenuItem";
            this.importarDeTXTToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.importarDeTXTToolStripMenuItem.Text = "Import from TXT";
            this.importarDeTXTToolStripMenuItem.Click += new System.EventHandler(this.importarDeTXTToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(177, 6);
            // 
            // fecharArquivoToolStripMenuItem
            // 
            this.fecharArquivoToolStripMenuItem.Name = "fecharArquivoToolStripMenuItem";
            this.fecharArquivoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fecharArquivoToolStripMenuItem.Text = "Close File";
            this.fecharArquivoToolStripMenuItem.Click += new System.EventHandler(this.fecharArquivoToolStripMenuItem_Click);
            // 
            // fecharTodosToolStripMenuItem
            // 
            this.fecharTodosToolStripMenuItem.Name = "fecharTodosToolStripMenuItem";
            this.fecharTodosToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.fecharTodosToolStripMenuItem.Text = "Close All Files";
            this.fecharTodosToolStripMenuItem.Click += new System.EventHandler(this.fecharTodosToolStripMenuItem_Click);
            // 
            // sobreToolStripMenuItem
            // 
            this.sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            this.sobreToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.sobreToolStripMenuItem.Text = "About";
            this.sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // toolStripSearch
            // 
            this.toolStripSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelSearch,
            this.toolStripTextBoxSearch,
            this.toolStripButtonSearch,
            this.toolStripButtonClearSearch,
            this.toolStripButtonPrevResult,
            this.toolStripButtonNextResult,
            this.toolStripLabelResults,
            this.toolStripSeparatorSearch});
            this.toolStripSearch.Location = new System.Drawing.Point(0, 24);
            this.toolStripSearch.Name = "toolStripSearch";
            this.toolStripSearch.Size = new System.Drawing.Size(984, 25);
            this.toolStripSearch.TabIndex = 3;
            this.toolStripSearch.Text = "toolStrip1";
            // 
            // toolStripLabelSearch
            // 
            this.toolStripLabelSearch.Name = "toolStripLabelSearch";
            this.toolStripLabelSearch.Size = new System.Drawing.Size(68, 22);
            this.toolStripLabelSearch.Text = "Search:";
            // 
            // toolStripTextBoxSearch
            // 
            this.toolStripTextBoxSearch.Name = "toolStripTextBoxSearch";
            this.toolStripTextBoxSearch.Size = new System.Drawing.Size(200, 25);
            this.toolStripTextBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBoxSearch_KeyDown);
            // 
            // toolStripButtonSearch
            // 
            this.toolStripButtonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSearch.Name = "toolStripButtonSearch";
            this.toolStripButtonSearch.Size = new System.Drawing.Size(70, 22);
            this.toolStripButtonSearch.Text = "Search";
            this.toolStripButtonSearch.Click += new System.EventHandler(this.toolStripButtonSearch_Click);
            // 
            // toolStripButtonClearSearch
            // 
            this.toolStripButtonClearSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonClearSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonClearSearch.Name = "toolStripButtonClearSearch";
            this.toolStripButtonClearSearch.Size = new System.Drawing.Size(47, 22);
            this.toolStripButtonClearSearch.Text = "Clear";
            this.toolStripButtonClearSearch.Click += new System.EventHandler(this.toolStripButtonClearSearch_Click);
            // 
            // toolStripButtonPrevResult
            // 
            this.toolStripButtonPrevResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonPrevResult.Enabled = false;
            this.toolStripButtonPrevResult.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPrevResult.Name = "toolStripButtonPrevResult";
            this.toolStripButtonPrevResult.Size = new System.Drawing.Size(62, 22);
            this.toolStripButtonPrevResult.Text = "< Previous";
            this.toolStripButtonPrevResult.Click += new System.EventHandler(this.toolStripButtonPrevResult_Click);
            // 
            // toolStripButtonNextResult
            // 
            this.toolStripButtonNextResult.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonNextResult.Enabled = false;
            this.toolStripButtonNextResult.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonNextResult.Name = "toolStripButtonNextResult";
            this.toolStripButtonNextResult.Size = new System.Drawing.Size(59, 22);
            this.toolStripButtonNextResult.Text = "Next >";
            this.toolStripButtonNextResult.Click += new System.EventHandler(this.toolStripButtonNextResult_Click);
            // 
            // toolStripLabelResults
            // 
            this.toolStripLabelResults.Name = "toolStripLabelResults";
            this.toolStripLabelResults.Size = new System.Drawing.Size(60, 22);
            this.toolStripLabelResults.Text = "0 results";
            // 
            // toolStripSeparatorSearch
            // 
            this.toolStripSeparatorSearch.Name = "toolStripSeparatorSearch";
            this.toolStripSeparatorSearch.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 49);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelDataGrid);
            this.splitContainer1.Size = new System.Drawing.Size(984, 492);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 4;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listBoxArquivos);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.listBoxLinguagens);
            this.splitContainer2.Size = new System.Drawing.Size(200, 492);
            this.splitContainer2.SplitterDistance = 250;
            this.splitContainer2.TabIndex = 0;
            // 
            // listBoxArquivos
            // 
            this.listBoxArquivos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxArquivos.FormattingEnabled = true;
            this.listBoxArquivos.HorizontalScrollbar = true;
            this.listBoxArquivos.Location = new System.Drawing.Point(0, 0);
            this.listBoxArquivos.Name = "listBoxArquivos";
            this.listBoxArquivos.Size = new System.Drawing.Size(200, 250);
            this.listBoxArquivos.TabIndex = 0;
            this.listBoxArquivos.SelectedIndexChanged += new System.EventHandler(this.listBoxArquivos_SelectedIndexChanged);
            // 
            // listBoxLinguagens
            // 
            this.listBoxLinguagens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLinguagens.FormattingEnabled = true;
            this.listBoxLinguagens.Location = new System.Drawing.Point(0, 0);
            this.listBoxLinguagens.Name = "listBoxLinguagens";
            this.listBoxLinguagens.Size = new System.Drawing.Size(200, 238);
            this.listBoxLinguagens.TabIndex = 0;
            this.listBoxLinguagens.SelectedIndexChanged += new System.EventHandler(this.listBoxLinguagens_SelectedIndexChanged);
            // 
            // panelDataGrid
            // 
            this.panelDataGrid.Controls.Add(this.dataGridView1);
            this.panelDataGrid.Controls.Add(this.vScrollBarTexts);
            this.panelDataGrid.Controls.Add(this.labelTextPosition);
            this.panelDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDataGrid.Location = new System.Drawing.Point(0, 0);
            this.panelDataGrid.Name = "panelDataGrid";
            this.panelDataGrid.Size = new System.Drawing.Size(780, 492);
            this.panelDataGrid.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.text});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(763, 492);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // text
            // 
            this.text.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.text.HeaderText = "Text";
            this.text.Name = "text";
            // 
            // vScrollBarTexts
            // 
            this.vScrollBarTexts.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBarTexts.Location = new System.Drawing.Point(763, 0);
            this.vScrollBarTexts.Name = "vScrollBarTexts";
            this.vScrollBarTexts.Size = new System.Drawing.Size(17, 492);
            this.vScrollBarTexts.TabIndex = 1;
            this.vScrollBarTexts.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBarTexts_Scroll);
            this.vScrollBarTexts.ValueChanged += new System.EventHandler(this.vScrollBarTexts_ValueChanged);
            // 
            // labelTextPosition
            // 
            this.labelTextPosition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTextPosition.BackColor = System.Drawing.SystemColors.Control;
            this.labelTextPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelTextPosition.Location = new System.Drawing.Point(3, 470);
            this.labelTextPosition.Name = "labelTextPosition";
            this.labelTextPosition.Size = new System.Drawing.Size(120, 20);
            this.labelTextPosition.TabIndex = 2;
            this.labelTextPosition.Text = "Text 1 of 100";
            this.labelTextPosition.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 541);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(984, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 563);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStripSearch);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "STRG Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStripSearch.ResumeLayout(false);
            this.toolStripSearch.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelDataGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarComoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exportarParaTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportarTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importarDeTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem fecharArquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fecharTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStripSearch;
        private System.Windows.Forms.ToolStripLabel toolStripLabelSearch;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonClearSearch;
        private System.Windows.Forms.ToolStripButton toolStripButtonPrevResult;
        private System.Windows.Forms.ToolStripButton toolStripButtonNextResult;
        private System.Windows.Forms.ToolStripLabel toolStripLabelResults;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorSearch;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox listBoxArquivos;
        private System.Windows.Forms.ListBox listBoxLinguagens;
        private System.Windows.Forms.Panel panelDataGrid;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn text;
        private System.Windows.Forms.VScrollBar vScrollBarTexts;
        private System.Windows.Forms.Label labelTextPosition;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}