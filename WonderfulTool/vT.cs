using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace WonderfulTool
{
    public partial class vT : Form
    {
        private List<List<HarvestMoonTexture>> texturesPerFile = new List<List<HarvestMoonTexture>>();
        private List<string> openFilePaths = new List<string>();
        private Image originalImage;
        private double zoomFactor = 1.0;
        private int zoomSteps = 0;

        // Adicionada referência à textura atualmente selecionada
        private HarvestMoonTexture currentTexture;

        public vT()
        {
            InitializeComponent();

            InitializeTranslations();

            // CORREÇÃO: Define o idioma e os textos ANTES de chamar LimparUI()
            // Isso garante que _currentLang não seja nulo quando GetString() for chamado.
            SetButtonTextByCulture();

            LimparUI();

            this.comboBoxBinFiles.SelectedIndexChanged += new System.EventHandler(this.ComboBoxBinFiles_SelectedIndexChanged);
            this.comboBoxImages.SelectedIndexChanged += new System.EventHandler(this.ComboBoxImages_SelectedIndexChanged);

            // Conecta os eventos de clique dos botões de zoom aos seus respectivos métodos.
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);

            // A chamada para ConfigurarMenuDeContexto() já ocorre dentro de SetButtonTextByCulture(),
            // então não é necessário chamá-la novamente aqui.
        }

        // --- Dicionário de Localização ---
        private static Dictionary<string, Dictionary<string, string>> _translations;
        private static string _currentLang;

        private static void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<string, string>>();

            // Inglês
            _translations["en"] = new Dictionary<string, string>
            {
                {"buttonAbrirArquivos", "Open File"},
                {"selectBinFile", "Select a file"},
                {"selectimage", "Select a texture"},
                {"textureOffset", "Texture offset"},
                {"paletteOffset", "Palette offset"},
                {"resolution", "Resolution"},
                {"palette", "Palette"},
                {"zoomLabel", "Zoom:"},
                {"saveAsPng", "Save as PNG"},
                {"importPng", "Import PNG"},
                {"noImageToSave", "No image to save."},
                {"warningTitle", "Warning"},
                {"imageSavedSuccess", "Image saved successfully!"},
                {"successTitle", "Success"},
                {"noTextureSelected", "No texture selected to replace."},
                {"errorDimensionsMismatch", "Error: The imported image ({0}x{1}) has different dimensions than the original ({2}x{3})."},
                {"incompatibleDimensionsTitle", "Incompatible Dimensions"},
                {"textureImportedSuccess", "Texture imported and saved successfully!"},
                {"errorDuringImport", "An error occurred during import:\n{0}"},
                {"criticalErrorTitle", "Critical Error"},
                {"errorReadingFile", "A critical error occurred while reading the file {0}:\n{1}"},
                {"errorTitle", "Error"},
                {"saveTextureTitle", "Save Texture as PNG"},
                {"importTextureTitle", "Select a PNG image to import"},
                {"hmTexFilter", "Harvest Moon TEX"},
                {"allFilesFilter", "All Files"},
                {"pngFilter", "PNG Image"}
            };

            // Português
            _translations["pt"] = new Dictionary<string, string>
            {
                {"buttonAbrirArquivos", "Abrir arquivo"},
                {"selectBinFile", "Escolha um arquivo"},
                {"selectimage", "Escolha uma textura"},
                {"textureOffset", "Offset da textura"},
                {"paletteOffset", "Offset da paleta"},
                {"resolution", "Resolução"},
                {"palette", "Paleta"},
                {"zoomLabel", "Zoom:"},
                {"saveAsPng", "Salvar como PNG"},
                {"importPng", "Importar PNG"},
                {"noImageToSave", "Nenhuma imagem para salvar."},
                {"warningTitle", "Aviso"},
                {"imageSavedSuccess", "Imagem salva com sucesso!"},
                {"successTitle", "Sucesso"},
                {"noTextureSelected", "Nenhuma textura selecionada para substituir."},
                {"errorDimensionsMismatch", "Erro: A imagem importada ({0}x{1}) tem dimensões diferentes da original ({2}x{3})."},
                {"incompatibleDimensionsTitle", "Dimensões Incompatíveis"},
                {"textureImportedSuccess", "Textura importada e salva com sucesso!"},
                {"errorDuringImport", "Ocorreu um erro durante a importação:\n{0}"},
                {"criticalErrorTitle", "Erro Crítico"},
                {"errorReadingFile", "Ocorreu um erro crítico ao ler o arquivo {0}:\n{1}"},
                {"errorTitle", "Erro"},
                {"saveTextureTitle", "Salvar Textura como PNG"},
                {"importTextureTitle", "Selecione uma imagem PNG para importar"},
                {"hmTexFilter", "Harvest Moon TEX"},
                {"allFilesFilter", "Todos os Arquivos"},
                {"pngFilter", "Imagem PNG"}
            };

            // Espanhol
            _translations["es"] = new Dictionary<string, string>
            {
                {"buttonAbrirArquivos", "Abrir archivo"},
                {"selectBinFile", "Elige un archivo"},
                {"selectimage", "Elige una textura"},
                {"textureOffset", "Offset de textura"},
                {"paletteOffset", "Offset de paleta"},
                {"resolution", "Resolución"},
                {"palette", "Paleta"},
                {"zoomLabel", "Zoom:"},
                {"saveAsPng", "Guardar como PNG"},
                {"importPng", "Importar PNG"},
                {"noImageToSave", "No hay imagen para guardar."},
                {"warningTitle", "Aviso"},
                {"imageSavedSuccess", "¡Imagen guardada con éxito!"},
                {"successTitle", "Éxito"},
                {"noTextureSelected", "No se ha seleccionado ninguna textura para reemplazar."},
                {"errorDimensionsMismatch", "Error: La imagen importada ({0}x{1}) tiene dimensiones diferentes a la original ({2}x{3})."},
                {"incompatibleDimensionsTitle", "Dimensiones Incompatibles"},
                {"textureImportedSuccess", "¡Textura importada y guardada con éxito!"},
                {"errorDuringImport", "Ocurrió un error durante la importación:\n{0}"},
                {"criticalErrorTitle", "Error Crítico"},
                {"errorReadingFile", "Ocurrió un error crítico al leer el archivo {0}:\n{1}"},
                {"errorTitle", "Error"},
                {"saveTextureTitle", "Guardar Textura como PNG"},
                {"importTextureTitle", "Seleccione una imagen PNG para importar"},
                {"hmTexFilter", "Harvest Moon TEX"},
                {"allFilesFilter", "Todos los Archivos"},
                {"pngFilter", "Imagen PNG"}
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
        private void SetButtonTextByCulture()
        {
            _currentLang = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            if (!_translations.ContainsKey(_currentLang))
            {
                _currentLang = "en"; // Padrão para inglês
            }

            buttonAbrirArquivos.Text = GetString("buttonAbrirArquivos");
            label1.Text = GetString("selectBinFile");
            label2.Text = GetString("selectimage");
            Enderecotextura.Text = GetString("textureOffset");
            paleta.Text = GetString("paletteOffset");
            Resolucao.Text = GetString("resolution");
            palettenumber.Text = GetString("palette");

            // Reconfigura o menu de contexto com o idioma correto
            ConfigurarMenuDeContexto();
        }
        private void buttonAbrirArquivos_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = $"{GetString("hmTexFilter")}|*.TEX|{GetString("allFilesFilter")}|*.*";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() != DialogResult.OK) return;

                LimparUI();

                foreach (var path in ofd.FileNames)
                {
                    try
                    {
                        var loadedTextures = new List<HarvestMoonTexture>();
                        using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var br = new BinaryReader(fs))
                        {
                            br.ReadInt32();
                            int quantidadeTexturas = br.ReadInt32();

                            if (quantidadeTexturas <= 0) continue;

                            int[] offsets = new int[quantidadeTexturas];
                            for (int i = 0; i < quantidadeTexturas; i++) { offsets[i] = br.ReadInt32(); }

                            for (int i = 0; i < quantidadeTexturas; i++)
                            {
                                br.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                                br.ReadInt32();
                                br.BaseStream.Seek(0x1C, SeekOrigin.Current);
                                int largura = (int)Math.Pow(2, br.ReadInt16());
                                int altura = (int)Math.Pow(2, br.ReadInt16());
                                int psm = br.ReadInt16();
                                int bpp = GetBppFromPsm(psm);

                                if (bpp == 0) continue;

                                br.BaseStream.Seek(0x1A, SeekOrigin.Current);
                                int offsetdapaleta = br.ReadInt32();
                                int tamanhodapaleta = br.ReadInt32();
                                int offsetdaimagem = br.ReadInt32();
                                int tamanhodaimagem = br.ReadInt32();

                                br.BaseStream.Seek(offsets[i] + offsetdaimagem, SeekOrigin.Begin);
                                byte[] rawImageBytes = br.ReadBytes(tamanhodaimagem);
                                br.BaseStream.Seek(offsets[i] + offsetdapaleta, SeekOrigin.Begin);
                                byte[] rawPaletteBytes = br.ReadBytes(tamanhodapaleta);

                                byte[] finalPaletteBytes = (bpp == 8) ? DetwiddlePalette(rawPaletteBytes) : rawPaletteBytes;
                                Bitmap finalImage = CreateBitmapFromData(largura, altura, bpp, rawImageBytes, finalPaletteBytes);

                                loadedTextures.Add(new HarvestMoonTexture
                                {
                                    Name = $"Textura {i + 1} ({largura}x{altura} {bpp}bpp)",
                                    Width = largura,
                                    Height = altura,
                                    BitsPerPixel = bpp,
                                    Image = finalImage,
                                    TextureOffset = offsets[i] + offsetdaimagem,
                                    PaletteOffset = offsets[i] + offsetdapaleta,
                                    RawPalette = finalPaletteBytes // Armazena a paleta já corrigida
                                });
                            }
                        }

                        if (loadedTextures.Count > 0)
                        {
                            texturesPerFile.Add(loadedTextures);
                            openFilePaths.Add(path);
                            comboBoxBinFiles.Items.Add(Path.GetFileName(path));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(GetString("errorReadingFile"), Path.GetFileName(path), ex.Message), GetString("errorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (comboBoxBinFiles.Items.Count > 0)
                {
                    comboBoxBinFiles.Enabled = true;
                    comboBoxBinFiles.SelectedIndex = 0;
                }
            }
        }

        private void ComboBoxBinFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxImages.Items.Clear();
            pictureBoxDisplay.Image = null;
            currentTexture = null;
            int fileIndex = comboBoxBinFiles.SelectedIndex;
            if (fileIndex < 0) return;

            foreach (var texture in texturesPerFile[fileIndex])
            {
                comboBoxImages.Items.Add(texture.Name);
            }

            if (comboBoxImages.Items.Count > 0)
            {
                comboBoxImages.Enabled = true;
                comboBoxImages.SelectedIndex = 0;
            }
        }

        private void ComboBoxImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            int fileIndex = comboBoxBinFiles.SelectedIndex;
            int imageIndex = comboBoxImages.SelectedIndex;
            if (fileIndex < 0 || imageIndex < 0) return;

            // Define a textura atual
            currentTexture = texturesPerFile[fileIndex][imageIndex];

            originalImage = currentTexture.Image;
            zoomSteps = 0;
            AtualizarZoomFactor();
            ApplyZoom();
            btnZoomIn.Enabled = true;
            btnZoomOut.Enabled = true;
            Resolucao.Text = GetString("resolution") + $": {currentTexture.Width}x{currentTexture.Height} ({currentTexture.BitsPerPixel}bpp)";
            Enderecotextura.Text = GetString("textureOffset") + $": 0x{currentTexture.TextureOffset:X}";
            paleta.Text = GetString("paletteOffset") + $": 0x{currentTexture.PaletteOffset:X}";
        }

        #region Lógica de Zoom
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            zoomSteps++;
            AtualizarZoomFactor();
            ApplyZoom();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (zoomFactor > 0.25)
            {
                zoomSteps--;
                AtualizarZoomFactor();
                ApplyZoom();
            }
        }

        private void AtualizarZoomFactor()
        {
            zoomFactor = Math.Pow(1.25, zoomSteps);
            zoomLevel.Text = $"{GetString("zoomLabel")} {Math.Round(zoomFactor * 100)}%";
        }

        private void ApplyZoom()
        {
            if (originalImage == null) return;

            int newW = (int)(originalImage.Width * zoomFactor);
            int newH = (int)(originalImage.Height * zoomFactor);

            if (newW < 1 || newH < 1) return;

            var zoomedImage = new Bitmap(newW, newH);
            using (Graphics g = Graphics.FromImage(zoomedImage))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newW, newH);
            }
            pictureBoxDisplay.Image = zoomedImage;
            pictureBoxDisplay.SizeMode = (newW <= pictureBoxDisplay.Width && newH <= pictureBoxDisplay.Height) ? PictureBoxSizeMode.CenterImage : PictureBoxSizeMode.Zoom;
        }
        #endregion

        #region Importar/Exportar
        private void ConfigurarMenuDeContexto()
        {
            var menuDeContexto = new ContextMenuStrip();
            var salvarItem = new ToolStripMenuItem(GetString("saveAsPng"));
            salvarItem.Click += (s, e) => SalvarImagemComoPng();
            var importarItem = new ToolStripMenuItem(GetString("importPng"));
            importarItem.Click += (s, e) => ImportarImagemPng();
            menuDeContexto.Items.Add(salvarItem);
            menuDeContexto.Items.Add(importarItem);
            pictureBoxDisplay.ContextMenuStrip = menuDeContexto;
        }

        private void SalvarImagemComoPng()
        {
            if (originalImage == null)
            {
                MessageBox.Show(GetString("noImageToSave"), GetString("warningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = $"{GetString("pngFilter")}|*.png";
                sfd.Title = GetString("saveTextureTitle");
                sfd.FileName = currentTexture.Name.Replace(":", "").Replace(" ", "_"); // Nome de arquivo sugerido
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    originalImage.Save(sfd.FileName, ImageFormat.Png);
                    MessageBox.Show(GetString("imageSavedSuccess"), GetString("successTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ImportarImagemPng()
        {
            if (currentTexture == null)
            {
                MessageBox.Show(GetString("noTextureSelected"), GetString("warningTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = $"{GetString("pngFilter")}|*.png";
                ofd.Title = GetString("importTextureTitle");
                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (var newBitmap = new Bitmap(ofd.FileName))
                    {
                        if (newBitmap.Width != currentTexture.Width || newBitmap.Height != currentTexture.Height)
                        {
                            MessageBox.Show(string.Format(GetString("errorDimensionsMismatch"), newBitmap.Width, newBitmap.Height, currentTexture.Width, currentTexture.Height), GetString("incompatibleDimensionsTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Converte a imagem para os índices da paleta
                        byte[] linearIndices = QuantizeImageToIndices(newBitmap, currentTexture.RawPalette);
                        byte[] finalPixelData;

                        // Se for 4bpp, precisamos empacotar os índices
                        if (currentTexture.BitsPerPixel == 4)
                        {
                            finalPixelData = new byte[linearIndices.Length / 2];
                            for (int i = 0; i < finalPixelData.Length; i++)
                            {
                                byte p1 = linearIndices[i * 2 + 0];
                                byte p2 = linearIndices[i * 2 + 1];
                                finalPixelData[i] = (byte)(p1 | (p2 << 4));
                            }
                        }
                        else
                        {
                            finalPixelData = linearIndices;
                        }

                        // Escreve os novos dados de pixel no arquivo .TEX
                        string filePath = openFilePaths[comboBoxBinFiles.SelectedIndex];
                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
                        {
                            fs.Seek(currentTexture.TextureOffset, SeekOrigin.Begin);
                            fs.Write(finalPixelData, 0, finalPixelData.Length);
                        }

                        // Atualiza a UI
                        currentTexture.Image = (Bitmap)newBitmap.Clone();
                        originalImage = currentTexture.Image;
                        ApplyZoom();
                        MessageBox.Show(GetString("textureImportedSuccess"), GetString("successTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Format(GetString("errorDuringImport"), ex.Message), GetString("criticalErrorTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private byte[] QuantizeImageToIndices(Bitmap image, byte[] palette)
        {
            byte[] indices = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    indices[y * image.Width + x] = (byte)FindClosestPaletteIndex(pixelColor, palette);
                }
            }
            return indices;
        }

        private int FindClosestPaletteIndex(Color pixel, byte[] palette)
        {
            int bestIndex = 0;
            int smallestDistance = int.MaxValue;
            int colorCount = palette.Length / 4;

            for (int i = 0; i < colorCount; i++)
            {
                int palR = palette[i * 4 + 0];
                int palG = palette[i * 4 + 1];
                int palB = palette[i * 4 + 2];
                int palA = (int)Math.Min(palette[i * 4 + 3] * 2, 255);

                // Cálculo de "distância" entre as cores
                int dist = (pixel.R - palR) * (pixel.R - palR) +
                             (pixel.G - palG) * (pixel.G - palG) +
                             (pixel.B - palB) * (pixel.B - palB) +
                             (pixel.A - palA) * (pixel.A - palA);

                if (dist < smallestDistance)
                {
                    smallestDistance = dist;
                    bestIndex = i;
                }
                if (dist == 0) return i; // Cor exata encontrada
            }
            return bestIndex;
        }

        #endregion

        #region Métodos Auxiliares
        private void LimparUI()
        {
            comboBoxBinFiles.Items.Clear();
            comboBoxImages.Items.Clear();
            texturesPerFile.Clear();
            openFilePaths.Clear();
            pictureBoxDisplay.Image = null;
            originalImage = null;
            currentTexture = null;
            Resolucao.Text = GetString("resolution") + ":";
            Enderecotextura.Text = GetString("textureOffset") + ":";
            paleta.Text = GetString("paletteOffset") + ":";
            zoomLevel.Text = GetString("zoomLabel");
            comboBoxBinFiles.Enabled = false;
            comboBoxImages.Enabled = false;
            btnZoomIn.Enabled = false;
            btnZoomOut.Enabled = false;
            pictureBoxDisplay.BackgroundImage = CriarFundoQuadriculado(pictureBoxDisplay.Width, pictureBoxDisplay.Height, 10);
            pictureBoxDisplay.BackgroundImageLayout = ImageLayout.Tile;
        }

        private Bitmap CreateBitmapFromData(int width, int height, int bpp, byte[] pixelIndices, byte[] palette) { var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb); var rect = new Rectangle(0, 0, width, height); var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat); byte[] rgba = new byte[Math.Abs(bmpData.Stride) * height]; if (bpp == 8) { for (int i = 0; i < width * height; i++) { if (i >= pixelIndices.Length) break; int y = i / width; int x = i % width; int colorIndex = pixelIndices[i]; int paletteOffset = colorIndex * 4; if (paletteOffset + 3 >= palette.Length) continue; rgba[y * bmpData.Stride + x * 4 + 0] = palette[paletteOffset + 2]; rgba[y * bmpData.Stride + x * 4 + 1] = palette[paletteOffset + 1]; rgba[y * bmpData.Stride + x * 4 + 2] = palette[paletteOffset + 0]; rgba[y * bmpData.Stride + x * 4 + 3] = (byte)Math.Min(palette[paletteOffset + 3] * 2, 255); } } else if (bpp == 4) { for (int i = 0; i < width * height; i++) { if (i / 2 >= pixelIndices.Length) break; int y = i / width; int x = i % width; byte packedByte = pixelIndices[i / 2]; int colorIndex = (i % 2 == 0) ? (packedByte & 0x0F) : (packedByte >> 4); int paletteOffset = colorIndex * 4; if (paletteOffset + 3 >= palette.Length) continue; rgba[y * bmpData.Stride + x * 4 + 0] = palette[paletteOffset + 2]; rgba[y * bmpData.Stride + x * 4 + 1] = palette[paletteOffset + 1]; rgba[y * bmpData.Stride + x * 4 + 2] = palette[paletteOffset + 0]; rgba[y * bmpData.Stride + x * 4 + 3] = (byte)Math.Min(palette[paletteOffset + 3] * 2, 255); } } System.Runtime.InteropServices.Marshal.Copy(rgba, 0, bmpData.Scan0, rgba.Length); bmp.UnlockBits(bmpData); return bmp; }

        private byte[] DetwiddlePalette(byte[] originalPalette) { if (originalPalette.Length != 1024) { return originalPalette; } var newPalette = new byte[1024]; int chunkSize = 32; for (int blockStart = 0; blockStart < 1024; blockStart += 128) { Array.Copy(originalPalette, blockStart, newPalette, blockStart, chunkSize); Array.Copy(originalPalette, blockStart + chunkSize * 2, newPalette, blockStart + chunkSize, chunkSize); Array.Copy(originalPalette, blockStart + chunkSize, newPalette, blockStart + chunkSize * 2, chunkSize); Array.Copy(originalPalette, blockStart + chunkSize * 3, newPalette, blockStart + chunkSize * 3, chunkSize); } return newPalette; }

        private int GetBppFromPsm(int psm) { switch (psm) { case 0x13: return 8; case 0x14: return 4; default: return 0; } }

        private Bitmap CriarFundoQuadriculado(int largura, int altura, int tamanhoQuadrado) { Bitmap fundo = new Bitmap(largura, altura); using (Graphics g = Graphics.FromImage(fundo)) { Color cor1 = Color.LightGray; Color cor2 = Color.White; for (int y = 0; y < altura; y += tamanhoQuadrado) for (int x = 0; x < largura; x += tamanhoQuadrado) using (var brush = new SolidBrush(((x / tamanhoQuadrado + y / tamanhoQuadrado) % 2 == 0) ? cor1 : cor2)) g.FillRectangle(brush, x, y, tamanhoQuadrado, tamanhoQuadrado); } return fundo; }
        #endregion

        public class HarvestMoonTexture
        {
            public string Name { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public int BitsPerPixel { get; set; }
            public Bitmap Image { get; set; }
            public long TextureOffset { get; set; }
            public long PaletteOffset { get; set; }
            public byte[] RawPalette { get; set; }
        }
    }
}