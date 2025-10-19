using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WonderfulTool
{
    public partial class mesEditor : Form
    {
        // PT: Formulário para a funcionalidade de localizar e substituir.
        // EN: Form for the find and replace functionality.
        private FindText _findReplaceForm;
        // PT: Pilha para armazenar o histórico de ações para a funcionalidade de desfazer (Undo).
        // EN: Stack to store the history of actions for the undo functionality.
        private Stack<string> _historicoUndo;
        // PT: Pilha para armazenar o histórico de ações para a funcionalidade de refazer (Redo).
        // EN: Stack to store the history of actions for the redo functionality.
        private Stack<string> _historicoRedo;
        // PT: Flag para evitar loops infinitos durante atualizações de texto.
        // EN: Flag to prevent infinite loops during text updates.
        private bool _alteracaoEmProgresso;
        // PT: Timer para agrupar as ações de digitação para a funcionalidade de desfazer.
        // EN: Timer to group typing actions for the undo functionality.
        private Timer _timerUndo;
        // PT: Enumeração para os tipos de arquivo suportados.
        // EN: Enumeration for the supported file types.
        private enum FileType { GameCubeMES, PS2MES, TextFile, Unknown }
        // PT: Caixa de listagem para exibir sugestões de autocompletar.
        // EN: ListBox to display autocomplete suggestions.
        private ListBox autoCompleteBox;
        // PT: Índice da linha atualmente sendo editada no DataGridView.
        // EN: Index of the row currently being edited in the DataGridView.
        private int _indiceLinhaEditada = -1;
        // PT: Coleção de fontes privadas para carregar a fonte do jogo.
        // EN: Private font collection to load the game's font.
        private PrivateFontCollection pfc = new PrivateFontCollection();
        // PT: Objeto de fonte que representa a fonte do jogo.
        // EN: Font object representing the game's font.
        private Font fonteDoJogo;
        // PT: Código da versão do jogo atualmente carregado (ex: "GMU", "PMU").
        // EN: Version code of the currently loaded game (e.g., "GMU", "PMU").
        private string codigoVersaoAtual = "";
        // PT: Dicionário para armazenar os dados das tags (ex: nomes de itens, pessoas).
        // EN: Dictionary to store tag data (e.g., item names, people names).
        private Dictionary<string, List<string>> dadosDasTags = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        // NOVO: Dicionário para fazer o lookup reverso (Nome -> ID) de forma rápida ao salvar.
        // PT: Dicionário para fazer o lookup reverso (Nome -> ID) de forma rápida ao salvar.
        // EN: Dictionary for quick reverse lookup (Name -> ID) when saving.
        private Dictionary<string, Dictionary<string, ushort>> reverseDadosDasTags = new Dictionary<string, Dictionary<string, ushort>>(StringComparer.OrdinalIgnoreCase);

        // PT: Mapeia nomes de instruções para a chave correspondente em 'dadosDasTags'.
        // EN: Maps instruction names to the corresponding key in 'dadosDasTags'.
        private readonly Dictionary<string, string> instructionNameToTagKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        // PT: Mapeia a instrução "CROP" para a lista de "farmcrop".
        // EN: Maps the "CROP" instruction to the "farmcrop" list.
        { "CROP", "farmcrop" },
        // PT: Mapeia a instrução "STRUCTURE" para a lista de "structure".
        // EN: Maps the "STRUCTURE" instruction to the "structure" list.
        { "STRUCTURE", "structure" },
        // PT: Mapeia a instrução "STRING" para a lista de "string".
        // EN: Maps the "STRING" instruction to the "string" list.
        { "STRING", "string" },
        // PT: Mapeia a instrução "PHRASE" para a lista de "phrase".
        // EN: Maps the "PHRASE" instruction to the "phrase" list.
        { "PHRASE", "phrase" },
        // PT: Mapeia a instrução "ITEM" para a lista de "item".
        // EN: Maps the "ITEM" instruction to the "item" list.
        { "ITEM", "item" },
        // PT: Mapeia a instrução "PEOPLE" para a lista de "people".
        // EN: Maps the "PEOPLE" instruction to the "people" list.
        { "PEOPLE", "people" }
    };

        // PT: Expressão regular para encontrar tags aninhadas (ex: {TAG-{VALOR}}).
        // EN: Regular expression to find nested tags (e.g., {TAG-{VALUE}}).
        private static readonly Regex NestedTagPattern = new Regex(@"\{([A-Z0-9_]+)-([^{}]+)\}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        // PT: Expressão regular para colapsar múltiplos espaços em um só.
        // EN: Regular expression to collapse multiple whitespace characters into a single one.
        private static readonly Regex CollapseWhitespacePattern = new Regex(@"\s+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // PT: Classe interna para representar uma instrução do script de texto.
        // EN: Inner class to represent a text script instruction.
        private class Instruction
        {
            // PT: Nome da instrução (ex: "COLOR", "ITEM").
            // EN: Name of the instruction (e.g., "COLOR", "ITEM").
            public string Name { get; set; }
            // PT: Código de operação (opcode) da instrução.
            // EN: Operation code (opcode) of the instruction.
            public ushort Opcode { get; set; }
            // PT: Lista dos tipos de parâmetros que a instrução espera.
            // EN: List of parameter types the instruction expects.
            public List<string> ParamTypes { get; set; } = new List<string>();
        }

        // PT: Dicionário que mapeia nomes de exibição para caminhos de arquivos de mapa.
        // EN: Dictionary that maps display names to map file paths.
        private readonly Dictionary<string, string> mapFiles = new Dictionary<string, string>();
        // PT: Mapa principal de caracteres (código -> caractere/string).
        // EN: Main character map (code -> character/string).
        private readonly Dictionary<ushort, string> masterMap = new Dictionary<ushort, string>();
        // PT: Mapa de instruções (opcode -> objeto Instruction).
        // EN: Instruction map (opcode -> Instruction object).
        private readonly Dictionary<ushort, Instruction> instructionMap = new Dictionary<ushort, Instruction>();
        // PT: Dicionário de subtabelas para parâmetros específicos.
        // EN: Dictionary of subtables for specific parameters.
        private readonly Dictionary<string, Dictionary<ushort, string>> subTables = new Dictionary<string, Dictionary<ushort, string>>();
        // PT: Lista para armazenar os bytes brutos de cada mensagem do arquivo .MES.
        // EN: List to store the raw bytes of each message from the .MES file.
        private readonly List<byte[]> rawMesMessages = new List<byte[]>();
        // PT: Caminho do arquivo atualmente aberto.
        // EN: Path of the currently open file.
        private string currentFilePath = "";
        // PT: Flag que indica se o arquivo é Big Endian (GameCube) ou Little Endian (PS2).
        // EN: Flag indicating if the file is Big Endian (GameCube) or Little Endian (PS2).
        private bool isBigEndian = false;

        // PT: Mapas reversos para facilitar a codificação (string -> código).
        // EN: Reverse maps to facilitate encoding (string -> code).
        private readonly Dictionary<string, ushort> reverseMasterMap = new Dictionary<string, ushort>();
        private readonly Dictionary<string, Instruction> reverseInstructionMap = new Dictionary<string, Instruction>();
        private readonly Dictionary<string, Dictionary<string, ushort>> reverseSubTables = new Dictionary<string, Dictionary<string, ushort>>();



        public mesEditor()
        {
            // PT: Inicializa os componentes do formulário desenhados no designer.
            // EN: Initializes the form components drawn in the designer.
            InitializeComponent();
        }

        #region Eventos do Formulário e Controles

        // PT: Evento disparado pelo Timer de Undo. Salva o estado atual do texto.
        // EN: Event triggered by the Undo Timer. Saves the current state of the text.
        private void TimerUndo_Tick(object sender, EventArgs e)
        {
            _timerUndo.Stop();
            // Adiciona o estado atual do texto na pilha de desfazer
            _historicoUndo.Push(txtEditorDetalhe.Text);
            // Uma nova ação limpa a pilha de refazer
            _historicoRedo.Clear();
        }

        // PT: Evento disparado quando o formulário é carregado. Realiza a inicialização da UI e dos dados.
        // EN: Event triggered when the form is loaded. Performs UI and data initialization.
        private void mesEditor_Load(object sender, EventArgs e)
        {
            // PT: Desabilita os menus de salvar, pois nenhum arquivo está aberto.
            // EN: Disables the save menus as no file is open.
            saveMenu.Enabled = false;
            saveAsMenu.Enabled = false;

            // PT: Popula o ComboBox de limite de caracteres se estiver vazio.
            // EN: Populates the character limit ComboBox if it is empty.
            if (this.comboBoxlimit.Items.Count == 0)
            {
                this.comboBoxlimit.Items.AddRange(new object[] { "21", "42" });
            }
            // PT: Define o valor padrão para o limite de caracteres.
            // EN: Sets the default value for the character limit.
            this.comboBoxlimit.SelectedIndex = 0;
            // PT: Define o tamanho mínimo do formulário.
            // EN: Sets the minimum size of the form.
            this.MinimumSize = new System.Drawing.Size(550, 400);

            // PT: Configurações iniciais do DataGridView.
            // EN: Initial settings for the DataGridView.
            dataGridText.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridText.Columns.Clear();
            dataGridText.ReadOnly = true;
            // PT: Adiciona as colunas ao DataGridView.
            // EN: Adds the columns to the DataGridView.
            dataGridText.Columns.Add("Message", "Message");
            dataGridText.Columns.Add("Original", "Texto Original");
            dataGridText.Columns.Add("Edited", "Editado");
            dataGridText.Columns.Add("Estado", "Estado");
            // PT: Configura as propriedades de cada coluna.
            // EN: Configures the properties of each column.
            dataGridText.Columns["Message"].ReadOnly = true;
            dataGridText.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridText.Columns["Original"].ReadOnly = true;
            dataGridText.Columns["Original"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridText.Columns["Edited"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridText.Columns["Edited"].ReadOnly = true;
            dataGridText.Columns["Estado"].ReadOnly = true;
            dataGridText.Columns["Estado"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            // PT: Habilita a quebra de linha automática nas células.
            // EN: Enables automatic line wrapping in cells.
            dataGridText.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            // PT: Ajusta a altura das linhas automaticamente com base no conteúdo.
            // EN: Adjusts row height automatically based on content.
            dataGridText.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // PT: Inicializa a caixa de autocompletar e carrega os arquivos de mapa.
            // EN: Initializes the autocomplete box and loads the map files.
            InitializeAutoCompleteBox();
            LoadSharedFiles();
            LoadMapFiles();

            // PT: Se houver mapas de codificação, dispara o evento para carregar o primeiro.
            // EN: If there are encoding maps, triggers the event to load the first one.
            if (comboBoxCodification.Items.Count > 0)
            {
                comboBoxCodification_SelectedIndexChanged(sender, e);
            }
            // PT: Se houver uma linha selecionada, preenche a caixa de edição detalhada.
            // EN: If a row is selected, fills the detailed edit box.
            if (dataGridText.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridText.SelectedRows[0];
                string textoParaEditar = selectedRow.Cells["Edited"].Value?.ToString() ?? "";
                txtEditorDetalhe.Text = textoParaEditar;
            }
            else
            {
                // PT: Se não, deixa a caixa de edição vazia.
                // EN: Otherwise, leaves the edit box empty.
                txtEditorDetalhe.Text = "";
            }
            // PT: Associa os manipuladores de eventos aos controles.
            // EN: Associates the event handlers with the controls.
            this.txtEditorDetalhe.TextChanged += new System.EventHandler(this.txtEditorDetalhe_TextChanged);
            this.dataGridText.SelectionChanged += new System.EventHandler(this.dataGridText_SelectionChanged);
            this.txtEditorDetalhe.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtEditorDetalhe_KeyDown);

            // PT: Tenta carregar a fonte customizada da pasta "Fonts".
            // EN: Tries to load the custom font from the "Fonts" folder.
            try
            {
                // PT: Define o caminho da pasta de fontes.
                // EN: Defines the path to the fonts folder.
                string pastaDeFontes = Path.Combine(Application.StartupPath, "Fonts");
                string caminhoDaFonte = null;

                // PT: Verifica se a pasta de fontes existe.
                // EN: Checks if the fonts folder exists.
                if (Directory.Exists(pastaDeFontes))
                {
                    // PT: Define as extensões de arquivo de fonte permitidas.
                    // EN: Defines the allowed font file extensions.
                    var extensoesPermitidas = new[] { ".otf", ".ttf", ".ttc" };
                    // PT: Busca por arquivos de fonte na pasta.
                    // EN: Searches for font files in the folder.
                    var arquivosDeFonte = Directory.GetFiles(pastaDeFontes)
                        .Where(f => extensoesPermitidas.Contains(Path.GetExtension(f).ToLower()))
                        .OrderBy(f => f)
                        .ToList();

                    // PT: Se encontrar apenas um arquivo, usa ele.
                    // EN: If only one file is found, use it.
                    if (arquivosDeFonte.Count == 1)
                    {
                        caminhoDaFonte = arquivosDeFonte[0];
                    }
                    // PT: Se encontrar mais de um, usa o primeiro e avisa o usuário.
                    // EN: If more than one is found, use the first one and notify the user.
                    else if (arquivosDeFonte.Count > 1)
                    {
                        caminhoDaFonte = arquivosDeFonte[0];
                        MessageBox.Show($"Foram encontrados {arquivosDeFonte.Count} arquivos de fonte na pasta 'Fonts'.\n" +
                                        $"O programa carregará o primeiro em ordem alfabética:\n" +
                                        $"{Path.GetFileName(caminhoDaFonte)}",
                                        "Aviso: Múltiplas Fontes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // PT: Se um caminho de fonte válido foi encontrado, carrega a fonte.
                // EN: If a valid font path was found, load the font.
                if (!string.IsNullOrEmpty(caminhoDaFonte) && File.Exists(caminhoDaFonte))
                {
                    pfc.AddFontFile(caminhoDaFonte);
                    fonteDoJogo = new Font(pfc.Families[0], 12f, FontStyle.Regular);
                }
                else
                {
                    // PT: Se nenhuma fonte foi encontrada, mostra um erro e usa uma fonte padrão.
                    // EN: If no font was found, show an error and use a default font.
                    MessageBox.Show("Nenhum arquivo de fonte (.otf, .ttf, .ttc) foi encontrado na pasta 'Fonts'.\n" +
                                    "Usando fonte padrão do sistema (Arial).",
                                    "Fonte não encontrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    fonteDoJogo = new Font("Arial", 12f);
                }
            }
            // PT: Captura exceções durante o carregamento da fonte.
            // EN: Catches exceptions during font loading.
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro inesperado ao carregar a fonte:\n" + ex.Message, "Erro Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                fonteDoJogo = new Font("Arial", 12f);
            }

            // PT: Inicializa as pilhas de Undo/Redo.
            // EN: Initializes the Undo/Redo stacks.
            _historicoUndo = new Stack<string>();
            _historicoRedo = new Stack<string>();
            _alteracaoEmProgresso = false;

            // PT: Configura o Timer para agrupar alterações e não sobrecarregar a pilha de Undo.
            // EN: Configures the Timer to group changes and not overload the Undo stack.
            _timerUndo = new Timer();
            _timerUndo.Interval = 100; // PT: Intervalo de 100ms de inatividade para salvar um estado. EN: 100ms inactivity interval to save a state.
            _timerUndo.Tick += TimerUndo_Tick;

        }
        // PT: Evento disparado quando a seleção no DataGridView muda.
        // EN: Event triggered when the selection in the DataGridView changes.
        private void dataGridText_SelectionChanged(object sender, EventArgs e)
        {
            // PT: Verifica se há alguma linha selecionada.
            // EN: Checks if any row is selected.
            if (dataGridText.SelectedRows.Count > 0)
            {
                // PT: Pega a primeira linha selecionada.
                // EN: Gets the first selected row.
                DataGridViewRow selectedRow = dataGridText.SelectedRows[0];
                // PT: Armazena o índice da linha e obtém o texto editado.
                // EN: Stores the row index and gets the edited text.
                _indiceLinhaEditada = selectedRow.Index;
                string textoParaEditar = selectedRow.Cells["Edited"].Value?.ToString() ?? "";
                // PT: Desativa temporariamente o evento TextChanged para evitar loops.
                // EN: Temporarily disables the TextChanged event to avoid loops.
                this.txtEditorDetalhe.TextChanged -= txtEditorDetalhe_TextChanged;
                txtEditorDetalhe.Text = textoParaEditar;
                this.txtEditorDetalhe.TextChanged += txtEditorDetalhe_TextChanged;
                // PT: Habilita a caixa de texto para edição.
                // EN: Enables the textbox for editing.
                txtEditorDetalhe.Enabled = true;
                // PT: Limpa o histórico de Undo/Redo ao selecionar uma nova linha.
                // EN: Clears the Undo/Redo history when selecting a new row.
                _historicoUndo.Clear();
                _historicoRedo.Clear();
                // PT: Adiciona o texto inicial como o primeiro estado no histórico de Undo.
                // EN: Adds the initial text as the first state in the Undo history.
                _historicoUndo.Push(txtEditorDetalhe.Text);
            }
            else
            {
                // PT: Se nenhuma linha estiver selecionada, desabilita a edição.
                // EN: If no row is selected, disables editing.
                _indiceLinhaEditada = -1;
                txtEditorDetalhe.Text = "";
                txtEditorDetalhe.Enabled = false;
            }

            // PT: Redesenha a pré-visualização do texto.
            // EN: Redraws the text preview.
            DesenharPreview();
        }
        // PT: Evento disparado quando o texto na caixa de edição detalhada muda.
        // EN: Event triggered when the text in the detailed edit box changes.
        private void txtEditorDetalhe_TextChanged(object sender, EventArgs e)
        {
            // PT: Se uma alteração já está em progresso, retorna para evitar loops.
            // EN: If a change is already in progress, return to avoid loops.
            if (_alteracaoEmProgresso) return;

            // PT: Se uma linha válida está selecionada, atualiza a célula correspondente no DataGridView.
            // EN: If a valid row is selected, updates the corresponding cell in the DataGridView.
            if (_indiceLinhaEditada > -1 && _indiceLinhaEditada < dataGridText.RowCount)
            {
                DataGridViewRow linhaParaAtualizar = dataGridText.Rows[_indiceLinhaEditada];
                string textoEditado = txtEditorDetalhe.Text;
                // PT: Atualiza o valor da célula "Editado" e "Estado".
                // EN: Updates the value of the "Edited" and "Estado" cells.
                linhaParaAtualizar.Cells["Edited"].Value = textoEditado;
                linhaParaAtualizar.Cells["Estado"].Value = CalculateLineStates(textoEditado);
                // PT: Atualiza o estilo da linha (ex: cor de fundo para erros).
                // EN: Updates the row style (e.g., background color for errors).
                AtualizarEstiloDaLinha(linhaParaAtualizar);
            }

            // PT: Lógica para o autocompletar.
            // EN: Logic for autocomplete.
            var tb = sender as TextBox;
            if (tb == null) return;

            // PT: Obtém a posição do cursor.
            // EN: Gets the cursor position.
            int cursorPos = tb.SelectionStart;
            if (cursorPos > 0)
            {
                // PT: Verifica se o cursor está dentro de uma tag iniciada com '{'.
                // EN: Checks if the cursor is inside a tag starting with '{'.
                int tagStartPos = tb.Text.LastIndexOf('{', cursorPos - 1);
                int tagEndPos = tb.Text.LastIndexOf('}', cursorPos - 1);

                // PT: Se o cursor está dentro de uma tag aberta.
                // EN: If the cursor is inside an open tag.
                if (tagStartPos != -1 && tagStartPos > tagEndPos)
                {
                    // PT: Extrai o conteúdo da tag até o cursor.
                    // EN: Extracts the tag content up to the cursor.
                    string contentInside = tb.Text.Substring(tagStartPos + 1, cursorPos - (tagStartPos + 1));
                    List<string> suggestions = new List<string>();

                    // PT: Se a tag contém um '-', trata como instrução com parâmetro.
                    // EN: If the tag contains a '-', treat it as an instruction with a parameter.
                    if (contentInside.Contains("-"))
                    {
                        var parts = contentInside.Split(new[] { '-' }, 2);
                        string instrName = parts[0];
                        string partialParam = parts.Length > 1 ? parts[1] : "";

                        // PT: Busca por sugestões nos dados das tags.
                        // EN: Searches for suggestions in the tag data.
                        if (dadosDasTags.ContainsKey(instrName))
                        {
                            suggestions = dadosDasTags[instrName]
                                .Where(name => name.StartsWith(partialParam, StringComparison.OrdinalIgnoreCase))
                                .Take(200) // Limita para performance
                                .ToList();
                        }
                    }
                    else
                    {
                        // PT: Se não, trata como nome de instrução.
                        // EN: Otherwise, treat it as an instruction name.
                        suggestions = reverseInstructionMap.Keys
                            .Where(k => k.StartsWith(contentInside, StringComparison.OrdinalIgnoreCase))
                            .OrderBy(k => k)
                            .ToList();
                    }

                    if (suggestions.Any())
                    {
                        // PT: Se houver sugestões, exibe a caixa de autocompletar.
                        // EN: If there are suggestions, display the autocomplete box.
                        autoCompleteBox.DataSource = suggestions;
                        Point cursorPoint = tb.GetPositionFromCharIndex(tagStartPos);
                        // PT: Posiciona a caixa de autocompletar abaixo do cursor.
                        // EN: Positions the autocomplete box below the cursor.
                        autoCompleteBox.Location = new Point(
                            tb.Left + cursorPoint.X,
                            tb.Top + cursorPoint.Y + tb.Font.Height + 2
                        );
                        autoCompleteBox.Visible = true;
                        autoCompleteBox.BringToFront();
                    }
                    // PT: Se não houver sugestões, esconde a caixa.
                    // EN: If there are no suggestions, hide the box.
                    else { autoCompleteBox.Visible = false; }
                }
                // PT: Se o cursor não está em uma tag, esconde a caixa.
                // EN: If the cursor is not in a tag, hide the box.
                else { autoCompleteBox.Visible = false; }
            }
            // PT: Se o cursor está no início, esconde a caixa.
            // EN: If the cursor is at the beginning, hide the box.
            else { autoCompleteBox.Visible = false; }

            // PT: Redesenha a pré-visualização.
            // EN: Redraws the preview.
            DesenharPreview();

            // PT: Reinicia o timer de Undo para registrar a alteração.
            // EN: Resets the Undo timer to register the change.
            _timerUndo.Stop();
            _timerUndo.Start();
        }
        private void txtEditorDetalhe_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb == null) return;

            // PT: Lógica de navegação e seleção na caixa de autocompletar.
            // EN: Navigation and selection logic in the autocomplete box.
            if (autoCompleteBox.Visible)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        e.SuppressKeyPress = true;
                        if (autoCompleteBox.SelectedIndex < autoCompleteBox.Items.Count - 1) autoCompleteBox.SelectedIndex++;
                        return;
                    case Keys.Up:
                        e.SuppressKeyPress = true;
                        if (autoCompleteBox.SelectedIndex > 0) autoCompleteBox.SelectedIndex--;
                        return;
                    case Keys.Enter:
                    case Keys.Tab:
                        e.SuppressKeyPress = true;
                        AutoCompleteBox_ItemSelected(sender, null);
                        return;
                    case Keys.Escape:
                        e.SuppressKeyPress = true;
                        autoCompleteBox.Visible = false;
                        return;
                }
            }

            // PT: Lógica para deletar uma tag inteira de uma vez.
            // EN: Logic to delete an entire tag at once.
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                int cursorPos = tb.SelectionStart;
                // PT: Se houver texto selecionado, não faz nada.
                // EN: If there is selected text, do nothing.
                if (tb.SelectionLength > 0) return;
                // PT: Encontra todas as tags no texto.
                // EN: Finds all tags in the text.
                MatchCollection matches = Regex.Matches(tb.Text, @"\{[^}]+\}");
                foreach (Match match in matches)
                {
                    // PT: Se o cursor está dentro de uma tag, seleciona e deleta a tag inteira.
                    // EN: If the cursor is inside a tag, select and delete the entire tag.
                    if (cursorPos > match.Index && cursorPos <= match.Index + match.Length)
                    {
                        e.SuppressKeyPress = true;
                        tb.Select(match.Index, match.Length);
                        tb.SelectedText = "";
                        break;
                    }
                }
            }
        }

        // PT: Evento disparado quando o item selecionado no ComboBox de codificação muda.
        // EN: Event triggered when the selected item in the encoding ComboBox changes.
        private void comboBoxCodification_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCodification.SelectedItem == null) return;
            string selectedMapName = comboBoxCodification.SelectedItem.ToString();
            // PT: Tenta obter o caminho do arquivo de mapa selecionado.
            // EN: Tries to get the path of the selected map file.
            if (mapFiles.TryGetValue(selectedMapName, out string mapPath))
            {
                // PT: Carrega o mapa de caracteres e atualiza as linhas da grade se houver dados.
                // EN: Loads the character map and updates the grid rows if there is data.
                LoadCharacterMap(mapPath);
                if (rawMesMessages.Any())
                {
                    UpdateAllRows();
                }
            }
        }
        // PT: Evento disparado quando o item selecionado no ComboBox de limite de caracteres muda.
        // EN: Event triggered when the selected item in the character limit ComboBox changes.
        private void comboBoxlimit_SelectedIndexChanged(object sender, EventArgs e)
        {
            // PT: Se houver linhas na grade, atualiza o estado de todas.
            // EN: If there are rows in the grid, updates the state of all of them.
            if (dataGridText.Rows.Count > 0)
            {
                UpdateAllRowsState();
            }
            // PT: Redesenha a pré-visualização com o novo limite.
            // EN: Redraws the preview with the new limit.
            DesenharPreview();
        }
        // PT: Desenha a pré-visualização do texto na PictureBox, simulando a aparência no jogo.
        // EN: Draws the text preview in the PictureBox, simulating the in-game appearance.
        private void DesenharPreview()
        {
            // PT: Usa o painel container da pré-visualização como referência de tamanho.
            // EN: Uses the preview container panel as a size reference.
            Control previewContainer = panelPreviewContainer ?? pictureBoxPreview.Parent;

            if (fonteDoJogo == null || previewContainer.Width <= 0 || comboBoxlimit.SelectedItem == null)
            {
                pictureBoxPreview.Image = null;
                return;
            }

            int bitmapWidth = previewContainer.ClientSize.Width - 20;
            // PT: Garante que a largura do bitmap seja no mínimo 1.
            // EN: Ensures the bitmap width is at least 1.
            if (bitmapWidth < 1) bitmapWidth = 1;

            // PT: A altura será calculada dinamicamente. Começamos com a altura do painel.
            // EN: The height will be calculated dynamically. We start with the panel's height.
            Bitmap bmp = new Bitmap(bitmapWidth, previewContainer.Height);
            float totalHeight = 5;

            // PT: Conjunto de prefixos de tags que não devem ser quebradas no meio.
            // EN: Set of tag prefixes that should not be broken in the middle.
            var unbreakableTagPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CLEAR", "PHRASE" , "COLOR", "COLORRGBA", "CHOICE", "CHOICEYN", "GOLD", "WAIT", "PLAYSFX", "FACE", "PREVINPUT", "TEXTSIZE"
    };

            // ETAPA 1: Pré-cálculo da altura total do texto
            // EN: STEP 1: Pre-calculation of the total text height
            using (Graphics g = Graphics.FromImage(bmp)) // PT: Usa um gráfico temporário para medição. EN: Uses a temporary graphics object for measurement.
            {
                string texto = txtEditorDetalhe.Text;
                int limitePorLinha = int.Parse(comboBoxlimit.SelectedItem.ToString());
                float posX = 5;
                float posY = 5;
                int charCount = 0;
                Brush pincelAtual = Brushes.White; // Pincel temporário, só para cálculo
                // PT: Divide o texto em segmentos (texto normal e tags).
                // EN: Splits the text into segments (normal text and tags).
                var segmentos = Regex.Split(texto, @"(\{[^}]+\})").Where(s => !string.IsNullOrEmpty(s));
                float lineHeight = fonteDoJogo.GetHeight(g);

                foreach (var segmento in segmentos)
                {
                    // PT: Variáveis para o texto a ser desenhado e se a tag é inquebrável.
                    // EN: Variables for the text to be drawn and whether the tag is unbreakable.
                    string textoParaDesenhar;
                    bool isUnbreakable = false;

                    if (segmento.StartsWith("{") && segmento.EndsWith("}"))
                    {
                        string tagInterna = segmento.Trim('{', '}');
                        string tagName = tagInterna.Contains("-") ? tagInterna.Split('-')[0] : tagInterna;
                        if (unbreakableTagPrefixes.Contains(tagName)) isUnbreakable = true;
                        textoParaDesenhar = ProcessarTagPreview(segmento, ref pincelAtual);
                    }
                    else
                    {
                        textoParaDesenhar = segmento.Replace(Environment.NewLine, "\n");
                    }
                    if (string.IsNullOrEmpty(textoParaDesenhar)) continue;

                    // PT: Se a tag for inquebrável, calcula sua largura e quebra de linha.
                    // EN: If the tag is unbreakable, calculates its width and line breaks.
                    if (isUnbreakable)
                    {
                        var partesDaTag = textoParaDesenhar.Split('\n');
                        for (int i = 0; i < partesDaTag.Length; i++)
                        {
                            string parte = partesDaTag[i];
                            if (!string.IsNullOrEmpty(parte))
                            {
                                posX += g.MeasureString(parte, fonteDoJogo).Width;
                            }
                            if (i < partesDaTag.Length - 1)
                            {
                                posY += lineHeight; posX = 5; charCount = 0;
                            }
                        }
                    }
                    else
                    {
                        // PT: Processa cada caractere do texto normal.
                        // EN: Processes each character of the normal text.
                        foreach (char c in textoParaDesenhar)
                        {
                            if (c == '\n') { posY += lineHeight; posX = 5; charCount = 0; continue; }
                            if (charCount == limitePorLinha) { posY += lineHeight; posX = 5; charCount = 0; }
                            posX += g.MeasureString(c.ToString(), fonteDoJogo).Width;
                            charCount++;
                        }
                    }
                }
                // PT: linha em branco adicional para facilitar a leitura.
                //EN: additional blank line for readability.
                float extraBottomPadding = lineHeight;
                // PT: Adiciona um preenchimento extra na parte inferior para melhor visualização.
                // EN: Adds extra padding at the bottom for better visualization.
                totalHeight = posY + lineHeight + extraBottomPadding;
                // PT: Adiciona a altura da linha atual mais uma linha extra de padding no final.
                // EN: Adds the height of the current line plus an extra line of padding at the end.
                // PT: Isso garante um espaço em branco para melhor visualização.
                // EN: This ensures a blank space for better visualization.
                totalHeight = posY + (lineHeight * 2);
            }
            // PT: Calcula a nova altura do PictureBox, garantindo que seja pelo menos a altura do container.
            // EN: Calculates the new height of the PictureBox, ensuring it is at least the height of the container.
            int newHeight = (int)Math.Max(previewContainer.ClientSize.Height, totalHeight);
            pictureBoxPreview.Size = new Size(bitmapWidth, newHeight);
            pictureBoxPreview.Location = new Point(0, 0);
            // PT: Se o container for rolável, ajusta o tamanho mínimo de rolagem.
            // EN: If the container is scrollable, adjusts the minimum scroll size.
            if (previewContainer is ScrollableControl scrollable)
            {
                scrollable.AutoScrollMinSize = new Size(bitmapWidth, newHeight);
            }
            // Descarta o bitmap de cálculo e cria o bitmap final com o tamanho correto
            // PT: Descarta o bitmap de cálculo e cria o bitmap final com o tamanho correto.
            // EN: Discards the calculation bitmap and creates the final bitmap with the correct size.
            bmp.Dispose();
            bmp = new Bitmap(bitmapWidth, newHeight);
            // ETAPA 2: Desenho real no bitmap final
            // EN: STEP 2: Actual drawing on the final bitmap
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(pictureBoxPreview.BackColor);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                string texto = txtEditorDetalhe.Text;
                int limitePorLinha = int.Parse(comboBoxlimit.SelectedItem.ToString());
                float posX = 5;
                float posY = 5;
                int charCount = 0;
                Brush pincelAtual = Brushes.White;
                var segmentos = Regex.Split(texto, @"(\{[^}]+\})").Where(s => !string.IsNullOrEmpty(s));
                float lineHeight = fonteDoJogo.GetHeight(g);

                foreach (var segmento in segmentos)
                {
                    // PT: A lógica aqui é uma repetição da etapa de cálculo, mas agora com o desenho real.
                    // EN: The logic here is a repetition of the calculation step, but now with the actual drawing.
                    string textoParaDesenhar;
                    bool isUnbreakable = false;
                    if (segmento.StartsWith("{") && segmento.EndsWith("}"))
                    {
                        string tagInterna = segmento.Trim('{', '}');
                        string tagName = tagInterna.Contains("-") ? tagInterna.Split('-')[0] : tagInterna;
                        if (unbreakableTagPrefixes.Contains(tagName)) isUnbreakable = true;
                        textoParaDesenhar = ProcessarTagPreview(segmento, ref pincelAtual);
                    }
                    else
                    {
                        textoParaDesenhar = segmento.Replace(Environment.NewLine, "\n");
                    }
                    if (string.IsNullOrEmpty(textoParaDesenhar)) continue;

                    // PT: Desenha o texto da tag inquebrável.
                    // EN: Draws the text of the unbreakable tag.
                    if (isUnbreakable)
                    {
                        var partesDaTag = textoParaDesenhar.Split('\n');
                        for (int i = 0; i < partesDaTag.Length; i++)
                        {
                            string parte = partesDaTag[i];
                            if (!string.IsNullOrEmpty(parte))
                            {
                                g.DrawString(parte, fonteDoJogo, pincelAtual, posX, posY);
                                posX += g.MeasureString(parte, fonteDoJogo).Width;
                            }
                            if (i < partesDaTag.Length - 1)
                            {
                                posY += lineHeight; posX = 5; charCount = 0;
                            }
                        }
                    }
                    else
                    {
                        // PT: Desenha o texto normal, caractere por caractere.
                        // EN: Draws the normal text, character by character.
                        foreach (char c in textoParaDesenhar)
                        {
                            if (c == '\n') { posY += lineHeight; posX = 5; charCount = 0; continue; }
                            if (charCount == limitePorLinha) { posY += lineHeight; posX = 5; charCount = 0; }
                            string charString = c.ToString();
                            g.DrawString(charString, fonteDoJogo, pincelAtual, posX, posY);
                            posX += g.MeasureString(charString, fonteDoJogo).Width;
                            charCount++;
                        }
                    }
                }
            }

            // PT: Libera a imagem antiga e atribui a nova ao PictureBox.
            // EN: Releases the old image and assigns the new one to the PictureBox.
            pictureBoxPreview.Image?.Dispose();
            pictureBoxPreview.Image = bmp;
        }

        // PT: Processa uma tag específica e retorna o texto a ser exibido na pré-visualização, atualizando o pincel de cor se necessário.
        // EN: Processes a specific tag and returns the text to be displayed in the preview, updating the color brush if necessary.
        private string ProcessarTagPreview(string tag, ref Brush pincelAtual)
        {
            string tagInterna = tag.Trim('{', '}');

            if (tagInterna.Equals("PREVINPUT-128", StringComparison.OrdinalIgnoreCase))
            {
                return "{ANIMALNAME}"; // Retorna um caractere de quebra de linha
            }

            // PT: Processa a tag de cor {COLOR-...}.
            // EN: Processes the {COLOR-...} tag.
            if (tagInterna.StartsWith("COLOR-", StringComparison.OrdinalIgnoreCase))
            {
                string cor = tagInterna.Substring(6);
                switch (cor.ToUpper())
                {
                    case "BLACK": pincelAtual = Brushes.Gray; break;
                    case "RED": pincelAtual = Brushes.Red; break;
                    case "GREEN": pincelAtual = Brushes.LimeGreen; break;
                    case "BLUE": pincelAtual = Brushes.DodgerBlue; break;
                    case "BROWN": pincelAtual = Brushes.SaddleBrown; break;
                    case "NORMAL": pincelAtual = Brushes.White; break;
                    default: pincelAtual = Brushes.White; break;
                }
                return null;
            }
            // PT: Processa a tag de cor RGBA {COLORRGBA-...}.
            // EN: Processes the {COLORRGBA-...} tag.
            if (tagInterna.StartsWith("COLORRGBA-", StringComparison.OrdinalIgnoreCase))
            {
                string hexValue = tagInterna.Substring(10).Replace("0x", "");
                if (uint.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint rgbaValue))
                {
                    // Converte de RGBA (formato do jogo) para ARGB (formato do C#/.NET)
                    uint r = (rgbaValue >> 24) & 0xFF;
                    uint g = (rgbaValue >> 16) & 0xFF;
                    uint b = (rgbaValue >> 8) & 0xFF;
                    uint a = rgbaValue & 0xFF;

                    // Cria a cor a partir dos componentes ARGB
                    Color corCustomizada = Color.FromArgb((int)a, (int)r, (int)g, (int)b);

                    if (corCustomizada.R == 0 && corCustomizada.G == 0 && corCustomizada.B == 0)
                    {
                        corCustomizada = Color.Gray; // Troca preto por cinza
                    }

                    // Cria um novo pincel com a cor customizada
                    pincelAtual = new SolidBrush(corCustomizada);
                }
                return null; // A tag de cor não gera texto
            }
            // PT: Processa a tag de escolha {CHOICE-...}.
            // EN: Processes the {CHOICE-...} tag.
            if (tagInterna.StartsWith("CHOICE-", StringComparison.OrdinalIgnoreCase))
            {
                // Se encontrar, retorna o emoji para ser desenhado no lugar da tag.
                return "\n\n►";
            }
            if (tagInterna.StartsWith("CHOICEYN-", StringComparison.OrdinalIgnoreCase))
            {
                return "\n\nYES\nNO";
            }
            // PT: Processa outras tags que têm uma representação textual simples.
            // EN: Processes other tags that have a simple textual representation.
            if (tagInterna.StartsWith("GOLD-", StringComparison.OrdinalIgnoreCase))
            {
                return "{GOLD}";
            }
            if (tagInterna.StartsWith("WAIT-", StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }
            if (tagInterna.StartsWith("PLAYSFX-", StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }
            if (tagInterna.StartsWith("FACE-", StringComparison.OrdinalIgnoreCase))
            {
                return "";
            }
            // PT: Processa tags genéricas com um parâmetro (ex: {ITEM-Milk}).
            // EN: Processes generic tags with a parameter (e.g., {ITEM-Milk}).
            if (tagInterna.Contains("-"))
            {
                var tagParts = tagInterna.Split(new[] { '-' }, 2);
                if (tagParts.Length == 2)
                {
                    string tagName = tagParts[0];
                    string tagValue = tagParts[1];

                    // Verifica se o valor do parâmetro é um número.
                    if (ushort.TryParse(tagValue, out _))
                    {
                        // PT: Se for um número, significa que a busca pelo nome falhou.
                        // EN: If it's a number, it means the name lookup failed.
                        // PT: Exibimos um placeholder genérico com o nome da tag em maiúsculas.
                        // EN: We display a generic placeholder with the tag name in uppercase.
                        return $"{{{tagName.ToUpper()}}}"; // Ex: {item}, {structure}, etc.
                    }
                    else
                    {
                        // PT: Se não for um número, é um nome válido (ex: "Pete"). Exibimos o nome.
                        // EN: If it's not a number, it's a valid name (e.g., "Pete"). We display the name.
                        return tagValue;
                    }
                }
            }

            // PT: Se a tag não for reconhecida, não retorna texto.
            // EN: If the tag is not recognized, returns no text.
            return null;
        }
        #endregion

        #region Lógica de Edição Avançada
        private void InitializeAutoCompleteBox()
        {
            // PT: Cria e configura a ListBox que será usada para o autocompletar.
            // EN: Creates and configures the ListBox to be used for autocomplete.
            autoCompleteBox = new ListBox
            {
                Visible = false,
                Font = new Font("Consolas", 9F),
            };

            autoCompleteBox.MouseDown += autoCompleteBox_MouseDown;
            this.Controls.Add(autoCompleteBox);
            autoCompleteBox.BringToFront();
        }
        // PT: Evento de clique do mouse na caixa de autocompletar para selecionar um item.
        // EN: Mouse click event on the autocomplete box to select an item.
        private void autoCompleteBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (autoCompleteBox.Items.Count == 0) return;
            int index = autoCompleteBox.IndexFromPoint(e.Location);
            if (index != -1)
            {
                autoCompleteBox.SelectedIndex = index;
                AutoCompleteBox_ItemSelected(sender, e);
            }
        }

        // PT: Insere o item selecionado da caixa de autocompletar no TextBox de edição.
        // EN: Inserts the selected item from the autocomplete box into the editing TextBox.
        private void AutoCompleteBox_ItemSelected(object sender, EventArgs e)
        {
            if (autoCompleteBox.SelectedItem == null) return;
            var tb = txtEditorDetalhe;
            if (tb == null) return;

            string selecionado = autoCompleteBox.SelectedItem.ToString();
            int cursorPos = tb.SelectionStart;
            int tagStartPos = tb.Text.LastIndexOf('{', cursorPos - 1);
            if (tagStartPos == -1) return;

            string textoAntesDaTag = tb.Text.Substring(0, tagStartPos + 1);
            string textoDepoisDoCursor = tb.Text.Substring(cursorPos);
            string contentInside = tb.Text.Substring(tagStartPos + 1, cursorPos - (tagStartPos + 1));
            string tagFinalParaInserir;

            // PT: Se o conteúdo já tem um hífen, completa o parâmetro.
            // EN: If the content already has a hyphen, completes the parameter.
            if (contentInside.Contains("-"))
            {
                string instrName = contentInside.Split('-')[0];
                tagFinalParaInserir = $"{instrName}-{selecionado}}}";
            }
            // PT: Se não, completa o nome da instrução.
            // EN: Otherwise, completes the instruction name.
            else
            {
                string nomeDaTag = selecionado;
                if (reverseInstructionMap.TryGetValue(nomeDaTag, out Instruction instruction) && instruction.ParamTypes.Any())
                {
                    tagFinalParaInserir = nomeDaTag + "-";
                }
                else
                {
                    tagFinalParaInserir = nomeDaTag + "}";
                }
            }

            // PT: Atualiza o texto no TextBox com a tag completada.
            // EN: Updates the text in the TextBox with the completed tag.
            tb.TextChanged -= txtEditorDetalhe_TextChanged;
            tb.Text = textoAntesDaTag + tagFinalParaInserir + textoDepoisDoCursor;
            tb.SelectionStart = textoAntesDaTag.Length + tagFinalParaInserir.Length;
            tb.TextChanged += txtEditorDetalhe_TextChanged;

            autoCompleteBox.Visible = false;
            tb.Focus();
        }


        #endregion

        #region Lógica do Menu "Arquivo"
        // PT: Manipulador de clique para o item de menu "Abrir".
        // EN: Click handler for the "Open" menu item.
        private async void openMenu_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Mensagem (*.mes, *.txt)|*.mes;*.txt|Todos os Arquivos (*.*)|*.*",
                Title = "Abrir Arquivo"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // PT: Atualiza o caminho do arquivo e o título da janela.
                    // EN: Updates the file path and the window title.
                    currentFilePath = openFileDialog.FileName;
                    this.Text = $"Mes Editor By Angel333119 - {Path.GetFileName(currentFilePath)}";
                    // PT: Limpa os dados antigos.
                    // EN: Clears the old data.
                    dataGridText.Rows.Clear();
                    rawMesMessages.Clear();
                    this.Cursor = Cursors.WaitCursor;
                    // PT: Determina o tipo de arquivo e o carrega de forma assíncrona.
                    // EN: Determines the file type and loads it asynchronously.
                    FileType fileType = DetermineFileType(currentFilePath);
                    switch (fileType)
                    {
                        case FileType.GameCubeMES:
                        case FileType.PS2MES:
                            await Task.Run(() => LoadMesFile(currentFilePath, fileType));
                            break;
                        case FileType.TextFile:
                            await Task.Run(() => LoadTxtFile(currentFilePath));
                            break;
                        default:
                            MessageBox.Show("Não foi possível determinar o tipo do arquivo.", "Erro");
                            break;
                    }
                    this.Cursor = Cursors.Default;
                }
            }
        }
        // PT: Manipulador de clique para o item de menu "Salvar".
        // EN: Click handler for the "Save" menu item.
        private void saveMenu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath) || !Path.GetExtension(currentFilePath).Equals(".MES", StringComparison.OrdinalIgnoreCase))
            {
                saveAsMenu_Click(sender, e);
                return;
            }
            WriteMesFile(currentFilePath);
        }
        // PT: Manipulador de clique para o item de menu "Salvar Como".
        // EN: Click handler for the "Save As" menu item.
        private void saveAsMenu_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog
            {
                Filter = "Arquivo MES (*.mes)|*.mes",
                Title = "Salvar Como..."
            })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WriteMesFile(saveFileDialog.FileName);
                }
            }
        }
        #endregion

        #region Métodos de Leitura e Decodificação
        // PT: Carrega um arquivo de texto (.txt) e preenche o DataGridView.
        // EN: Loads a text file (.txt) and populates the DataGridView.
        private void LoadTxtFile(string path)
        {
            try
            {
                var allLines = File.ReadAllLines(path);
                var messages = new List<string>();
                var currentMessageBuilder = new StringBuilder();
                bool isReadingMessage = false;
                foreach (var line in allLines)
                {
                    // PT: Procura por delimitadores de mensagem.
                    // EN: Looks for message delimiters.
                    if (line.StartsWith("--- message"))
                    {
                        if (isReadingMessage)
                        {
                            messages.Add(currentMessageBuilder.ToString().TrimEnd());
                        }
                        currentMessageBuilder.Clear();
                        isReadingMessage = true;
                    }
                    else if (isReadingMessage)
                    {
                        if (currentMessageBuilder.Length > 0)
                        {
                            currentMessageBuilder.AppendLine();
                        }
                        currentMessageBuilder.Append(line);
                    }
                }
                if (isReadingMessage)
                {
                    messages.Add(currentMessageBuilder.ToString().TrimEnd());
                }
                // PT: Invoca a atualização da UI na thread principal.
                // EN: Invokes the UI update on the main thread.
                this.Invoke((MethodInvoker)delegate
                {
                    saveMenu.Enabled = false;
                    saveAsMenu.Enabled = true;
                    for (int i = 0; i < messages.Count; i++)
                    {
                        string text = messages[i];
                        string stateText = CalculateLineStates(text);
                        dataGridText.Rows.Add(i, text, text, stateText);
                    }
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Ocorreu um erro ao ler o arquivo .TXT:\n{ex.Message}", "Erro de Leitura");
                });
            }
        }
        // PT: Carrega um arquivo de mensagem (.mes) e armazena os dados brutos.
        // EN: Loads a message file (.mes) and stores the raw data.
        private void LoadMesFile(string path, FileType fileType)
        {
            try
            {
                isBigEndian = (fileType == FileType.GameCubeMES);
                byte[] fileBytes = File.ReadAllBytes(path);

                //Lista para guardar as mensagens decodificadas na ordem correta.
                var decodedMessages = new List<string>();
                //Dicionário para cachear textos já decodificados (Ponteiro -> Texto).
                var pointerCache = new Dictionary<uint, string>();

                rawMesMessages.Clear(); // Limpa a lista antiga antes de carregar.

                using (var br = new BinaryReader(new MemoryStream(fileBytes)))
                {
                    br.BaseStream.Seek(4, SeekOrigin.Begin);
                    // PT: Lê a contagem de textos, considerando a endianidade.
                    // EN: Reads the text count, considering endianness.
                    uint textCount = isBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                    uint[] pointers = new uint[textCount];
                    // PT: Lê a tabela de ponteiros.
                    // EN: Reads the pointer table.
                    for (int i = 0; i < textCount; i++)
                    {
                        pointers[i] = isBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                    }
                    // PT: Itera sobre os ponteiros para extrair os bytes de cada mensagem.
                    // EN: Iterates over the pointers to extract the bytes of each message.
                    for (int i = 0; i < textCount; i++)
                    {
                        uint currentPointer = pointers[i];

                        //Lógica de cache
                        if (pointerCache.ContainsKey(currentPointer))
                        {
                            // Se o ponteiro já foi lido, reutiliza o texto do cache.
                            decodedMessages.Add(pointerCache[currentPointer]);
                            // Adiciona um array de bytes vazio a rawMesMessages para manter a contagem de mensagens consistente,
                            // embora a lógica de salvamento não use isso. Isso é apenas para consistência.
                            rawMesMessages.Add(new byte[0]);
                        }
                        else
                        {
                            // Se for um ponteiro novo, lê e decodifica.
                            uint nextPointer = (i + 1 < textCount) ? pointers[i + 1] : (uint)br.BaseStream.Length;

                            // Lógica para encontrar o fim real da mensagem em caso de ponteiros duplicados não sequenciais
                            if (currentPointer > 0)
                            {
                                uint realNextPointer = (uint)br.BaseStream.Length;
                                // Encontra o próximo ponteiro *diferente* para delimitar o tamanho
                                for (int j = i + 1; j < textCount; j++)
                                {
                                    if (pointers[j] > currentPointer)
                                    {
                                        realNextPointer = pointers[j];
                                        break;
                                    }
                                }
                                nextPointer = realNextPointer;
                            }


                            if (currentPointer >= br.BaseStream.Length)
                            {
                                decodedMessages.Add("");
                                rawMesMessages.Add(new byte[0]);
                                continue;
                            }

                            // PT: Calcula o tamanho da mensagem e a lê.
                            // EN: Calculates the message size and reads it.
                            int messageLength = (int)(nextPointer - currentPointer);
                            if (messageLength <= 0)
                            {
                                decodedMessages.Add("");
                                rawMesMessages.Add(new byte[0]);
                                // Cache de mensagens vazias também
                                if (currentPointer > 0) pointerCache[currentPointer] = "";
                                continue;
                            }

                            br.BaseStream.Seek(currentPointer, SeekOrigin.Begin);
                            byte[] messageBytes = br.ReadBytes(messageLength);
                            rawMesMessages.Add(messageBytes);

                            string decodedText = DecodeMessage(messageBytes);
                            decodedMessages.Add(decodedText);
                            // Adiciona o texto recém-decodificado ao cache.
                            if (currentPointer > 0) pointerCache[currentPointer] = decodedText;
                        }
                    }
                }
                // PT: Invoca a atualização da UI na thread principal.
                // EN: Invokes the UI update on the main thread.
                this.Invoke((MethodInvoker)delegate
                {
                    dataGridText.Rows.Clear();
                    saveMenu.Enabled = true;
                    saveAsMenu.Enabled = true;
                    //Popula o DataGridView com as mensagens decodificadas e cacheadas.
                    for (int i = 0; i < decodedMessages.Count; i++)
                    {
                        string text = decodedMessages[i];
                        string stateText = CalculateLineStates(text);
                        int rowIndex = dataGridText.Rows.Add(i, text, text, stateText);
                        AtualizarEstiloDaLinha(dataGridText.Rows[rowIndex]);
                    }
                });
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show($"Ocorreu um erro ao ler o arquivo .MES:\n{ex.Message}", "Erro de Leitura");
                });
            }
        }
        // PT: Processa uma instrução específica, decodificando seus parâmetros e construindo a representação em tag.
        // EN: Processes a specific instruction, decoding its parameters and building the tag representation.
        private void ProcessInstruction(Instruction instruction, byte[] data, ref int i, StringBuilder sb)
        {
            string instructionName = instruction.Name;
            sb.Append($"{{{instructionName}");
            // PT: Lógica específica (hardcoded) para instruções com tratamento especial, para manter paridade com o mes-unpacker.
            // EN: Specific (hardcoded) logic for instructions with special handling, to maintain parity with mes-unpacker.
            switch (instructionName.ToUpper())
            {
                case "FACE": // 1 byte (person id) + 2 bytes (param)
                    try
                    {
                        ushort personId = data[i];
                        ushort param2 = (ushort)((data[i + 1] << 8) | data[i + 2]);
                        string personName = personId.ToString();
                        if (dadosDasTags.TryGetValue("people", out var pList) && personId < pList.Count)
                            personName = pList[personId];
                        sb.Append($"-{personName.ToUpper()}-{param2}");
                        i += 3;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "PEOPLE": // 1 byte (person id)
                    try
                    {
                        ushort personId = data[i];
                        string personName = personId.ToString();
                        if (dadosDasTags.TryGetValue("people", out var pList) && personId < pList.Count)
                            personName = pList[personId];
                        sb.Append($"-{personName}");
                        i += 1;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "PHRASE":
                case "PHRASE2":
                    try
                    {
                        ushort id = data[i]; // Lê apenas 1 byte
                        string name = id.ToString();
                        string key = "phrase";

                        if (dadosDasTags.TryGetValue(key, out var list) && id < list.Count)
                            name = list[id];

                        sb.Append($"-{name}");
                        i += 1; // Avança apenas 1 byte
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "ITEM":
                case "CROP":
                case "STRING":
                    try
                    {
                        ushort id = (ushort)((data[i] << 8) | data[i + 1]);
                        string name = id.ToString();
                        string key = instructionNameToTagKey.ContainsKey(instructionName) ? instructionNameToTagKey[instructionName] : instructionName.ToLower();

                        if (dadosDasTags.TryGetValue(key, out var list) && id < list.Count)
                            name = list[id];

                        sb.Append($"-{name}");
                        i += 2;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "PLAYSFX": // 2 bytes (sfx id)
                    try
                    {
                        ushort sfxId = (ushort)((data[i] << 8) | data[i + 1]);
                        string sfxName = sfxId.ToString();
                        if (subTables.TryGetValue("PARAM_SFXID_SHORT", out var sfxTable) && sfxTable.ContainsKey(sfxId))
                            sfxName = sfxTable[sfxId];
                        sb.Append($"-{sfxName}");
                        i += 2;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "GOLD": // Parâmetros de 1 byte cada
                    try
                    {
                        byte param1 = data[i];
                        byte param2 = data[i + 1];
                        sb.Append($"-{param1}-{param2}");
                        // A letra 'G' é tratada como texto normal que vem depois da tag
                        i += 2;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                case "CHOICE": // 2 bytes (params)
                    try
                    {
                        sb.Append($"-{data[i]}-{data[i + 1]}");
                        i += 2;
                    }
                    catch (IndexOutOfRangeException) { sb.Append("-ERRO_PARAM"); }
                    break;

                default:
                    // PT: Lógica genérica para processar os parâmetros da instrução com base nos tipos definidos no mapa.
                    // EN: Generic logic to process instruction parameters based on the types defined in the map.
                    foreach (string paramType in instruction.ParamTypes)
                    {
                        sb.Append("-");
                        try
                        {
                            string lookupKey = instructionNameToTagKey.ContainsKey(instruction.Name)
                                                ? instructionNameToTagKey[instruction.Name]
                                                : instruction.Name.ToLower();

                            // PT: Processa parâmetros de 1 byte.
                            // EN: Processes 1-byte parameters.
                            if (paramType.EndsWith("_BYTE") || paramType.EndsWith("COLORID"))
                            {
                                ushort id = data[i];
                                if (dadosDasTags.TryGetValue(lookupKey, out var list) && id < list.Count)
                                    sb.Append(list[id]);
                                else if (subTables.TryGetValue(paramType, out var idTable) && idTable.ContainsKey(id))
                                    sb.Append(idTable[id]);
                                else
                                    sb.Append(id);
                                i += 1;
                            }
                            // PT: Processa parâmetros de 2 bytes.
                            // EN: Processes 2-byte parameters.
                            else if (paramType.EndsWith("_SHORT") || paramType.EndsWith("ID"))
                            {
                                if (i + 1 >= data.Length) { sb.Append("ERRO"); break; }
                                ushort id = (ushort)((data[i] << 8) | data[i + 1]);
                                if (dadosDasTags.TryGetValue(lookupKey, out var list) && id < list.Count)
                                    sb.Append(list[id]);
                                else if (subTables.TryGetValue(paramType, out var idTable) && idTable.ContainsKey(id))
                                    sb.Append(idTable[id]);
                                else
                                    sb.Append(id);
                                i += 2;
                            }
                            // PT: Processa parâmetros de 4 bytes (inteiro).
                            // EN: Processes 4-byte parameters (integer).
                            else if (paramType == "PARAM_INT")
                            {
                                if (i + 3 >= data.Length) { sb.Append("ERRO"); break; }
                                uint p_int = (uint)(data[i] << 24 | data[i + 1] << 16 | data[i + 2] << 8 | data[i + 3]);
                                if (instruction.Name.Equals("COLORRGBA", StringComparison.OrdinalIgnoreCase))
                                {
                                    // Formato hexadecimal específico para COLORRGBA
                                    sb.Append($"0x{p_int:x8}");
                                }
                                else
                                {
                                    // Formato decimal padrão para todas as outras instruções com PARAM_INT
                                    sb.Append(p_int);
                                }
                                i += 4;
                            }
                            // PT: Fallback para parâmetros desconhecidos.
                            // EN: Fallback for unknown parameters.
                            else
                            {
                                sb.Append(data[i]);
                                i++;
                            }
                        }
                        catch (IndexOutOfRangeException) { sb.Append("ERRO_PARAM"); break; }
                    }
                    break;
            }

            sb.Append("}");
        }
        // PT: Decodifica um array de bytes de uma mensagem em uma string de texto legível.
        // EN: Decodes a byte array of a message into a readable text string.
        private string DecodeMessage(byte[] data)
        {
            if (masterMap.Count == 0 && data.Length > 0) return "[ERRO: Mapa de caracteres não carregado]";
            if (data.Length == 0) return "";

            var sb = new StringBuilder();
            sb.Clear();
            int i = 0;
            while (i < data.Length)
            {
                ushort code1 = data[i];
                if (code1 == 0x00) break;

                ushort code2 = 0;
                bool isTwoByteCandidate = (i + 1 < data.Length);
                if (isTwoByteCandidate)
                {
                    code2 = (ushort)((code1 << 8) | data[i + 1]);
                }
                // PT: 1. Tenta decodificar como uma instrução de 2 bytes.
                // EN: 1. Tries to decode as a 2-byte instruction.
                if (isTwoByteCandidate && instructionMap.ContainsKey(code2))
                {
                    i += 2; // Avança o índice pelos 2 bytes da instrução
                    ProcessInstruction(instructionMap[code2], data, ref i, sb);
                    continue;
                }
                // PT: 2. Tenta decodificar como um caractere de 2 bytes.
                // EN: 2. Tries to decode as a 2-byte character.
                if (isTwoByteCandidate && masterMap.ContainsKey(code2))
                {
                    sb.Append(masterMap[code2]);
                    i += 2;
                    continue;
                }
                // PT: 3. Se não for 2 bytes, tenta como uma instrução de 1 byte.
                // EN: 3. If not 2 bytes, tries as a 1-byte instruction.
                if (instructionMap.ContainsKey(code1))
                {
                    i += 1; // Avança o índice pelo byte da instrução
                    ProcessInstruction(instructionMap[code1], data, ref i, sb);
                    continue;
                }
                // PT: 4. Tenta como um caractere de 1 byte.
                // EN: 4. Tries as a 1-byte character.
                if (masterMap.ContainsKey(code1))
                {
                    string charValue = masterMap[code1];
                    // PT: Trata caracteres especiais como quebra de linha e espaço.
                    // EN: Handles special characters like newline and space.
                    if (charValue == @"[\n]") sb.Append(Environment.NewLine);
                    else if (charValue == @"[ ]") sb.Append(" ");
                    else sb.Append(charValue);
                    i += 1;
                    continue;
                }
                // PT: 5. Se não encontrou nada, registra como um código desconhecido.
                // EN: 5. If nothing was found, registers it as an unknown code.
                if (code1 >= 0x80 && isTwoByteCandidate)
                {
                    // Adiciona uma verificação para não tratar um terminador como parte de um símbolo.
                    // Se o próximo byte for nulo (fim da mensagem), é mais provável que 'code1' seja
                    // um código desconhecido de 1 byte, e não o início de um código de 2 bytes.
                    if (data[i + 1] == 0x00)
                    {
                        sb.Append($"{{{code1}}}");
                        i += 1;
                        continue; // Continua para o loop principal, que encontrará o terminador 0x00 e sairá.
                    }

                    // PT: Tratamento especial para a tag {SYMBOL-id}.
                    // EN: Special handling for the {SYMBOL-id} tag.
                    if (code2 >= 0x8000)
                    {
                        // Subtrai 0x8000 e formata como o MES-Unpacker
                        ushort symbolId = (ushort)(code2 - 0x8000);
                        sb.Append($"{{SYMBOL-{symbolId}}}");
                    }
                    else
                    {
                        sb.Append($"{{{code2}}}");
                    }
                    i += 2;
                }
                else
                {
                    sb.Append($"{{{code1}}}");
                    i += 1;
                }
            }
            return sb.ToString();
        }
        #endregion

        #region Métodos de Escrita e Codificação
        // PT: Escreve os dados do DataGridView em um arquivo .MES no caminho especificado.
        // EN: Writes the DataGridView data to a .MES file at the specified path.
        private void WriteMesFile(string outputPath)
        {
            try
            {
                var messagesToEncode = new List<string>();
                foreach (DataGridViewRow row in dataGridText.Rows)
                {
                    if (row.IsNewRow) continue;
                    messagesToEncode.Add(row.Cells["Edited"].Value?.ToString() ?? "");
                }

                var encodedMessages = new List<byte[]>();
                // PT: Codifica cada mensagem de texto em um array de bytes.
                // EN: Encodes each text message into a byte array.
                foreach (string msg in messagesToEncode)
                {
                    encodedMessages.Add(EncodeMessage(msg));
                }

                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var bw = new BinaryWriter(fs))
                {
                    // PT: Escreve o "magic number" do arquivo, dependendo da endianidade.
                    // EN: Writes the file's "magic number", depending on endianness.
                    uint magic = isBigEndian ? 0xB0B0C3CD : 0xCDC3B0B0;
                    if (magic == 0) magic = 0xB0B0C3CD;

                    bw.Write(magic);
                    // PT: Escreve o número de mensagens.
                    // EN: Writes the number of messages.
                    uint messageCount = (uint)encodedMessages.Count;
                    bw.Write(isBigEndian ? SwapEndianness(messageCount) : messageCount);

                    // PT: Reserva espaço para a tabela de ponteiros.
                    // EN: Reserves space for the pointer table.
                    long pointerTableOffset = bw.BaseStream.Position;
                    bw.BaseStream.Seek(messageCount * 4, SeekOrigin.Current);

                    var pointers = new List<uint>();
                    long currentDataOffset = bw.BaseStream.Position;

                    // PT: Escreve os dados de cada mensagem, alinhando em 4 bytes.
                    // EN: Writes the data for each message, aligning to 4 bytes.
                    foreach (byte[] messageData in encodedMessages)
                    {
                        while (currentDataOffset % 4 != 0)
                        {
                            bw.Write((byte)0x00);
                            currentDataOffset++;
                        }

                        pointers.Add((uint)currentDataOffset);
                        bw.Write(messageData);
                        currentDataOffset += messageData.Length;
                    }

                    // PT: Adiciona preenchimento final para alinhar o arquivo em 32 bytes.
                    // EN: Adds final padding to align the file to 32 bytes.
                    long finalPadding = currentDataOffset % 32;
                    if (finalPadding != 0)
                    {
                        for (int i = 0; i < 32 - finalPadding; i++)
                        {
                            bw.Write((byte)0x00);
                        }
                    }

                    // PT: Volta para o início da tabela de ponteiros e escreve os offsets corretos.
                    // EN: Returns to the beginning of the pointer table and writes the correct offsets.
                    long endOfDataOffset = bw.BaseStream.Position;
                    bw.BaseStream.Seek(pointerTableOffset, SeekOrigin.Begin);
                    foreach (uint pointer in pointers)
                    {
                        bw.Write(isBigEndian ? SwapEndianness(pointer) : pointer);
                    }

                    bw.BaseStream.Seek(endOfDataOffset, SeekOrigin.Begin);
                }

                // PT: Atualiza o estado da UI e notifica o usuário do sucesso.
                // EN: Updates the UI state and notifies the user of success.
                currentFilePath = outputPath;
                this.Text = $"Mes Editor - {Path.GetFileName(currentFilePath)}";
                saveMenu.Enabled = true;
                MessageBox.Show($"Arquivo salvo com sucesso em:\n{outputPath}", "Sucesso");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao salvar o arquivo .MES:\n{ex.Message}\n\nStackTrace:\n{ex.StackTrace}", "Erro de Escrita");
            }
        }

        // PT: Codifica uma string de texto legível em um array de bytes, usando os mapas de caracteres e instruções.
        // EN: Encodes a readable text string into a byte array, using the character and instruction maps.
        private byte[] EncodeMessage(string text)
        {
            var result = new List<byte>();
            var parts = Regex.Split(text, @"(\{[^}]+\})");

            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                // PT: Se a parte é uma tag (começa com '{' e termina com '}').
                // EN: If the part is a tag (starts with '{' and ends with '}').
                if (part.StartsWith("{") && part.EndsWith("}"))
                {
                    string tagContent = part.Substring(1, part.Length - 2);
                    var tagParts = tagContent.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    string instructionName = tagParts[0];

                    // Busca a instrução para obter seu opcode
                    if (reverseInstructionMap.TryGetValue(instructionName, out Instruction instruction))
                    {
                        // PT: Escreve o opcode (pode ser de 1 ou 2 bytes).
                        // EN: Writes the opcode (can be 1 or 2 bytes).
                        if (instruction.Opcode >= 0x80) result.Add((byte)(instruction.Opcode >> 8));
                        result.Add((byte)(instruction.Opcode & 0xFF));
                        // PT: Lógica específica (hardcoded) para escrever os parâmetros de certas instruções.
                        // EN: Specific (hardcoded) logic to write the parameters of certain instructions.
                        switch (instructionName.ToUpper())
                        {
                            case "FACE": // Formato: {FACE-NomePessoa-Numero}
                                if (tagParts.Length > 2)
                                {
                                    string personName = tagParts[1];
                                    if (reverseDadosDasTags.TryGetValue("people", out var pMap) && pMap.TryGetValue(personName, out ushort personId))
                                        result.Add((byte)personId); // Escreve o ID (1 byte)
                                    else if (byte.TryParse(personName, out byte pId))
                                        result.Add(pId); // Fallback se o nome for um número

                                    // PT: Escreve o segundo parâmetro (2 bytes).
                                    // EN: Writes the second parameter (2 bytes).
                                    if (ushort.TryParse(tagParts[2], out ushort param2))
                                    {
                                        result.Add((byte)(param2 >> 8));
                                        result.Add((byte)(param2 & 0xFF));
                                    }
                                }
                                break;

                            case "PEOPLE": // Formato: {PEOPLE-NomePessoa}
                                if (tagParts.Length > 1)
                                {
                                    string personName = tagParts[1];
                                    if (reverseDadosDasTags.TryGetValue("people", out var pMap) && pMap.TryGetValue(personName, out ushort personId))
                                        result.Add((byte)personId); // Escreve o ID (1 byte)
                                    else if (byte.TryParse(personName, out byte pId))
                                        result.Add(pId);
                                }
                                break;

                            case "PHRASE":
                            case "PHRASE2":
                                if (tagParts.Length > 1)
                                {
                                    string name = tagParts[1];
                                    string key = "phrase";

                                    // Tenta encontrar o ID no mapa reverso
                                    if (reverseDadosDasTags.TryGetValue(key, out var reverseMap) && reverseMap.TryGetValue(name, out ushort id))
                                    {
                                        result.Add((byte)id); // Escreve APENAS 1 byte
                                    }
                                    // Se não encontrar, tenta converter o nome para um número (fallback)
                                    else if (byte.TryParse(name, out byte numId))
                                    {
                                        result.Add(numId);
                                    }
                                }
                                break;

                            case "ITEM":
                            case "CROP":
                            case "STRING":
                            case "PLAYSFX":
                                if (tagParts.Length > 1)
                                {
                                    string name = tagParts[1];
                                    string key = instructionNameToTagKey.ContainsKey(instructionName) ? instructionNameToTagKey[instructionName] : instructionName.ToLower();
                                    Dictionary<string, ushort> reverseMap = null;

                                    // PT: Seleciona o mapa reverso correto para o lookup.
                                    // EN: Selects the correct reverse map for the lookup.
                                    if (instructionName.ToUpper() == "PLAYSFX")
                                    {
                                        if (reverseSubTables.ContainsKey("PARAM_SFXID_SHORT"))
                                            reverseMap = reverseSubTables["PARAM_SFXID_SHORT"];
                                    }
                                    else
                                    {
                                        if (reverseDadosDasTags.ContainsKey(key))
                                            reverseMap = reverseDadosDasTags[key];
                                    }

                                    ushort id;
                                    // PT: Normaliza o nome para o lookup e tenta encontrar o ID.
                                    // EN: Normalizes the name for the lookup and tries to find the ID.
                                    string normalizedName = NormalizeTagDisplayName(name);
                                    if (reverseMap != null && (reverseMap.TryGetValue(name, out id) || reverseMap.TryGetValue(normalizedName, out id)))
                                    {
                                        result.Add((byte)(id >> 8));
                                        result.Add((byte)(id & 0xFF));
                                    }
                                    else
                                    {
                                        string numericCandidate = string.IsNullOrEmpty(normalizedName) ? name : normalizedName;
                                        // PT: Se não encontrar, tenta converter o nome para um ID numérico como fallback.
                                        // EN: If not found, tries to convert the name to a numeric ID as a fallback.
                                        if (ushort.TryParse(numericCandidate, out ushort numId))
                                        {
                                            result.Add((byte)(numId >> 8));
                                            result.Add((byte)(numId & 0xFF));
                                        }
                                    }
                                }
                                break;

                            case "GOLD": // Formato {GOLD-p1-p2}
                                if (tagParts.Length > 2 && byte.TryParse(tagParts[1], out byte gParam1) && byte.TryParse(tagParts[2].TrimEnd('G'), out byte gParam2))
                                {
                                    result.Add(gParam1);
                                    result.Add(gParam2);
                                }
                                break;

                            case "CHOICE": // Formato: {CHOICE-1-5}
                                if (tagParts.Length > 2 && byte.TryParse(tagParts[1], out byte cParam1) && byte.TryParse(tagParts[2], out byte cParam2))
                                {
                                    result.Add(cParam1);
                                    result.Add(cParam2);
                                }
                                break;

                            default:
                                // PT: Lógica genérica para instruções não tratadas especificamente.
                                // EN: Generic logic for instructions not specifically handled.
                                if (tagParts.Length > 1 && instruction.ParamTypes.Any())
                                {
                                    string paramValueStr = tagParts[1];
                                    string paramType = instruction.ParamTypes[0];

                                    if (paramType.EndsWith("_BYTE") || paramType.EndsWith("COLORID"))
                                    {
                                        if (byte.TryParse(paramValueStr, out byte val))
                                            result.Add(val);
                                    }
                                    else if (paramType.EndsWith("_SHORT") || paramType.EndsWith("ID"))
                                    {
                                        if (ushort.TryParse(paramValueStr, out ushort val))
                                        {
                                            result.Add((byte)(val >> 8));
                                            result.Add((byte)(val & 0xFF));
                                        }
                                    }
                                    else if (paramType == "PARAM_INT")
                                    {
                                        if (uint.TryParse(paramValueStr, out uint val))
                                        {
                                            result.Add((byte)(val >> 24)); result.Add((byte)(val >> 16));
                                            result.Add((byte)(val >> 8)); result.Add((byte)(val & 0xFF));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    // PT: Lógica para texto normal.
                    // EN: Logic for normal text.
                    string remainingText = part.Replace(Environment.NewLine, "\n");
                    foreach (char c in remainingText)
                    {
                        string s = c.ToString();
                        // PT: Converte caracteres especiais para sua representação no mapa.
                        // EN: Converts special characters to their map representation.
                        if (s == "\n") s = @"[\n]";
                        if (s == " ") s = @"[ ]";

                        // PT: Busca o código do caractere no mapa reverso e o adiciona ao resultado.
                        // EN: Looks up the character code in the reverse map and adds it to the result.
                        if (reverseMasterMap.TryGetValue(s, out ushort code))
                        {
                            if (code >= 0x80) result.Add((byte)(code >> 8));
                            result.Add((byte)(code & 0xFF));
                        }
                    }
                }
            }
            // PT: Adiciona o terminador nulo (0x00) no final da mensagem.
            // EN: Adds the null terminator (0x00) at the end of the message.
            result.Add(0x00); // Adiciona o terminador nulo
            return result.ToArray();
        }
        #endregion

        #region Métodos de Gerenciamento de Mapas
        // PT: Carrega os arquivos de mapa de caracteres da pasta "CharacterMaps" e os adiciona ao ComboBox.
        // EN: Loads the character map files from the "CharacterMaps" folder and adds them to the ComboBox.
        private void LoadMapFiles()
        {
            string mapsPath = Path.Combine(Application.StartupPath, "CharacterMaps");
            if (!Directory.Exists(mapsPath))
            {
                Directory.CreateDirectory(mapsPath);
                MessageBox.Show("Pasta 'CharacterMaps' não encontrada. Criei uma para você.", "Aviso");
                return;
            }
            mapFiles.Clear();
            comboBoxCodification.Items.Clear();
            // PT: Filtra para não incluir arquivos "shared_".
            // EN: Filters to not include "shared_" files.
            string[] files = Directory.GetFiles(mapsPath, "*.map")
                                      .Where(p => !Path.GetFileName(p).StartsWith("shared_")).ToArray();
            foreach (string file in files)
            {
                try
                {
                    // PT: Lê a primeira linha do arquivo para obter o nome de exibição.
                    // EN: Reads the first line of the file to get the display name.
                    string displayName = File.ReadLines(file).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(displayName) && !mapFiles.ContainsKey(displayName))
                    {
                        mapFiles.Add(displayName, file);
                        comboBoxCodification.Items.Add(displayName);
                    }
                }
                catch (Exception ex) { MessageBox.Show($"Erro ao ler o arquivo de mapa: {Path.GetFileName(file)}\n{ex.Message}", "Erro de Mapa"); }
            }
            if (comboBoxCodification.Items.Count > 0) comboBoxCodification.SelectedIndex = 0;
        }
        // PT: Carrega arquivos de mapa compartilhados (shared_*.map).
        // EN: Loads shared map files (shared_*.map).
        private void LoadSharedFiles()
        {
            string mapsPath = Path.Combine(Application.StartupPath, "CharacterMaps");
            if (!Directory.Exists(mapsPath)) return;
            string[] sharedFiles = Directory.GetFiles(mapsPath, "shared_*.map");
            foreach (string file in sharedFiles)
            {
                LoadCharacterMap(file, isShared: true);
            }
        }
        // PT: Lê e processa um arquivo de mapa de caracteres, preenchendo os dicionários de mapeamento.
        // EN: Reads and processes a character map file, populating the mapping dictionaries.
        private void LoadCharacterMap(string filePath, bool isShared = false)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Arquivo de mapa não encontrado em:\n{filePath}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!isShared)
            {
                // PT: Se não for um mapa compartilhado, limpa os mapas existentes.
                // EN: If it's not a shared map, clears the existing maps.
                masterMap.Clear();
                instructionMap.Clear();
                reverseMasterMap.Clear();
                reverseInstructionMap.Clear();
                isBigEndian = false;
                codigoVersaoAtual = "";
            }

            try
            {
                var lines = File.ReadAllLines(filePath);

                if (!isShared)
                {
                    // PT: Procura pela linha de versão para identificar o jogo.
                    // EN: Looks for the version line to identify the game.
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("#Version"))
                        {
                            Match match = Regex.Match(trimmedLine, @"(GMU|GFU|PMU|GMJ|GFJ|PMJ)");
                            if (match.Success)
                            {
                                codigoVersaoAtual = match.Value;
                                break;
                            }
                        }
                    }
                }

                string currentSection = "";
                string currentSubTableKey = "";
                // PT: Pula a primeira linha (nome de exibição) se não for um mapa compartilhado.
                // EN: Skips the first line (display name) if it's not a shared map.
                IEnumerable<string> contentLines = isShared ? lines : lines.Skip(1);
                foreach (var line in contentLines)
                {
                    string trimmedLine = line.Trim();
                    // PT: Ignora linhas vazias, comentários ou separadores.
                    // EN: Ignores empty lines, comments, or separators.
                    if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#") || trimmedLine.StartsWith("="))
                        continue;
                    // PT: Identifica a seção atual do mapa (Caracteres, Bytecodes, etc.).
                    // EN: Identifies the current map section (Caracteres, Bytecodes, etc.).
                    if (trimmedLine == "Caracteres" || trimmedLine == "Bytecodes" || trimmedLine == "Instruções")
                    {
                        currentSection = trimmedLine;
                        continue;
                    }
                    if (trimmedLine.StartsWith("SubTabela:"))
                    {
                        currentSection = "SubTabela";
                        currentSubTableKey = trimmedLine.Split(':')[1];
                        if (!subTables.ContainsKey(currentSubTableKey))
                        {
                            subTables.Add(currentSubTableKey, new Dictionary<ushort, string>());
                            reverseSubTables.Add(currentSubTableKey, new Dictionary<string, ushort>());
                        }
                        continue;
                    }
                    // PT: Processa uma linha de mapeamento (código=valor).
                    // EN: Processes a mapping line (code=value).
                    if (trimmedLine.Contains("="))
                    {
                        var parts = trimmedLine.Split(new[] { '=' }, 2);
                        if (parts.Length != 2) continue;
                        // PT: Tenta converter o código hexadecimal para um valor ushort.
                        // EN: Tries to convert the hexadecimal code to a ushort value.
                        if (ushort.TryParse(parts[0].Trim(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort parsedValue))
                        {
                            string charValue = parts[1];
                            // PT: Adiciona o mapeamento à seção correta.
                            // EN: Adds the mapping to the correct section.
                            switch (currentSection)
                            {
                                case "Caracteres":
                                case "Bytecodes":
                                    masterMap[parsedValue] = charValue;
                                    if (!reverseMasterMap.ContainsKey(charValue)) reverseMasterMap[charValue] = parsedValue;
                                    break;
                                case "Instruções":
                                    // PT: Processa uma definição de instrução.
                                    // EN: Processes an instruction definition.
                                    var instructionParts = charValue.Split(',');
                                    var inst = new Instruction { Name = instructionParts[0].Trim(), Opcode = parsedValue };
                                    if (instructionParts.Length > 1)
                                    {
                                        inst.ParamTypes.AddRange(instructionParts.Skip(1).Select(p => p.Trim()));
                                    }
                                    instructionMap[parsedValue] = inst;
                                    if (!reverseInstructionMap.ContainsKey(inst.Name)) reverseInstructionMap[inst.Name] = inst;
                                    if (!inst.ParamTypes.Any() && !reverseInstructionMap.ContainsKey(charValue.Trim()))
                                    {
                                        reverseInstructionMap[charValue.Trim()] = inst;
                                    }
                                    break;
                                case "SubTabela":
                                    // PT: Adiciona um mapeamento a uma subtabela.
                                    // EN: Adds a mapping to a subtable.
                                    if (!string.IsNullOrEmpty(currentSubTableKey))
                                    {
                                        string subValue = charValue.Trim();
                                        subTables[currentSubTableKey][parsedValue] = subValue;
                                        if (!reverseSubTables[currentSubTableKey].ContainsKey(subValue)) reverseSubTables[currentSubTableKey][subValue] = parsedValue;
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocorreu um erro ao carregar o mapa de caracteres '{Path.GetFileName(filePath)}':\n{ex.Message}", "Erro de Mapa");
            }

            // PT: Se um código de versão foi encontrado, carrega os arquivos .mes associados às tags.
            // EN: If a version code was found, loads the .mes files associated with the tags.
            if (!isShared && !string.IsNullOrEmpty(codigoVersaoAtual))
            {
                CarregarMesFilesDasTags();
            }
        }
        // PT: Carrega os nomes de itens, pessoas, etc., de arquivos .mes específicos para popular as sugestões de autocompletar.
        // EN: Loads item names, people names, etc., from specific .mes files to populate autocomplete suggestions.
        private void CarregarMesFilesDasTags()
        {
            dadosDasTags.Clear();
            reverseDadosDasTags.Clear();

            // PT: Mapeia o código da versão para o nome da pasta correspondente.
            // EN: Maps the version code to the corresponding folder name.
            var mapaVersaoParaPasta = new Dictionary<string, string>
    {
        { "GMU", "awl" }, { "GMJ", "awljpn" }, { "PMU", "awlse" },
        { "GFU", "anawl" }, { "GFJ", "anawljpn" }, { "PMJ", "awlsejpn" }
    };

            // PT: Mapeia o nome da tag para o nome do arquivo .mes.
            // EN: Maps the tag name to the .mes file name.
            var mapaTagParaArquivo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "people", "people.mes" }, { "farmcrop", "farmcrop.mes" },
        { "item", "item.mes" }, { "structure", "structure.mes" },
        { "string", "string.mes" }, { "phrase", "phrase.mes" }
    };

            if (!mapaVersaoParaPasta.TryGetValue(codigoVersaoAtual, out string nomeDaPasta))
            {
                return;
            }

            string basePath = Path.Combine(Application.StartupPath, "MESFILES", nomeDaPasta);

            if (!Directory.Exists(basePath))
            {
                return;
            }

            foreach (var par in mapaTagParaArquivo)
            {
                string nomeTag = par.Key;
                string nomeArquivo = par.Value;
                string caminhoCompleto = Path.Combine(basePath, nomeArquivo);

                if (File.Exists(caminhoCompleto))
                {
                    try
                    {
                        // PT: Extrai as mensagens do arquivo .mes.
                        // EN: Extracts the messages from the .mes file.
                        List<string> mensagens = ExtrairMensagensDeArquivoMes(caminhoCompleto);

                        var mensagensProcessadas = new List<string>();
                        var nomesExistentes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        // PT: Processa as mensagens para garantir nomes únicos, adicionando sufixos se necessário.
                        // EN: Processes the messages to ensure unique names, adding suffixes if necessary.
                        foreach (string nome in mensagens)
                        {
                            string baseName = NormalizeTagDisplayName(nome);
                            string candidate = string.IsNullOrEmpty(baseName) ? string.Empty : baseName;
                            int suffix = 1;
                            while (!nomesExistentes.Add(candidate))
                            {
                                suffix++;
                                candidate = string.IsNullOrEmpty(baseName) ? $"({suffix})" : $"{baseName} ({suffix})";
                            }
                            mensagensProcessadas.Add(candidate);
                        }
                        dadosDasTags[nomeTag] = mensagensProcessadas;

                        // PT: Cria o mapa reverso (nome -> ID) para esta tag.
                        // EN: Creates the reverse map (name -> ID) for this tag.
                        var nameToIdMap = new Dictionary<string, ushort>(StringComparer.OrdinalIgnoreCase);
                        for (int i = 0; i < mensagensProcessadas.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(mensagensProcessadas[i]))
                            {
                                nameToIdMap[mensagensProcessadas[i]] = (ushort)i;
                            }
                        }
                        reverseDadosDasTags[nomeTag] = nameToIdMap;
                    }
                    catch { /* Ignora erros em arquivos .mes individuais */ }
                }
            }
        }
        #endregion

        #region Funções Auxiliares
        // PT: Normaliza um valor de tag para exibição, removendo tags aninhadas e espaços extras.
        // EN: Normalizes a tag value for display, removing nested tags and extra spaces.
        private string NormalizeTagDisplayName(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                return rawValue ?? string.Empty;

            // Substitui quebras de linha por espaço
            string result = rawValue.Replace("\r", " ").Replace("\n", " ");

            int safetyCounter = 0;
            // PT: Remove recursivamente as tags aninhadas.
            // EN: Recursively removes nested tags.
            while (safetyCounter < 10)
            {
                string replaced = NestedTagPattern.Replace(result, match =>
                {
                    var inst = match.Groups[1].Value.ToUpperInvariant();
                    var param = match.Groups[2].Value;

                    // Instruções decorativas que não devem virar texto dentro do nome:
                    if (inst == "COLOR" || inst == "PAUSE" || inst == "WAIT" || inst == "ICON")
                    {
                        // remove completamente a tag (ou substitui por espaço para evitar concatenação)
                        return " ";
                    }

                    // Se for uma tag que tem sentido manter o parâmetro como parte do nome,
                    // podemos devolver param. Caso contrário, remover.
                    // Ajustar aqui se houver mais instruções específicas para preservar.
                    return param;
                });

                if (replaced.Equals(result, StringComparison.Ordinal))
                    break;
                result = replaced;
                safetyCounter++;
            }

            // Colapsa múltiplos espaços em um único espaço.
            result = CollapseWhitespacePattern.Replace(result, " ").Trim();

            return result.ToUpperInvariant(); // Converte para maiúsculas no final
        }
        // PT: Atualiza todas as linhas no DataGridView, decodificando as mensagens brutas.
        // EN: Updates all rows in the DataGridView by decoding the raw messages.
        private void UpdateAllRows()
        {
            dataGridText.Rows.Clear();
            for (int i = 0; i < rawMesMessages.Count; i++)
            {
                string decodedText = DecodeMessage(rawMesMessages[i]);
                string stateText = CalculateLineStates(decodedText);

                int rowIndex = dataGridText.Rows.Add(i, decodedText, decodedText, stateText);

                DataGridViewRow novaLinha = dataGridText.Rows[rowIndex];

                AtualizarEstiloDaLinha(novaLinha);
            }
        }
        // PT: Atualiza apenas a coluna "Estado" de todas as linhas no DataGridView.
        // EN: Updates only the "Estado" column of all rows in the DataGridView.
        private void UpdateAllRowsState()
        {
            foreach (DataGridViewRow row in dataGridText.Rows)
            {
                if (row.IsNewRow) continue;
                string editedText = row.Cells["Edited"].Value?.ToString() ?? "";
                row.Cells["Estado"].Value = CalculateLineStates(editedText);

                AtualizarEstiloDaLinha(row);
            }
        }
        // PT: Calcula o estado de cada linha de texto (contagem de caracteres/limite) e retorna como uma string.
        // EN: Calculates the state of each line of text (character count/limit) and returns it as a string.
        private string CalculateLineStates(string text)
        {
            if (comboBoxlimit == null || comboBoxlimit.SelectedItem == null)
            {
                return "";
            }
            if (string.IsNullOrEmpty(text))
            {
                return "0/" + comboBoxlimit.SelectedItem.ToString();
            }

            int limit = int.Parse(comboBoxlimit.SelectedItem.ToString());
            // PT: 1. Lista dos tipos de tags cujo conteúdo (parâmetro) deve ser CONTADO.
            // EN: 1. List of tag types whose content (parameter) should be COUNTED.
            var contentTagTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "people", "farmcrop", "item", "structure", "string", "phrase"
    };

            var countableTextBuilder = new StringBuilder();
            // PT: 2. Divide o texto em "texto normal" e "tags".
            // EN: 2. Splits the text into "normal text" and "tags".
            var parts = Regex.Split(text, @"(\{[^}]+\})");

            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                if (part.StartsWith("{") && part.EndsWith("}"))
                {
                    // PT: É uma tag. Vamos analisá-la.
                    // EN: It's a tag. Let's analyze it.
                    string tagContent = part.Substring(1, part.Length - 2);
                    var tagParts = tagContent.Split(new[] { '-' }, 2);
                    // PT: Adiciona casos especiais para tags com tamanho fixo predefinido.
                    // EN: Adds special cases for tags with a predefined fixed size.
                    if (tagContent.Equals("DOGNAME", StringComparison.OrdinalIgnoreCase))
                    {
                        // PT: Adiciona 7 caracteres de placeholder para a contagem.
                        // EN: Adds 7 placeholder characters for the count.
                        countableTextBuilder.Append(new string('X', 7));
                        continue;
                    }
                    // PT: Adiciona casos especiais para tags com tamanho fixo predefinido.
                    // EN: Adds special cases for tags with a predefined fixed size.
                    if (tagContent.Equals("FARMNAME", StringComparison.OrdinalIgnoreCase))
                    {
                        countableTextBuilder.Append(new string('X', 7));
                        continue;
                    }
                    else if (tagContent.StartsWith("PREVINPUT-", StringComparison.OrdinalIgnoreCase))
                    {
                        countableTextBuilder.Append(new string('X', 7));
                        continue;
                    }
                    // PT: 3. Verifica se é uma tag de conteúdo (ex: "item") e se tem um valor após o hífen.
                    // EN: 3. Checks if it's a content tag (e.g., "item") and has a value after the hyphen.
                    if (tagParts.Length == 2 && contentTagTypes.Contains(tagParts[0]))
                    {
                        // PT: Se for, adiciona o VALOR da tag (o nome) ao nosso texto para contagem.
                        // EN: If so, adds the VALUE of the tag (the name) to our text for counting.
                        countableTextBuilder.Append(tagParts[1]);
                    }
                    // PT: Se não for uma tag de conteúdo (ex: {PAUSE}, {COLOR-RED}), não fazemos nada,
                    // EN: If it's not a content tag (e.g., {PAUSE}, {COLOR-RED}), we do nothing,
                    // PT: efetivamente contando-a como 0 caracteres.
                    // EN: effectively counting it as 0 characters.
                }
                else
                {
                    // PT: É texto normal, então adicionamos ao nosso texto para contagem.
                    // EN: It's normal text, so we add it to our text for counting.
                    countableTextBuilder.Append(part);
                }
            }
            // PT: 4. A partir daqui, o processo é o mesmo de antes, mas usando o texto processado.
            // EN: 4. From here, the process is the same as before, but using the processed text.
            string strippedText = countableTextBuilder.ToString();
            string[] lines = strippedText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return string.Join(Environment.NewLine, lines.Select(line => $"{line.Length}/{limit}"));
        }
        // PT: Inverte a ordem dos bytes de um valor uint (Big Endian <-> Little Endian).
        // EN: Inverts the byte order of a uint value (Big Endian <-> Little Endian).
        private uint SwapEndianness(uint value) => (value & 0xFF) << 24 | (value & 0xFF00) << 8 | (value & 0xFF0000) >> 8 | (value & 0xFF000000) >> 24;
        // PT: Inverte a ordem dos bytes de um valor ushort.
        // EN: Inverts the byte order of a ushort value.
        private ushort SwapEndianness(ushort value) => (ushort)((value & 0xFF) << 8 | (value & 0xFF00) >> 8);
        // PT: Determina o tipo do arquivo (.mes ou .txt) lendo os primeiros 4 bytes (magic number).
        // EN: Determines the file type (.mes or .txt) by reading the first 4 bytes (magic number).
        private FileType DetermineFileType(string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    if (fs.Length < 4) return FileType.TextFile;
                    using (var br = new BinaryReader(fs))
                    {
                        // PT: Lê o magic number e compara com os valores conhecidos.
                        // EN: Reads the magic number and compares it with known values.
                        uint magic = br.ReadUInt32();
                        if (magic == 0xB0B0C3CD) return FileType.GameCubeMES;
                        if (magic == 0xCDC3B0B0) return FileType.PS2MES;
                        return FileType.TextFile;
                    }
                }
            }
            catch
            {
                return FileType.Unknown;
            }
        }
        #endregion

        // PT: Manipulador de clique para o item de menu "Arquivo atual para TXT".
        // EN: Click handler for the "Current file to TXT" menu item.
        private void extrairParaTXT_Click(object sender, EventArgs e)
        {
            if (dataGridText.Rows.Count == 0 || (dataGridText.Rows.Count == 1 && dataGridText.Rows[0].IsNewRow))
            {
                MessageBox.Show("Não há mensagens para extrair.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Arquivo de Texto (*.txt)|*.txt|Todos os Arquivos (*.*)|*.*";
                saveFileDialog.Title = "Extrair para arquivo de texto...";

                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    saveFileDialog.FileName = Path.GetFileNameWithoutExtension(currentFilePath) + ".txt";
                }

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();

                        // PT: Itera sobre as linhas da grade e constrói o conteúdo do arquivo de texto.
                        // EN: Iterates over the grid rows and builds the text file content.
                        foreach (DataGridViewRow row in dataGridText.Rows)
                        {
                            if (row.IsNewRow) continue;

                            sb.AppendLine($"--- message {row.Index} ---");

                            string messageText = row.Cells["Edited"].Value?.ToString() ?? "";
                            sb.AppendLine(messageText);
                        }

                        File.WriteAllText(saveFileDialog.FileName, sb.ToString().TrimEnd());

                        MessageBox.Show("Arquivo de texto extraído com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocorreu um erro ao salvar o arquivo:\n{ex.Message}", "Erro de Salvamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        // PT: Manipuladores de clique para extração em lote com mapas específicos.
        // EN: Click handlers for batch extraction with specific maps.
        private void extractWonderfulLifeUSA_Click(object sender, EventArgs e)
        {
            ProcessBatchExtraction("GMU", "PMU");
        }
        private void extractWonderfulLifeJAP_Click(object sender, EventArgs e)
        {
            ProcessBatchExtraction("GMJ", "PMJ");
        }
        private void extractAnotherWonderfulLifeUSA_Click(object sender, EventArgs e)
        {
            // PT: PS2 não se aplica para Another Wonderful Life.
            // EN: PS2 does not apply for Another Wonderful Life.
            ProcessBatchExtraction("GFU", null);
        }

        private void extractAnotherWonderfulLifeJAP_Click(object sender, EventArgs e)
        {
            // PT: PS2 não se aplica para Another Wonderful Life.
            // EN: PS2 does not apply for Another Wonderful Life.
            ProcessBatchExtraction("GFJ", null);
        }

        // PT: Processa a extração em lote de múltiplos arquivos .MES para .TXT.
        // EN: Processes the batch extraction of multiple .MES files to .TXT.
        private void ProcessBatchExtraction(string gameCubeVersionCode, string ps2VersionCode)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Selecione um ou mais arquivos .MES para extrair";
                ofd.Filter = "Arquivos de Mensagem (*.mes)|*.mes";
                ofd.Multiselect = true;

                if (ofd.ShowDialog() != DialogResult.OK) return;

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Selecione a pasta para salvar os arquivos .txt extraídos";
                    if (fbd.ShowDialog() != DialogResult.OK) return;

                    string destinationFolder = fbd.SelectedPath;
                    int successCount = 0;
                    var failedFiles = new List<string>();

                    this.Cursor = Cursors.WaitCursor;

                    // PT: Itera sobre cada arquivo .MES selecionado.
                    // EN: Iterates over each selected .MES file.
                    foreach (string mesFilePath in ofd.FileNames)
                    {
                        try
                        {
                            FileType fileType = DetermineFileType(mesFilePath);
                            string targetVersionCode = "";

                            // PT: Determina o código de versão a ser usado com base no tipo de arquivo.
                            // EN: Determines the version code to use based on the file type.
                            if (fileType == FileType.GameCubeMES)
                            {
                                targetVersionCode = gameCubeVersionCode;
                            }
                            // PT: Se for PS2, verifica se um código de versão de PS2 foi fornecido.
                            // EN: If it's PS2, checks if a PS2 version code was provided.
                            else if (fileType == FileType.PS2MES)
                            {
                                if (string.IsNullOrEmpty(ps2VersionCode))
                                {
                                    throw new Exception("Versão de PS2 (Little Endian) não é aplicável para esta extração.");
                                }
                                targetVersionCode = ps2VersionCode;
                            }
                            else
                            {
                                throw new Exception("Não é um arquivo de GameCube ou PS2.");
                            }

                            // PT: Encontra e carrega o mapa de caracteres correspondente.
                            // EN: Finds and loads the corresponding character map.
                            string mapFilePath = FindMapFileByVersion(targetVersionCode);
                            if (string.IsNullOrEmpty(mapFilePath))
                            {
                                throw new Exception($"Nenhum mapa encontrado com o código de versão '{targetVersionCode}'.");
                            }

                            LoadCharacterMap(mapFilePath);

                            // PT: Extrai as mensagens do arquivo .MES.
                            // EN: Extracts the messages from the .MES file.
                            List<string> messages = ExtrairMensagensDeArquivoMes(mesFilePath);

                            // PT: Constrói o conteúdo do arquivo .TXT.
                            // EN: Builds the content of the .TXT file.
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < messages.Count; i++)
                            {
                                sb.AppendLine($"--- message {i} ---");
                                sb.AppendLine(messages[i]);
                            }

                            // PT: Salva o arquivo .TXT.
                            // EN: Saves the .TXT file.
                            string outputTxtPath = Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(mesFilePath) + ".txt");
                            File.WriteAllText(outputTxtPath, sb.ToString().TrimEnd());
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            failedFiles.Add($"{Path.GetFileName(mesFilePath)}: {ex.Message}");
                        }
                    }

                    this.Cursor = Cursors.Default;

                    // PT: Exibe um relatório da operação.
                    // EN: Displays a report of the operation.
                    var report = new StringBuilder();
                    report.AppendLine($"{successCount} de {ofd.FileNames.Length} arquivos extraídos com sucesso.");
                    if (failedFiles.Any())
                    {
                        report.AppendLine("\nOcorreram os seguintes erros:");
                        foreach (string error in failedFiles)
                        {
                            report.AppendLine($"- {error}");
                        }
                    }
                    MessageBox.Show(report.ToString(), "Extração Específica Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // PT: Encontra o caminho de um arquivo de mapa com base no código de versão do jogo.
        // EN: Finds the path of a map file based on the game's version code.
        private string FindMapFileByVersion(string versionCode)
        {
            string mapsPath = Path.Combine(Application.StartupPath, "CharacterMaps");
            if (!Directory.Exists(mapsPath)) return null;

            string[] mapFiles = Directory.GetFiles(mapsPath, "*.map")
                                         .Where(p => !Path.GetFileName(p).StartsWith("shared_")).ToArray();

            foreach (string mapFile in mapFiles)
            {
                try
                {
                    // PT: Lê as linhas do arquivo de mapa para encontrar a diretiva #Version.
                    // EN: Reads the lines of the map file to find the #Version directive.
                    var lines = File.ReadLines(mapFile);
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (trimmedLine.StartsWith("#Version"))
                        {
                            if (line.Contains($"{versionCode}"))
                            {
                                return mapFile;
                            }
                        }
                    }
                }
                catch { /* Ignora erros de leitura */ }
            }

            return null;
        }

        // PT: Extrai todas as mensagens de um único arquivo .MES e as retorna como uma lista de strings.
        // EN: Extracts all messages from a single .MES file and returns them as a list of strings.
        private List<string> ExtrairMensagensDeArquivoMes(string filePath)
        {
            FileType fileType = DetermineFileType(filePath);
            if (fileType != FileType.GameCubeMES && fileType != FileType.PS2MES)
            {
                throw new InvalidOperationException("O arquivo selecionado não é um formato .MES válido.");
            }
            bool localIsBigEndian = (fileType == FileType.GameCubeMES);

            //Lista para guardar as mensagens decodificadas na ordem correta.
            var decodedMessages = new List<string>();
            //Dicionário para cachear textos já decodificados (Ponteiro -> Texto).
            var pointerCache = new Dictionary<uint, string>();

            byte[] fileBytes = File.ReadAllBytes(filePath);

            using (var br = new BinaryReader(new MemoryStream(fileBytes)))
            {
                br.BaseStream.Seek(4, SeekOrigin.Begin);
                uint textCount = localIsBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                uint[] pointers = new uint[textCount];
                for (int i = 0; i < textCount; i++)
                {
                    pointers[i] = localIsBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                }

                for (int i = 0; i < textCount; i++)
                {
                    uint currentPointer = pointers[i];

                    // Lógica de cache
                    if (pointerCache.ContainsKey(currentPointer))
                    {
                        // Se o ponteiro já foi lido, reutiliza o texto do cache.
                        decodedMessages.Add(pointerCache[currentPointer]);
                    }
                    else
                    {
                        // Se for um ponteiro novo, lê e decodifica.
                        uint nextPointer = (i + 1 < textCount) ? pointers[i + 1] : (uint)br.BaseStream.Length;

                        // Lógica para encontrar o fim real da mensagem em caso de ponteiros duplicados não sequenciais
                        if (currentPointer > 0)
                        {
                            uint realNextPointer = (uint)br.BaseStream.Length;
                            // Encontra o próximo ponteiro *diferente* para delimitar o tamanho
                            for (int j = i + 1; j < textCount; j++)
                            {
                                if (pointers[j] > currentPointer)
                                {
                                    realNextPointer = pointers[j];
                                    break;
                                }
                            }
                            nextPointer = realNextPointer;
                        }

                        if (currentPointer >= br.BaseStream.Length)
                        {
                            decodedMessages.Add("");
                            continue;
                        }

                        int messageLength = (int)(nextPointer - currentPointer);
                        if (messageLength <= 0)
                        {
                            decodedMessages.Add("");
                            // Cache de mensagens vazias também
                            if (currentPointer > 0) pointerCache[currentPointer] = "";
                            continue;
                        }
                        br.BaseStream.Seek(currentPointer, SeekOrigin.Begin);
                        byte[] messageBytes = br.ReadBytes(messageLength);

                        string decodedText = DecodeMessage(messageBytes);
                        decodedMessages.Add(decodedText);
                        // Adiciona o texto recém-decodificado ao cache.
                        if (currentPointer > 0) pointerCache[currentPointer] = decodedText;
                    }
                }
            }

            return decodedMessages;
        }

        // PT: Manipuladores de clique para inserção em lote com mapas específicos.
        // EN: Click handlers for batch insertion with specific maps.
        private void insertNGCWonderfulLifeUSA_Click(object sender, EventArgs e)
        {
            ProcessBatchInsertion("GMU", true);
        }

        private void insertPS2WonderfulLifeUSA_Click(object sender, EventArgs e)
        {
            ProcessBatchInsertion("PMU", false);
        }

        private void insertAnotherWonderfulLifeUSA_Click(object sender, EventArgs e)
        {
            ProcessBatchInsertion("GFU", true);
        }

        // PT: Processa a inserção em lote de múltiplos arquivos .TXT para .MES.
        // EN: Processes the batch insertion of multiple .TXT files to .MES.
        private void ProcessBatchInsertion(string versionCode, bool forceBigEndian)
        {
            string mapFilePath = FindMapFileByVersion(versionCode);
            if (string.IsNullOrEmpty(mapFilePath))
            {
                MessageBox.Show($"Não foi possível encontrar um arquivo .map com o código de versão '{versionCode}'.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            LoadCharacterMap(mapFilePath);

            // PT: Pede ao usuário para selecionar os arquivos .TXT.
            // EN: Asks the user to select the .TXT files.
            OpenFileDialog txtFileDialog = new OpenFileDialog
            {
                Title = "Selecione os arquivos .TXT traduzidos",
                Filter = "Arquivos de Texto (*.txt)|*.txt",
                Multiselect = true
            };
            if (txtFileDialog.ShowDialog() != DialogResult.OK) return;

            // PT: Pede ao usuário para selecionar a pasta de destino.
            // EN: Asks the user to select the destination folder.
            FolderBrowserDialog destinationFolderDialog = new FolderBrowserDialog
            {
                Description = "Selecione a pasta para SALVAR os novos arquivos .MES"
            };
            if (destinationFolderDialog.ShowDialog() != DialogResult.OK) return;

            this.Cursor = Cursors.WaitCursor;
            int successCount = 0;
            var failedFiles = new List<string>();

            // PT: Itera sobre cada arquivo .TXT selecionado.
            // EN: Iterates over each selected .TXT file.
            foreach (string txtPath in txtFileDialog.FileNames)
            {
                string txtFileName = Path.GetFileName(txtPath);
                try
                {
                    List<string> txtMessages = ParseTxtFile(txtPath);
                    if (!txtMessages.Any())
                    {
                        throw new InvalidDataException("O arquivo .TXT está vazio ou em um formato inválido.");
                    }

                    // PT: Codifica as mensagens de texto em bytes.
                    // EN: Encodes the text messages into bytes.
                    var encodedMessages = txtMessages.Select(msg => EncodeMessage(msg)).ToList();
                    string newMesName = Path.GetFileNameWithoutExtension(txtPath) + ".mes";
                    string newMesPath = Path.Combine(destinationFolderDialog.SelectedPath, newMesName);

                    // PT: Escreve o novo arquivo .MES.
                    // EN: Writes the new .MES file.
                    WriteNewMesFile(newMesPath, encodedMessages, forceBigEndian);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failedFiles.Add($"{txtFileName}: {ex.Message}");
                }
            }

            this.Cursor = Cursors.Default;

            // PT: Exibe um relatório da operação.
            // EN: Displays a report of the operation.
            var report = new StringBuilder();
            report.AppendLine($"{successCount} de {txtFileDialog.FileNames.Length} arquivos inseridos com sucesso.");
            if (failedFiles.Any())
            {
                report.AppendLine("\nOcorreram os seguintes erros:");
                foreach (string error in failedFiles)
                {
                    report.AppendLine($"- {error}");
                }
            }
            MessageBox.Show(report.ToString(), "Inserção em Lote Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // PT: Analisa um arquivo .TXT e extrai as mensagens dele.
        // EN: Parses a .TXT file and extracts the messages from it.
        private List<string> ParseTxtFile(string txtPath)
        {
            var messages = new List<string>();
            var lines = File.ReadAllLines(txtPath);
            var currentMessageBuilder = new StringBuilder();
            bool isReadingMessage = false;

            foreach (var line in lines)
            {
                // PT: A lógica de análise é a mesma de LoadTxtFile.
                // EN: The parsing logic is the same as in LoadTxtFile.
                if (line.StartsWith("--- message"))
                {
                    if (isReadingMessage)
                    {
                        messages.Add(currentMessageBuilder.ToString().TrimEnd('\r', '\n'));
                    }
                    currentMessageBuilder.Clear();
                    isReadingMessage = true;
                }
                else if (isReadingMessage)
                {
                    if (currentMessageBuilder.Length > 0)
                    {
                        currentMessageBuilder.AppendLine();
                    }
                    currentMessageBuilder.Append(line);
                }
            }

            if (isReadingMessage)
            {
                messages.Add(currentMessageBuilder.ToString().TrimEnd('\r', '\n'));
            }

            return messages;
        }

        // PT: Escreve um novo arquivo .MES a partir de uma lista de mensagens já codificadas.
        // EN: Writes a new .MES file from a list of already encoded messages.
        private void WriteNewMesFile(string outputPath, List<byte[]> encodedMessages, bool isBigEndian)
        {
            using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var bw = new BinaryWriter(fs))
            {
                // PT: A lógica de escrita é a mesma de WriteMesFile.
                // EN: The writing logic is the same as in WriteMesFile.
                uint magic = isBigEndian ? 0xB0B0C3CD : 0xCDC3B0B0;
                bw.Write(magic);

                uint messageCount = (uint)encodedMessages.Count;
                bw.Write(isBigEndian ? SwapEndianness(messageCount) : messageCount);

                // PT: Reserva espaço para a tabela de ponteiros.
                // EN: Reserves space for the pointer table.
                long pointerTableOffset = bw.BaseStream.Position;
                bw.BaseStream.Seek(messageCount * 4, SeekOrigin.Current);

                var pointers = new List<uint>();
                long currentDataOffset = bw.BaseStream.Position;

                foreach (byte[] messageData in encodedMessages)
                {
                    while (currentDataOffset % 4 != 0)
                    {
                        bw.Write((byte)0x00);
                        currentDataOffset++;
                    }
                    pointers.Add((uint)currentDataOffset);
                    bw.Write(messageData);
                    currentDataOffset += messageData.Length;
                }

                long finalPadding = currentDataOffset % 32;
                if (finalPadding != 0)
                {
                    for (int i = 0; i < 32 - finalPadding; i++)
                    {
                        bw.Write((byte)0x00);
                    }
                }

                // PT: Volta e escreve a tabela de ponteiros.
                // EN: Goes back and writes the pointer table.
                bw.BaseStream.Seek(pointerTableOffset, SeekOrigin.Begin);
                foreach (uint pointer in pointers)
                {
                    bw.Write(isBigEndian ? SwapEndianness(pointer) : pointer);
                }
            }
        }

        // PT: Manipulador de clique para extrair vários arquivos .MES usando o mapa de caracteres atualmente carregado.
        // EN: Click handler to extract multiple .MES files using the currently loaded character map.
        private void extrairVáriosArquivosComOMapaAtual_Click(object sender, EventArgs e)
        {
            // PT: Verifica se um mapa está carregado a partir da seleção do ComboBox.
            // EN: Checks if a map is loaded from the ComboBox selection.
            if (masterMap.Count == 0 || comboBoxCodification.SelectedItem == null)
            {
                MessageBox.Show("Nenhum mapa de caracteres está carregado! Por favor, selecione uma codificação na lista.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Mensagem (*.mes)|*.mes",
                Title = "Selecione os arquivos MES para extrair com o mapa atual",
                Multiselect = true
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.Cursor = Cursors.WaitCursor;
                    int successCount = 0;
                    var failedFiles = new List<string>();

                    foreach (var mesFilePath in openFileDialog.FileNames)
                    {
                        try
                        {
                            var decodedMessages = new List<string>();
                            var pointerCache = new Dictionary<uint, string>();

                            byte[] fileBytes = File.ReadAllBytes(mesFilePath);
                            FileType fileType = DetermineFileType(mesFilePath);
                            bool localIsBigEndian = (fileType == FileType.GameCubeMES);

                            using (var br = new BinaryReader(new MemoryStream(fileBytes)))
                            {
                                br.BaseStream.Seek(4, SeekOrigin.Begin);
                                uint textCount = localIsBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                                uint[] pointers = new uint[textCount];
                                for (int i = 0; i < textCount; i++)
                                {
                                    pointers[i] = localIsBigEndian ? SwapEndianness(br.ReadUInt32()) : br.ReadUInt32();
                                }

                                for (int i = 0; i < textCount; i++)
                                {
                                    uint currentPointer = pointers[i];
                                    if (pointerCache.ContainsKey(currentPointer))
                                    {
                                        decodedMessages.Add(pointerCache[currentPointer]);
                                    }
                                    else
                                    {
                                        uint nextPointer;
                                        uint realNextPointer = (uint)br.BaseStream.Length;
                                        for (int j = i + 1; j < textCount; j++)
                                        {
                                            if (pointers[j] > currentPointer)
                                            {
                                                realNextPointer = pointers[j];
                                                break;
                                            }
                                        }
                                        nextPointer = realNextPointer;

                                        if (currentPointer >= br.BaseStream.Length)
                                        {
                                            decodedMessages.Add("");
                                            continue;
                                        }

                                        int messageLength = (int)(nextPointer - currentPointer);
                                        if (messageLength <= 0)
                                        {
                                            decodedMessages.Add("");
                                            if (currentPointer > 0) pointerCache[currentPointer] = "";
                                            continue;
                                        }

                                        br.BaseStream.Seek(currentPointer, SeekOrigin.Begin);
                                        byte[] messageBytes = br.ReadBytes(messageLength);

                                        // USA O MAPA ATUALMENTE CARREGADO
                                        string decodedText = DecodeMessage(messageBytes);
                                        decodedMessages.Add(decodedText);
                                        if (currentPointer > 0) pointerCache[currentPointer] = decodedText;
                                    }
                                }
                            }

                            // PT: Constrói o conteúdo do arquivo .TXT.
                            // EN: Builds the content of the .TXT file.
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < decodedMessages.Count; i++)
                            {
                                sb.AppendLine($"--- message {i} ---");
                                sb.AppendLine(decodedMessages[i]);
                            }

                            // PT: Salva o arquivo .TXT.
                            // EN: Saves the .TXT file.
                            string outputTxt = Path.ChangeExtension(mesFilePath, ".txt");
                            File.WriteAllText(outputTxt, sb.ToString().TrimEnd(), Encoding.UTF8);
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            failedFiles.Add($"{Path.GetFileName(mesFilePath)}: {ex.Message}");
                        }
                    }

                    this.Cursor = Cursors.Default;

                    // PT: Exibe um relatório da operação.
                    // EN: Displays a report of the operation.
                    var report = new StringBuilder();
                    report.AppendLine($"{successCount} de {openFileDialog.FileNames.Length} arquivos extraídos com sucesso.");
                    if (failedFiles.Any())
                    {
                        report.AppendLine("\nOcorreram os seguintes erros:");
                        foreach (string error in failedFiles)
                        {
                            report.AppendLine($"- {error}");
                        }
                    }
                    MessageBox.Show(report.ToString(), "Extração Concluída", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // PT: Manipulador de clique para inserir vários arquivos .TXT usando o mapa de caracteres atualmente carregado.
        // EN: Click handler to insert multiple .TXT files using the currently loaded character map.
        private void inserirVáriosArquivosComOMapaAtual_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog
            {
                Filter = "Arquivos de Texto (*.txt)|*.txt",
                Title = "Selecione os arquivos TXT para inserir",
                Multiselect = true
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var txtFilePath in openFileDialog.FileNames)
                    {
                        try
                        {
                            // PT: Verifica se um mapa está carregado.
                            // EN: Checks if a map is loaded.
                            if (masterMap.Count == 0)
                            {
                                MessageBox.Show("Nenhum mapa de caracteres está carregado!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            var allLines = File.ReadAllLines(txtFilePath);
                            // PT: Analisa o arquivo .TXT para extrair as mensagens.
                            // EN: Parses the .TXT file to extract the messages.
                            var messages = new List<string>();
                            var currentMessage = new StringBuilder();
                            bool isReading = false;

                            foreach (var line in allLines)
                            {
                                if (line.StartsWith("--- message"))
                                {
                                    if (isReading) messages.Add(currentMessage.ToString().TrimEnd());
                                    currentMessage.Clear();
                                    isReading = true;
                                }
                                else if (isReading)
                                {
                                    if (currentMessage.Length > 0) currentMessage.AppendLine();
                                    currentMessage.Append(line);
                                }
                            }
                            if (isReading) messages.Add(currentMessage.ToString().TrimEnd());

                            // PT: Codifica as mensagens em bytes.
                            // EN: Encodes the messages into bytes.
                            var encodedMessages = new List<byte[]>();
                            foreach (string msg in messages)
                            {
                                encodedMessages.Add(EncodeMessage(msg));
                            }

                            // PT: Escreve o novo arquivo .MES.
                            // EN: Writes the new .MES file.
                            string outputMes = Path.ChangeExtension(txtFilePath, ".mes");
                            WriteMesFile(outputMes);

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao processar {txtFilePath}:\n{ex.Message}");
                        }
                    }

                    MessageBox.Show("Inserção concluída!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        // PT: Atualiza o estilo de uma linha do DataGridView (cor de fundo) com base no seu estado (se excedeu o limite de caracteres).
        // EN: Updates the style of a DataGridView row (background color) based on its state (if it exceeded the character limit).
        private void AtualizarEstiloDaLinha(DataGridViewRow row)
        {
            string estadoTexto = row.Cells["Estado"].Value?.ToString() ?? "";
            bool limiteExcedido = false;

            if (!string.IsNullOrEmpty(estadoTexto))
            {
                // PT: Divide o texto da célula "Estado" em múltiplas linhas, se houver.
                // EN: Splits the text of the "Estado" cell into multiple lines, if any.
                string[] linhasDeEstado = estadoTexto.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                foreach (string linha in linhasDeEstado)
                {
                    // PT: Para cada linha, divide o valor atual e o limite (ex: "8/21").
                    // EN: For each line, splits the current value and the limit (e.g., "8/21").
                    string[] partes = linha.Split('/');
                    if (partes.Length == 2)
                    {
                        // PT: Tenta converter as partes para números.
                        // EN: Tries to convert the parts to numbers.
                        if (int.TryParse(partes[0], out int atual) && int.TryParse(partes[1], out int limite))
                        {
                            // PT: Se o valor atual for maior que o limite, marca como excedido.
                            // EN: If the current value is greater than the limit, marks as exceeded.
                            if (atual > limite)
                            {
                                limiteExcedido = true;
                                break;
                            }
                        }
                    }
                }
            }
            // PT: Aplica a cor de fundo com base na verificação.
            // EN: Applies the background color based on the check.
            if (limiteExcedido)
            {
                row.DefaultCellStyle.BackColor = Color.LightCoral; // Vermelho claro
            }
            else
            {
                // PT: Se não excedeu, volta para a cor padrão do DataGridView.
                // EN: If not exceeded, returns to the default DataGridView color.
                row.DefaultCellStyle.BackColor = dataGridText.DefaultCellStyle.BackColor;
            }
        }
        // PT: Processa teclas de atalho do formulário (Ctrl+F, Ctrl+H, Ctrl+S, etc.).
        // EN: Processes form shortcut keys (Ctrl+F, Ctrl+H, Ctrl+S, etc.).
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // PT: Atalho para Localizar (Ctrl+F).
            // EN: Shortcut for Find (Ctrl+F).
            if (keyData == (Keys.Control | Keys.F))
            {
                AbrirJanelaLocalizar(false);
                return true;
            }
            // PT: Atalho para Substituir (Ctrl+H).
            // EN: Shortcut for Replace (Ctrl+H).
            if (keyData == (Keys.Control | Keys.H))
            {
                AbrirJanelaLocalizar(true);
                return true;
            }
            // PT: Atalho para Salvar (Ctrl+S).
            // EN: Shortcut for Save (Ctrl+S).
            if (keyData == (Keys.Control | Keys.S))
            {
                if (saveMenu.Enabled)
                {
                    saveMenu_Click(this, EventArgs.Empty);
                    return true;
                }
            }
            // PT: Atalho para Abrir (Ctrl+O).
            // EN: Shortcut for Open (Ctrl+O).
            if (keyData == (Keys.Control | Keys.O))
            {
                openMenu_Click(this, EventArgs.Empty);
                return true;
            }
            // PT: Atalho para Desfazer (Undo) (Ctrl+Z).
            // EN: Shortcut for Undo (Ctrl+Z).
            if (keyData == (Keys.Control | Keys.Z))
            {
                if (_historicoUndo.Count > 1)
                {
                    _alteracaoEmProgresso = true;
                    _timerUndo.Stop();

                    string estadoAtual = _historicoUndo.Pop();
                    _historicoRedo.Push(estadoAtual);

                    txtEditorDetalhe.Text = _historicoUndo.Peek();

                    _alteracaoEmProgresso = false;
                }
                return true;
            }
            // PT: Atalho para Refazer (Redo) (Ctrl+Y).
            // EN: Shortcut for Redo (Ctrl+Y).
            if (keyData == (Keys.Control | Keys.Y))
            {
                if (_historicoRedo.Count > 0)
                {
                    _alteracaoEmProgresso = true;
                    _timerUndo.Stop();

                    string estadoParaRestaurar = _historicoRedo.Pop();
                    _historicoUndo.Push(estadoParaRestaurar);

                    txtEditorDetalhe.Text = estadoParaRestaurar;

                    _alteracaoEmProgresso = false;
                }
                return true;
            }

            // PT: Se não for um atalho conhecido, passa para o processamento padrão.
            // EN: If it's not a known shortcut, passes it to the default processing.
            return base.ProcessCmdKey(ref msg, keyData);
        }
        // PT: Abre ou foca a janela de "Localizar e Substituir".
        // EN: Opens or focuses the "Find and Replace" window.
        private void AbrirJanelaLocalizar(bool mostrarAbaSubstituir)
        {
            // PT: Verifica se o formulário já está aberto; se não estiver, cria um novo.
            // EN: Checks if the form is already open; if not, creates a new one.
            if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            {
                _findReplaceForm = new FindText(this.dataGridText);
                _findReplaceForm.Owner = this;
                _findReplaceForm.Show();
            }
            else
            {
                // PT: Se já estiver aberto, apenas o traz para a frente.
                // EN: If it's already open, just brings it to the front.
                _findReplaceForm.Activate();
            }

            // PT: Tenta encontrar o TabControl no formulário FindText para selecionar a aba correta.
            // EN: Tries to find the TabControl on the FindText form to select the correct tab.
            if (_findReplaceForm.Controls["tabControl1"] is TabControl tabControl)
            {
                // PT: Aba 0 = Localizar, Aba 1 = Substituir.
                // EN: Tab 0 = Find, Tab 1 = Replace.
                tabControl.SelectedIndex = mostrarAbaSubstituir ? 1 : 0;
            }
        }
    }
}