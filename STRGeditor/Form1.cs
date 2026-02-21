using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace STRGeditor
{
    public partial class Form1 : Form
    {
        // Estrutura para armazenar múltiplos arquivos STRG
        class STRGFileData
        {
            public string FilePath { get; set; }
            public uint Magic { get; set; }
            public uint Version { get; set; }
            public uint Langnumb { get; set; }
            public uint Textnumb { get; set; }
            public uint Nametable { get; set; }
            public uint Nts { get; set; }
            public byte[] Ntc { get; set; }
            public List<string> LanguageCodes { get; set; }
            public string[][] Texts { get; set; }
            public string FileName { get { return Path.GetFileName(FilePath); } }
            public bool IsModified { get; set; } = false;
            public int SelectedLangIndex { get; set; } = 0;
            public bool IsSingleSharedText { get; set; } = false;

            public STRGFileData()
            {
                LanguageCodes = new List<string>();
            }
        }

        // Classe para armazenar resultados de pesquisa
        class SearchResult
        {
            public int FileIndex { get; set; }
            public int LanguageIndex { get; set; }
            public int TextIndex { get; set; }
            public string FileName { get; set; }
            public string LanguageCode { get; set; }
            public string FoundText { get; set; }
            public string FullText { get; set; }
            public int MatchPosition { get; set; }
        }

        // Lista de arquivos carregados
        private List<STRGFileData> loadedFiles = new List<STRGFileData>();
        private int currentFileIndex = -1;
        private int currentLangIndex = -1;
        private bool isChangingFile = false;

        // Variáveis para busca
        private List<SearchResult> allSearchResults = new List<SearchResult>();
        private int currentResultIndex = -1;
        private bool searchActive = false;
        private string lastSearchText = "";
        private bool isNavigatingSearch = false;

        uint bigendian(uint le)
        {
            uint pr = le >> 24;
            uint se = le >> 8 & 0x00FF00;
            uint te = le << 24;
            uint qu = le << 8 & 0x00FF0000;
            return pr | se | te | qu;
        }

        public Form1()
        {
            InitializeComponent();
            ConfigurarDataGridView();
            listBoxArquivos.SelectionMode = SelectionMode.One;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
        }

        private void ConfigurarDataGridView()
        {
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.CellEndEdit += dataGridView1_CellEndEdit;
            dataGridView1.CellBeginEdit += dataGridView1_CellBeginEdit;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            // Configurar para não entrar em modo de edição automaticamente
            dataGridView1.EditMode = DataGridViewEditMode.EditOnEnter;
        }

        // Métodos auxiliares para conversão de quebras de linha
        private string ConvertNewlinesForDisplay(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Primeiro, proteger as sequências de escape
            text = text.Replace(@"\\", "{{ESCAPED_BACKSLASH}}");
            text = text.Replace(@"\n", "{{ESCAPED_NEWLINE}}");

            // Agora, substituir as quebras de linha
            text = text.Replace("\n", Environment.NewLine);

            // Restaurar as sequências de escape
            text = text.Replace("{{ESCAPED_NEWLINE}}", @"\n");
            text = text.Replace("{{ESCAPED_BACKSLASH}}", @"\\");

            return text;
        }

        private string ConvertNewlinesForStorage(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // Primeiro, proteger as sequências de escape
            text = text.Replace(@"\\", "{{ESCAPED_BACKSLASH}}");
            text = text.Replace(@"\n", "{{ESCAPED_NEWLINE}}");

            // Substituir Environment.NewLine por \n
            text = text.Replace("\r\n", "\n");
            text = text.Replace("\r", "\n");

            // Restaurar as sequências de escape
            text = text.Replace("{{ESCAPED_NEWLINE}}", @"\n");
            text = text.Replace("{{ESCAPED_BACKSLASH}}", @"\\");

            return text;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox textBox)
            {
                textBox.Multiline = true;
                textBox.AcceptsReturn = true;
                textBox.ScrollBars = ScrollBars.Vertical;
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Cancelar edição durante navegação de busca
            if (isNavigatingSearch)
            {
                e.Cancel = true;
                return;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentCell != null && dataGridView1.CurrentCell.Value != null)
            {
                string cellValue = dataGridView1.CurrentCell.Value.ToString();

                // Converter para armazenamento
                string convertedValue = ConvertNewlinesForStorage(cellValue);

                // Salvar no modelo
                if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
                {
                    int rowIndex = e.RowIndex;
                    if (rowIndex >= 0 && rowIndex < loadedFiles[currentFileIndex].Textnumb)
                    {
                        loadedFiles[currentFileIndex].Texts[currentLangIndex][rowIndex] = convertedValue;
                        loadedFiles[currentFileIndex].IsModified = true;
                        UpdateWindowTitle();
                    }
                }

                // Converter para exibição e colocar no grid
                string displayValue = ConvertNewlinesForDisplay(convertedValue);
                dataGridView1.CurrentCell.Value = displayValue;

                // Atualizar altura da linha
                dataGridView1.Rows[e.RowIndex].Height = dataGridView1.Rows[e.RowIndex].GetPreferredHeight(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells, true);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && !isNavigatingSearch)
            {
                vScrollBarTexts.Value = Math.Min(dataGridView1.CurrentRow.Index, vScrollBarTexts.Maximum);
                UpdateTextPositionLabel();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Prevenir edição automática durante busca
            if (searchActive && isNavigatingSearch)
            {
                dataGridView1.EndEdit();
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.Shift)
            {
                e.SuppressKeyPress = true;

                int rowIndex = dataGridView1.CurrentCell.RowIndex;
                int columnIndex = dataGridView1.CurrentCell.ColumnIndex;

                if (rowIndex >= 0 && columnIndex >= 0)
                {
                    DataGridViewCell cell = dataGridView1.Rows[rowIndex].Cells[columnIndex];

                    if (cell.Value == null)
                        cell.Value = string.Empty;

                    cell.Value += Environment.NewLine;
                    dataGridView1.CurrentCell = cell;

                    if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
                    {
                        loadedFiles[currentFileIndex].IsModified = true;
                        UpdateWindowTitle();
                    }
                }
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (searchActive && allSearchResults.Count > 0)
                {
                    if (e.Shift)
                        NavigateToPreviousResult();
                    else
                        NavigateToNextResult();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.F)
            {
                toolStripTextBoxSearch.Focus();
                toolStripTextBoxSearch.SelectAll();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                if (searchActive && allSearchResults.Count > 0)
                {
                    NavigateToNextResult();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.Control && e.Shift && e.KeyCode == Keys.G)
            {
                if (searchActive && allSearchResults.Count > 0)
                {
                    NavigateToPreviousResult();
                    e.SuppressKeyPress = true;
                }
            }
            else if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.PageUp)
            {
                UpdateTextPositionLabel();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                // Cancelar edição
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }
            }
        }

        private STRGFileData LoadSTRGFile(string filePath)
        {
            FileStream arq = new FileStream(filePath, FileMode.Open);
            BinaryReader br = new BinaryReader(arq);

            STRGFileData fileData = new STRGFileData();
            fileData.FilePath = filePath;

            fileData.Magic = bigendian(br.ReadUInt32());
            fileData.Version = bigendian(br.ReadUInt32());
            fileData.Langnumb = bigendian(br.ReadUInt32());
            fileData.Textnumb = bigendian(br.ReadUInt32());
            fileData.Nametable = bigendian(br.ReadUInt32());
            fileData.Nts = bigendian(br.ReadUInt32());
            uint val = fileData.Nts + 0x18;
            fileData.Ntc = br.ReadBytes((int)fileData.Nts);

            arq.Seek(val, SeekOrigin.Begin);

            for (int i = 0; i < fileData.Langnumb; i++)
            {
                byte[] btexto = br.ReadBytes(4);
                string texto = Encoding.ASCII.GetString(btexto);
                fileData.LanguageCodes.Add(texto);
            }

            fileData.Texts = new string[fileData.Langnumb][];

            for (int i = 0; i < fileData.Langnumb; i++)
            {
                fileData.Texts[i] = new string[fileData.Textnumb];
                uint str_block_len = bigendian(br.ReadUInt32());

                for (int j = 0; j < fileData.Textnumb; j++)
                {
                    uint prpt = bigendian(br.ReadUInt32());

                    long pos_antiga = arq.Position;

                    long primeirotexto = (4 + (long)fileData.Textnumb * 4) * (long)fileData.Langnumb + val + (long)fileData.Langnumb * 4;

                    arq.Seek(primeirotexto + prpt, SeekOrigin.Begin);

                    uint textSize = bigendian(br.ReadUInt32());
                    int textSizeInt = (int)(textSize - 1);

                    byte[] textg = br.ReadBytes(textSizeInt);
                    string textt = Encoding.UTF8.GetString(textg);

                    arq.Seek(pos_antiga, SeekOrigin.Begin);

                    fileData.Texts[i][j] = textt;
                }
            }

            br.Close();
            arq.Close();

            DetectSingleSharedText(fileData);

            return fileData;
        }

        private void DetectSingleSharedText(STRGFileData fileData)
        {
            if (fileData.Textnumb != 1)
            {
                fileData.IsSingleSharedText = false;
                return;
            }

            string firstText = fileData.Texts[0][0];
            bool allTextsSame = true;

            for (int i = 1; i < fileData.Langnumb; i++)
            {
                if (fileData.Texts[i][0] != firstText)
                {
                    allTextsSame = false;
                    break;
                }
            }

            fileData.IsSingleSharedText = allTextsSame;

            if (allTextsSame)
            {
                Console.WriteLine($"File {fileData.FileName} detected as single shared text: '{firstText}'");
            }
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "DKCR or MP 3 STRG|*.STRG|All files (*.*)|*.*";
            openFileDialog1.Title = "Select STRG Files";
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (CheckForUnsavedChanges())
                {
                    SaveCurrentTexts();
                }

                loadedFiles.Clear();
                listBoxArquivos.Items.Clear();
                listBoxLinguagens.Items.Clear();
                dataGridView1.Rows.Clear();

                currentFileIndex = -1;
                currentLangIndex = -1;

                int singleTextFiles = 0;
                int normalFiles = 0;

                foreach (string fileName in openFileDialog1.FileNames)
                {
                    try
                    {
                        STRGFileData fileData = LoadSTRGFile(fileName);

                        if (fileData.IsSingleSharedText)
                        {
                            singleTextFiles++;
                        }
                        else
                        {
                            normalFiles++;
                        }

                        loadedFiles.Add(fileData);
                        string displayName = fileData.FileName;
                        if (fileData.IsSingleSharedText)
                        {
                            displayName += " [Single Text]";
                        }
                        listBoxArquivos.Items.Add(displayName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file {Path.GetFileName(fileName)}: {ex.Message}",
                                      "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (listBoxArquivos.Items.Count > 0)
                {
                    listBoxArquivos.SelectedIndex = 0;
                }

                if (singleTextFiles > 0)
                {
                    toolStripStatusLabel1.Text = $"Loaded: {normalFiles} normal files, {singleTextFiles} single text files (non-editable)";
                }
                else
                {
                    toolStripStatusLabel1.Text = $"Loaded: {normalFiles} files";
                }

                UpdateWindowTitle();
            }

            openFileDialog1.Dispose();
        }

        private void ResetarEstado()
        {
            loadedFiles.Clear();
            listBoxArquivos.Items.Clear();
            listBoxLinguagens.Items.Clear();
            dataGridView1.Rows.Clear();
            currentFileIndex = -1;
            currentLangIndex = -1;
            UpdateWindowTitle();
        }

        private void listBoxArquivos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isChangingFile || listBoxArquivos.SelectedIndex < 0) return;

            isChangingFile = true;

            try
            {
                // Não salvar durante navegação de pesquisa
                if (!isNavigatingSearch)
                {
                    SaveCurrentTexts();
                }

                int newIndex = listBoxArquivos.SelectedIndex;

                if (newIndex >= 0 && newIndex < loadedFiles.Count)
                {
                    currentFileIndex = newIndex;
                    STRGFileData fileData = loadedFiles[currentFileIndex];

                    listBoxLinguagens.Items.Clear();
                    foreach (string langCode in fileData.LanguageCodes)
                    {
                        listBoxLinguagens.Items.Add(langCode);
                    }

                    dataGridView1.Rows.Clear();

                    if (listBoxLinguagens.Items.Count > 0)
                    {
                        int savedLangIndex = fileData.SelectedLangIndex;
                        if (savedLangIndex >= 0 && savedLangIndex < listBoxLinguagens.Items.Count)
                        {
                            listBoxLinguagens.SelectedIndex = savedLangIndex;
                        }
                        else
                        {
                            if (currentLangIndex >= 0 && currentLangIndex < listBoxLinguagens.Items.Count)
                            {
                                listBoxLinguagens.SelectedIndex = currentLangIndex;
                            }
                            else
                            {
                                listBoxLinguagens.SelectedIndex = 0;
                            }
                        }
                    }
                    else
                    {
                        dataGridView1.Rows.Clear();
                        currentLangIndex = -1;
                    }

                    UpdateStatus();
                    UpdateWindowTitle();

                    dataGridView1.ReadOnly = fileData.IsSingleSharedText;
                    if (fileData.IsSingleSharedText)
                    {
                        dataGridView1.BackgroundColor = System.Drawing.Color.LightGray;
                        toolStripStatusLabel1.Text = $"File {fileData.FileName}: Single shared text (non-editable)";
                    }
                    else
                    {
                        dataGridView1.BackgroundColor = System.Drawing.Color.White;
                    }

                    SetupTextScrollBar();
                }
            }
            finally
            {
                isChangingFile = false;
            }
        }

        private void listBoxLinguagens_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxLinguagens.SelectedIndex < 0 || currentFileIndex < 0) return;

            // Não salvar durante navegação de pesquisa
            if (!isNavigatingSearch)
            {
                SaveCurrentTexts();
            }

            int langIndex = listBoxLinguagens.SelectedIndex;

            if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
            {
                STRGFileData fileData = loadedFiles[currentFileIndex];
                currentLangIndex = langIndex;
                fileData.SelectedLangIndex = langIndex;

                dataGridView1.Rows.Clear();

                if (langIndex >= 0 && langIndex < fileData.Langnumb)
                {
                    for (int i = 0; i < fileData.Textnumb; i++)
                    {
                        string text = fileData.Texts[langIndex][i] ?? string.Empty;
                        // Converter para exibição ao adicionar
                        string displayText = ConvertNewlinesForDisplay(text);
                        dataGridView1.Rows.Add(displayText);
                    }
                }

                UpdateStatus();
                SetupTextScrollBar();
            }
        }

        private void SaveCurrentTexts()
        {
            if (currentFileIndex < 0 || currentFileIndex >= loadedFiles.Count) return;
            if (currentLangIndex < 0) return;

            STRGFileData fileData = loadedFiles[currentFileIndex];

            if (fileData.IsSingleSharedText) return;
            if (currentLangIndex >= fileData.Langnumb) return;
            if (dataGridView1.Rows.Count == 0) return;

            bool hasChanges = false;

            // Forçar término de edição se houver célula em modo de edição
            if (dataGridView1.IsCurrentCellInEditMode)
            {
                dataGridView1.EndEdit();
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (i >= fileData.Textnumb) break;

                string cellValue = "";
                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    cellValue = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    // Converter para armazenamento
                    cellValue = ConvertNewlinesForStorage(cellValue);
                }

                string currentText = fileData.Texts[currentLangIndex][i] ?? "";

                if (cellValue != currentText)
                {
                    fileData.Texts[currentLangIndex][i] = cellValue;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                fileData.IsModified = true;
                UpdateWindowTitle();
            }
        }

        private void UpdateStatus()
        {
            if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
            {
                STRGFileData fileData = loadedFiles[currentFileIndex];
                if (currentLangIndex >= 0 && currentLangIndex < fileData.LanguageCodes.Count)
                {
                    string modifiedIndicator = fileData.IsModified ? "* " : "";
                    string singleTextIndicator = fileData.IsSingleSharedText ? "[SINGLE TEXT] " : "";
                    toolStripStatusLabel1.Text =
                        $"{singleTextIndicator}{modifiedIndicator}File: {fileData.FileName} | Language: {fileData.LanguageCodes[currentLangIndex]} | Texts: {fileData.Textnumb}";
                }
                else
                {
                    string modifiedIndicator = fileData.IsModified ? "* " : "";
                    string singleTextIndicator = fileData.IsSingleSharedText ? "[SINGLE TEXT] " : "";
                    toolStripStatusLabel1.Text = $"{singleTextIndicator}{modifiedIndicator}File: {fileData.FileName} | No language selected";
                }
            }
            else
            {
                toolStripStatusLabel1.Text = "Ready";
            }
        }

        private void UpdateWindowTitle()
        {
            if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
            {
                STRGFileData fileData = loadedFiles[currentFileIndex];
                string modifiedIndicator = fileData.IsModified ? "*" : "";
                string singleTextIndicator = fileData.IsSingleSharedText ? "[Single Text] " : "";
                this.Text = $"STRG Editor - {singleTextIndicator}{fileData.FileName}{modifiedIndicator}";
            }
            else if (loadedFiles.Count > 0)
            {
                int modifiedCount = 0;
                int singleTextCount = 0;
                foreach (STRGFileData file in loadedFiles)
                {
                    if (file.IsModified) modifiedCount++;
                    if (file.IsSingleSharedText) singleTextCount++;
                }

                if (modifiedCount > 0 || singleTextCount > 0)
                {
                    string modifiers = "";
                    if (singleTextCount > 0) modifiers += $"{singleTextCount} single text";
                    if (modifiedCount > 0)
                    {
                        if (modifiers != "") modifiers += ", ";
                        modifiers += $"{modifiedCount} modified";
                    }
                    this.Text = $"STRG Editor - {loadedFiles.Count} files loaded ({modifiers})";
                }
                else
                {
                    this.Text = $"STRG Editor - {loadedFiles.Count} files loaded";
                }
            }
            else
            {
                this.Text = "STRG Editor";
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileIndex >= 0)
            {
                // Forçar salvamento de todas as alterações do grid
                SaveCurrentTexts();

                STRGFileData currentFile = loadedFiles[currentFileIndex];

                if (currentFile.IsSingleSharedText)
                {
                    MessageBox.Show($"File '{currentFile.FileName}' has single shared text and cannot be modified.",
                                  "Save Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    // Garantir que todos os dados estão sincronizados
                    SyncGridToModel();

                    SaveSTRGFile(currentFile, currentFile.FilePath);
                    currentFile.IsModified = false;
                    UpdateWindowTitle();
                    UpdateStatus();

                    MessageBox.Show($"File saved: {currentFile.FileName}",
                                  "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}",
                                  "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No file selected.", "Save Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SyncGridToModel()
        {
            if (currentFileIndex < 0 || currentFileIndex >= loadedFiles.Count) return;
            if (currentLangIndex < 0) return;

            STRGFileData fileData = loadedFiles[currentFileIndex];

            if (fileData.IsSingleSharedText) return;
            if (dataGridView1.Rows.Count == 0) return;

            // Garantir término de qualquer edição em andamento
            if (dataGridView1.IsCurrentCellInEditMode)
            {
                dataGridView1.EndEdit();
            }

            // Sincronizar cada linha do grid com o modelo
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (i >= fileData.Textnumb) break;

                if (dataGridView1.Rows[i].Cells[0].Value != null)
                {
                    string cellValue = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    string storageValue = ConvertNewlinesForStorage(cellValue);

                    // Atualizar diretamente no modelo
                    fileData.Texts[currentLangIndex][i] = storageValue;
                }
            }
        }

        private void salvarTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count == 0)
            {
                MessageBox.Show("No files loaded.", "Save All",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Salvar alterações do arquivo atual
            SaveCurrentTexts();

            int savedCount = 0;
            int errorCount = 0;
            int skippedCount = 0;
            List<string> savedFiles = new List<string>();

            foreach (STRGFileData fileData in loadedFiles)
            {
                try
                {
                    if (fileData.IsSingleSharedText)
                    {
                        skippedCount++;
                        continue;
                    }

                    if (fileData.IsModified)
                    {
                        // Garantir sincronização do grid com o modelo antes de salvar
                        if (currentFileIndex >= 0 && loadedFiles[currentFileIndex] == fileData)
                        {
                            SyncGridToModel();
                        }

                        SaveSTRGFile(fileData, fileData.FilePath);
                        fileData.IsModified = false;
                        savedCount++;
                        savedFiles.Add(fileData.FileName);
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    MessageBox.Show($"Error saving file {fileData.FileName}: {ex.Message}",
                                  "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            UpdateWindowTitle();

            if (savedCount > 0)
            {
                string message = $"Successfully saved {savedCount} file(s):\n\n";
                foreach (string fileName in savedFiles)
                {
                    message += $"• {fileName}\n";
                }

                if (skippedCount > 0)
                {
                    message += $"\nSkipped {skippedCount} file(s) with single shared text.";
                }

                if (errorCount > 0)
                {
                    message += $"\nFailed to save {errorCount} file(s).";
                }

                MessageBox.Show(message, "Save All Complete",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (errorCount == 0 && skippedCount == 0)
            {
                MessageBox.Show("No files needed saving (no modifications detected).",
                              "Save All", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (skippedCount > 0)
            {
                MessageBox.Show($"No files saved. {skippedCount} file(s) with single shared text were skipped.",
                              "Save All", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            salvar_arq_com_dialogo(false);
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            salvar_arq_com_dialogo(true);
        }

        private void salvar_arq_com_dialogo(bool Comprimir)
        {
            if (currentFileIndex < 0) return;

            // Forçar salvamento das alterações atuais
            SaveCurrentTexts();

            STRGFileData currentFile = loadedFiles[currentFileIndex];

            if (currentFile.IsSingleSharedText)
            {
                MessageBox.Show($"File '{currentFile.FileName}' has single shared text and cannot be saved as another file.",
                              "Save As Blocked", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.Filter = "DKCR or MP 3 STRG|*.STRG|All files (*.*)|*.*";
            SaveFileDialog1.Title = "Save a STRG File";
            SaveFileDialog1.FileName = Path.GetFileName(currentFile.FilePath);

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Garantir sincronização final
                SyncGridToModel();

                SaveSTRGFile(currentFile, SaveFileDialog1.FileName, Comprimir);
                currentFile.IsModified = false;
                UpdateWindowTitle();
                MessageBox.Show($"File saved as: {SaveFileDialog1.FileName}",
                              "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveSTRGFile(STRGFileData fileData, string FileName, bool shouldCompress = false)
        {
            FileStream arq = new FileStream(FileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(arq);

            bw.Write(bigendian(fileData.Magic));
            bw.Write(bigendian(fileData.Version));
            bw.Write(bigendian(fileData.Langnumb));
            bw.Write(bigendian(fileData.Textnumb));
            bw.Write(bigendian(fileData.Nametable));
            bw.Write(bigendian(fileData.Nts));
            bw.Write(fileData.Ntc);

            for (int i = 0; i < fileData.Langnumb; i++)
            {
                bw.Write(Encoding.ASCII.GetBytes(fileData.LanguageCodes[i]));
            }

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw2 = new BinaryWriter(ms);

            for (int i = 0; i < fileData.Langnumb; i++)
            {
                uint tamanho_strings = 0;

                for (int j = 0; j < fileData.Textnumb; j++)
                {
                    byte[] texto_utf8 = Encoding.UTF8.GetBytes(fileData.Texts[i][j]);
                    tamanho_strings += (uint)texto_utf8.Length + 1;
                }

                bw.Write(bigendian(tamanho_strings));

                for (int j = 0; j < fileData.Textnumb; j++)
                {
                    bw.Write(bigendian((uint)ms.Position));
                    byte[] texto_bytes = Encoding.UTF8.GetBytes(fileData.Texts[i][j]);
                    bw2.Write(bigendian((uint)texto_bytes.Length + 1));
                    bw2.Write(texto_bytes);
                    bw2.Write('\0');
                }
            }

            bw.Write(ms.ToArray());
            ms.Dispose();
            arq.Dispose();

            if (shouldCompress) File.WriteAllBytes(FileName, Compress(File.ReadAllBytes(FileName)));
        }

        private byte[] Compress(byte[] Data)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                DeflateStream Compressor = new DeflateStream(Stream, CompressionLevel.Optimal);
                Compressor.Write(Data, 0, Data.Length);
                Compressor.Close();

                using (MemoryStream NewStream = new MemoryStream())
                {
                    BinaryWriter Writer = new BinaryWriter(NewStream);
                    Writer.Write((ushort)0xda78);
                    Writer.Write(Stream.ToArray());
                    Writer.Write(bigendian(Adler32(Data)));
                    return NewStream.ToArray();
                }
            }
        }

        private uint Adler32(byte[] Data)
        {
            const int MOD_ADLER = 65521;

            uint a = 1, b = 0;
            for (int Index = 0; Index < Data.Length; Index++)
            {
                a = (a + Data[Index]) % MOD_ADLER;
                b = (b + a) % MOD_ADLER;
            }

            return (b << 16) | a;
        }

        private bool CheckForUnsavedChanges()
        {
            if (loadedFiles.Count == 0) return false;

            foreach (STRGFileData fileData in loadedFiles)
            {
                if (fileData.IsModified && !fileData.IsSingleSharedText) return true;
            }

            return false;
        }

        private void sobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("STRG editor created by Angel333119\nModified by HeitorSpectre with TXT export/import features\nBug fixes and tool improvements",
                          "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void arquivoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        // ============================================
        // EXPORT/IMPORT
        // ============================================

        private void exportarParaTXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileIndex < 0)
            {
                MessageBox.Show("Please select a file first.", "Export Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentLangIndex < 0)
            {
                MessageBox.Show("Please select a language first.", "Export Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            STRGFileData currentFile = loadedFiles[currentFileIndex];

            if (currentFile.IsSingleSharedText)
            {
                MessageBox.Show($"File '{currentFile.FileName}' has single shared text and will not be exported.\n\n" +
                              "These files maintain the same string for all languages and should not be modified.",
                              "Export Skipped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                SaveCurrentTexts();
                string langCode = listBoxLinguagens.SelectedItem.ToString();
                string txtFilePath = Path.ChangeExtension(currentFile.FilePath, $".{langCode}.txt");

                ExportToTxt(currentFile, langCode, currentLangIndex, txtFilePath);

                MessageBox.Show($"Texts exported successfully to:\n{txtFilePath}",
                              "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting file: {ex.Message}", "Export Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportarTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count == 0)
            {
                MessageBox.Show("No STRG files loaded.", "Export Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveCurrentTexts();

            int totalExported = 0;
            int totalSkipped = 0;
            int singleTextSkipped = 0;
            List<string> exportedFiles = new List<string>();

            foreach (STRGFileData fileData in loadedFiles)
            {
                try
                {
                    if (fileData.IsSingleSharedText)
                    {
                        singleTextSkipped++;
                        continue;
                    }

                    if (fileData.SelectedLangIndex >= 0 && fileData.SelectedLangIndex < fileData.LanguageCodes.Count)
                    {
                        int langIndex = fileData.SelectedLangIndex;
                        string langCode = fileData.LanguageCodes[langIndex];
                        string txtFilePath = Path.ChangeExtension(fileData.FilePath, $".{langCode}.txt");

                        ExportToTxt(fileData, langCode, langIndex, txtFilePath);
                        totalExported++;
                        exportedFiles.Add(Path.GetFileName(txtFilePath));
                    }
                    else
                    {
                        totalSkipped++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting {fileData.FileName}: {ex.Message}",
                                  "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (totalExported > 0)
            {
                string message = $"Successfully exported {totalExported} file(s):\n\n";
                foreach (string fileName in exportedFiles)
                {
                    message += $"• {fileName}\n";
                }

                if (singleTextSkipped > 0)
                {
                    message += $"\n{singleTextSkipped} file(s) with single shared text were ignored.";
                }

                if (totalSkipped > 0)
                {
                    message += $"\n{totalSkipped} file(s) ignored (language not selected or invalid).";
                }

                MessageBox.Show(message, "Export All Complete",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (singleTextSkipped > 0)
            {
                MessageBox.Show($"No files exported. {singleTextSkipped} file(s) with single shared text were ignored.\n\n" +
                              "These files maintain the same string for all languages and should not be modified.",
                              "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No files were exported.",
                              "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ExportToTxt(STRGFileData fileData, string langCode, int langIndex, string txtFilePath)
        {
            using (StreamWriter sw = new StreamWriter(txtFilePath, false, Encoding.UTF8))
            {
                sw.WriteLine("=== STRG TEXT EXPORT ===");
                sw.WriteLine($"FILE: {fileData.FileName}");
                sw.WriteLine($"PATH: {fileData.FilePath}");
                sw.WriteLine($"LANGUAGE: {langCode}");
                sw.WriteLine($"LANGUAGE_INDEX: {langIndex}");
                sw.WriteLine($"TEXT_COUNT: {fileData.Textnumb}");
                sw.WriteLine($"EXPORT_DATE: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sw.WriteLine("=== TEXTS ===");
                sw.WriteLine();

                for (int i = 0; i < fileData.Textnumb; i++)
                {
                    sw.WriteLine($"#[{fileData.FileName}|{langCode}|{i}]");

                    string textToExport = fileData.Texts[langIndex][i] ?? string.Empty;

                    textToExport = textToExport.Replace("\\", "\\\\");
                    textToExport = textToExport.Replace("\n", "\\n");
                    textToExport = textToExport.Replace("\r", "");

                    sw.WriteLine(textToExport);
                    sw.WriteLine();
                }

                sw.WriteLine("=== END OF FILE ===");
            }
        }

        private void importarDeTXTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckForUnsavedChanges())
            {
                DialogResult result = MessageBox.Show("You have unsaved changes. Save before importing?",
                                                    "Unsaved Changes",
                                                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    salvarTodosToolStripMenuItem_Click(sender, e);
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.Title = "Import texts from TXT file";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                int totalImported = 0;
                int totalErrors = 0;
                int singleTextSkipped = 0;
                List<string> importedFiles = new List<string>();

                foreach (string txtFile in openFileDialog.FileNames)
                {
                    try
                    {
                        string finalizedPath;
                        int importResult = ImportFromTxt(txtFile, out finalizedPath);

                        if (importResult == 1)
                        {
                            totalImported++;
                            if (!string.IsNullOrEmpty(finalizedPath))
                            {
                                importedFiles.Add(Path.GetFileName(finalizedPath));
                            }
                        }
                        else if (importResult == 0)
                        {
                            totalErrors++;
                        }
                        else if (importResult == -1)
                        {
                            singleTextSkipped++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing {Path.GetFileName(txtFile)}: {ex.Message}",
                                      "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        totalErrors++;
                    }
                }

                // Atualizar o grid com os novos dados
                if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count && currentLangIndex >= 0)
                {
                    STRGFileData currentFile = loadedFiles[currentFileIndex];
                    dataGridView1.Rows.Clear();
                    for (int i = 0; i < currentFile.Textnumb; i++)
                    {
                        string text = currentFile.Texts[currentLangIndex][i] ?? string.Empty;
                        string displayText = ConvertNewlinesForDisplay(text);
                        dataGridView1.Rows.Add(displayText);
                    }
                }

                string message = $"Import completed:\n";
                message += $"• Successfully imported: {totalImported} file(s)\n";

                if (singleTextSkipped > 0)
                {
                    message += $"• Skipped {singleTextSkipped} file(s) with single shared text (not modifiable)\n";
                }

                if (importedFiles.Count > 0)
                {
                    message += $"Files saved in 'arquivos finalizados' folder:\n";
                    foreach (string fileName in importedFiles)
                    {
                        message += $"  • {fileName}\n";
                    }
                }

                message += $"• Failed: {totalErrors} file(s)";

                MessageBox.Show(message, "Import Complete", MessageBoxButtons.OK,
                              totalErrors > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
        }

        private int ImportFromTxt(string txtFilePath, out string finalizedPath)
        {
            finalizedPath = null;

            string[] lines = File.ReadAllLines(txtFilePath, Encoding.UTF8);

            if (lines.Length < 5 || !lines[0].Contains("STRG TEXT EXPORT"))
            {
                MessageBox.Show($"Invalid TXT format in {Path.GetFileName(txtFilePath)}",
                              "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            string fileName = "";
            string filePath = "";
            string language = "";
            int languageIndex = -1;
            int textCount = 0;

            for (int i = 0; i < Math.Min(lines.Length, 20); i++)
            {
                if (lines[i].StartsWith("FILE: ")) fileName = lines[i].Substring(6).Trim();
                if (lines[i].StartsWith("PATH: ")) filePath = lines[i].Substring(6).Trim();
                if (lines[i].StartsWith("LANGUAGE: ")) language = lines[i].Substring(10).Trim();
                if (lines[i].StartsWith("LANGUAGE_INDEX: ")) int.TryParse(lines[i].Substring(16).Trim(), out languageIndex);
                if (lines[i].StartsWith("TEXT_COUNT: ")) int.TryParse(lines[i].Substring(12).Trim(), out textCount);
            }

            STRGFileData targetFile = null;
            bool wasAlreadyLoaded = false;

            foreach (STRGFileData file in loadedFiles)
            {
                if (file.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase) ||
                    file.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase))
                {
                    targetFile = file;
                    wasAlreadyLoaded = true;
                    break;
                }
            }

            if (targetFile == null)
            {
                if (File.Exists(filePath))
                {
                    try
                    {
                        targetFile = LoadSTRGFile(filePath);
                    }
                    catch
                    {
                        MessageBox.Show($"Could not load STRG file: {fileName}",
                                      "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0;
                    }
                }
                else
                {
                    MessageBox.Show($"STRG file not found: {fileName}\nPath: {filePath}",
                                  "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }

            if (targetFile.IsSingleSharedText)
            {
                MessageBox.Show($"File '{targetFile.FileName}' has single shared text and cannot be modified via TXT import.\n\n" +
                              "This file maintains the same string for all languages and any modification would corrupt its original structure.",
                              "Import Skipped", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return -1;
            }

            int langIndex = languageIndex;
            if (langIndex == -1)
                langIndex = targetFile.LanguageCodes.IndexOf(language);

            if (langIndex < 0 || langIndex >= targetFile.LanguageCodes.Count)
            {
                MessageBox.Show($"Language '{language}' not found in file {fileName}",
                              "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            Dictionary<int, string> importedTexts = new Dictionary<int, string>();
            int currentTextIndex = -1;
            StringBuilder currentText = new StringBuilder();
            bool inTextSection = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("=== TEXTS ==="))
                {
                    inTextSection = true;
                    continue;
                }

                if (lines[i].Contains("=== END OF FILE ==="))
                {
                    break;
                }

                if (inTextSection && lines[i].StartsWith("#["))
                {
                    if (currentTextIndex >= 0 && currentText.Length > 0)
                    {
                        string text = currentText.ToString().TrimEnd();
                        text = ConvertLiteralNewlinesToReal(text);
                        importedTexts[currentTextIndex] = text;
                    }

                    string tag = lines[i].Trim();
                    int start = tag.IndexOf('|', tag.IndexOf('|') + 1) + 1;
                    int end = tag.IndexOf(']', start);

                    if (int.TryParse(tag.Substring(start, end - start), out int newIndex))
                    {
                        currentTextIndex = newIndex;
                        currentText.Clear();
                    }
                }
                else if (inTextSection && currentTextIndex >= 0 && !string.IsNullOrWhiteSpace(lines[i]))
                {
                    currentText.AppendLine(lines[i]);
                }
            }

            if (currentTextIndex >= 0 && currentText.Length > 0)
            {
                string text = currentText.ToString().TrimEnd();
                text = ConvertLiteralNewlinesToReal(text);
                importedTexts[currentTextIndex] = text;
            }

            int appliedCount = 0;
            foreach (var kvp in importedTexts)
            {
                if (kvp.Key >= 0 && kvp.Key < targetFile.Textnumb)
                {
                    targetFile.Texts[langIndex][kvp.Key] = kvp.Value;
                    appliedCount++;
                }
            }

            if (appliedCount > 0)
            {
                string originalDir = Path.GetDirectoryName(targetFile.FilePath);
                string finalizedDir = Path.Combine(originalDir, "arquivos finalizados");

                if (!Directory.Exists(finalizedDir))
                {
                    Directory.CreateDirectory(finalizedDir);
                }

                finalizedPath = Path.Combine(finalizedDir, targetFile.FileName);
                SaveSTRGFile(targetFile, finalizedPath);

                if (wasAlreadyLoaded)
                {
                    targetFile.IsModified = true;
                    UpdateWindowTitle();
                }
            }

            return appliedCount > 0 ? 1 : 0;
        }

        private string ConvertLiteralNewlinesToReal(string text)
        {
            text = text.Replace("\\\\", "{{TEMP_DOUBLE_BACKSLASH}}");
            text = text.Replace("\\n", "\n");
            text = text.Replace("{{TEMP_DOUBLE_BACKSLASH}}", "\\");
            text = text.Replace("\r", "");
            return text;
        }

        private void fecharArquivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxArquivos.SelectedIndex >= 0 && listBoxArquivos.SelectedIndex < loadedFiles.Count)
            {
                STRGFileData fileToClose = loadedFiles[listBoxArquivos.SelectedIndex];
                if (fileToClose.IsModified && !fileToClose.IsSingleSharedText)
                {
                    DialogResult result = MessageBox.Show($"File '{fileToClose.FileName}' has unsaved changes. Save before closing?",
                                                        "Unsaved Changes",
                                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        SaveCurrentTexts();
                        SaveSTRGFile(fileToClose, fileToClose.FilePath);
                        fileToClose.IsModified = false;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                SaveCurrentTexts();

                int selectedIndex = listBoxArquivos.SelectedIndex;
                loadedFiles.RemoveAt(selectedIndex);
                listBoxArquivos.Items.RemoveAt(selectedIndex);

                if (listBoxArquivos.Items.Count > 0)
                {
                    int newIndex = Math.Min(selectedIndex, listBoxArquivos.Items.Count - 1);
                    listBoxArquivos.SelectedIndex = newIndex;
                }
                else
                {
                    ResetarEstado();
                    toolStripStatusLabel1.Text = "Ready";
                }

                UpdateWindowTitle();
            }
        }

        private void fecharTodosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (loadedFiles.Count > 0)
            {
                bool hasUnsavedChanges = CheckForUnsavedChanges();

                if (hasUnsavedChanges)
                {
                    DialogResult result = MessageBox.Show("Some files have unsaved changes. Save before closing all?",
                                                        "Unsaved Changes",
                                                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        salvarTodosToolStripMenuItem_Click(sender, e);
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                ResetarEstado();
                toolStripStatusLabel1.Text = "Ready";
            }
        }

        // ============================================
        // BARRA DE ROLAGEM PARA TEXTOS
        // ============================================

        private void SetupTextScrollBar()
        {
            if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
            {
                STRGFileData fileData = loadedFiles[currentFileIndex];

                vScrollBarTexts.Minimum = 0;
                vScrollBarTexts.Maximum = Math.Max(0, (int)fileData.Textnumb - 1);
                vScrollBarTexts.Value = 0;
                vScrollBarTexts.LargeChange = 1;
                vScrollBarTexts.SmallChange = 1;
                vScrollBarTexts.Visible = fileData.Textnumb > 1;

                UpdateTextPositionLabel();
            }
            else
            {
                vScrollBarTexts.Visible = false;
                labelTextPosition.Text = "No texts";
            }
        }

        private void UpdateTextPositionLabel()
        {
            if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
            {
                STRGFileData fileData = loadedFiles[currentFileIndex];
                int currentRow = dataGridView1.CurrentRow != null ? dataGridView1.CurrentRow.Index : 0;
                labelTextPosition.Text = $"Text {currentRow + 1} of {fileData.Textnumb}";
            }
        }

        private void vScrollBarTexts_Scroll(object sender, ScrollEventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && e.NewValue >= 0 && e.NewValue < dataGridView1.Rows.Count)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.NewValue].Cells[0].Selected = true;
                dataGridView1.CurrentCell = dataGridView1.Rows[e.NewValue].Cells[0];
                dataGridView1.FirstDisplayedScrollingRowIndex = e.NewValue;

                UpdateTextPositionLabel();
            }
        }

        private void vScrollBarTexts_ValueChanged(object sender, EventArgs e)
        {
            if (vScrollBarTexts.Value >= 0 && vScrollBarTexts.Value < dataGridView1.Rows.Count)
            {
                dataGridView1.FirstDisplayedScrollingRowIndex = vScrollBarTexts.Value;
                UpdateTextPositionLabel();
            }
        }

        // ============================================
        // SISTEMA DE PESQUISA (CORRIGIDO)
        // ============================================

        private void toolStripTextBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchInAllFiles(toolStripTextBoxSearch.Text.Trim());
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                if (searchActive && allSearchResults.Count > 0)
                {
                    if (e.Shift)
                        NavigateToPreviousResult();
                    else
                        NavigateToNextResult();
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void toolStripButtonSearch_Click(object sender, EventArgs e)
        {
            SearchInAllFiles(toolStripTextBoxSearch.Text.Trim());
        }

        private void toolStripButtonClearSearch_Click(object sender, EventArgs e)
        {
            ClearSearch();
        }

        private void toolStripButtonPrevResult_Click(object sender, EventArgs e)
        {
            NavigateToPreviousResult();
        }

        private void toolStripButtonNextResult_Click(object sender, EventArgs e)
        {
            NavigateToNextResult();
        }

        private void SearchInAllFiles(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ClearSearch();
                return;
            }

            // Forçar salvamento de alterações atuais antes da pesquisa
            SaveCurrentTexts();

            ClearRowHighlights();

            searchActive = true;
            lastSearchText = searchText;
            allSearchResults.Clear();
            currentResultIndex = -1;

            for (int fileIndex = 0; fileIndex < loadedFiles.Count; fileIndex++)
            {
                STRGFileData fileData = loadedFiles[fileIndex];

                if (fileData.IsSingleSharedText) continue;

                int selectedLangIndex = fileData.SelectedLangIndex;
                if (selectedLangIndex < 0 || selectedLangIndex >= fileData.LanguageCodes.Count)
                {
                    selectedLangIndex = 0;
                }

                for (int textIndex = 0; textIndex < fileData.Textnumb; textIndex++)
                {
                    string text = fileData.Texts[selectedLangIndex][textIndex] ?? "";

                    int matchIndex = text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                    if (matchIndex >= 0)
                    {
                        allSearchResults.Add(new SearchResult
                        {
                            FileIndex = fileIndex,
                            LanguageIndex = selectedLangIndex,
                            TextIndex = textIndex,
                            FileName = fileData.FileName,
                            LanguageCode = fileData.LanguageCodes[selectedLangIndex],
                            FoundText = searchText,
                            FullText = text,
                            MatchPosition = matchIndex
                        });
                    }
                }
            }

            UpdateSearchUI();

            if (allSearchResults.Count > 0)
            {
                NavigateToSearchResult(0);
            }
            else
            {
                toolStripStatusLabel1.Text = $"No results found for '{searchText}' (in selected language only)";
            }
        }

        private void UpdateSearchUI()
        {
            toolStripLabelResults.Text = $"{allSearchResults.Count} results";
            toolStripButtonPrevResult.Enabled = allSearchResults.Count > 0;
            toolStripButtonNextResult.Enabled = allSearchResults.Count > 0;

            if (allSearchResults.Count > 0)
            {
                toolStripStatusLabel1.Text = $"Found {allSearchResults.Count} results for '{lastSearchText}'";
            }
        }

        private void NavigateToSearchResult(int resultIndex)
        {
            isNavigatingSearch = true;

            try
            {
                // Garantir que nenhuma célula está em modo de edição
                if (dataGridView1.IsCurrentCellInEditMode)
                {
                    dataGridView1.EndEdit();
                }

                // Limpar destaques anteriores
                ClearRowHighlights();

                if (allSearchResults.Count == 0 || resultIndex < 0 || resultIndex >= allSearchResults.Count)
                    return;

                // Salvar alterações do arquivo atual ANTES de navegar
                SaveCurrentTexts();

                SearchResult result = allSearchResults[resultIndex];
                currentResultIndex = resultIndex;

                // Primeiro, limpar completamente o grid
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                Application.DoEvents();

                // Se o arquivo não for o atual, mudar para ele
                if (currentFileIndex != result.FileIndex)
                {
                    // Mudar o índice diretamente sem disparar SaveCurrentTexts
                    // (já salvamos acima e isNavigatingSearch = true)
                    int previousFileIndex = currentFileIndex;
                    currentFileIndex = result.FileIndex;

                    // Atualizar a lista de idiomas
                    STRGFileData fileData = loadedFiles[currentFileIndex];
                    listBoxLinguagens.Items.Clear();
                    foreach (string langCode in fileData.LanguageCodes)
                    {
                        listBoxLinguagens.Items.Add(langCode);
                    }

                    // Atualizar o listBoxArquivos sem disparar eventos
                    listBoxArquivos.SelectedIndexChanged -= listBoxArquivos_SelectedIndexChanged;
                    listBoxArquivos.SelectedIndex = result.FileIndex;
                    listBoxArquivos.SelectedIndexChanged += listBoxArquivos_SelectedIndexChanged;
                }

                // Garantir que estamos no arquivo correto
                if (currentFileIndex >= 0 && currentFileIndex < loadedFiles.Count)
                {
                    STRGFileData fileData = loadedFiles[currentFileIndex];

                    // Verificar se precisamos mudar o idioma
                    if (currentLangIndex != result.LanguageIndex)
                    {
                        // Mudar o índice diretamente
                        currentLangIndex = result.LanguageIndex;
                        fileData.SelectedLangIndex = result.LanguageIndex;

                        // Atualizar o listBoxLinguagens sem disparar eventos
                        listBoxLinguagens.SelectedIndexChanged -= listBoxLinguagens_SelectedIndexChanged;
                        if (result.LanguageIndex < listBoxLinguagens.Items.Count)
                        {
                            listBoxLinguagens.SelectedIndex = result.LanguageIndex;
                        }
                        listBoxLinguagens.SelectedIndexChanged += listBoxLinguagens_SelectedIndexChanged;
                    }

                    // Agora carregar os textos diretamente no grid
                    dataGridView1.Rows.Clear();
                    if (currentLangIndex >= 0 && currentLangIndex < fileData.Langnumb)
                    {
                        for (int i = 0; i < fileData.Textnumb; i++)
                        {
                            string text = fileData.Texts[currentLangIndex][i] ?? string.Empty;
                            string displayText = ConvertNewlinesForDisplay(text);
                            dataGridView1.Rows.Add(displayText);
                        }
                    }

                    // Verificar se há linhas suficientes
                    if (dataGridView1.Rows.Count > result.TextIndex)
                    {
                        // Apenas seleciona a célula sem entrar em modo de edição
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[result.TextIndex].Cells[0].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[result.TextIndex].Cells[0];

                        // Rola até a linha
                        dataGridView1.FirstDisplayedScrollingRowIndex = result.TextIndex;

                        // Destaca a linha temporariamente (sem ativar edição)
                        HighlightRowTemporarily(result.TextIndex);

                        // Atualizar status
                        toolStripStatusLabel1.Text = $"Result {currentResultIndex + 1} of {allSearchResults.Count}: {result.FileName} ({result.LanguageCode})";

                        // Atualizar botões de navegação
                        UpdateNavigationButtons();

                        // Atualizar a barra de rolagem
                        UpdateTextPositionLabel();

                        // Atualizar o status geral
                        UpdateStatus();
                    }
                }
            }
            finally
            {
                isNavigatingSearch = false;
            }
        }

        private void HighlightRowTemporarily(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                // Salva a cor original
                Color originalColor = dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor;

                // Destaca com cor amarela
                dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Yellow;

                // Configura um timer para remover o destaque
                Timer highlightTimer = new Timer();
                highlightTimer.Interval = 1500; // 1.5 segundos
                highlightTimer.Tick += (sender, e) =>
                {
                    if (rowIndex < dataGridView1.Rows.Count)
                    {
                        dataGridView1.Rows[rowIndex].DefaultCellStyle.BackColor = originalColor;
                    }
                    highlightTimer.Stop();
                    highlightTimer.Dispose();
                };
                highlightTimer.Start();
            }
        }

        private void ClearRowHighlights()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;
            }
        }

        private void NavigateToNextResult()
        {
            if (allSearchResults.Count == 0) return;

            int nextIndex = currentResultIndex + 1;
            if (nextIndex >= allSearchResults.Count)
                nextIndex = 0; // Voltar ao primeiro (loop)

            NavigateToSearchResult(nextIndex);
        }

        private void NavigateToPreviousResult()
        {
            if (allSearchResults.Count == 0) return;

            int prevIndex = currentResultIndex - 1;
            if (prevIndex < 0)
                prevIndex = allSearchResults.Count - 1; // Ir para o último (loop)

            NavigateToSearchResult(prevIndex);
        }

        private void ClearSearch()
        {
            searchActive = false;
            lastSearchText = "";
            allSearchResults.Clear();
            currentResultIndex = -1;

            ClearRowHighlights();

            toolStripLabelResults.Text = "0 results";
            toolStripButtonPrevResult.Enabled = false;
            toolStripButtonNextResult.Enabled = false;
            toolStripTextBoxSearch.Text = "";

            toolStripStatusLabel1.Text = "Search cleared";
        }

        private void UpdateNavigationButtons()
        {
            toolStripButtonPrevResult.Enabled = allSearchResults.Count > 0;
            toolStripButtonNextResult.Enabled = allSearchResults.Count > 0;

            if (allSearchResults.Count > 0)
            {
                toolStripButtonPrevResult.Text = $"< Previous ({currentResultIndex + 1}/{allSearchResults.Count})";
                toolStripButtonNextResult.Text = $"Next ({currentResultIndex + 1}/{allSearchResults.Count}) >";
            }
            else
            {
                toolStripButtonPrevResult.Text = "< Previous";
                toolStripButtonNextResult.Text = "Next >";
            }
        }
    }
}