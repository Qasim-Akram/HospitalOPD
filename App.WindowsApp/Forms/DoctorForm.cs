using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Forms
{
    public class DoctorForm : Form
    {
        private readonly DoctorService _service;
        private readonly FormMode _mode;
        private readonly Doctor _doctor;

        private TextBox _txtName, _txtSpec, _txtPhone, _txtEmail;
        private CheckBox _chkAvailable;
        private Button _btnSave, _btnCancel;

        public DoctorForm(DoctorService service, FormMode mode, Doctor doctor)
        {
            _service = service;
            _mode = mode;
            _doctor = doctor;
            BuildUI();
            if (doctor != null) PopulateFields();
            if (mode == FormMode.View) SetReadOnly();
        }

        private void BuildUI()
        {
            this.Text = _mode == FormMode.Add ? "Add Doctor" : _mode == FormMode.Edit ? "Edit Doctor" : "Doctor Details";
            this.Size = new Size(460, 380);
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
                AutoSize = true,
                BackColor = Theme.Background
            };
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            _txtName = UIHelper.MakeTextBox(260);
            _txtSpec = UIHelper.MakeTextBox(260);
            _txtPhone = UIHelper.MakeTextBox(260);
            _txtEmail = UIHelper.MakeTextBox(260);
            _chkAvailable = new CheckBox { Text = "Available for appointments", Font = Theme.FontBody, Checked = true, AutoSize = true };

            AddRow(formPanel, "Full Name *", _txtName);
            AddRow(formPanel, "Specialization *", _txtSpec);
            AddRow(formPanel, "Phone", _txtPhone);
            AddRow(formPanel, "Email", _txtEmail);
            AddRow(formPanel, "Status", _chkAvailable);

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

        private void PopulateFields()
        {
            _txtName.Text = _doctor.FullName;
            _txtSpec.Text = _doctor.Specialization;
            _txtPhone.Text = _doctor.Phone;
            _txtEmail.Text = _doctor.Email;
            _chkAvailable.Checked = _doctor.IsAvailable;
        }

        private void SetReadOnly()
        {
            _txtName.ReadOnly = true;
            _txtSpec.ReadOnly = true;
            _txtPhone.ReadOnly = true;
            _txtEmail.ReadOnly = true;
            _chkAvailable.Enabled = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_mode == FormMode.View) { this.DialogResult = DialogResult.OK; return; }

            if (string.IsNullOrWhiteSpace(_txtName.Text)) { UIHelper.Error("Full name is required."); return; }
            if (string.IsNullOrWhiteSpace(_txtSpec.Text)) { UIHelper.Error("Specialization is required."); return; }

            var doctor = new Doctor
            {
                DoctorId = _doctor?.DoctorId ?? 0,
                FullName = _txtName.Text.Trim(),
                Specialization = _txtSpec.Text.Trim(),
                Phone = _txtPhone.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                IsAvailable = _chkAvailable.Checked
            };

            bool ok = _mode == FormMode.Add ? _service.Add(doctor) : _service.Update(doctor);
            if (ok)
            {
                UIHelper.Success(_mode == FormMode.Add ? "Doctor added successfully." : "Doctor updated.");
                this.DialogResult = DialogResult.OK;
            }
            else UIHelper.Error("Operation failed. Please try again.");
        }
    }
}
