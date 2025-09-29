using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WonderfulTool
{
    public partial class FindText : Form
    {
        // Variável para guardar a referência do DataGridView do formulário principal
        private readonly DataGridView _dataGridView;

        // Variáveis para controlar a posição da última busca
        private int _lastFoundRow = -1;
        private int _lastFoundCharIndex = -1;

        // Flag para evitar loops infinitos ao sincronizar os controles das abas
        private bool _isSyncing = false;

        // O construtor agora recebe o DataGridView como um parâmetro
        public FindText(DataGridView dgv)
        {
            InitializeComponent();
            _dataGridView = dgv;
        }

        private void FindText_Load(object sender, EventArgs e)
        {
            // Começa na primeira aba e foca na caixa de texto
            tabControl1.SelectedIndex = 0;
            txtFind1.Focus();
        }

        #region Sincronização dos Controles entre as Abas

        // Sincroniza as caixas de texto "Localizar"
        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;

            if (sender == txtFind1)
                txtFind2.Text = txtFind1.Text;
            else
                txtFind1.Text = txtFind2.Text;

            // Qualquer alteração no texto reseta a busca
            _lastFoundRow = -1;
            _lastFoundCharIndex = -1;

            _isSyncing = false;
        }

        // Sincroniza os checkboxes "Diferenciar maiúsculas/minúsculas"
        private void chkMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;

            if (sender == chkMatchCase1)
                chkMatchCase2.Checked = chkMatchCase1.Checked;
            else
                chkMatchCase1.Checked = chkMatchCase2.Checked;

            _isSyncing = false;
        }

        // Sincroniza os checkboxes "Coincidir palavra inteira"
        private void chkWholeWord_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;

            if (sender == chkWholeWord1)
                chkWholeWord2.Checked = chkWholeWord1.Checked;
            else
                chkWholeWord1.Checked = chkWholeWord2.Checked;

            _isSyncing = false;
        }


        // Sincroniza os checkboxes "Pesquisa circular"
        private void chkWrapAround_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;

            if (sender == chkWrapAround1)
                chkWrapAround2.Checked = chkWrapAround1.Checked;
            else
                chkWrapAround1.Checked = chkWrapAround2.Checked;

            _isSyncing = false;
        }

        #endregion

        #region Lógica dos Botões

        // Os dois botões "Localizar próximo" chamam a mesma função
        private void btnFindNext1_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void btnFindNext2_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            Replace();
        }

        private void btnReplaceAll_Click(object sender, EventArgs e)
        {
            ReplaceAll();
        }

        #endregion

        #region Funções Principais de Localizar e Substituir

        private SearchOptions GetSearchOptions()
        {
            // Usamos os controles da primeira aba como fonte da verdade, pois estão sincronizados
            return new SearchOptions
            {
                Term = txtFind1.Text,
                Replacement = txtReplace.Text,
                MatchCase = chkMatchCase1.Checked,
                WholeWord = chkWholeWord1.Checked,
                WrapAround = chkWrapAround1.Checked
            };
        }

        private void FindNext()
        {
            var options = GetSearchOptions();
            if (string.IsNullOrEmpty(options.Term) || _dataGridView.Rows.Count == 0) return;

            // Configura o Regex baseado nas opções
            string pattern = options.WholeWord ? $@"\b{Regex.Escape(options.Term)}\b" : Regex.Escape(options.Term);
            var regexOptions = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            var regex = new Regex(pattern, regexOptions);

            int startRow = _lastFoundRow != -1 ? _lastFoundRow : (_dataGridView.CurrentRow?.Index ?? 0);
            int currentRowIndex = startRow;

            for (int i = 0; i < _dataGridView.Rows.Count; i++)
            {
                // Começa a busca do ponto em que parou na linha
                int searchFromCharIndex = (currentRowIndex == _lastFoundRow) ? _lastFoundCharIndex + 1 : 0;

                DataGridViewRow row = _dataGridView.Rows[currentRowIndex];
                if (!row.IsNewRow)
                {
                    string cellValue = row.Cells["Edited"].Value?.ToString() ?? "";
                    Match match = regex.Match(cellValue, searchFromCharIndex);

                    if (match.Success)
                    {
                        // Encontrou!
                        _dataGridView.ClearSelection();
                        row.Selected = true;
                        _dataGridView.CurrentCell = row.Cells["Edited"];
                        _lastFoundRow = currentRowIndex;
                        _lastFoundCharIndex = match.Index;
                        this.Activate(); // Traz a janela para frente
                        return;
                    }
                }

                // Avança para a próxima linha e reseta o índice do caractere
                _lastFoundCharIndex = -1;
                currentRowIndex++;
                if (currentRowIndex >= _dataGridView.Rows.Count)
                {
                    if (options.WrapAround)
                    {
                        if (startRow == 0) // Já deu a volta completa
                        {
                            break;
                        }
                        currentRowIndex = 0; // Volta para o início
                    }
                    else
                    {
                        break; // Para a busca se não for circular
                    }
                }
            }

            MessageBox.Show($"Não foi possível encontrar \"{options.Term}\".", "Localizar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _lastFoundRow = -1;
            _lastFoundCharIndex = -1;
        }

        private void Replace()
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                FindNext();
                return;
            }

            var options = GetSearchOptions();
            DataGridViewRow selectedRow = _dataGridView.SelectedRows[0];
            string originalText = selectedRow.Cells["Edited"].Value?.ToString() ?? "";

            string pattern = options.WholeWord ? $@"\b{Regex.Escape(options.Term)}\b" : Regex.Escape(options.Term);
            var regexOptions = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            var regex = new Regex(pattern, regexOptions);

            // Verifica se o texto na célula selecionada corresponde ao que foi encontrado por último
            string textToSearchIn = originalText.Substring(_lastFoundCharIndex);
            Match match = regex.Match(textToSearchIn);

            if (selectedRow.Index == _lastFoundRow && match.Success && match.Index == 0)
            {
                // Substitui a ocorrência encontrada
                var sb = new StringBuilder(originalText);
                sb.Remove(_lastFoundCharIndex, options.Term.Length);
                sb.Insert(_lastFoundCharIndex, options.Replacement);
                selectedRow.Cells["Edited"].Value = sb.ToString();
            }

            FindNext();
        }

        private void ReplaceAll()
        {
            var options = GetSearchOptions();
            if (string.IsNullOrEmpty(options.Term)) return;

            int count = 0;
            string pattern = options.WholeWord ? $@"\b{Regex.Escape(options.Term)}\b" : Regex.Escape(options.Term);
            var regexOptions = options.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
            var regex = new Regex(pattern, regexOptions);

            foreach (DataGridViewRow row in _dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                string originalText = row.Cells["Edited"].Value?.ToString() ?? "";
                if (regex.IsMatch(originalText))
                {
                    string newText = regex.Replace(originalText, options.Replacement);
                    if (originalText != newText)
                    {
                        row.Cells["Edited"].Value = newText;
                        count++;
                    }
                }
            }

            MessageBox.Show($"{count} ocorrência(s) substituída(s).", "Substituir Tudo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _lastFoundRow = -1;
            _lastFoundCharIndex = -1;
        }

        // Classe auxiliar para organizar as opções de busca
        private class SearchOptions
        {
            public string Term { get; set; }
            public string Replacement { get; set; }
            public bool MatchCase { get; set; }
            public bool WholeWord { get; set; }
            public bool WrapAround { get; set; }
        }

        #endregion
    }
}