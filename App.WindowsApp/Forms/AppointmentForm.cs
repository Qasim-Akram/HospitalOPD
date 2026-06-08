using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Forms
{
    public class AppointmentForm : Form
    {
        private readonly AppointmentService _service;
        private readonly PatientService _patientService;
        private readonly DoctorService _doctorService;
        private readonly FormMode _mode;
        private readonly Appointment _appointment;

        private ComboBox _cboPatient, _cboDoctor, _cboStatus;
        private DateTimePicker _dtpDate;
        private TextBox _txtReason, _txtNotes;
        private Button _btnSave, _btnCancel;

        public AppointmentForm(AppointmentService service, PatientService ps, DoctorService ds, FormMode mode, Appointment appointment)
        {
            _service = service;
            _patientService = ps;
            _doctorService = ds;
            _mode = mode;
            _appointment = appointment;
            BuildUI();
            LoadDropdowns();
            if (appointment != null) PopulateFields();
            if (mode == FormMode.View) SetReadOnly();
        }

        private void BuildUI()
        {
            this.Text = _mode == FormMode.Add ? "New Appointment" : _mode == FormMode.Edit ? "Edit Appointment" : "Appointment Details";
            this.Size = new Size(480, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Theme.Background;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var titleBar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Theme.Sidebar };
            var lblTitle = new Label
            {
                Text = this.Text,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            titleBar.Controls.Add(lblTitle);

            var formPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(24, 16, 24, 8),
                AutoSize = true,
                BackColor = Theme.Background
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _cboPatient = new ComboBox { Width = 290, Font = Theme.FontBody, DropDownStyle = ComboBoxStyle.DropDownList };
            _cboDoctor = new ComboBox { Width = 290, Font = Theme.FontBody, DropDownStyle = ComboBoxStyle.DropDownList };
            _dtpDate = new DateTimePicker { Width = 290, Font = Theme.FontBody, Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy  HH:mm", ShowUpDown = false, Value = DateTime.Now };
            _txtReason = UIHelper.MakeTextBox(290);
            _txtNotes = new TextBox { Width = 290, Height = 55, Font = Theme.FontBody, Multiline = true, BorderStyle = BorderStyle.FixedSingle };
            _cboStatus = new ComboBox { Width = 290, Font = Theme.FontBody, DropDownStyle = ComboBoxStyle.DropDownList };
            _cboStatus.Items.AddRange(new object[] { "Scheduled", "Completed", "Cancelled" });
            _cboStatus.SelectedIndex = 0;

            AddRow(formPanel, "Patient *", _cboPatient);
            AddRow(formPanel, "Doctor *", _cboDoctor);
            AddRow(formPanel, "Date & Time *", _dtpDate);
            AddRow(formPanel, "Reason", _txtReason);
            AddRow(formPanel, "Status *", _cboStatus);
            AddRow(formPanel, "Notes", _txtNotes);

            var btnPanel = new Panel { Dock = DockStyle.Bottom, Height = 52, BackColor = Theme.Background, Padding = new Padding(20, 10, 20, 10) };
            _btnSave = new Button
            {
                Text = _mode == FormMode.View ? "Close" : "Save",
                Width = 100,
                Height = 34,
                BackColor = _mode == FormMode.View ? Theme.TextMuted : Theme.Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Font = Theme.FontBody,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            _btnCancel = new Button
            {
                Text = "Cancel",
                Width = 90,
                Height = 34,
                BackColor = Color.FromArgb(235, 237, 240),
                ForeColor = Theme.TextDark,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Right,
                Font = Theme.FontBody,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };

            _btnSave.Click += BtnSave_Click;
            _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            if (_mode == FormMode.View) _btnCancel.Visible = false;

            btnPanel.Controls.Add(_btnSave);
            if (_mode != FormMode.View) btnPanel.Controls.Add(_btnCancel);

            this.Controls.Add(formPanel);
            this.Controls.Add(titleBar);
            this.Controls.Add(btnPanel);
        }

        private void AddRow(TableLayoutPanel panel, string label, Control control)
        {
            panel.Controls.Add(new Label { Text = label, Font = Theme.FontBody, ForeColor = Theme.TextMuted, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Margin = new Padding(0, 10, 0, 0) });
            panel.Controls.Add(control);
        }

        private void LoadDropdowns()
        {
            var patients = _patientService.GetAll();
            foreach (var p in patients)
                _cboPatient.Items.Add(new ComboItem(p.PatientId, p.FullName));

            var doctors = _doctorService.GetAll();
            foreach (var d in doctors)
                _cboDoctor.Items.Add(new ComboItem(d.DoctorId, d.FullName + " — " + d.Specialization));

            if (_cboPatient.Items.Count > 0) _cboPatient.SelectedIndex = 0;
            if (_cboDoctor.Items.Count > 0) _cboDoctor.SelectedIndex = 0;
        }

        private void PopulateFields()
        {
            foreach (ComboItem item in _cboPatient.Items)
                if (item.Id == _appointment.PatientId) { _cboPatient.SelectedItem = item; break; }
            foreach (ComboItem item in _cboDoctor.Items)
                if (item.Id == _appointment.DoctorId) { _cboDoctor.SelectedItem = item; break; }

            _dtpDate.Value = _appointment.AppointmentDate;
            _txtReason.Text = _appointment.Reason;
            _cboStatus.SelectedItem = _appointment.Status.ToString();
            _txtNotes.Text = _appointment.Notes;
        }

        private void SetReadOnly()
        {
            _cboPatient.Enabled = false;
            _cboDoctor.Enabled = false;
            _dtpDate.Enabled = false;
            _txtReason.ReadOnly = true;
            _txtNotes.ReadOnly = true;
            _cboStatus.Enabled = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_mode == FormMode.View) { this.DialogResult = DialogResult.OK; return; }
            if (_cboPatient.SelectedItem == null) { UIHelper.Error("Please select a patient."); return; }
            if (_cboDoctor.SelectedItem == null) { UIHelper.Error("Please select a doctor."); return; }

            var appointment = new Appointment
            {
                AppointmentId = _appointment?.AppointmentId ?? 0,
                PatientId = ((ComboItem)_cboPatient.SelectedItem).Id,
                DoctorId = ((ComboItem)_cboDoctor.SelectedItem).Id,
                AppointmentDate = _dtpDate.Value,
                Reason = _txtReason.Text.Trim(),
                Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), _cboStatus.SelectedItem.ToString()),
                Notes = _txtNotes.Text.Trim()
            };

            bool ok = _mode == FormMode.Add ? _service.Add(appointment) : _service.Update(appointment);
            if (ok)
            {
                UIHelper.Success(_mode == FormMode.Add ? "Appointment booked successfully." : "Appointment updated.");
                this.DialogResult = DialogResult.OK;
            }
            else UIHelper.Error("Operation failed. Please try again.");
        }

        private class ComboItem
        {
            public int Id { get; }
            private string _display;
            public ComboItem(int id, string display) { Id = id; _display = display; }
            public override string ToString() => _display;
        }
    }
}
