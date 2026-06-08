using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Core.Models;
using App.Core.Services;
using App.WindowsApp.Helpers;

namespace App.WindowsApp.Views
{
    public class DashboardView : UserControl
    {
        private readonly PatientService _patientService;
        private readonly DoctorService _doctorService;
        private readonly AppointmentService _appointmentService;

        private Label _lblTotal;
        private Label _lblScheduled;
        private Label _lblCompleted;
        private Label _lblDoctors;
        private Panel _chartAppointments;
        private Panel _chartStatus;

        public DashboardView(PatientService ps, DoctorService ds, AppointmentService as_)
        {
            _patientService = ps;
            _doctorService = ds;
            _appointmentService = as_;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            this.BackColor = Theme.Background;
            this.Padding = new Padding(24, 20, 24, 20);

            var headerPanel = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = Theme.Background };
            var lblHeader = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Theme.TextDark,
                Location = new Point(0, 8),
                AutoSize = true
            };
            var lblSub = new Label
            {
                Text = "Welcome back — here's your OPD overview for today.",
                Font = Theme.FontSubtitle,
                ForeColor = Theme.TextMuted,
                Location = new Point(2, 44),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSub);
            headerPanel.Controls.Add(lblHeader);

            var cardsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = Theme.Background,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 8)
            };

            _lblTotal = new Label();
            _lblDoctors = new Label();
            _lblScheduled = new Label();
            _lblCompleted = new Label();

            cardsPanel.Controls.Add(MakeStatCard("Total Patients", _lblTotal, Theme.Accent));
            cardsPanel.Controls.Add(MakeStatCard("Doctors", _lblDoctors, Color.FromArgb(102, 16, 242)));
            cardsPanel.Controls.Add(MakeStatCard("Scheduled", _lblScheduled, Theme.WarningAmber));
            cardsPanel.Controls.Add(MakeStatCard("Completed", _lblCompleted, Theme.SuccessGreen));

            var chartsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Theme.Background,
                Padding = new Padding(0, 8, 0, 0)
            };
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            chartsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));

            _chartAppointments = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(0, 0, 8, 0) };
            _chartStatus = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(8, 0, 0, 0) };

            _chartAppointments.Paint += DrawBarChart;
            _chartStatus.Paint += DrawPieChart;

            chartsPanel.Controls.Add(_chartAppointments, 0, 0);
            chartsPanel.Controls.Add(_chartStatus, 1, 0);

            this.Controls.Add(chartsPanel);
            this.Controls.Add(cardsPanel);
            this.Controls.Add(headerPanel);
        }

        private Panel MakeStatCard(string label, Label valueLabel, Color accent)
        {
            var card = new Panel
            {
                Width = 200,
                Height = 110,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 16, 0)
            };

            var accentBar = new Panel
            {
                Height = 5,
                Dock = DockStyle.Top,
                BackColor = accent
            };

            valueLabel.Font = Theme.FontCardValue;
            valueLabel.ForeColor = accent;
            valueLabel.Text = "—";
            valueLabel.AutoSize = false;
            valueLabel.Width = 180;
            valueLabel.Height = 46;
            valueLabel.Location = new Point(16, 18);
            valueLabel.TextAlign = ContentAlignment.MiddleLeft;

            var lbl = new Label
            {
                Text = label,
                Font = Theme.FontCardLabel,
                ForeColor = Theme.TextMuted,
                AutoSize = false,
                Width = 180,
                Height = 20,
                Location = new Point(16, 66),
                TextAlign = ContentAlignment.MiddleLeft
            };

            card.Controls.Add(accentBar);
            card.Controls.Add(valueLabel);
            card.Controls.Add(lbl);
            return card;
        }

        private void LoadData()
        {
            try
            {
                _lblTotal.Text = _patientService.GetTotalCount().ToString();
                _lblDoctors.Text = _doctorService.GetTotalCount().ToString();
                _lblScheduled.Text = _appointmentService.GetCountByStatus(AppointmentStatus.Scheduled).ToString();
                _lblCompleted.Text = _appointmentService.GetCountByStatus(AppointmentStatus.Completed).ToString();
                _chartAppointments.Invalidate();
                _chartStatus.Invalidate();
            }
            catch { }
        }

        private void DrawBarChart(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var panel = (Panel)sender;
            int w = panel.Width, h = panel.Height;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);
            g.DrawString("Appointments by Status", new Font("Segoe UI", 10f, FontStyle.Bold), new SolidBrush(Theme.TextDark), 16, 14);

            try
            {
                int scheduled = _appointmentService.GetCountByStatus(AppointmentStatus.Scheduled);
                int completed = _appointmentService.GetCountByStatus(AppointmentStatus.Completed);
                int cancelled = _appointmentService.GetCountByStatus(AppointmentStatus.Cancelled);

                var values = new[] { scheduled, completed, cancelled };
                var labels = new[] { "Scheduled", "Completed", "Cancelled" };
                var colors = new[] { Theme.WarningAmber, Theme.SuccessGreen, Theme.DangerRed };
                int max = Math.Max(1, Math.Max(scheduled, Math.Max(completed, cancelled)));

                int chartLeft = 60, chartBottom = h - 40, chartTop = 50;
                int chartHeight = chartBottom - chartTop;
                int barWidth = 50, gap = 30;
                int startX = chartLeft + 20;

                for (int i = 0; i < 3; i++)
                {
                    int barH = (int)((double)values[i] / max * chartHeight);
                    int x = startX + i * (barWidth + gap);
                    int y = chartBottom - barH;

                    g.FillRectangle(new SolidBrush(colors[i]), x, y, barWidth, barH);
                    g.DrawString(values[i].ToString(), new Font("Segoe UI", 9f, FontStyle.Bold),
                        new SolidBrush(Theme.TextDark), x + barWidth / 2 - 6, y - 18);
                    g.DrawString(labels[i], new Font("Segoe UI", 8f),
                        new SolidBrush(Theme.TextMuted), x + barWidth / 2 - 22, chartBottom + 8);
                }

                using (var pen = new Pen(Theme.BorderColor))
                    g.DrawLine(pen, chartLeft, chartTop, chartLeft, chartBottom + 2);
                using (var pen = new Pen(Theme.BorderColor))
                    g.DrawLine(pen, chartLeft, chartBottom, w - 16, chartBottom);
            }
            catch { }
        }

        private void DrawPieChart(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var panel = (Panel)sender;
            int w = panel.Width, h = panel.Height;

            g.FillRectangle(new SolidBrush(Color.White), 0, 0, w, h);
            g.DrawString("Appointment Distribution", new Font("Segoe UI", 10f, FontStyle.Bold), new SolidBrush(Theme.TextDark), 14, 14);

            try
            {
                int scheduled = _appointmentService.GetCountByStatus(AppointmentStatus.Scheduled);
                int completed = _appointmentService.GetCountByStatus(AppointmentStatus.Completed);
                int cancelled = _appointmentService.GetCountByStatus(AppointmentStatus.Cancelled);
                int total = scheduled + completed + cancelled;
                if (total == 0) total = 1;

                var values = new[] { scheduled, completed, cancelled };
                var colors = new[] { Theme.WarningAmber, Theme.SuccessGreen, Theme.DangerRed };
                var labels = new[] { "Scheduled", "Completed", "Cancelled" };

                int cx = w / 2, cy = (h - 60) / 2 + 50;
                int radius = Math.Min(w, h - 80) / 2 - 20;
                float startAngle = -90f;

                for (int i = 0; i < 3; i++)
                {
                    float sweep = (float)values[i] / total * 360f;
                    g.FillPie(new SolidBrush(colors[i]), cx - radius, cy - radius, radius * 2, radius * 2, startAngle, sweep);
                    startAngle += sweep;
                }

                g.FillEllipse(new SolidBrush(Color.White), cx - radius / 2, cy - radius / 2, radius, radius);
                g.DrawString(total.ToString(), new Font("Segoe UI", 14f, FontStyle.Bold),
                    new SolidBrush(Theme.TextDark), cx - 14, cy - 14);

                int legendY = h - 55;
                for (int i = 0; i < 3; i++)
                {
                    int lx = 14 + i * (w / 3);
                    g.FillRectangle(new SolidBrush(colors[i]), lx, legendY, 12, 12);
                    g.DrawString(labels[i], new Font("Segoe UI", 7.5f), new SolidBrush(Theme.TextMuted), lx + 16, legendY);
                }
            }
            catch { }
        }
    }
}
