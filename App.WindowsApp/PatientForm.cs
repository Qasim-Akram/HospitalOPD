using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Forms
{
    public class PatientForm : Form
    {
        private readonly PatientService _service;
        private readonly FormMode _mode;
        private readonly Patient _patient;

        private TextBox _txtName, _txtPhone, _txtAddress;
        private ComboBox _cboGender;
        private DateTimePicker _dtpDob;
        private Button _btnSave, _btnCancel;

        public PatientForm(PatientService service, FormMode mode, Patient patient)
        {
            _service = service;
            _mode = mode;
            _patient = patient;
            BuildUI();
            if (patient != null) PopulateFields();
            if (mode == FormMode.View) SetReadOnly();
        }

        private void BuildUI()
        {
            this.Text = _mode == FormMode.Add ? "Add Patient" : _mode == FormMode.Edit ? "Edit Patient" : "Patient Details";
            this.Size = new Size(460, 420);
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
                Padding = new Padding(24, 16, 24, 16),
                AutoSize = true
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            formPanel.BackColor = Theme.Background;

            _txtName = UIHelper.MakeTextBox(260);
            _cboGender = new ComboBox { Width = 260, Font = Theme.FontBody, DropDownStyle = ComboBoxStyle.DropDownList };
            _cboGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            _cboGender.SelectedIndex = 0;
            _dtpDob = new DateTimePicker { Width = 260, Font = Theme.FontBody, Format = DateTimePickerFormat.Short, MaxDate = DateTime.Today };
            _txtPhone = UIHelper.MakeTextBox(260);
            _txtAddress = new TextBox { Width = 260, Height = 60, Font = Theme.FontBody, Multiline = true, BorderStyle = BorderStyle.FixedSingle };

            AddRow(formPanel, "Full Name *", _txtName);
            AddRow(formPanel, "Gender *", _cboGender);
            AddRow(formPanel, "Date of Birth *", _dtpDob);
            AddRow(formPanel, "Phone", _txtPhone);
            AddRow(formPanel, "Address", _txtAddress);

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
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 8, 0)
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

        private void PopulateFields()
        {
            _txtName.Text = _patient.FullName;
            _cboGender.SelectedItem = _patient.Gender;
            _dtpDob.Value = _patient.DateOfBirth;
            _txtPhone.Text = _patient.Phone;
            _txtAddress.Text = _patient.Address;
        }

        private void SetReadOnly()
        {
            _txtName.ReadOnly = true;
            _txtPhone.ReadOnly = true;
            _txtAddress.ReadOnly = true;
            _cboGender.Enabled = false;
            _dtpDob.Enabled = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_mode == FormMode.View) { this.DialogResult = DialogResult.OK; return; }

            if (string.IsNullOrWhiteSpace(_txtName.Text)) { UIHelper.Error("Full name is required."); return; }

            var patient = new Patient
            {
                PatientId = _patient?.PatientId ?? 0,
                FullName = _txtName.Text.Trim(),
                Gender = _cboGender.SelectedItem?.ToString() ?? "Male",
                DateOfBirth = _dtpDob.Value.Date,
                Phone = _txtPhone.Text.Trim(),
                Address = _txtAddress.Text.Trim(),
                RegisteredOn = _patient?.RegisteredOn ?? DateTime.Now
            };

            bool ok = _mode == FormMode.Add ? _service.Add(patient) : _service.Update(patient);
            if (ok)
            {
                UIHelper.Success(_mode == FormMode.Add ? "Patient added successfully." : "Patient updated.");
                this.DialogResult = DialogResult.OK;
            }
            else UIHelper.Error("Operation failed. Please try again.");
        }
    }
}
