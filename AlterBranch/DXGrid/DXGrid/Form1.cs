using DevExpress.Data;
using DevExpress.Utils.Layout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DXGrid.Structures;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DXGrid
{
    public partial class Form1 : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public Form1()
        {
            InitializeComponent();
            CellEvents();
            DBUpdate.DbVersionUpdate();
            DBUpdate.DbStructureUpdate();
            GetDataFromDB(@"SELECT * FROM [GridProjectTable]");
            sqlDataSource1.FillAsync();
            gridControl.Paint += GridControl_PaintEx;
        }

        #region Отрисовка границ шапки
        private void GridControl_PaintEx(object sender, PaintEventArgs e)
        {
            DrawSeparators(e);
        }

        private void DrawSeparators(PaintEventArgs e)
        {
            BandedGridViewInfo vi = bandedGridView1.GetViewInfo() as BandedGridViewInfo;
            Rectangle clientBounds = vi.ViewRects.Rows;
            clientBounds.Height = vi.ViewRects.RowsTotalHeight;

            if (clientBounds.IntersectsWith(vi.ViewRects.Footer))
                clientBounds.Height -= clientBounds.Bottom - vi.ViewRects.Footer.Top;

            int right = vi.BandsInfo[0].Bounds.Right;
            clientBounds.X += right;
            clientBounds.Width -= right;

            for (int i = 2; i < vi.BandsInfo.Count - 1; i++)
            {
                DrawBandSeparator(vi.BandsInfo[i], e, clientBounds);
            }
        }

        readonly int separatorWidth = 2;
        private void DrawBandSeparator(GridBandInfoArgs item, PaintEventArgs e, Rectangle clientRect)
        {
            Rectangle rect = item.Bounds;
            rect.Height = clientRect.Bottom - rect.Top;
            rect.X = rect.Right - separatorWidth;
            rect.Width = separatorWidth;
            if (rect.IntersectsWith(e.ClipRectangle) && rect.IntersectsWith(clientRect))
                DrawSeparatorLine(e, rect);
        }

        private void DrawSeparatorLine(PaintEventArgs e, Rectangle rect)
        {

            e.Graphics.FillRectangle(Brushes.Gray, rect);
        }
        #endregion
        private void CellEvents()
        {
            GridView gw = gridControl.MainView as GridView;
            #region Columns
            GridColumn colFullName = gw.Columns["FullName"];
            GridColumn colD3S1358 = gw.Columns["D3S1358"];
            GridColumn colD1S1656 = gw.Columns["D1S1656"];
            GridColumn colD2S441 = gw.Columns["D2S441"];
            GridColumn colD10S1248 = gw.Columns["D10S1248"];
            GridColumn colD13S317 = gw.Columns["D13S317"];
            GridColumn colPenta_E = gw.Columns["Penta_E"];
            GridColumn colD16S539 = gw.Columns["D16S539"];
            GridColumn colD18S51 = gw.Columns["D18S51"];
            GridColumn colD2S1338 = gw.Columns["D2S1338"];
            GridColumn colCSF1PO = gw.Columns["CSF1PO"];
            GridColumn colPenra_D = gw.Columns["Penra_D"];
            GridColumn colTH01 = gw.Columns["TH01"];
            GridColumn colvWa = gw.Columns["vWa"];
            GridColumn colD21S11 = gw.Columns["D21S11"];
            GridColumn colD7S820 = gw.Columns["D7S820"];
            GridColumn colD5S818 = gw.Columns["D5S818"];
            GridColumn colTPOX = gw.Columns["TPOX"];
            GridColumn colD8S1179 = gw.Columns["D8S1179"];
            GridColumn colD12S391 = gw.Columns["D12S391"];
            GridColumn colD19S433 = gw.Columns["D19S433"];
            GridColumn colSE33 = gw.Columns["SE33"];
            GridColumn colD22S1048 = gw.Columns["D22S1048"];
            GridColumn colDYS391 = gw.Columns["DYS391"];
            GridColumn colFGA = gw.Columns["FGA"];
            GridColumn colDYS576 = gw.Columns["DYS576"];
            GridColumn colDYS570 = gw.Columns["DYS570"];
            GridColumn colAMEL = gw.Columns["AMEL"];
            #endregion

            gw.CellValueChanged += Gw_CellValueChanged;
            gw.ShowingEditor += Gw_ShowingEditor;
        }

        private string cellvalue;
        /// <summary>
        /// Добавление данных выбранной ячейки в глобальную переменную для проверок
        /// </summary>
        private void Gw_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var gw = gridControl.MainView as GridView;

            if (gw.FocusedColumn.Caption != "Ranging")
            {
                if (gw.FocusedValue != null)
                {
                    cellvalue = gw.FocusedValue.ToString();
                }
            }
        }

        /// <summary>
        /// Обновление данных в ячейке и БД
        /// </summary>
        private void Gw_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.Caption != "Ranging") // Обновление
            {
                var row = e.RowHandle + 1;

                var col = e.Column.Caption;
                string newValue = Convert.ToString(e.Value);

                Console.WriteLine(string.IsNullOrEmpty(newValue) ? "null" : newValue);

                var gw = gridControl.MainView as GridView;

                ImportDataStruct thisRowObject = gw.FocusedRowObject as ImportDataStruct;

                if (!string.IsNullOrEmpty(newValue) && col != "Delete")
                {
                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                    {
                        Connect.Open();
                        SQLiteCommand cmd = new SQLiteCommand
                        {
                            Connection = Connect,
                            CommandText = $@"UPDATE GridProjectTable
                                             SET {col} = (@newValue)
                                             WHERE GUID = '{thisRowObject.GUID}'"
                        };
                        cmd.Parameters.AddWithValue("@newValue", newValue);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        Connect.Close();
                    }
                }
                else if (e.Column.Name != "Delete")
                {
                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                    {
                        Connect.Open();
                        SQLiteCommand cmd = new SQLiteCommand
                        {
                            Connection = Connect,
                            CommandText = $@"UPDATE GridProjectTable
                                             SET {col} = NULL
                                             WHERE GUID = '{thisRowObject.GUID}'"
                        };
                        cmd.Parameters.AddWithValue("@newValue", newValue);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        Connect.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Получает данные с БД и вносит в грид с обновлением
        /// </summary>
        /// <param name="command"> SQLite запрос/param>
        private void GetDataFromDB(string command)
        {
            List<ImportDataStruct> dataList = new List<ImportDataStruct>();
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = command // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();
                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    ImportDataStruct dataObject = new ImportDataStruct
                    {
                        FullName = Convert.ToString(sqlReader["FullName"]),
                        Telegram = Convert.ToString(sqlReader["Telegram"]),
                        D3S1358_1 = Convert.ToString(sqlReader?["D3S1358_1"]),
                        D1S1656_1 = Convert.ToString(sqlReader?["D1S1656_1"]),
                        D2S441_1 = Convert.ToString(sqlReader?["D2S441_1"]),
                        D10S1248_1 = Convert.ToString(sqlReader?["D10S1248_1"]),
                        D13S317_1 = Convert.ToString(sqlReader?["D13S317_1"]),
                        Penta_E_1 = Convert.ToString(sqlReader?["Penta_E_1"]),
                        D16S539_1 = Convert.ToString(sqlReader?["D16S539_1"]),
                        D18S51_1 = Convert.ToString(sqlReader?["D18S51_1"]),
                        D2S1338_1 = Convert.ToString(sqlReader?["D2S1338_1"]),
                        CSF1PO_1 = Convert.ToString(sqlReader?["CSF1PO_1"]),
                        Penta_D_1 = Convert.ToString(sqlReader?["Penta_D_1"]),
                        TH01_1 = Convert.ToString(sqlReader?["TH01_1"]),
                        vWa_1 = Convert.ToString(sqlReader?["vWa_1"]),
                        D21S11_1 = Convert.ToString(sqlReader?["D21S11_1"]),
                        D7S820_1 = Convert.ToString(sqlReader?["D7S820_1"]),
                        D5S818_1 = Convert.ToString(sqlReader?["D5S818_1"]),
                        TPOX_1 = Convert.ToString(sqlReader?["TPOX_1"]),
                        D8S1179_1 = Convert.ToString(sqlReader?["D8S1179_1"]),
                        D12S391_1 = Convert.ToString(sqlReader?["D12S391_1"]),
                        D19S433_1 = Convert.ToString(sqlReader?["D19S433_1"]),
                        SE33_1 = Convert.ToString(sqlReader?["SE33_1"]),
                        D22S1045_1 = Convert.ToString(sqlReader?["D22S1045_1"]),
                        DYS391 = Convert.ToString(sqlReader?["DYS391"]),
                        FGA_1 = Convert.ToString(sqlReader?["FGA_1"]),
                        DYS576 = Convert.ToString(sqlReader?["DYS576"]),
                        DYS570 = Convert.ToString(sqlReader?["DYS570"]),
                        D3S1358_2 = Convert.ToString(sqlReader?["D3S1358_2"]),
                        D1S1656_2 = Convert.ToString(sqlReader?["D1S1656_2"]),
                        D2S441_2 = Convert.ToString(sqlReader?["D2S441_2"]),
                        D10S1248_2 = Convert.ToString(sqlReader?["D10S1248_2"]),
                        D13S317_2 = Convert.ToString(sqlReader?["D13S317_2"]),
                        Penta_E_2 = Convert.ToString(sqlReader?["Penta_E_2"]),
                        D16S539_2 = Convert.ToString(sqlReader?["D16S539_2"]),
                        D18S51_2 = Convert.ToString(sqlReader?["D18S51_2"]),
                        D2S1338_2 = Convert.ToString(sqlReader?["D2S1338_2"]),
                        CSF1PO_2 = Convert.ToString(sqlReader?["CSF1PO_2"]),
                        Penta_D_2 = Convert.ToString(sqlReader?["Penta_D_2"]),
                        TH01_2 = Convert.ToString(sqlReader?["TH01_2"]),
                        vWa_2 = Convert.ToString(sqlReader?["vWa_2"]),
                        D21S11_2 = Convert.ToString(sqlReader?["D21S11_2"]),
                        D7S820_2 = Convert.ToString(sqlReader?["D7S820_2"]),
                        D5S818_2 = Convert.ToString(sqlReader?["D5S818_2"]),
                        TPOX_2 = Convert.ToString(sqlReader?["TPOX_2"]),
                        D8S1179_2 = Convert.ToString(sqlReader?["D8S1179_2"]),
                        D12S391_2 = Convert.ToString(sqlReader?["D12S391_2"]),
                        D19S433_2 = Convert.ToString(sqlReader?["D19S433_2"]),
                        SE33_2 = Convert.ToString(sqlReader?["SE33_2"]),
                        D22S1045_2 = Convert.ToString(sqlReader?["D22S1045_2"]),
                        FGA_2 = Convert.ToString(sqlReader?["FGA_2"]),
                        AMEL = Convert.ToString(sqlReader?["AMEL"]),
                        GUID = Convert.ToString(sqlReader?["GUID"]),
                        IntCount = Convert.ToInt32(sqlReader?["IntCount"])
                    };
                    dataList.Add(dataObject);
                }
                gridControl.DataSource = null;
                gridControl.DataSource = dataList;
                Connect.Close();
            }
        }

        /// <summary>
        /// Обновление грида и поиск совпадений с сортировкой
        /// </summary>
        private void FindIntersectionsButton_Click(object sender, EventArgs e)
        {
            SetIntToNull();

            var gridView = gridControl.MainView as GridView;
            var focusedObject = gridView.FocusedRowObject as ImportDataStruct;

            if (!columnSearchCheckBox.Checked)
                FindIntersections(focusedObject.GUID, true);
            else
                FindIntersectionsInBand(focusedObject.GUID, true);
        }

        private void FindIntersectionsInBand(string objGuid, bool findOnce)
        {
            var gw = gridControl.MainView as GridView;
            var row = 0;
            int maxRows = 0;

            while (!string.IsNullOrEmpty((string)gw.GetRowCellValue(row, gw.Columns[1]))) // Вычисление максимального количества записей
            {
                row++;
                maxRows = row;
            }

            for (int ii = 0; ii < gw.Columns.Count; ii++) // Выделяет весь текущий обьект
            {
                gw.SelectCell(gw.FocusedRowHandle, gw.Columns[ii]);
            }

            ImportDataStruct rowObject = new ImportDataStruct();
            for (int rrow = 0; rrow < maxRows; rrow++)
            {
                gw.FocusedRowHandle = row;
                rowObject = gw.FocusedRowObject as ImportDataStruct;
                if (rowObject.GUID == objGuid)
                    break;
            }

            List<GridBand> bands = new List<GridBand>()
            {
                D3S1358Band,
                D1S1656Band,
                D2S441Band,
                D10S1248Band,
                D13S317Band,
                Penta_EBand,
                D16S539Band,
                D18S51Band,
                D2S1338Band,
                CSF1POBand,
                Penta_DBand,
                THO1Band,
                vWaBand,
                D21S11Band,
                D7S820Band,
                D5S810Band,
                TROXBand,
                D8S1179Band,
                D12S391Band,
                D19S433Band,
                SE33Band,
                D22S1045Band,
                DYS391Band,
                FGABand,
                DYS576Band,
                DYS570Band,
                AMELBand
            };

            foreach (GridBand band in bands)
            {
                switch (band.Columns.Count)
                {
                    case 2:
                        {
                            
                                var firstCol = band.Columns[0];
                                var secondCol = band.Columns[1];

                                var valuesToFind = new List<string>()
                                {
                                    (string)gw.GetRowCellValue(gw.FocusedRowHandle, firstCol),
                                    (string)gw.GetRowCellValue(gw.FocusedRowHandle, secondCol)
                                };

                                

                                for (int rowIndex = 0; rowIndex < gw.RowCount; rowIndex++)
                                {
                                    var thisRowData = gw.GetRow(rowIndex) as ImportDataStruct;
                                    var curguid = thisRowData.GUID;

                                    var firstColValue = (string)gw.GetRowCellValue(rowIndex, firstCol);
                                    var secondColValue = (string)gw.GetRowCellValue(rowIndex, secondCol);

                                    if (valuesToFind.Any(a => a == firstColValue))
                                    {
                                        IncrementInt(curguid);
                                    }

                                    if (valuesToFind.Any(a => a == secondColValue))
                                    {
                                        IncrementInt(curguid);
                                    }
                                }
                            break;
                        }

                    case 1:
                        {
                            var firstCol = band.Columns[0];

                            var valuesToFind = new List<string>()
                        {
                            (string)gw.GetRowCellValue(0, firstCol),
                        };

                            for (int rowIndex = 0; rowIndex < gw.RowCount; rowIndex++)
                            {
                                var thisRowData = gw.GetRow(rowIndex) as ImportDataStruct;
                                var curguid = thisRowData.GUID;

                                var firstColValue = (string)gw.GetRowCellValue(rowIndex, firstCol);

                                if (valuesToFind.Any(a => a == firstColValue))
                                {
                                    IncrementInt(curguid);
                                }
                            }
                            break;
                        }
                }
            }

            gridControl.DataSource = null;
            GetDataFromDB(@"SELECT * FROM GridProjectTable ORDER BY IntCount DESC");

            foreach (GridBand band in bands)
            {
                switch (band.Columns.Count)
                {
                    case 2:
                        {
                            
                            var firstCol = band.Columns[0];
                            var secondCol = band.Columns[1];

                            var valuesToFind = new List<string>()
                            {
                                (string)gw.GetRowCellValue(gw.FocusedRowHandle, firstCol),
                                (string)gw.GetRowCellValue(gw.FocusedRowHandle, secondCol)
                            };

                            for (int rowIndex = 0; rowIndex < gw.RowCount; rowIndex++)
                            {
                                var firstColValue = (string)gw.GetRowCellValue(rowIndex, firstCol);
                                var secondColValue = (string)gw.GetRowCellValue(rowIndex, secondCol);

                                if (valuesToFind.Any(a => a == firstColValue))
                                {
                                    gw.SelectCell(rowIndex, firstCol);
                                }

                                if (valuesToFind.Any(a => a == secondColValue))
                                {
                                    gw.SelectCell(rowIndex, secondCol);
                                }

                                var thisRowData = gw.GetRow(rowIndex) as ImportDataStruct;
                                var curguid = thisRowData.GUID;
                            }
                            break;
                        }

                    case 1:
                        {
                            var firstCol = band.Columns[0];

                            var valuesToFind = new List<string>()
                        {
                            (string)gw.GetRowCellValue(0, firstCol),
                        };

                            for (int rowIndex = 0; rowIndex < gw.RowCount; rowIndex++)
                            {
                                var firstColValue = (string)gw.GetRowCellValue(rowIndex, firstCol);

                                if (valuesToFind.Any(a => a == firstColValue))
                                {
                                    gw.SelectCell(rowIndex, firstCol);
                                }
                            }
                            break;
                        }
                }
            }

            Console.WriteLine(rowObject.FullName);
        }

        private void SetIntToNull()
        {
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3")) // Сброс колонки в БД на дефолт
            {
                Connect.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"UPDATE GridProjectTable
                                     SET IntCount = 0"
                };
                cmd.ExecuteNonQuery();
                Connect.Close();
            }
        }
        private void IncrementInt(string curguid)
        {
            int IntCount = 0;
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"SELECT IntCount
                                     FROM GridProjectTable
                                     WHERE GUID = '{curguid}'"
                };
                SQLiteDataReader sqlReader = cmd.ExecuteReader();

                while (sqlReader.Read())
                {
                    IntCount = Convert.ToInt32(sqlReader["IntCount"]);
                }
            }

            IntCount++;

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"UPDATE GridProjectTable
                                     SET IntCount = {IntCount}
                                     WHERE GUID = '{curguid}'"
                };
                cmd.Prepare();
                cmd.ExecuteNonQuery();
                Connect.Close();
            }
        }

        /// <summary>
        /// Выделяет все совпадения в БД и отрисовывает грид с сортировкой по убыванию
        /// </summary>
        private void FindIntersections(string objGuid, bool findOnce)
        {
            var gw = gridControl.MainView as GridView;
            var row = 0;
            int maxRows = 0;
            int maxInt = 0;
            int IntCount = 0;

            while (!string.IsNullOrEmpty((string)gw.GetRowCellValue(row, gw.Columns[1]))) // Вычисление максимального количества записей
            {
                row++;
                maxRows = row;
            }

            ImportDataStruct rowObject = new ImportDataStruct();
            for (int rrow = 0; rrow < maxRows; rrow++)
            {
                gw.FocusedRowHandle = row;
                rowObject = gw.FocusedRowObject as ImportDataStruct;
                if (rowObject.GUID == objGuid)
                {
                    break;
                }
            }

            Console.WriteLine(rowObject.FullName);
            List<string> valuesToFind = new List<string>
            {
                rowObject.FullName,
                rowObject.Telegram,
                rowObject.D3S1358_1, // have
                rowObject.D3S1358_2, // have
                rowObject.D1S1656_1, // have
                rowObject.D1S1656_2, // have
                rowObject.D2S441_1, // have
                rowObject.D2S441_2, // have      
                rowObject.D10S1248_1,  // have
                rowObject.D10S1248_2,  // have   
                rowObject.D13S317_1, // have
                rowObject.D13S317_2,  // have    
                rowObject.Penta_E_1, // have
                rowObject.Penta_E_2, // have
                rowObject.D16S539_1, // have
                rowObject.D16S539_2, // have  
                rowObject.D18S51_1, // have
                rowObject.D18S51_2, // have     
                rowObject.D2S1338_1, // have
                rowObject.D2S1338_2, // have     
                rowObject.CSF1PO_1, // have
                rowObject.CSF1PO_2, // have
                rowObject.Penta_D_1, // have
                rowObject.Penta_D_2, // have
                rowObject.TH01_1, // have
                rowObject.TH01_2, // have
                rowObject.vWa_1, // have
                rowObject.vWa_2, // have
                rowObject.D21S11_1, // have
                rowObject.D21S11_2, // have
                rowObject.D7S820_1, // have
                rowObject.D7S820_2, // have
                rowObject.D5S818_1, // have
                rowObject.D5S818_2, // have
                rowObject.TPOX_1, // have
                rowObject.TPOX_2, // have
                rowObject.D8S1179_1, // have
                rowObject.D8S1179_2, // have
                rowObject.D12S391_1, // have
                rowObject.D12S391_2, // have
                rowObject.D19S433_1, // have
                rowObject.D19S433_2, // have
                rowObject.SE33_1, // have
                rowObject.SE33_2, // have
                rowObject.D22S1045_1, // have
                rowObject.D22S1045_2, // have
                rowObject.DYS391, // have
                rowObject.FGA_1, // have
                rowObject.FGA_2, // have
                rowObject.DYS576, // have
                rowObject.DYS570, // have
                rowObject.GUID // have
            };

            #region Поиск совпадений и запись их в БД
            if (valuesToFind[0] != string.Empty)
            {
                GridColumnReadOnlyCollection cols = gw.SortedColumns;
                foreach (GridColumn column in cols)
                {
                    column.SortOrder = ColumnSortOrder.None;
                }

                SetIntToNull();

                if (findOnce == true)
                {
                    for (int ii = 0; ii < gw.Columns.Count; ii++) // Выделяет весь текущий обьект
                    {
                        gw.SelectCell(gw.FocusedRowHandle, gw.Columns[ii]);
                    }
                }

                var findValueIndex = 1;
                for (int columnIndex = 2; columnIndex < gw.Columns.Count; columnIndex++) // Поиск совпадений и запись их количества в бд
                {
                    for (int rowIndex = 0; rowIndex < maxRows; rowIndex++)
                    {
                        if (findValueIndex > valuesToFind.Count - 1)
                        {
                            break;
                        }

                        string ObjectCellValue = valuesToFind[findValueIndex];
                        if (string.IsNullOrEmpty(ObjectCellValue))
                        {
                            continue;
                        }

                        string CellValue = Convert.ToString(gw.GetRowCellValue(rowIndex, gw.Columns[columnIndex]));
                        if (string.IsNullOrEmpty(CellValue))
                        {
                            continue;
                        }

                        var thisRowData = gw.GetRow(rowIndex) as ImportDataStruct;

                        if (String.IsNullOrEmpty(CellValue))
                        {
                            continue;
                        }

                        bool anyIntersect = ObjectCellValue == CellValue;
                        var curguid = thisRowData.GUID;
                        if (anyIntersect)
                        {
                            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                            {
                                Connect.Open();
                                SQLiteCommand cmd = new SQLiteCommand
                                {
                                    Connection = Connect,
                                    CommandText = $@"SELECT IntCount
                                                     FROM GridProjectTable
                                                     WHERE GUID = '{curguid}'"
                                };
                                SQLiteDataReader sqlReader = cmd.ExecuteReader();

                                while (sqlReader.Read())
                                {
                                    IntCount = Convert.ToInt32(sqlReader["IntCount"]);
                                }
                            }

                            IntCount++;

                            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                            {
                                Connect.Open();
                                SQLiteCommand cmd = new SQLiteCommand
                                {
                                    Connection = Connect,
                                    CommandText = $@"UPDATE GridProjectTable
                                                     SET IntCount = {IntCount}
                                                     WHERE GUID = '{curguid}'"
                                };
                                cmd.Prepare();
                                cmd.ExecuteNonQuery();
                                Connect.Close();
                            }
                        }
                    }
                    findValueIndex++;
                }

                if (findOnce == true)
                {
                    gridControl.DataSource = null;
                    GetDataFromDB(@"SELECT * FROM GridProjectTable");
                }

                List<int> counts = new List<int>();
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect.Open();
                    SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = $@"SELECT IntCount
                                         FROM GridProjectTable"
                    };
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        counts.Add(Convert.ToInt32(sqlReader.GetValue(0)));
                    }
                    maxInt = counts.OrderByDescending(a => a).ToList()[1];
                }

                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect.Open();
                    SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = $@"UPDATE GridProjectTable
                                         SET MaxInt = {maxInt}
                                         WHERE GUID = '{rowObject.GUID}'"
                    };
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    Connect.Close();
                }
                #endregion

                int kekw = 0;
                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect.Open();
                    SQLiteCommand cmd = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = $@"SELECT MaxInt
                                         FROM GridProjectTable
                                         WHERE GUID = '{rowObject.GUID}'"
                    };
                    SQLiteDataReader sqlReader = cmd.ExecuteReader();

                    while (sqlReader.Read())
                    {
                        kekw = Convert.ToInt32(sqlReader.GetValue(0));
                    }
                    Console.WriteLine(kekw);
                }

                #region Поиск совпадений и отрисовка
                if (findOnce == true)
                {
                    findValueIndex = 1;
                    for (int columnIndex = 2; columnIndex < gw.Columns.Count; columnIndex++) // Вторым циклом выделяются все совпадения в уже отсортированном гриде
                    {
                        for (int rowIndex = 0; rowIndex < maxRows; rowIndex++)
                        {
                            if (findValueIndex > valuesToFind.Count - 1)
                            {
                                break;
                            }

                            string ObjectCellValue = valuesToFind[findValueIndex];
                            if (string.IsNullOrEmpty(ObjectCellValue))
                            {
                                continue;
                            }

                            string CellValue = Convert.ToString(gw.GetRowCellValue(rowIndex, gw.Columns[columnIndex]));
                            if (string.IsNullOrEmpty(CellValue))
                            {
                                continue;
                            }

                            var thisRowData = gw.GetRow(rowIndex) as ImportDataStruct;

                            bool anyIntersect = ObjectCellValue == CellValue;
                            if (anyIntersect)
                            {
                                gw.SelectCell(rowIndex, gw.Columns[columnIndex]);
                            }
                        }
                        findValueIndex++;
                    }
                    gw.Columns["IntCount"].SortOrder = ColumnSortOrder.Descending;
                    #endregion
                }
            }
        }
        /// <summary>
        /// Добавляет новую запись в бд и обновляет грид
        /// </summary>
        private void newRowButton_Click(object sender, EventArgs e)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var gw = gridControl.MainView as GridView;
            GridColumnReadOnlyCollection cols = gw.SortedColumns;
            int maxRows = 0;

            foreach (GridColumn column in cols)
            {
                column.SortOrder = ColumnSortOrder.None;
            }

            maxRows = gw.RowCount;
            Random r1 = new Random();

            var a = e as MouseEventArgs;
            if (a.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift) // Заполнение тестовыми данными если нажат шифт
            {
                for (int x = 0; x < 25; x++)
                {
                    var randomName = new List<string>();
                    var randomNum = new List<string>();
                    randomName.Add($"Test{x}");

                    for (int iii = 0; iii < 56; iii++)
                    {
                        int a1 = r1.Next(1, 20);
                        randomNum.Add($"{a1}");
                    }

                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                    {
                        Connect.Open();
                        SQLiteCommand Command = new SQLiteCommand
                        {
                            Connection = Connect,
                            CommandText = $@"INSERT INTO GridProjectTable (FullName, D3S1358_1, D3S1358_2, D1S1656_1, D1S1656_2, D2S441_1, D2S441_2, D10S1248_1, D10S1248_2, D13S317_1, D13S317_2, Penta_E_1,  Penta_E_2, D16S539_1,  D16S539_2, D18S51_1,  D18S51_2, D2S1338_1,  D2S1338_2, CSF1PO_1,  CSF1PO_2, Penta_D_1,  Penta_D_2, TH01_1,  TH01_2, vWa_1,  vWa_2, D21S11_1,  D21S11_2, D7S820_1,  D7S820_2, D5S818_1,  D5S818_2, TPOX_1,  TPOX_2, D8S1179_1,  D8S1179_2, D12S391_1,  D12S391_2, D19S433_1,  D19S433_2, SE33_1, D22S1045_1,  D22S1045_2, DYS391, FGA_1,  FGA_2, DYS576, DYS570, AMEL, GUID)
                                             VALUES ('{randomName[0]}', '{randomNum[1]}', '{randomNum[2]}', '{randomNum[3]}', '{randomNum[4]}', '{randomNum[5]}', '{randomNum[6]}', '{randomNum[7]}', '{randomNum[8]}', '{randomNum[9]}', '{randomNum[10]}', '{randomNum[11]}', '{randomNum[12]}', '{randomNum[13]}', '{randomNum[14]}', '{randomNum[15]}', '{randomNum[16]}', '{randomNum[17]}', '{randomNum[18]}', '{randomNum[19]}', '{randomNum[20]}', '{randomNum[21]}', '{randomNum[22]}', '{randomNum[23]}', '{randomNum[24]}', '{randomNum[25]}', '{randomNum[26]}', '{randomNum[28]}', '{randomNum[29]}', '{randomNum[30]}', '{randomNum[31]}', '{randomNum[32]}', '{randomNum[33]}', '{randomNum[34]}', '{randomNum[35]}', '{randomNum[36]}', '{randomNum[37]}', '{randomNum[38]}', '{randomNum[39]}', '{randomNum[40]}', '{randomNum[41]}', '{randomNum[42]}', '{randomNum[43]}', '{randomNum[45]}', '{randomNum[46]}', '{randomNum[46]}', '{randomNum[47]}', '{randomNum[48]}', '{randomNum[49]}', '{randomNum[50]}',  '{Guid.NewGuid()}')" // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
                        };
                        Command.ExecuteNonQuery();
                        Connect.Close();
                        gridControl.DataSource = null;
                        GetDataFromDB(@"SELECT * FROM [GridProjectTable]");
                    }
                }
            }
            else
            {
                foreach (GridColumn column in cols)
                {
                    column.SortOrder = ColumnSortOrder.None;
                }



                using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
                {
                    Connect.Open();
                    SQLiteCommand Command = new SQLiteCommand
                    {
                        Connection = Connect,
                        CommandText = $@"INSERT INTO GridProjectTable (FullName, D3S1358_1, D3S1358_2, D1S1656_1, D1S1656_2, D2S441_1, D2S441_2, D10S1248_1, D10S1248_2, D13S317_1, D13S317_2, Penta_E_1,  Penta_E_2, D16S539_1,  D16S539_2, D18S51_1,  D18S51_2, D2S1338_1,  D2S1338_2, CSF1PO_1,  CSF1PO_2, Penta_D_1,  Penta_D_2, TH01_1,  TH01_2, vWa_1,  vWa_2, D21S11_1,  D21S11_2, D7S820_1,  D7S820_2, D5S818_1,  D5S818_2, TPOX_1,  TPOX_2, D8S1179_1,  D8S1179_2, D12S391_1,  D12S391_2, D19S433_1,  D19S433_2, SE33_1, D22S1045_1,  D22S1045_2, DYS391, FGA_1,  FGA_2, DYS576, DYS570, AMEL,  AMEL, GUID) 
                                         VALUES (null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, '{Guid.NewGuid()}')" // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
                    };
                    Command.ExecuteNonQuery();
                    Connect.Close();
                    gridControl.DataSource = null;
                    GetDataFromDB(@"SELECT * FROM [GridProjectTable]");
                }
            }
            watch.Stop();
        }

        /// <summary>
        /// Удаляет запись и обновляет грид
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteGridButton_Click(object sender, EventArgs e)
        {
            var gridView = gridControl.MainView as GridView;
            ImportDataStruct rowObject = gridView.FocusedRowObject as ImportDataStruct;
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"DELETE FROM GridProjectTable
                                         WHERE GUID = '{rowObject.GUID}'"
                };
                Command.ExecuteNonQuery();
                Connect.Close();
            }
            gridControl.DataSource = null;
            GetDataFromDB(@"SELECT * FROM GridProjectTable");
        }

        /// <summary>
        /// Открытие телеграм-чата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repositoryItemHyperLinkEdit1_OpenLink(object sender, DevExpress.XtraEditors.Controls.OpenLinkEventArgs e)
        {
            var gw = gridControl.MainView as GridView;
            var getRowData = gw.FocusedRowObject as ImportDataStruct;
            var userName = getRowData.Telegram.Trim(new char[] { '@' });

            Process.Start($"tg://resolve?domain={userName}");
        }

        /// <summary>
        /// Открыта ли первая страница
        /// </summary>
        int mainPageState = 0;
        private void allObjectsIntersections_Click(object sender, EventArgs e)
        {

            if (mainPageState == 0)
            {
                gridControl.Visible = false;
                gridControl1.Visible = true;
                mainPageState = 1;
            }
            else if (mainPageState == 1)
            {
                gridControl.Visible = true;
                gridControl1.Visible = false;
                mainPageState = 0;
            }
            AllIntPageLoad();
            //List<Name_Count_DataStruct> dataList = new List<Name_Count_DataStruct>();
            //using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            //{
            //    Connect.Open();
            //    SQLiteCommand Command = new SQLiteCommand
            //    {
            //        Connection = Connect,
            //        CommandText = $@"SELECT MaxInt, FullName, GUID
            //                         FROM GridProjectTable
            //                         ORDER BY MaxInt DESC" // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
            //    };
            //    SQLiteDataReader sqlReader = Command.ExecuteReader();
            //    while (sqlReader.Read()) // считываем и вносим в лист все параметры
            //    {
            //        Name_Count_DataStruct dataObject = new Name_Count_DataStruct();
            //        dataObject.FullName = Convert.ToString(sqlReader?.GetValue(1));
            //        dataObject.MaxInt = Convert.ToInt32(sqlReader?.GetValue(0));
            //        dataObject.GUID = Convert.ToString(sqlReader?.GetValue(2));
            //        dataList.Add(dataObject);
            //    }

            //    List<Name_Count_DataStruct> kekwSource = new List<Name_Count_DataStruct>();
            //    var groupedDataList = dataList.Where(a => a.MaxInt != 0).GroupBy(x => x.MaxInt).Select(x => x).ToList();
            //    var gw1 = gridControl1.MainView as GridView;

            //    gw1.RowHeight = 36;
            //    int startY = 58;
            //    for (int i = 0; i < groupedDataList.Count; i++)
            //    {

            //        StackPanel sp = new StackPanel();
            //        sp.Height = 36;
            //        sp.Width = 1835;
            //        sp.Location = new Point(83, startY);
            //        sp.Visible = true;
            //        sp.Parent = gridControl1;
            //        sp.Name = "sp" + 1;

            //        startY = sp.Bottom + 3;

            //        int groupW = 0;
            //        var names = dataList.Where(a => a.MaxInt == groupedDataList[i].Key).OrderBy(a => a.FullName).Select(a => new Name_Count_DataStruct { FullName = a.FullName, GUID = a.GUID });
            //        foreach (var name in names)
            //        {
            //            Label lb = new Label();
            //            lb.Text = name.FullName;
            //            lb.Tag = name.GUID;
            //            lb.Name = "lb" + name.FullName;
            //            lb.TextAlign = ContentAlignment.MiddleCenter;
            //            lb.Parent = sp;
            //            lb.Height = 34;
            //            lb.BackColor = Color.LightGray;
            //            lb.MouseEnter += Lb_MouseEnter;
            //            lb.MouseLeave += Lb_MouseLeave;
            //            lb.Click += Lb_Click;

            //            groupW += lb.Width;
            //            if (groupW > sp.Width)
            //            {
            //                problemRow = i;
            //                sp.AutoScroll = true;
            //                sp.AutoScrollMargin = new Size(15, 0);
            //                sp.Height += 10;
            //                startY = sp.Bottom + 1;
            //                gw1.CalcRowHeight += gridView1_CalcRowHeight;
            //            }
            //        }
            //        Name_Count_DataStruct obj = new Name_Count_DataStruct();
            //        obj.MaxInt = groupedDataList[i].Key;
            //        kekwSource.Add(obj);
            //    }
            //    gridControl1.DataSource = kekwSource;
            //    Connect.Close();
            //}
        }

        /// <summary>
        /// Загрузка страницы с совпадениями "все со всеми"
        /// </summary>
        private void AllIntPageLoad()
        {
            List<Name_Count_DataStruct> dataList = new List<Name_Count_DataStruct>();
            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3"))
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"SELECT MaxInt, FullName, GUID
                                     FROM GridProjectTable
                                     ORDER BY MaxInt DESC" // выборка записей с заполненной ячейкой формата изображения, можно другой запрос составить
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();
                while (sqlReader.Read()) // считываем и вносим в лист все параметры
                {
                    Name_Count_DataStruct dataObject = new Name_Count_DataStruct
                    {
                        FullName = Convert.ToString(sqlReader?.GetValue(1)),
                        MaxInt = Convert.ToInt32(sqlReader?.GetValue(0)),
                        GUID = Convert.ToString(sqlReader?.GetValue(2))
                    };
                    dataList.Add(dataObject);
                }

                List<Name_Count_DataStruct> kekwSource = new List<Name_Count_DataStruct>();
                var groupedDataList = dataList.Where(a => a.MaxInt != 0).GroupBy(x => x.MaxInt).Select(x => x).ToList();
                var gw1 = gridControl1.MainView as GridView;

                gw1.RowHeight = 36;
                int startY = 58;
                for (int i = 0; i < groupedDataList.Count; i++)
                {

                    StackPanel sp = new StackPanel
                    {
                        Height = 36,
                        Width = 1835,
                        Location = new Point(83, startY),
                        Visible = true,
                        Parent = gridControl1,
                        Name = "sp" + 1
                    };

                    startY = sp.Bottom + 3;

                    int groupW = 0;
                    var names = dataList.Where(a => a.MaxInt == groupedDataList[i].Key).OrderBy(a => a.FullName).Select(a => new Name_Count_DataStruct { FullName = a.FullName, GUID = a.GUID });
                    foreach (var name in names)
                    {
                        Label lb = new Label
                        {
                            Text = name.FullName,
                            Tag = name.GUID,
                            Name = "lb" + name.FullName,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Parent = sp,
                            Height = 34,
                            BackColor = Color.LightGray
                        };
                        lb.MouseEnter += Lb_MouseEnter;
                        lb.MouseLeave += Lb_MouseLeave;
                        lb.Click += Lb_Click;

                        groupW += lb.Width;
                        if (groupW > sp.Width)
                        {
                            problemRow = i;
                            sp.AutoScroll = true;
                            sp.AutoScrollMargin = new Size(15, 0);
                            sp.Height += 10;
                            startY = sp.Bottom + 1;
                            gw1.CalcRowHeight += gridView1_CalcRowHeight;
                        }
                    }
                    Name_Count_DataStruct obj = new Name_Count_DataStruct
                    {
                        MaxInt = groupedDataList[i].Key
                    };
                    kekwSource.Add(obj);
                }
                gridControl1.DataSource = kekwSource;
                Connect.Close();
            }
        }

        /// <summary>
        /// Отрисовка совпадений по клику из списка "все со всеми"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Lb_Click(object sender, EventArgs e)
        {
            GetDataFromDB(@"SELECT * FROM GridProjectTable");
            Label lb = sender as Label;
            GridView gw = gridControl.MainView as GridView;
            var guid = lb.Tag;

            int maxRows = 0;
            int i = 0;
            while (!string.IsNullOrEmpty((string)gw.GetRowCellValue(i, gw.Columns[1]))) // Вычисление максимального количества записей
            {
                i++;
                maxRows = i;
            }

            for (int row = 0; row < maxRows; row++)
            {
                gw.FocusedRowHandle = row;
                var rowObject = gw.FocusedRowObject as ImportDataStruct;
                if ((string)rowObject.GUID == (string)guid)
                {
                    gridControl1.Visible = false;
                    gridControl.Visible = true;
                    FindIntersections(rowObject.GUID, true);
                    break;
                }
            }


        }

        int problemRow = -1;
        private void gridView1_CalcRowHeight(object sender, RowHeightEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null) return;
            if (e.RowHandle >= 0 && e.RowHandle == problemRow)
            {
                var currentHeight = view.RowHeight;
                e.RowHeight = currentHeight + 19;
            }
        }

        #region Обработка ивентов
        private void Lb_MouseLeave(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            lb.BackColor = Color.LightGray;
            Cursor = Cursors.Arrow;
        }

        private void Lb_MouseEnter(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            lb.BackColor = Color.DarkGray;
            Cursor = Cursors.Hand;

        }

        private void gw1HyperLink_OpenLink(object sender, DevExpress.XtraEditors.Controls.OpenLinkEventArgs e)
        {
            var gw1 = gridControl1.MainView as GridView;
        }

        private void allObjSearch_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            GetDataFromDB(@"SELECT * FROM GridProjectTable");
            var gw = gridControl.MainView as GridView;
            var minRow = 0;
            int maxRows = 0;
            while (!string.IsNullOrEmpty((string)gw.GetRowCellValue(minRow, gw.Columns[1]))) // Вычисление максимального количества записей
            {
                minRow++;
                maxRows = minRow;
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source =" + Application.StartupPath + @"\GridProjectDB.db; version = 3")) // Сброс колонки в БД на дефолт
            {
                Connect.Open();
                SQLiteCommand cmd = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = $@"UPDATE GridProjectTable
                                     SET MaxInt = 0"
                };
                cmd.ExecuteNonQuery();
                Connect.Close();
            }

            for (int row = 0; row < maxRows; row++)
            {
                gw.FocusedRowHandle = row;
                var focusedObject = gw.FocusedRowObject as ImportDataStruct;
                var guid = focusedObject.GUID;

                FindIntersections(guid, false);
            }

            if (mainPageState == 0)
            {
                gridControl.Visible = false;
                gridControl1.Visible = true;
                mainPageState = 1;
            }
            AllIntPageLoad();

            Cursor.Current = Cursors.Arrow;
        }
        #endregion
    }
}
