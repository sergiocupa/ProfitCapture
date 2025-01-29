

namespace ProfitCapture.UI.Template
{
    public class DataGridViewTemplate
    {
        public static void EsquemaBrancoLinhaInferior(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            grid.AutoGenerateColumns = false;
            grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = true;
            grid.RowHeadersVisible = false;
            grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            grid.GridColor = Color.FromArgb(230, 240, 250);
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            grid.ReadOnly = true;

            grid.RowTemplate.Resizable = DataGridViewTriState.True;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }


        public static void EsquemaBrancoLinhaAlternada(DataGridView grid, bool CabecalhoVisivel = true, bool readOnly = true)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            grid.ColumnHeadersVisible = CabecalhoVisivel;
            grid.AutoGenerateColumns = false;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = true;
            grid.RowHeadersVisible = false;
            grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
            grid.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.Font = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Regular);
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowsDefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(235, 245, 255);
            grid.ReadOnly = readOnly;

            grid.RowTemplate.Resizable = DataGridViewTriState.True;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

        }


        public static DataGridViewTextBoxColumn CriarColunaOculta()
        {
            return new DataGridViewTextBoxColumn()
            {
                DataPropertyName = "DataObject",
                HeaderText = "DataObject",
                Name = "DataObject",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Visible = false,
                ReadOnly = true
            };
        }

        public static DataGridViewTextBoxColumn CriarColunaTexto(string nomeCampo, string textoCabecalho, int Largura, bool readOnly = true, bool percent_scale = false)
        {
            if (Largura > 0)
            {
                if (percent_scale)
                {
                    return new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = nomeCampo,
                        HeaderText = textoCabecalho,
                        Name = nomeCampo,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                        ReadOnly = readOnly,
                        FillWeight = Largura,
                        SortMode = DataGridViewColumnSortMode.Automatic

                    };
                }
                else
                {
                    return new DataGridViewTextBoxColumn()
                    {
                        DataPropertyName = nomeCampo,
                        HeaderText = textoCabecalho,
                        Name = nomeCampo,
                        AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                        Width = Largura,
                        ReadOnly = readOnly,
                        Resizable = DataGridViewTriState.True,
                        SortMode = DataGridViewColumnSortMode.Automatic
                    };
                }
            }
            else
            {
                return new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = nomeCampo,
                    HeaderText = textoCabecalho,
                    Name = nomeCampo,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = true,
                    Resizable = DataGridViewTriState.True,
                    SortMode = DataGridViewColumnSortMode.Automatic
                };
            }
        }
        public static DataGridViewTextBoxColumn CriarColunaTexto(string nomeCampo, string textoCabecalho, bool readOnly = true)
        {
            return new DataGridViewTextBoxColumn()
            {
                DataPropertyName = nomeCampo,
                HeaderText = textoCabecalho,
                Name = nomeCampo,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.True,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
        }

        public static DataGridViewTextBoxColumn CriarColunaNumero(string nomeCampo, string textoCabecalho, bool readOnly = true)
        {
            return new DataGridViewTextBoxColumn()
            {
                DataPropertyName = nomeCampo,
                HeaderText = textoCabecalho,
                Name = nomeCampo,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.True,
                ValueType = typeof(int)
            };
        }

        public static DataGridViewTextBoxColumn CriarColunaNumero(string nomeCampo, string textoCabecalho, int largura, bool readOnly = true)
        {
            return new DataGridViewTextBoxColumn()
            {
                DataPropertyName = nomeCampo,
                HeaderText = textoCabecalho,
                Name = nomeCampo,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                Width = largura,
                ReadOnly = readOnly,
                Resizable = DataGridViewTriState.True,
                ValueType = typeof(int)
            };
        }

        public static DataGridViewButtonColumn CriarColunaBotao(string nomeCampo, string textoCabecalho, int Largura = 0, Font? fonte = null, string textoBotao = "")
        {
            if (Largura > 0)
            {
                var coluna = new DataGridViewButtonColumn()
                {
                    DataPropertyName = nomeCampo,
                    HeaderText = textoCabecalho,
                    Name = nomeCampo,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = Largura,
                    ReadOnly = true,
                    Text = textoBotao
                };
                if (fonte != null)
                    coluna.DefaultCellStyle.Font = fonte;
                return coluna;
            }
            else
            {
                var coluna = new DataGridViewButtonColumn()
                {
                    DataPropertyName = nomeCampo,
                    HeaderText = textoCabecalho,
                    Name = nomeCampo,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = true,
                    Text = textoBotao
                };
                if (fonte != null)
                    coluna.DefaultCellStyle.Font = fonte;
                return coluna;
            }
        }

        public static DataGridViewCheckBoxColumn CriarColunaSelecao(string nomeCampo, string textoCabecalho, int Largura = 0)
        {
            if (Largura > 0)
            {
                return new DataGridViewCheckBoxColumn()
                {
                    DataPropertyName = nomeCampo,
                    HeaderText = textoCabecalho,
                    Name = nomeCampo,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = Largura,
                    ReadOnly = false
                };
            }
            else
            {
                return new DataGridViewCheckBoxColumn()
                {
                    DataPropertyName = nomeCampo,
                    HeaderText = textoCabecalho,
                    Name = nomeCampo,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    ReadOnly = false
                };
            }
        }
    }
}
