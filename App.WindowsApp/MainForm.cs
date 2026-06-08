using System;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Services;
using App.WindowsApp.Helpers;
using App.WindowsApp.Views;

namespace App.WindowsApp
{
    public partial class MainForm : Form
    {
        private Panel _sidebar;
        private Panel _contentPanel;
        private Button _btnDashboard;
        private Button _btnPatients;
        private Button _btnDoctors;
        private Button _btnAppointments;
        private Button _activeNav;
        private StatusStrip _statusBar;
        private ToolStripStatusLabel _statusLabel;

        private readonly PatientService _patientService = new PatientService();
        private readonly DoctorService _doctorService = new DoctorService();
        private readonly AppointmentService _appointmentService = new AppointmentService();

        public MainForm()
        {
            InitializeComponent();
            BuildUI();
            ShowView(_btnDashboard, new DashboardView(_patientService, _doctorService, _appointmentService));
        }

        private void BuildUI()
        {
            this.Text = "Hospital OPD Management System";
            this.MinimumSize = new Size(1100, 680);
            this.Size = new Size(1280, 760);
            this.BackColor = Theme.Background;
            this.Font = Theme.FontBody;
            this.StartPosition = FormStartPosition.CenterScreen;

            _sidebar = new Panel
            {
                Width = 220,
                Dock = DockStyle.Left,
                BackColor = Theme.Sidebar
            };

            var logoPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(18, 30, 54),
                Padding = new Padding(18, 0, 0, 0)
            };
            var logoLabel = new Label
            {
                Text = "HospitalOPD",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };
            logoPanel.Controls.Add(logoLabel);

            var sectionLabel = new Label
            {
                Text = "  NAVIGATION",
                ForeColor = Color.FromArgb(100, 120, 155),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 32,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(18, 0, 0, 4)
            };

            _btnDashboard = UIHelper.MakeNavButton("Dashboard");
            _btnPatients = UIHelper.MakeNavButton("Patients");
            _btnDoctors = UIHelper.MakeNavButton("Doctors");
            _btnAppointments = UIHelper.MakeNavButton("Appointments");

            _btnDashboard.Click += (s, e) => ShowView(_btnDashboard, new DashboardView(_patientService, _doctorService, _appointmentService));
            _btnPatients.Click += (s, e) => ShowView(_btnPatients, new PatientsView(_patientService));
            _btnDoctors.Click += (s, e) => ShowView(_btnDoctors, new DoctorsView(_doctorService));
            _btnAppointments.Click += (s, e) => ShowView(_btnAppointments, new AppointmentsView(_appointmentService, _patientService, _doctorService));

            foreach (var btn in new[] { _btnAppointments, _btnDoctors, _btnPatients, _btnDashboard })
            {
                btn.MouseEnter += NavBtn_MouseEnter;
                btn.MouseLeave += NavBtn_MouseLeave;
            }

            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.FromArgb(18, 30, 54)
            };
            var lblVersion = new Label
            {
                Text = "v1.0  |  Hospital OPD",
                ForeColor = Color.FromArgb(80, 100, 130),
                Font = new Font("Segoe UI", 8f),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            bottomPanel.Controls.Add(lblVersion);

            _sidebar.Controls.Add(bottomPanel);
            _sidebar.Controls.Add(_btnAppointments);
            _sidebar.Controls.Add(_btnDoctors);
            _sidebar.Controls.Add(_btnPatients);
            _sidebar.Controls.Add(sectionLabel);
            _sidebar.Controls.Add(_btnDashboard);
            _sidebar.Controls.Add(logoPanel);

            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.Background,
                Padding = new Padding(0)
            };

            _statusBar = new StatusStrip { BackColor = Theme.Sidebar };
            _statusLabel = new ToolStripStatusLabel
            {
                Text = "Ready",
                ForeColor = Color.FromArgb(160, 180, 210)
            };
            _statusBar.Items.Add(_statusLabel);

            this.Controls.Add(_contentPanel);
            this.Controls.Add(_sidebar);
            this.Controls.Add(_statusBar);
        }

        private void ShowView(Button navBtn, UserControl view)
        {
            if (_activeNav != null)
            {
                _activeNav.BackColor = Theme.Sidebar;
                _activeNav.ForeColor = Color.FromArgb(200, 215, 235);
                _activeNav.Font = Theme.FontNav;
            }
            _activeNav = navBtn;
            navBtn.BackColor = Theme.SidebarActive;
            navBtn.ForeColor = Color.White;
            navBtn.Font = Theme.FontNavBold;

            _contentPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(view);
            _statusLabel.Text = $"Viewing: {navBtn.Text.Trim()}";
        }

        private void NavBtn_MouseEnter(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn != _activeNav) btn.BackColor = Theme.SidebarHover;
        }

        private void NavBtn_MouseLeave(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn != _activeNav) btn.BackColor = Theme.Sidebar;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ResumeLayout(false);
        }
    }
}
