using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WonderfulTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeTranslations();
            SetButtonTextByCulture(); // Define os textos dos botões com base no idioma do SO
        }

        // --- Dicionário de Localização ---
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _currentLang;

        private static void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();

            // Inglês (Padrão)
            _translations["en"] = new Dictionary<string, string>
            {
                {"btnExtractAfs", "Extract"},
                {"btnInsertAfs", "Insert"},
                {"btnCompress", "Compress"},
                {"textureViewer", "Texture Viewer"},
                {"btnOptimizedCompress", "Optimized Compress"},
                {"btnExtractClz", "Extract"},
                {"titleSelectAfs", "Choose a Harvest Moon game file..."},
                {"titleSelectCompress", "Choose a file to compress..."},
                {"titleSelectDecompress", "Choose a CLZ file to decompress..."},
                {"msgExtractionComplete", "Extraction complete!"},
                {"msgRepackComplete", "Repacking complete!"},
                {"msgCompressCompletePack", "Compression complete (Pack): {0}"},
                {"msgCompressCompletePack2", "Compression complete (Pack2): {0}"},
                {"msgDecompressComplete", "Decompression complete: {0}"},
                {"msgSuccess", "Success"},
                {"msgError", "Error"},
                {"errInvalidAfs", "Invalid or inconsistent AFS/DAT file!"},
                {"errAfsDatSameFolder", "The files {0}.AFS and {0}.DAT must be in the same directory!"},
                {"errFileNotFoundInFolder", "File '{0}' not found in folder '{1}'."},
                {"errTableBinNotFound", "File 'table.bin' not found in folder '{0}'."},
                {"errAfsFolderMissing", "Make sure the .AFS and .DAT files, and the folder with the extracted files, are in the same directory."},
                {"errInvalidClz", "Invalid CLZ file."}
            };

            // Português (Brasil e Portugal)
            _translations["pt"] = new Dictionary<string, string>
            {
                {"buttonAbrirArquivos", "Abrir Arquivo"},
                {"btnExtractAfs", "Extrair"},
                {"btnInsertAfs", "Inserir"},
                {"btnCompress", "Comprimir"},
                {"textureViewer", "Visualizador de Texturas"},
                {"btnOptimizedCompress", "Comprimir Otimizado"},
                {"btnExtractClz", "Extrair"},
                {"titleSelectAfs", "Escolha um arquivo do jogo Harvest Moon..."},
                {"titleSelectCompress", "Escolha um arquivo para comprimir..."},
                {"titleSelectDecompress", "Escolha um arquivo CLZ para descomprimir..."},
                {"msgExtractionComplete", "Extração concluída!"},
                {"msgRepackComplete", "Reempacotamento concluído!"},
                {"msgCompressCompletePack", "Compressão concluída (Pack): {0}"},
                {"msgCompressCompletePack2", "Compressão concluída (Pack2): {0}"},
                {"msgDecompressComplete", "Descompressão concluída: {0}"},
                {"msgSuccess", "Sucesso"},
                {"msgError", "Erro"},
                {"errInvalidAfs", "Arquivo AFS/DAT inválido ou inconsistente!"},
                {"errAfsDatSameFolder", "Os arquivos {0}.AFS e {0}.DAT precisam estar na mesma pasta!"},
                {"errFileNotFoundInFolder", "Arquivo '{0}' não encontrado na pasta '{1}'."},
                {"errTableBinNotFound", "Arquivo 'table.bin' não encontrado na pasta '{0}'."},
                {"errAfsFolderMissing", "Certifique-se que os arquivos .AFS e .DAT, e a pasta com os arquivos extraídos, estão no mesmo diretório."},
                {"errInvalidClz", "Arquivo CLZ inválido."}

            };

            // Espanhol
            _translations["es"] = new Dictionary<string, string>
            {
                {"btnExtractAfs", "Extraer"},
                {"btnInsertAfs", "Insertar"},
                {"btnCompress", "Comprimir"},
                {"textureViewer", "Visor de Texturas"},
                {"btnOptimizedCompress", "Compresión Optimizada"},
                {"btnExtractClz", "Extraer"},
                {"titleSelectAfs", "Elija un archivo del juego Harvest Moon..."},
                {"titleSelectCompress", "Elija un archivo para comprimir..."},
                {"titleSelectDecompress", "Elija un archivo CLZ para descomprimir..."},
                {"msgExtractionComplete", "¡Extracción completada!"},
                {"msgRepackComplete", "¡Reempaquetado completado!"},
                {"msgCompressCompletePack", "Compresión completada (Pack): {0}"},
                {"msgCompressCompletePack2", "Compresión completada (Pack2): {0}"},
                {"msgDecompressComplete", "Descompresión completada: {0}"},
                {"msgSuccess", "Éxito"},
                {"msgError", "Error"},
                {"errInvalidAfs", "¡Archivo AFS/DAT inválido o inconsistente!"},
                {"errAfsDatSameFolder", "¡Los archivos {0}.AFS y {0}.DAT deben estar en la misma carpeta!"},
                {"errFileNotFoundInFolder", "Archivo '{0}' no encontrado en la carpeta '{1}'."},
                {"errTableBinNotFound", "Archivo 'table.bin' no encontrado en la carpeta '{0}'."},
                {"errAfsFolderMissing", "Asegúrese de que los archivos .AFS y .DAT, y la carpeta con los archivos extraídos, estén en el mismo directorio."},
                {"errInvalidClz", "Archivo CLZ inválido."}
            };
        }

        private static string GetString(string key)
        {
            if (_translations.ContainsKey(_currentLang) && _translations[_currentLang].ContainsKey(key))
            {
                return _translations[_currentLang][key];
            }
            // Retorna o inglês como padrão se a tradução não for encontrada
            return _translations["en"][key];
        }


        public static int padding800(int tamanho)
        {
            if (tamanho <= 0) return 0;
            return ((tamanho - 1) / 0x800 + 1) * 0x800;
        }

        private void SetButtonTextByCulture()
        {
            _currentLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (!_translations.ContainsKey(_currentLang))
            {
                _currentLang = "en"; // Padrão para inglês
            }

            button1.Text = GetString("btnExtractAfs");
            button2.Text = GetString("btnInsertAfs");
            CLZCompact.Text = GetString("btnCompress");
            optimizedCLZCompact.Text = GetString("btnOptimizedCompress");
            CLZExtract.Text = GetString("btnExtractClz");
            TextureViewer.Text = GetString("textureViewer");
        }

        private void SetButtonsEnabled(bool enabled)
        {
            button1.Enabled = enabled;
            button2.Enabled = enabled;
            CLZCompact.Enabled = enabled;
            optimizedCLZCompact.Enabled = enabled;
            CLZExtract.Enabled = enabled;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivo Harvest Moon|*.AFS|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = GetString("titleSelectAfs");
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SetButtonsEnabled(false);
                this.Cursor = Cursors.WaitCursor;

                await Task.Run(() =>
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        string arquivoDAT = Path.ChangeExtension(file, "DAT");

                        if (File.Exists(arquivoDAT))
                        {
                            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                            using (FileStream stream2 = File.Open(arquivoDAT, FileMode.Open, FileAccess.Read))
                            {
                                // A lógica de extração permanece a mesma, apenas é executada em segundo plano.
                                BinaryReader brAFS = new BinaryReader(stream);
                                BinaryReader brDAT = new BinaryReader(stream2);

                                int magic = brAFS.ReadInt32();
                                int quantidadedearquivoAFS = brAFS.ReadInt32();
                                int quantidadedearquivoDAT = brDAT.ReadInt32();

                                if (magic == 0x00534641 && quantidadedearquivoAFS == quantidadedearquivoDAT)
                                {
                                    string outFolder = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
                                    if (!Directory.Exists(outFolder))
                                    {
                                        Directory.CreateDirectory(outFolder);
                                    }

                                    brDAT.BaseStream.Seek(0x10, SeekOrigin.Begin);
                                    stream.Seek(8, SeekOrigin.Begin);

                                    var fileInfos = new List<Tuple<string, int, int>>();

                                    for (int i = 0; i < quantidadedearquivoDAT; i++)
                                    {
                                        int offset = brAFS.ReadInt32();
                                        int size = brAFS.ReadInt32();

                                        long currentDatPos = brDAT.BaseStream.Position;
                                        byte[] bytesnome = brDAT.ReadBytes(0x1C);
                                        string nomearquivo = Encoding.Default.GetString(bytesnome).Split('\0')[0];
                                        brDAT.BaseStream.Seek(currentDatPos + 0x20, SeekOrigin.Begin);

                                        fileInfos.Add(new Tuple<string, int, int>(nomearquivo, offset, size));
                                    }

                                    int tableOffset = brAFS.ReadInt32();
                                    int tableSize = brAFS.ReadInt32();

                                    foreach (var info in fileInfos)
                                    {
                                        brAFS.BaseStream.Seek(info.Item2, SeekOrigin.Begin);
                                        byte[] bytesarquivo = brAFS.ReadBytes(info.Item3);
                                        File.WriteAllBytes(Path.Combine(outFolder, info.Item1), bytesarquivo);
                                    }

                                    brAFS.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
                                    byte[] bytearquivo = brAFS.ReadBytes(tableSize);
                                    File.WriteAllBytes(Path.Combine(outFolder, "table.bin"), bytearquivo);
                                }
                                else
                                {
                                    MessageBox.Show(GetString("errInvalidAfs"), GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                            MessageBox.Show(string.Format(GetString("errAfsDatSameFolder"), fileNameWithoutExt));
                        }
                    }
                });

                SetButtonsEnabled(true);
                this.Cursor = Cursors.Default;
                MessageBox.Show(GetString("msgExtractionComplete"), GetString("msgSuccess"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Arquivo Harvest Moon|*.AFS|Todos os arquivos (*.*)|*.*";
            openFileDialog1.Title = GetString("titleSelectAfs");
            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SetButtonsEnabled(false);
                this.Cursor = Cursors.WaitCursor;
                bool success = true;

                await Task.Run(() =>
                {
                    foreach (String file in openFileDialog1.FileNames)
                    {
                        string arquivoDAT = Path.ChangeExtension(file, "DAT");
                        string inFolder = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));

                        if (File.Exists(arquivoDAT) && Directory.Exists(inFolder))
                        {
                            var originalDatBytes = File.ReadAllBytes(arquivoDAT);
                            using (var brDatOriginal = new BinaryReader(new MemoryStream(originalDatBytes)))
                            {
                                int fileCount = brDatOriginal.ReadInt32();
                                var fileNames = new List<string>();
                                brDatOriginal.BaseStream.Seek(0x10, SeekOrigin.Begin);
                                for (int i = 0; i < fileCount; i++)
                                {
                                    byte[] nameBytes = brDatOriginal.ReadBytes(0x1C);
                                    fileNames.Add(Encoding.Default.GetString(nameBytes).Split('\0')[0]);
                                    brDatOriginal.BaseStream.Seek(4, SeekOrigin.Current);
                                }

                                var fileEntries = new List<Tuple<string, byte[]>>();
                                foreach (var fileName in fileNames)
                                {
                                    string filePath = Path.Combine(inFolder, fileName);
                                    if (!File.Exists(filePath))
                                    {
                                        MessageBox.Show(string.Format(GetString("errFileNotFoundInFolder"), fileName, inFolder), GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        success = false;
                                        return;
                                    }
                                    fileEntries.Add(Tuple.Create(fileName, File.ReadAllBytes(filePath)));
                                }

                                string tablePath = Path.Combine(inFolder, "table.bin");
                                if (!File.Exists(tablePath))
                                {
                                    MessageBox.Show(string.Format(GetString("errTableBinNotFound"), inFolder), GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    success = false;
                                    return;
                                }
                                byte[] tableData = File.ReadAllBytes(tablePath);

                                using (var newAfsStream = File.Create(file))
                                using (var newDatStream = File.Create(arquivoDAT))
                                using (var bwAfs = new BinaryWriter(newAfsStream))
                                using (var bwDat = new BinaryWriter(newDatStream))
                                {
                                    bwAfs.Write(0x00534641);
                                    bwAfs.Write(fileCount);

                                    bwDat.Write(originalDatBytes, 0, 0x10);

                                    int headerSize = 8 + (fileCount + 1) * 8;
                                    int currentOffset = padding800(headerSize);

                                    var offsets = new List<int>();
                                    var sizes = new List<int>();

                                    foreach (var entry in fileEntries)
                                    {
                                        offsets.Add(currentOffset);
                                        sizes.Add(entry.Item2.Length);
                                        currentOffset += padding800(entry.Item2.Length);
                                    }

                                    for (int i = 0; i < fileCount; i++)
                                    {
                                        bwAfs.Write(offsets[i]);
                                        bwAfs.Write(sizes[i]);

                                        byte[] nameBytes = new byte[0x1C];
                                        byte[] currentNameBytes = Encoding.Default.GetBytes(fileEntries[i].Item1);
                                        Array.Copy(currentNameBytes, nameBytes, currentNameBytes.Length);
                                        bwDat.Write(nameBytes);
                                        bwDat.Write(sizes[i]);
                                    }

                                    bwAfs.Write(currentOffset);
                                    bwAfs.Write(tableData.Length);

                                    for (int i = 0; i < fileCount; i++)
                                    {
                                        newAfsStream.Seek(offsets[i], SeekOrigin.Begin);
                                        bwAfs.Write(fileEntries[i].Item2);
                                    }

                                    newAfsStream.Seek(currentOffset, SeekOrigin.Begin);
                                    bwAfs.Write(tableData);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(GetString("errAfsFolderMissing"), GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            success = false;
                            return;
                        }
                    }
                });

                SetButtonsEnabled(true);
                this.Cursor = Cursors.Default;
                if (success)
                {
                    MessageBox.Show(GetString("msgRepackComplete"), GetString("msgSuccess"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async void CLZCompact_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Qualquer Arquivo|*.*", Title = GetString("titleSelectCompress") };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string inputPath = ofd.FileName;
            string outputPath = inputPath + ".clz";

            SetButtonsEnabled(false);
            this.Cursor = Cursors.WaitCursor;
            string resultMessage = "";
            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    byte[] inputData = File.ReadAllBytes(inputPath);
                    byte[] compressed = CLZ.Pack(inputData);
                    File.WriteAllBytes(outputPath, compressed);
                    resultMessage = string.Format(GetString("msgCompressCompletePack"), outputPath);
                    success = true;
                }
                catch (Exception ex)
                {
                    resultMessage = $"{GetString("msgError")}: {ex.Message}\n\n{ex.StackTrace}";
                }
            });

            SetButtonsEnabled(true);
            this.Cursor = Cursors.Default;
            if (success)
            {
                MessageBox.Show(resultMessage, GetString("msgSuccess"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resultMessage, GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void optimizedCLZCompact_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Qualquer Arquivo|*.*", Title = GetString("titleSelectCompress") };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string inputPath = ofd.FileName;
            string outputPath = inputPath + ".clz";

            SetButtonsEnabled(false);
            this.Cursor = Cursors.WaitCursor;
            string resultMessage = "";
            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    byte[] inputData = File.ReadAllBytes(inputPath);
                    byte[] compressed = CLZ.Pack2(inputData);
                    File.WriteAllBytes(outputPath, compressed);
                    resultMessage = string.Format(GetString("msgCompressCompletePack2"), outputPath);
                    success = true;
                }
                catch (Exception ex)
                {
                    resultMessage = $"{GetString("msgError")}: {ex.Message}\n\n{ex.StackTrace}";
                }
            });

            SetButtonsEnabled(true);
            this.Cursor = Cursors.Default;
            if (success)
            {
                MessageBox.Show(resultMessage, GetString("msgSuccess"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resultMessage, GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CLZExtract_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Arquivos CLZ|*.clz|Todos os arquivos (*.*)|*.*", Title = GetString("titleSelectDecompress") };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string inputPath = ofd.FileName;
            string outputPath = Path.ChangeExtension(inputPath, null);

            SetButtonsEnabled(false);
            this.Cursor = Cursors.WaitCursor;
            string resultMessage = "";
            bool success = false;

            await Task.Run(() =>
            {
                try
                {
                    byte[] inputData = File.ReadAllBytes(inputPath);
                    byte[] decompressed = CLZ.Unpack(inputData);
                    if (decompressed != null)
                    {
                        File.WriteAllBytes(outputPath, decompressed);
                        resultMessage = string.Format(GetString("msgDecompressComplete"), outputPath);
                        success = true;
                    }
                    else
                    {
                        resultMessage = GetString("errInvalidClz");
                    }
                }
                catch (Exception ex)
                {
                    resultMessage = $"{GetString("msgError")}: {ex.Message}\n\n{ex.StackTrace}";
                }
            });

            SetButtonsEnabled(true);
            this.Cursor = Cursors.Default;
            if (success)
            {
                MessageBox.Show(resultMessage, GetString("msgSuccess"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(resultMessage, GetString("msgError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static class CLZ
        {
            public static bool Use64BitHash = false;

            public static byte[] Unpack(byte[] data)
            {
                if (data == null || data.Length < 16) return null;
                if (data[0] != 'C' || data[1] != 'L' || data[2] != 'Z' || data[3] != 0x00) return null;

                int decompressedSize = (data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7];
                if (decompressedSize < 0) return null;
                if (decompressedSize == 0) return new byte[0];

                var output = new MemoryStream(decompressedSize);
                byte[] window = new byte[4096];
                int windowOfs = 0;
                int inIdx = 16;
                int bits = 0, bitCount = 0;

                while (output.Length < decompressedSize)
                {
                    if (bitCount == 0)
                    {
                        if (inIdx >= data.Length) break;
                        bits = data[inIdx++];
                        bitCount = 8;
                    }

                    if ((bits & 1) != 0) // Reference
                    {
                        if (inIdx + 1 >= data.Length) break;
                        int b1 = data[inIdx++];
                        int b2 = data[inIdx++];

                        int windowDelta = b1 | ((b2 & 0xF0) << 4);
                        windowDelta = (windowDelta & 0x800) != 0 ? windowDelta - 0x1000 : windowDelta;
                        int length = (b2 & 0x0F) + 3;

                        for (int i = 0; i < length; i++)
                        {
                            int srcIdx = windowOfs + windowDelta;
                            while (srcIdx < 0) srcIdx += 4096;
                            srcIdx %= 4096;

                            byte val = window[srcIdx];
                            if (output.Length < decompressedSize) output.WriteByte(val);
                            window[windowOfs] = val;
                            windowOfs = (windowOfs + 1) % 4096;
                        }
                    }
                    else // Literal
                    {
                        if (inIdx >= data.Length) break;
                        byte val = data[inIdx++];
                        if (output.Length < decompressedSize) output.WriteByte(val);
                        window[windowOfs] = val;
                        windowOfs = (windowOfs + 1) % 4096;
                    }
                    bits >>= 1;
                    bitCount--;
                }
                return output.ToArray();
            }

            public static byte[] Pack(byte[] input) => Use64BitHash ? Pack64(input) : Pack32(input);
            public static byte[] Pack2(byte[] input) => Use64BitHash ? Pack2_64(input) : Pack2_32(input);

            public static byte[] Pack32(byte[] input)
            {
                const int WINDOW_SIZE = 4096;
                const int ARRAY_SIZE = 16384;
                const int MAX_HEDGE = 18;
                const int BUFFER_SIZE = 16384;
                const int BUFFER_MAX_OUT = BUFFER_SIZE - 18;
                const uint PRIME_A = 37u, PRIME_B = 54059u, PRIME_C = 76963u;

                var hashTables = new CLZHashTable32[MAX_HEDGE - 2];
                for (int i = 0; i < MAX_HEDGE - 2; i++) hashTables[i] = new CLZHashTable32(i + 3);

                const int HASH_BUFFER_SIZE = MAX_HEDGE * 2;
                uint[][] hashes = new uint[HASH_BUFFER_SIZE][];
                for (int i = 0; i < HASH_BUFFER_SIZE; i++) { hashes[i] = new uint[MAX_HEDGE + 1]; hashes[i][0] = PRIME_A; }

                var clzqueue = new Queue<Tuple<CLZHashTable32.Node, int>>[MAX_HEDGE - 1];
                for (int i = 0; i < MAX_HEDGE - 1; i++) clzqueue[i] = new Queue<Tuple<CLZHashTable32.Node, int>>();

                byte[] buffer = new byte[BUFFER_SIZE];
                buffer[0] = 0;
                int numEntries = 0, bufferOfs = 1, bufferTypeOfs = 0;
                byte[] window = new byte[ARRAY_SIZE];
                int windowOfs = 0, decompSize = 0, hedgeSize = 0;

                using (var ms = new MemoryStream())
                {
                    ms.Write(new byte[16], 0, 16);
                    bool skipSubstr = false;
                    int deltaB = 0, longestB = 0, prevHashOfs = 0, lastDecomp = 0;
                    var inputStream = new MemoryStream(input);

                    while (true)
                    {
                        if (hedgeSize < MAX_HEDGE + 1 && inputStream.Position < inputStream.Length)
                        {
                            int diff = (ARRAY_SIZE - WINDOW_SIZE) - hedgeSize;
                            int winpos = (windowOfs + hedgeSize) % ARRAY_SIZE;
                            int rem = Math.Min(ARRAY_SIZE - winpos, diff);
                            int amt = 0;
                            if (rem > 0) amt += inputStream.Read(window, winpos, rem);
                            if (inputStream.Position < inputStream.Length && diff > rem)
                            {
                                rem = diff - rem;
                                amt += inputStream.Read(window, 0, rem);
                            }
                            hedgeSize += amt;
                        }
                        if (hedgeSize <= 0) break;

                        int deltaA = 0, longestA = 0;
                        int hashAdvOfs = windowOfs + Math.Min(hedgeSize, MAX_HEDGE + 1);
                        while (prevHashOfs < hashAdvOfs)
                        {
                            uint hx = (uint)(sbyte)window[prevHashOfs % ARRAY_SIZE] * PRIME_C;
                            for (int j = 1; j <= MAX_HEDGE; j++)
                            {
                                int hashofs = (HASH_BUFFER_SIZE + prevHashOfs - j + 1) % HASH_BUFFER_SIZE;
                                hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                            }
                            prevHashOfs++;
                        }

                        while (lastDecomp < decompSize)
                        {
                            for (int j = 3; j <= Math.Min(lastDecomp + 1, MAX_HEDGE); j++)
                            {
                                int startOfs = lastDecomp - j + 1;
                                uint hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                                var nodeInfo = hashTables[j - 3].AddNode(window, ARRAY_SIZE, startOfs, hash);
                                clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable32.Node, int>(nodeInfo.Item1, startOfs));
                            }
                            lastDecomp++;
                        }

                        if (!skipSubstr)
                        {
                            int maxsub = Math.Min(Math.Min(1 + decompSize, MAX_HEDGE), hedgeSize);
                            for (int j = maxsub; longestA == 0 && j >= 3; j--)
                            {
                                uint hash = hashes[windowOfs % HASH_BUFFER_SIZE][j];
                                int prev = hashTables[j - 3].GetLast(window, ARRAY_SIZE, windowOfs, hash);
                                if (prev != windowOfs) { longestA = j; deltaA = windowOfs - prev; }
                            }
                        }
                        else { deltaA = deltaB; longestA = longestB; skipSubstr = false; }

                        for (int j = 3; j <= Math.Min(lastDecomp + 1, MAX_HEDGE); j++)
                        {
                            int startOfs = lastDecomp - j + 1;
                            uint hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                            var nodeInfo = hashTables[j - 3].AddNode(window, ARRAY_SIZE, startOfs, hash);
                            clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable32.Node, int>(nodeInfo.Item1, startOfs));
                        }
                        lastDecomp++;

                        longestB = 0; deltaB = 0;
                        int maxsubB = Math.Min(Math.Min(2 + decompSize, MAX_HEDGE), hedgeSize);
                        for (int j = maxsubB; longestB == 0 && j >= 3; j--)
                        {
                            uint hash = hashes[(windowOfs + 1) % HASH_BUFFER_SIZE][j];
                            int prev = hashTables[j - 3].GetLast(window, ARRAY_SIZE, windowOfs + 1, hash);
                            if (prev != windowOfs + 1) { longestB = j; deltaB = windowOfs + 1 - prev; }
                        }

                        if ((longestA + 3 - 2) < (longestB + 3 - 3) || longestA < 3) longestA = 1;

                        for (int q = 0; q < MAX_HEDGE - 2; q++)
                        {
                            while (clzqueue[q].Count > 0 && (decompSize + longestA + 1) - clzqueue[q].Peek().Item2 > WINDOW_SIZE)
                                hashTables[q].RemoveNode(clzqueue[q].Dequeue());
                        }

                        if (longestA >= 3)
                        {
                            buffer[bufferTypeOfs] |= (byte)(1 << numEntries++);
                            deltaA = -deltaA;
                            buffer[bufferOfs++] = (byte)(deltaA & 0xFF);
                            buffer[bufferOfs++] = (byte)(((deltaA >> 4) & 0xF0) | ((longestA - 3) & 0x0F));
                            windowOfs += longestA; hedgeSize -= longestA; decompSize += longestA;
                        }
                        else
                        {
                            buffer[bufferOfs++] = window[windowOfs % ARRAY_SIZE];
                            numEntries++; hedgeSize--; decompSize++; windowOfs++; skipSubstr = true;
                        }

                        if (numEntries >= 8)
                        {
                            numEntries = 0;
                            if (bufferOfs >= BUFFER_MAX_OUT) { ms.Write(buffer, 0, bufferOfs); bufferOfs = 0; }
                            bufferTypeOfs = bufferOfs++;
                            buffer[bufferTypeOfs] = 0;
                        }
                    }

                    if (bufferOfs > 1) ms.Write(buffer, 0, (numEntries == 0) ? bufferOfs - 1 : bufferOfs);

                    byte[] final = ms.ToArray();
                    int finalSize = decompSize;
                    final[0] = (byte)'C'; final[1] = (byte)'L'; final[2] = (byte)'Z';
                    final[4] = (byte)(finalSize >> 24); final[5] = (byte)(finalSize >> 16); final[6] = (byte)(finalSize >> 8); final[7] = (byte)finalSize;
                    final[12] = (byte)(finalSize >> 24); final[13] = (byte)(finalSize >> 16); final[14] = (byte)(finalSize >> 8); final[15] = (byte)finalSize;
                    return final;
                }
            }

            public static byte[] Pack64(byte[] input)
            {
                const int WINDOW_SIZE = 4096;
                const int ARRAY_SIZE = 16384;
                const int MAX_HEDGE = 18;
                const int BUFFER_SIZE = 16384;
                const int BUFFER_MAX_OUT = BUFFER_SIZE - 18;
                const ulong PRIME_A = 37ul, PRIME_B = 54059ul, PRIME_C = 76963ul;

                var hashTables = new CLZHashTable64[MAX_HEDGE - 2];
                for (int i = 0; i < MAX_HEDGE - 2; i++) hashTables[i] = new CLZHashTable64(i + 3);

                const int HASH_BUFFER_SIZE = MAX_HEDGE * 2;
                ulong[][] hashes = new ulong[HASH_BUFFER_SIZE][];
                for (int i = 0; i < HASH_BUFFER_SIZE; i++) { hashes[i] = new ulong[MAX_HEDGE + 1]; hashes[i][0] = PRIME_A; }

                var clzqueue = new Queue<Tuple<CLZHashTable64.Node, int>>[MAX_HEDGE - 1];
                for (int i = 0; i < MAX_HEDGE - 1; i++) clzqueue[i] = new Queue<Tuple<CLZHashTable64.Node, int>>();

                byte[] buffer = new byte[BUFFER_SIZE];
                buffer[0] = 0;
                int numEntries = 0, bufferOfs = 1, bufferTypeOfs = 0;
                byte[] window = new byte[ARRAY_SIZE];
                int windowOfs = 0, decompSize = 0, hedgeSize = 0;

                using (var ms = new MemoryStream())
                {
                    ms.Write(new byte[16], 0, 16);
                    bool skipSubstr = false;
                    int deltaB = 0, longestB = 0, prevHashOfs = 0, lastDecomp = 0;
                    var inputStream = new MemoryStream(input);

                    while (true)
                    {
                        if (hedgeSize < MAX_HEDGE + 1 && inputStream.Position < inputStream.Length)
                        {
                            int diff = (ARRAY_SIZE - WINDOW_SIZE) - hedgeSize;
                            int winpos = (windowOfs + hedgeSize) % ARRAY_SIZE;
                            int rem = Math.Min(ARRAY_SIZE - winpos, diff);
                            int amt = 0;
                            if (rem > 0) amt += inputStream.Read(window, winpos, rem);
                            if (inputStream.Position < inputStream.Length && diff > rem)
                            {
                                rem = diff - rem;
                                amt += inputStream.Read(window, 0, rem);
                            }
                            hedgeSize += amt;
                        }
                        if (hedgeSize <= 0) break;

                        int deltaA = 0, longestA = 0;
                        int hashAdvOfs = windowOfs + Math.Min(hedgeSize, MAX_HEDGE + 1);
                        while (prevHashOfs < hashAdvOfs)
                        {
                            ulong hx = (ulong)(sbyte)window[prevHashOfs % ARRAY_SIZE] * PRIME_C;
                            for (int j = 1; j <= MAX_HEDGE; j++)
                            {
                                int hashofs = (HASH_BUFFER_SIZE + prevHashOfs - j + 1) % HASH_BUFFER_SIZE;
                                hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                            }
                            prevHashOfs++;
                        }

                        while (lastDecomp < decompSize)
                        {
                            for (int j = 3; j <= Math.Min(lastDecomp + 1, MAX_HEDGE); j++)
                            {
                                int startOfs = lastDecomp - j + 1;
                                ulong hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                                var nodeInfo = hashTables[j - 3].AddNode(window, ARRAY_SIZE, startOfs, hash);
                                clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable64.Node, int>(nodeInfo.Item1, startOfs));
                            }
                            lastDecomp++;
                        }

                        if (!skipSubstr)
                        {
                            int maxsub = Math.Min(Math.Min(1 + decompSize, MAX_HEDGE), hedgeSize);
                            for (int j = maxsub; longestA == 0 && j >= 3; j--)
                            {
                                ulong hash = hashes[windowOfs % HASH_BUFFER_SIZE][j];
                                int prev = hashTables[j - 3].GetLast(window, ARRAY_SIZE, windowOfs, hash);
                                if (prev != windowOfs) { longestA = j; deltaA = windowOfs - prev; }
                            }
                        }
                        else { deltaA = deltaB; longestA = longestB; skipSubstr = false; }

                        for (int j = 3; j <= Math.Min(lastDecomp + 1, MAX_HEDGE); j++)
                        {
                            int startOfs = lastDecomp - j + 1;
                            ulong hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                            var nodeInfo = hashTables[j - 3].AddNode(window, ARRAY_SIZE, startOfs, hash);
                            clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable64.Node, int>(nodeInfo.Item1, startOfs));
                        }
                        lastDecomp++;

                        longestB = 0; deltaB = 0;
                        int maxsubB = Math.Min(Math.Min(2 + decompSize, MAX_HEDGE), hedgeSize);
                        for (int j = maxsubB; longestB == 0 && j >= 3; j--)
                        {
                            ulong hash = hashes[(windowOfs + 1) % HASH_BUFFER_SIZE][j];
                            int prev = hashTables[j - 3].GetLast(window, ARRAY_SIZE, windowOfs + 1, hash);
                            if (prev != windowOfs + 1) { longestB = j; deltaB = windowOfs + 1 - prev; }
                        }

                        if ((longestA + 3 - 2) < (longestB + 3 - 3) || longestA < 3) longestA = 1;

                        for (int q = 0; q < MAX_HEDGE - 2; q++)
                        {
                            while (clzqueue[q].Count > 0 && (decompSize + longestA + 1) - clzqueue[q].Peek().Item2 > WINDOW_SIZE)
                                hashTables[q].RemoveNode(clzqueue[q].Dequeue());
                        }

                        if (longestA >= 3)
                        {
                            buffer[bufferTypeOfs] |= (byte)(1 << numEntries++);
                            deltaA = -deltaA;
                            buffer[bufferOfs++] = (byte)(deltaA & 0xFF);
                            buffer[bufferOfs++] = (byte)(((deltaA >> 4) & 0xF0) | ((longestA - 3) & 0x0F));
                            windowOfs += longestA; hedgeSize -= longestA; decompSize += longestA;
                        }
                        else
                        {
                            buffer[bufferOfs++] = window[windowOfs % ARRAY_SIZE];
                            numEntries++; hedgeSize--; decompSize++; windowOfs++; skipSubstr = true;
                        }

                        if (numEntries >= 8)
                        {
                            numEntries = 0;
                            if (bufferOfs >= BUFFER_MAX_OUT) { ms.Write(buffer, 0, bufferOfs); bufferOfs = 0; }
                            bufferTypeOfs = bufferOfs++;
                            buffer[bufferTypeOfs] = 0;
                        }
                    }

                    if (bufferOfs > 1) ms.Write(buffer, 0, (numEntries == 0) ? bufferOfs - 1 : bufferOfs);

                    byte[] final = ms.ToArray();
                    int finalSize = decompSize;
                    final[0] = (byte)'C'; final[1] = (byte)'L'; final[2] = (byte)'Z';
                    final[4] = (byte)(finalSize >> 24); final[5] = (byte)(finalSize >> 16); final[6] = (byte)(finalSize >> 8); final[7] = (byte)finalSize;
                    final[12] = (byte)(finalSize >> 24); final[13] = (byte)(finalSize >> 16); final[14] = (byte)(finalSize >> 8); final[15] = (byte)finalSize;
                    return final;
                }
            }

            public static byte[] Pack2_32(byte[] input)
            {
                int decompSize = input.Length;
                if (decompSize == 0) return new byte[] { 0x43, 0x4C, 0x5A, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                const int MAX_SUB = 18;
                const int WINDOW_SIZE = 4096;
                const uint PRIME_A = 37u, PRIME_B = 54059u, PRIME_C = 76963u;

                var hashTables = new CLZHashTable32[MAX_SUB - 2];
                for (int i = 0; i < MAX_SUB - 2; i++) hashTables[i] = new CLZHashTable32(i + 3, false);

                const int HASH_BUFFER_SIZE = MAX_SUB * 2;
                uint[][] hashes = new uint[HASH_BUFFER_SIZE][];
                for (int i = 0; i < HASH_BUFFER_SIZE; i++) { hashes[i] = new uint[MAX_SUB + 1]; hashes[i][0] = PRIME_A; }

                var clzqueue = new Queue<Tuple<CLZHashTable32.Node, int>>[MAX_SUB - 1];
                for (int i = 0; i < MAX_SUB - 1; i++) clzqueue[i] = new Queue<Tuple<CLZHashTable32.Node, int>>();

                int[] compBits = new int[decompSize + 1];
                int[] dlpair = new int[decompSize + 1];
                for (int i = 1; i <= decompSize; i++) compBits[i] = int.MaxValue;

                for (int i = 0; i < Math.Min(MAX_SUB - 1, decompSize); i++)
                {
                    uint hx = (uint)(sbyte)input[i] * PRIME_C;
                    for (int j = 1; j <= Math.Min(decompSize, MAX_SUB); j++)
                    {
                        int hashofs = (HASH_BUFFER_SIZE + i - j + 1) % HASH_BUFFER_SIZE;
                        hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                    }
                }

                for (int i = 0; i < decompSize; i++)
                {
                    if (i + MAX_SUB - 1 < decompSize)
                    {
                        int ofs = i + MAX_SUB - 1;
                        uint hx = (uint)(sbyte)input[ofs] * PRIME_C;
                        for (int j = 1; j <= MAX_SUB; j++)
                        {
                            int hashofs = (HASH_BUFFER_SIZE + ofs - j + 1) % HASH_BUFFER_SIZE;
                            hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                        }
                    }

                    for (int j = 3; j <= Math.Min(i, MAX_SUB); j++)
                    {
                        int startOfs = i - j;
                        uint hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                        var nodeInfo = hashTables[j - 3].AddNode(input, decompSize, startOfs, hash);
                        clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable32.Node, int>(nodeInfo.Item1, startOfs));
                    }

                    int[] currentDlpairs = new int[MAX_SUB];
                    int longest = 0;
                    int maxsub = Math.Min(Math.Min(MAX_SUB, decompSize - i), i);
                    for (int j = maxsub; !(longest > 0) && j >= 3; j--)
                    {
                        uint hash = hashes[i % HASH_BUFFER_SIZE][j];
                        int prev = hashTables[j - 3].GetLast(input, decompSize, i, hash);
                        if (prev != i)
                        {
                            for (int t = Math.Max(longest, 2); t < j; t++) currentDlpairs[t] = i - prev;
                            longest = j;
                        }
                    }

                    for (int q = 0; q < MAX_SUB - 2; q++)
                    {
                        while (clzqueue[q].Count > 0 && (i + 1 - clzqueue[q].Peek().Item2) > WINDOW_SIZE)
                            hashTables[q].RemoveNode(clzqueue[q].Dequeue());
                    }

                    if (i + 1 <= decompSize && compBits[i + 1] > compBits[i] + 9)
                    {
                        compBits[i + 1] = compBits[i] + 9;
                        dlpair[i + 1] = 1;
                    }

                    for (int t = 3; t <= longest; t++)
                    {
                        if (i + t <= decompSize && compBits[i + t] > compBits[i] + 17)
                        {
                            compBits[i + t] = compBits[i] + 17;
                            dlpair[i + t] = (-currentDlpairs[t - 1] << 16) | t;
                        }
                    }
                }

                int temp_ofs = decompSize;
                int temp_del = dlpair[temp_ofs];
                while (temp_ofs > 0)
                {
                    temp_ofs -= temp_del & 0xffff;
                    int temp = dlpair[temp_ofs];
                    dlpair[temp_ofs] = temp_del;
                    temp_del = temp;
                }

                using (var ms = new MemoryStream())
                {
                    int aofs = 0;
                    while (aofs < decompSize)
                    {
                        byte control = 0;
                        var mini = new MemoryStream();

                        for (int bit = 0; bit < 8 && aofs < decompSize; bit++)
                        {
                            int del = dlpair[aofs];
                            int len = del & 0xFFFF;

                            if (len < 3)
                            {
                                mini.WriteByte(input[aofs]);
                                aofs++;
                            }
                            else
                            {
                                control |= (byte)(1 << bit);
                                byte b1 = (byte)(del >> 16);
                                byte b2 = (byte)(((del >> 20) & 0xf0) | ((len - 3) & 0x0f));
                                mini.WriteByte(b1);
                                mini.WriteByte(b2);
                                aofs += len;
                            }
                        }
                        ms.WriteByte(control);
                        ms.Write(mini.ToArray(), 0, (int)mini.Length);
                    }

                    byte[] finalBody = ms.ToArray();
                    byte[] final = new byte[16 + finalBody.Length];
                    final[0] = (byte)'C'; final[1] = (byte)'L'; final[2] = (byte)'Z';
                    final[4] = (byte)(decompSize >> 24); final[5] = (byte)(decompSize >> 16); final[6] = (byte)(decompSize >> 8); final[7] = (byte)decompSize;
                    final[12] = (byte)(decompSize >> 24); final[13] = (byte)(decompSize >> 16); final[14] = (byte)(decompSize >> 8); final[15] = (byte)decompSize;
                    finalBody.CopyTo(final, 16);
                    return final;
                }
            }

            public static byte[] Pack2_64(byte[] input)
            {
                int decompSize = input.Length;
                if (decompSize == 0) return new byte[] { 0x43, 0x4C, 0x5A, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                const int MAX_SUB = 18;
                const int WINDOW_SIZE = 4096;
                const ulong PRIME_A = 37ul, PRIME_B = 54059ul, PRIME_C = 76963ul;

                var hashTables = new CLZHashTable64[MAX_SUB - 2];
                for (int i = 0; i < MAX_SUB - 2; i++) hashTables[i] = new CLZHashTable64(i + 3, false);

                const int HASH_BUFFER_SIZE = MAX_SUB * 2;
                ulong[][] hashes = new ulong[HASH_BUFFER_SIZE][];
                for (int i = 0; i < HASH_BUFFER_SIZE; i++) { hashes[i] = new ulong[MAX_SUB + 1]; hashes[i][0] = PRIME_A; }

                var clzqueue = new Queue<Tuple<CLZHashTable64.Node, int>>[MAX_SUB - 1];
                for (int i = 0; i < MAX_SUB - 1; i++) clzqueue[i] = new Queue<Tuple<CLZHashTable64.Node, int>>();

                int[] compBits = new int[decompSize + 1];
                int[] dlpair = new int[decompSize + 1];
                for (int i = 1; i <= decompSize; i++) compBits[i] = int.MaxValue;

                for (int i = 0; i < Math.Min(MAX_SUB - 1, decompSize); i++)
                {
                    ulong hx = (ulong)(sbyte)input[i] * PRIME_C;
                    for (int j = 1; j <= Math.Min(decompSize, MAX_SUB); j++)
                    {
                        int hashofs = (HASH_BUFFER_SIZE + i - j + 1) % HASH_BUFFER_SIZE;
                        hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                    }
                }

                for (int i = 0; i < decompSize; i++)
                {
                    if (i + MAX_SUB - 1 < decompSize)
                    {
                        int ofs = i + MAX_SUB - 1;
                        ulong hx = (ulong)(sbyte)input[ofs] * PRIME_C;
                        for (int j = 1; j <= MAX_SUB; j++)
                        {
                            int hashofs = (HASH_BUFFER_SIZE + ofs - j + 1) % HASH_BUFFER_SIZE;
                            hashes[hashofs][j] = (hashes[hashofs][j - 1] * PRIME_B) ^ hx;
                        }
                    }

                    for (int j = 3; j <= Math.Min(i, MAX_SUB); j++)
                    {
                        int startOfs = i - j;
                        ulong hash = hashes[startOfs % HASH_BUFFER_SIZE][j];
                        var nodeInfo = hashTables[j - 3].AddNode(input, decompSize, startOfs, hash);
                        clzqueue[j - 3].Enqueue(new Tuple<CLZHashTable64.Node, int>(nodeInfo.Item1, startOfs));
                    }

                    int[] currentDlpairs = new int[MAX_SUB];
                    int longest = 0;
                    int maxsub = Math.Min(Math.Min(MAX_SUB, decompSize - i), i);
                    for (int j = maxsub; !(longest > 0) && j >= 3; j--)
                    {
                        ulong hash = hashes[i % HASH_BUFFER_SIZE][j];
                        int prev = hashTables[j - 3].GetLast(input, decompSize, i, hash);
                        if (prev != i)
                        {
                            for (int t = Math.Max(longest, 2); t < j; t++) currentDlpairs[t] = i - prev;
                            longest = j;
                        }
                    }

                    for (int q = 0; q < MAX_SUB - 2; q++)
                    {
                        while (clzqueue[q].Count > 0 && (i + 1 - clzqueue[q].Peek().Item2) > WINDOW_SIZE)
                            hashTables[q].RemoveNode(clzqueue[q].Dequeue());
                    }

                    if (i + 1 <= decompSize && compBits[i + 1] > compBits[i] + 9)
                    {
                        compBits[i + 1] = compBits[i] + 9;
                        dlpair[i + 1] = 1;
                    }

                    for (int t = 3; t <= longest; t++)
                    {
                        if (i + t <= decompSize && compBits[i + t] > compBits[i] + 17)
                        {
                            compBits[i + t] = compBits[i] + 17;
                            dlpair[i + t] = (-currentDlpairs[t - 1] << 16) | t;
                        }
                    }
                }

                int temp_ofs = decompSize;
                int temp_del = dlpair[temp_ofs];
                while (temp_ofs > 0)
                {
                    temp_ofs -= temp_del & 0xffff;
                    int temp = dlpair[temp_ofs];
                    dlpair[temp_ofs] = temp_del;
                    temp_del = temp;
                }

                using (var ms = new MemoryStream())
                {
                    int aofs = 0;
                    while (aofs < decompSize)
                    {
                        byte control = 0;
                        var mini = new MemoryStream();

                        for (int bit = 0; bit < 8 && aofs < decompSize; bit++)
                        {
                            int del = dlpair[aofs];
                            int len = del & 0xFFFF;

                            if (len < 3)
                            {
                                mini.WriteByte(input[aofs]);
                                aofs++;
                            }
                            else
                            {
                                control |= (byte)(1 << bit);
                                byte b1 = (byte)(del >> 16);
                                byte b2 = (byte)(((del >> 20) & 0xf0) | ((len - 3) & 0x0f));
                                mini.WriteByte(b1);
                                mini.WriteByte(b2);
                                aofs += len;
                            }
                        }
                        ms.WriteByte(control);
                        ms.Write(mini.ToArray(), 0, (int)mini.Length);
                    }

                    byte[] finalBody = ms.ToArray();
                    byte[] final = new byte[16 + finalBody.Length];
                    final[0] = (byte)'C'; final[1] = (byte)'L'; final[2] = (byte)'Z';
                    final[4] = (byte)(decompSize >> 24); final[5] = (byte)(decompSize >> 16); final[6] = (byte)(decompSize >> 8); final[7] = (byte)decompSize;
                    final[12] = (byte)(decompSize >> 24); final[13] = (byte)(decompSize >> 16); final[14] = (byte)(decompSize >> 8); final[15] = (byte)decompSize;
                    finalBody.CopyTo(final, 16);
                    return final;
                }
            }
        }

        public class CLZHashTable32
        {
            public class Node { public uint Hash; public int LastIndex; public Node Prev, Next; }
            private readonly int _strLen;
            private readonly Stack<Node> _allocPool = new Stack<Node>();
            private readonly uint _numBuckets;
            private readonly Node[] _buckets;
            private readonly bool _useCircularBuffer;

            public CLZHashTable32(int strLen, bool useCircularBuffer = true, uint numBuckets = 4097)
            {
                _strLen = strLen; _useCircularBuffer = useCircularBuffer; _numBuckets = _makePrime(numBuckets); _buckets = new Node[_numBuckets];
            }

            public void RemoveNode(Tuple<Node, int> pair)
            {
                Node nodeptr = pair.Item1;
                if (nodeptr.LastIndex == pair.Item2)
                {
                    uint idx = nodeptr.Hash % _numBuckets;
                    if (nodeptr.Next != null) nodeptr.Next.Prev = nodeptr.Prev;
                    if (nodeptr.Prev != null) nodeptr.Prev.Next = nodeptr.Next; else _buckets[idx] = nodeptr.Next;
                    _allocPool.Push(nodeptr);
                }
            }

            public Tuple<Node, int> AddNode(byte[] buffer, int bufferSize, int startOfs, uint hash)
            {
                uint idx = hash % _numBuckets;
                Node prev = null, next = _buckets[idx];
                while (next != null && !(hash == next.Hash && _isEqual(buffer, bufferSize, startOfs, next.LastIndex)))
                { prev = next; next = next.Next; }
                int prevIndex = startOfs;
                if (next == null)
                {
                    next = _allocPool.Count > 0 ? _allocPool.Pop() : new Node();
                    next.Prev = prev; next.Hash = hash;
                    if (prev != null) prev.Next = next; else _buckets[idx] = next;
                    next.Next = null;
                }
                else prevIndex = next.LastIndex;
                next.LastIndex = startOfs;
                return new Tuple<Node, int>(next, prevIndex);
            }

            public int GetLast(byte[] buffer, int bufferSize, int defOfs, uint hash)
            {
                uint idx = hash % _numBuckets;
                Node next = _buckets[idx];
                while (next != null && !(hash == next.Hash && _isEqual(buffer, bufferSize, defOfs, next.LastIndex)))
                { next = next.Next; }
                return next != null ? next.LastIndex : defOfs;
            }

            private bool _isEqual(byte[] buffer, int bufferSize, int ofs1, int ofs2)
            {
                for (int i = 0; i < _strLen; i++)
                    if (buffer[(_useCircularBuffer ? (ofs1 + i) % bufferSize : ofs1 + i)] != buffer[(_useCircularBuffer ? (ofs2 + i) % bufferSize : ofs2 + i)]) return false;
                return true;
            }

            private static uint _makePrime(uint s) { s |= 1; while (!_isPrime(s)) s += 2; return s; }
            private static bool _isPrime(uint n)
            {
                if (n < 2) return false;
                if (n == 2 || n == 7 || n == 61) return true;
                if (n % 2 == 0) return false;

                uint d = n - 1;
                while ((d & 1) == 0) d >>= 1;

                uint[] test_nums = { 2, 7, 61 };
                foreach (uint test_num in test_nums)
                {
                    if (test_num >= n) break;
                    ulong x = _modExp(test_num, d, n);
                    if (x != 1 && x != n - 1)
                    {
                        bool found = false;
                        uint temp_d = d;
                        while (temp_d < n - 1)
                        {
                            x = (x * x) % n;
                            if (x == n - 1) { found = true; break; }
                            temp_d <<= 1;
                        }
                        if (!found) return false;
                    }
                }
                return true;
            }
            private static ulong _modExp(ulong x, ulong e, ulong m)
            {
                ulong result = 1; x %= m;
                while (e > 0) { if ((e & 1) == 1) result = (result * x) % m; e >>= 1; x = (x * x) % m; }
                return result;
            }
        }

        public class CLZHashTable64
        {
            public class Node { public ulong Hash; public int LastIndex; public Node Prev, Next; }
            private readonly int _strLen;
            private readonly Stack<Node> _allocPool = new Stack<Node>();
            private readonly uint _numBuckets;
            private readonly Node[] _buckets;
            private readonly bool _useCircularBuffer;

            public CLZHashTable64(int strLen, bool useCircularBuffer = true, uint numBuckets = 4097)
            {
                _strLen = strLen; _useCircularBuffer = useCircularBuffer; _numBuckets = _makePrime(numBuckets); _buckets = new Node[_numBuckets];
            }

            public void RemoveNode(Tuple<Node, int> pair)
            {
                Node nodeptr = pair.Item1;
                if (nodeptr.LastIndex == pair.Item2)
                {
                    uint idx = (uint)(nodeptr.Hash % _numBuckets);
                    if (nodeptr.Next != null) nodeptr.Next.Prev = nodeptr.Prev;
                    if (nodeptr.Prev != null) nodeptr.Prev.Next = nodeptr.Next; else _buckets[idx] = nodeptr.Next;
                    _allocPool.Push(nodeptr);
                }
            }

            public Tuple<Node, int> AddNode(byte[] buffer, int bufferSize, int startOfs, ulong hash)
            {
                uint idx = (uint)(hash % _numBuckets);
                Node prev = null, next = _buckets[idx];
                while (next != null && !(hash == next.Hash && _isEqual(buffer, bufferSize, startOfs, next.LastIndex)))
                { prev = next; next = next.Next; }
                int prevIndex = startOfs;
                if (next == null)
                {
                    next = _allocPool.Count > 0 ? _allocPool.Pop() : new Node();
                    next.Prev = prev; next.Hash = hash;
                    if (prev != null) prev.Next = next; else _buckets[idx] = next;
                    next.Next = null;
                }
                else prevIndex = next.LastIndex;
                next.LastIndex = startOfs;
                return new Tuple<Node, int>(next, prevIndex);
            }

            public int GetLast(byte[] buffer, int bufferSize, int defOfs, ulong hash)
            {
                uint idx = (uint)(hash % _numBuckets);
                Node next = _buckets[idx];
                while (next != null && !(hash == next.Hash && _isEqual(buffer, bufferSize, defOfs, next.LastIndex)))
                { next = next.Next; }
                return next != null ? next.LastIndex : defOfs;
            }

            private bool _isEqual(byte[] buffer, int bufferSize, int ofs1, int ofs2)
            {
                for (int i = 0; i < _strLen; i++)
                    if (buffer[(_useCircularBuffer ? (ofs1 + i) % bufferSize : ofs1 + i)] != buffer[(_useCircularBuffer ? (ofs2 + i) % bufferSize : ofs2 + i)]) return false;
                return true;
            }

            private static uint _makePrime(uint s) { s |= 1; while (!_isPrime(s)) s += 2; return s; }
            private static bool _isPrime(uint n)
            {
                if (n < 2) return false;
                if (n == 2 || n == 7 || n == 61) return true;
                if (n % 2 == 0) return false;

                uint d = n - 1;
                while ((d & 1) == 0) d >>= 1;

                uint[] test_nums = { 2, 7, 61 };
                foreach (uint test_num in test_nums)
                {
                    if (test_num >= n) break;
                    ulong x = _modExp(test_num, d, n);
                    if (x != 1 && x != n - 1)
                    {
                        bool found = false;
                        uint temp_d = d;
                        while (temp_d < n - 1)
                        {
                            x = (x * x) % n;
                            if (x == n - 1) { found = true; break; }
                            temp_d <<= 1;
                        }
                        if (!found) return false;
                    }
                }
                return true;
            }
            private static ulong _modExp(ulong x, ulong e, ulong m)
            {
                ulong result = 1; x %= m;
                while (e > 0) { if ((e & 1) == 1) result = (result * x) % m; e >>= 1; x = (x * x) % m; }
                return result;
            }
        }

        private void TextureViewer_Click(object sender, EventArgs e)
        {
            this.Hide(); //Esconde o formulário principal
            vT tx = new vT(); //Define form2 como o formulário de visualização e extração gráfica do PS2
            tx.FormClosed += (s, args) => this.Show(); //Fecha o programa se fechar o formulário
            tx.Show(); //Mostra o formulário
        }

        private void texEditor_Click(object sender, EventArgs e)
        {
            this.Hide(); //Esconde o formulário principal
            mesEditor mesedit = new mesEditor(); //Define form2 como o formulário de visualização e extração gráfica do PS2
            mesedit.FormClosed += (s, args) => this.Show(); //Fecha o programa se fechar o formulário
            mesedit.Show(); //Mostra o formulário
        }
    }
}