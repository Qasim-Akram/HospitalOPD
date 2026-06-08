using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Forms;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Views
{
    public class AppointmentsView : UserControl
    {
        private readonly AppointmentService _service;
        private readonly PatientService _patientService;
        private readonly DoctorService _doctorService;
        private DataGridView _grid;
        private TextBox _txtSearch;
        private ComboBox _cboStatus;
        private Label _lblCount;

        public AppointmentsView(AppointmentService service, PatientService ps, DoctorService ds)
        {
            _service = service;
            _patientService = ps;
            _doctorService = ds;
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
                Text = "Appointments",
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
                Width = 180,
                Height = 30,
                Font = Theme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Search..."
            };

            _cboStatus = new ComboBox
            {
                Width = 130,
                Font = Theme.FontBody,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboStatus.Items.AddRange(new object[] { "All", "Scheduled", "Completed", "Cancelled" });
            _cboStatus.SelectedIndex = 0;

            btnAdd.Location = new Point(0, 8);
            btnEdit.Location = new Point(98, 8);
            btnView.Location = new Point(196, 8);
            btnDelete.Location = new Point(294, 8);
            btnRefresh.Location = new Point(392, 8);
            _cboStatus.Location = new Point(500, 10);
            _txtSearch.Location = new Point(toolbarPanel.Width - 190, 10);
            _txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            btnAdd.Click += (s, e) => OpenForm(FormMode.Add, null);
            btnEdit.Click += (s, e) => { var a = GetSelected(); if (a != null) OpenForm(FormMode.Edit, a); };
            btnView.Click += (s, e) => { var a = GetSelected(); if (a != null) OpenForm(FormMode.View, a); };
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRefresh.Click += (s, e) => LoadData();
            _txtSearch.TextChanged += (s, e) => LoadData();
            _cboStatus.SelectedIndexChanged += (s, e) => LoadData();

            toolbarPanel.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnView, btnDelete, btnRefresh, _cboStatus, _txtSearch });

            var gridPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
            _grid = UIHelper.MakeGrid();
            _grid.Columns.AddRange(
                new DataGridViewTextBoxColumn { HeaderText = "#", DataPropertyName = "AppointmentId", Width = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None },
                new DataGridViewTextBoxColumn { HeaderText = "Patient", DataPropertyName = "PatientName" },
                new DataGridViewTextBoxColumn { HeaderText = "Doctor", DataPropertyName = "DoctorName" },
                new DataGridViewTextBoxColumn { HeaderText = "Specialization", DataPropertyName = "DoctorSpecialization" },
                new DataGridViewTextBoxColumn { HeaderText = "Date & Time", DataPropertyName = "AppointmentDate" },
                new DataGridViewTextBoxColumn { HeaderText = "Reason", DataPropertyName = "Reason" },
                new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 100, AutoSizeMode = DataGridViewAutoSizeColumnMode.None }
            );
            _grid.Columns["AppointmentDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            _grid.CellFormatting += Grid_CellFormatting;
            gridPanel.Controls.Add(_grid);

            this.Controls.Add(gridPanel);
            this.Controls.Add(toolbarPanel);
            this.Controls.Add(headerPanel);
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_grid.Columns[e.ColumnIndex].DataPropertyName == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();
                e.CellStyle.ForeColor = status == "Scheduled" ? Theme.WarningAmber
                    : status == "Completed" ? Theme.SuccessGreen
                    : Theme.DangerRed;
                e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            }
        }

        private void LoadData()
        {
            string kw = _txtSearch.Text.Trim();
            string statusFilter = _cboStatus.SelectedItem?.ToString() ?? "All";

            System.Collections.Generic.List<Appointment> data;

            if (!string.IsNullOrEmpty(kw))
                data = _service.Search(kw);
            else if (statusFilter != "All")
                data = _service.GetByStatus((AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), statusFilter));
            else
                data = _service.GetAll();

            _grid.DataSource = data;
            _lblCount.Text = $"{data.Count} appointment(s) found";
        }

        private Appointment GetSelected()
        {
            if (_grid.SelectedRows.Count == 0) { UIHelper.Error("Please select an appointment first."); return null; }
            return (Appointment)_grid.SelectedRows[0].DataBoundItem;
        }

        private void OpenForm(FormMode mode, Appointment appointment)
        {
            using (var f = new AppointmentForm(_service, _patientService, _doctorService, mode, appointment))
            {
                if (f.ShowDialog() == DialogResult.OK) LoadData();
            }
        }

        private void DeleteSelected()
        {
            var a = GetSelected();
            if (a == null) return;
            if (!UIHelper.Confirm($"Delete appointment for \"{a.PatientName}\"? This cannot be undone.")) return;
            if (_service.Delete(a.AppointmentId))
            {
                UIHelper.Success("Appointment deleted.");
                LoadData();
            }
            else UIHelper.Error("Could not delete appointment.");
        }
    }
}
