using System;
using System.Windows.Forms;
using System.Drawing;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Forms;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Views
{
    public class PatientsView : UserControl
    {
        private readonly PatientService _service;
        private DataGridView _grid;
        private TextBox _txtSearch;
        private Label _lblCount;

        public PatientsView(PatientService service)
        {
            _service = service;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            this.BackColor = Theme.Background;
            this.Padding = new Padding(24, 20, 24, 20);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Theme.Background };
            var lblHeader = new Label
            {
                Text = "Patients",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Theme.TextDark,
                Location = new Point(0, 4),
                AutoSize = true
            };
            _lblCount = new Label
            {
                Font = Theme.FontSmall,
                ForeColor = Theme.TextMuted,
                Location = new Point(2, 38),
                AutoSize = true
            };
            headerPanel.Controls.Add(_lblCount);
            headerPanel.Controls.Add(lblHeader);

            var toolbarPanel = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Theme.Background };
            var btnAdd = UIHelper.MakeActionButton("+ Add", Theme.Accent);
            var btnEdit = UIHelper.MakeActionButton("Edit", Theme.Sidebar);
            var btnView = UIHelper.MakeActionButton("View", Color.FromArgb(80, 120, 180));
            var btnDelete = UIHelper.MakeActionButton("Delete", Theme.DangerRed);
            var btnRefresh = UIHelper.MakeActionButton("Refresh", Theme.TextMuted);

            _txtSearch = new TextBox
            {
                Width = 220,
                Height = 30,
                Font = Theme.FontBody,
                BorderStyle = BorderStyle.FixedSingle
            };

            btnAdd.Location = new Point(0, 8);
            btnEdit.Location = new Point(98, 8);
            btnView.Location = new Point(196, 8);
            btnDelete.Location = new Point(294, 8);
            btnRefresh.Location = new Point(392, 8);
            _txtSearch.Location = new Point(toolbarPanel.Width - 230, 10);
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnAdd.Click += (s, e) => OpenForm(FormMode.Add, null);
            btnEdit.Click += (s, e) => { var p = GetSelected(); if (p != null) OpenForm(FormMode.Edit, p); };
            btnView.Click += (s, e) => { var p = GetSelected(); if (p != null) OpenForm(FormMode.View, p); };
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRefresh.Click += (s, e) => LoadData();
            _txtSearch.TextChanged += (s, e) => Search();

            toolbarPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnView, btnDelete, btnRefresh, _txtSearch });

            var gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
            _grid = UIHelper.MakeGrid();
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "PatientId", Width = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Full Name", DataPropertyName = "FullName" },
                new DataGridViewTextBoxColumn { HeaderText = "Gender", DataPropertyName = "Gender", Width = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Age", DataPropertyName = "Age", Width = 60, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", DataPropertyName = "Phone" },
                new DataGridViewTextBoxColumn { HeaderText = "Address", DataPropertyName = "Address" },
                new DataGridViewTextBoxColumn { HeaderText = "Registered On", DataPropertyName = "RegisteredOn" }
            );
            _grid.Columns[6].DefaultCellStyle.Format = "dd/MM/yyyy";
            gridPanel.Controls.Add(_grid);

            this.Controls.Add(gridPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);
        }

        private void LoadData(string keyword = "")
        {
            var data = string.IsNullOrWhiteSpace(keyword) ? _service.GetAll() : _service.Search(keyword);
            _grid.DataSource = data;
            _lblCount.Text = $"{data.Count} patient(s) found";
        }

        private void Search() => LoadData(_txtSearch.Text.Trim());

        private Patient GetSelected()
        {
            if (_grid.SelectedRows.Count == 0) { UIHelper.Error("Please select a patient first."); return null; }
            return (Patient)_grid.SelectedRows[0].DataBoundItem;
        }

        private void OpenForm(FormMode mode, Patient patient)
        {
            using (var f = new PatientForm(_service, mode, patient))
            {
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void DeleteSelected()
        {
            var p = GetSelected();
            if (p == null) return;
            if (!UIHelper.Confirm($"Delete patient \"{p.FullName}\"? This cannot be undone.")) return;
            if (_service.Delete(p.PatientId))
            {
                UIHelper.Success("Patient deleted.");
                LoadData();
            }
            else UIHelper.Error("Could not delete patient. They may have existing appointments.");
        }
    }
}
