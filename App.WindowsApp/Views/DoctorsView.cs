using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Forms;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Views
{
    public class DoctorsView : UserControl
    {
        private readonly DoctorService _service;
        private DataGridView _grid;
        private TextBox _txtSearch;
        private Label _lblCount;

        public DoctorsView(DoctorService service)
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
                Text = "Doctors",
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
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Search by name or specialization..."
            };

            btnAdd.Location = new Point(0, 8);
            btnEdit.Location = new Point(98, 8);
            btnView.Location = new Point(196, 8);
            btnDelete.Location = new Point(294, 8);
            btnRefresh.Location = new Point(392, 8);
            _txtSearch.Location = new Point(toolbarPanel.Width - 230, 10);
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnAdd.Click += (s, e) => OpenForm(FormMode.Add, null);
            btnEdit.Click += (s, e) => { var d = GetSelected(); if (d != null) OpenForm(FormMode.Edit, d); };
            btnView.Click += (s, e) => { var d = GetSelected(); if (d != null) OpenForm(FormMode.View, d); };
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRefresh.Click += (s, e) => LoadData();
            _txtSearch.TextChanged += (s, e) => Search();

            toolbarPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnView, btnDelete, btnRefresh, _txtSearch });

            var gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
            _grid = UIHelper.MakeGrid();
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "DoctorId", Width = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Full Name", DataPropertyName = "FullName" },
                new DataGridViewTextBoxColumn { HeaderText = "Specialization", DataPropertyName = "Specialization" },
                new DataGridViewTextBoxColumn { HeaderText = "Phone", DataPropertyName = "Phone" },
                new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "Email" },
                new DataGridViewCheckBoxColumn { HeaderText = "Available", DataPropertyName = "IsAvailable", Width = 80, AutoSizeMode = DataGridViewAutoSizeColumnMode.None }
            );
            gridPanel.Controls.Add(_grid);

            this.Controls.Add(gridPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);
        }

        private void LoadData(string keyword = "")
        {
            var data = string.IsNullOrWhiteSpace(keyword) ? _service.GetAll() : _service.Search(keyword);
            _grid.DataSource = data;
            _lblCount.Text = $"{data.Count} doctor(s) found";
        }

        private void Search() => LoadData(_txtSearch.Text.Trim());

        private Doctor GetSelected()
        {
            if (_grid.SelectedRows.Count == 0) { UIHelper.Error("Please select a doctor first."); return null; }
            return (Doctor)_grid.SelectedRows[0].DataBoundItem;
        }

        private void OpenForm(FormMode mode, Doctor doctor)
        {
            using (var f = new DoctorForm(_service, mode, doctor))
            {
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void DeleteSelected()
        {
            var d = GetSelected();
            if (d == null) return;
            if (!UIHelper.Confirm($"Delete Dr. \"{d.FullName}\"? This cannot be undone.")) return;
            if (_service.Delete(d.DoctorId))
            {
                UIHelper.Success("Doctor deleted.");
                LoadData();
            }
            else UIHelper.Error("Could not delete doctor. They may have existing appointments.");
        }
    }
}
