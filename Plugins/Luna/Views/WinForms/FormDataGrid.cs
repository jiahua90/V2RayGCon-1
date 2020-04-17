﻿using Luna.Resources.Langs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Luna.Views.WinForms
{
    public partial class FormDataGrid : Form
    {
        readonly int MAX_TITLE_LEN = 60;
        readonly int UPDATE_INTERVAL = 500;

        private readonly string title;
        private DataTable dataSource;
        private readonly int defColumn;
        private string filterKeyword = string.Empty;

        public List<List<string>> results = new List<List<string>>();
        VgcApis.Libs.Tasks.LazyGuy uiUpdater;

        public FormDataGrid(string title, DataTable dataSource, int defColumn)
        {
            InitializeComponent();
            this.title = title;
            this.dataSource = dataSource;
            this.defColumn = defColumn;
            VgcApis.Misc.UI.AutoSetFormIcon(this);
            Disposed += (s, a) => Cleanup();
        }

        private void FormDataGrid_Load(object sender, EventArgs e)
        {
            InitControls();
            uiUpdater = new VgcApis.Libs.Tasks.LazyGuy(UpdateUiLater, UPDATE_INTERVAL);
            UpdateUiLater();
        }

        #region private methods
        List<string> GetHeaders(DataGridView dataGrid)
        {
            var r = new List<string>();
            var columns = dataGrid.Columns;
            foreach (DataGridViewColumn column in columns)
            {
                r.Add(column.HeaderText.ToString());
            }
            return r;
        }

        List<List<string>> GetColumns(bool isIncludeHeader, bool isSelectedOnly)
        {
            var r = new List<List<string>>();
            if (isIncludeHeader)
            {
                r.Add(GetHeaders(dgvData));
            }

            if (isSelectedOnly)
            {
                AddSelectedRowsToDataTable(r);
            }
            else
            {
                AddAllRows(r);
            }
            return r;
        }

        private void AddAllRows(List<List<string>> dataTable)
        {
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (!row.IsNewRow)
                {
                    dataTable.Add(RowToList(row));
                }
            }
        }

        private void AddSelectedRowsToDataTable(List<List<string>> dataTable)
        {
            List<int> cache = new List<int>();
            List<DataGridViewRow> rows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in dgvData.SelectedRows)
            {
                AddToRows(cache, rows, row);
            }

            foreach (DataGridViewCell cell in dgvData.SelectedCells)
            {
                var r = dgvData.Rows[cell.RowIndex];
                AddToRows(cache, rows, r);
            }

            rows.Reverse();

            foreach (DataGridViewRow row in rows)
            {
                dataTable.Add(RowToList(row));
            }
        }

        void AddToRows(List<int> cache, List<DataGridViewRow> rows, DataGridViewRow row)
        {
            var idx = row.Index;
            if (row.IsNewRow || cache.Contains(idx))
            {
                return;
            }
            rows.Add(row);
            cache.Add(idx);
        }

        List<string> RowToList(DataGridViewRow row)
        {
            var l = new List<string>();
            foreach (DataGridViewCell cell in row.Cells)
            {
                l.Add(cell.Value.ToString());
            }
            return l;
        }

        DataTable CsvToDataTable(string csv)
        {
            var result = new DataTable();
            if (string.IsNullOrEmpty(csv))
            {
                return result;
            }

            var lines = csv.Split('\n');
            var len = lines.Count();
            if (len < 1)
            {
                return result;
            }

            var headers = lines[0].Split(',').Select(c => c ?? @"").ToList();
            var count = headers.Count();
            if (count < 1)
            {
                return result;
            }

            foreach (var header in headers)
            {
                result.Columns.Add(header);
            }

            for (int i = 1; i < len; i++)
            {
                var cells = lines[i].Split(',').Select(c => c ?? @"").ToArray();
                if (cells.Length != count)
                {
                    continue;
                }
                result.Rows.Add(cells);
            }

            return result;
        }

        string List2Csv(IEnumerable<IEnumerable<string>> contents)
        {
            var sb = new StringBuilder();
            foreach (var line in contents)
            {
                sb.Append(string.Join(@",", line));
                sb.Append('\n');
            }
            return sb.ToString();
        }

        void Cleanup()
        {
            uiUpdater?.Quit();
        }

        VgcApis.Libs.Tasks.Bar updating = new VgcApis.Libs.Tasks.Bar();
        void UpdateUiLater()
        {
            if (!updating.Install())
            {
                uiUpdater?.DoItLater();
                return;
            }

            UpdateUiThen(() => updating.Remove());
        }

        DataTable GetFilteredDataTable()
        {
            var ds = dataSource;
            var r = new DataTable();
            var idx = Math.Max(0, cboxColumnIdx.SelectedIndex);

            foreach (DataColumn column in ds.Columns)
            {
                r.Columns.Add(column.ToString());
            }

            foreach (DataRow row in ds.Rows)
            {
                var text = row[idx].ToString();
                if (VgcApis.Misc.Utils.MeasureSimilarityCi(text, filterKeyword) <= 0)
                {
                    continue;
                }

                var vs = new List<string>();
                foreach (string v in row.ItemArray)
                {
                    vs.Add(v);
                }
                r.Rows.Add(vs.ToArray());
            }

            return r;
        }

        void UpdateUiThen(Action next)
        {
            VgcApis.Misc.UI.RunInUiThreadIgnoreErrorThen(dgvData, () =>
            {
                var ds = GetFilteredDataTable();
                lbTotal.Text = ds.Rows.Count.ToString();
                dgvData.DataSource = ds;
            }, next);
        }

        void InitControls()
        {
            lbTitle.Text = VgcApis.Misc.Utils.AutoEllipsis(title, MAX_TITLE_LEN);
            toolTip1.SetToolTip(lbTitle, title);
            InitColumnsBox(dataSource);
        }

        void InitColumnsBox(DataTable dataSource)
        {
            var items = cboxColumnIdx.Items;
            items.Clear();
            foreach (DataColumn column in dataSource.Columns)
            {
                items.Add(column.ToString());
            }

            VgcApis.Misc.UI.ResetComboBoxDropdownMenuWidth(cboxColumnIdx);

            var count = cboxColumnIdx.Items.Count;
            if (count > 0)
            {
                // lua index starts from 1
                cboxColumnIdx.SelectedIndex = VgcApis.Misc.Utils.Clamp(
                    defColumn - 1, 0, count);
            }
        }

        void SetResult()
        {
            results = GetColumns(false, true);
        }

        private void SaveColumnsToFile(bool isSelectedOnly)
        {
            var content = GetColumns(true, isSelectedOnly);
            var text = List2Csv(content);
            VgcApis.Models.Datas.Enums.SaveFileErrorCode ok = VgcApis.Models.Datas.Enums.SaveFileErrorCode.Cancel;
            VgcApis.Misc.Utils.RunAsSTAThread(
                () => ok = VgcApis.Misc.UI.ShowSaveFileDialog(
                    VgcApis.Models.Consts.Files.CsvExt, text, out _));
            switch (ok)
            {
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Fail:
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.Fail);
                    break;
                case VgcApis.Models.Datas.Enums.SaveFileErrorCode.Success:
                    VgcApis.Misc.UI.MsgBoxAsync(I18N.Done);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region UI events
        private void btnCopy_Click(object sender, EventArgs e)
        {
            var content = GetColumns(true, true);
            var text = List2Csv(content);
            var success = false;
            VgcApis.Misc.Utils.RunAsSTAThread(
                () => success = VgcApis.Misc.Utils.CopyToClipboard(text));
            VgcApis.Misc.UI.MsgBoxAsync(success ? I18N.Done : I18N.Fail);
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SetResult();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tboxFilter_TextChanged(object sender, EventArgs e)
        {
            filterKeyword = tboxFilter.Text;
            uiUpdater?.DoItLater();
        }

        private void cboxColumnIdx_SelectedIndexChanged(object sender, EventArgs e)
        {
            tboxFilter.Text = @"";
            UpdateUiLater();
        }

        private void autosizeByHeaderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
        }

        private void autosizeByContentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void disableAutosizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        }

        private void importFromCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = null;

            VgcApis.Misc.Utils.RunAsSTAThread(() =>
            {
                text = VgcApis.Misc.UI.ReadFileContentFromDialog(
                    VgcApis.Models.Consts.Files.CsvExt);
            });

            if (text == null)
            {
                return;
            }

            this.dataSource = CsvToDataTable(text);
            tboxFilter.Text = @"";
            UpdateUiLater();
        }

        private void exportAllToCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveColumnsToFile(false);
        }

        private void exportSelectedToCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveColumnsToFile(true);
        }
        #endregion
    }
}