using System.Drawing;
using System.Windows.Forms;

namespace App.WindowsApp.Helpers
{
    public static class UIHelper
    {
        public static Button MakeNavButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 48,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(200, 215, 235),
                BackColor = Theme.Sidebar,
                Font = Theme.FontNav,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        public static Button MakeActionButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Height = 32,
                Width = 90,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                Font = Theme.FontBody,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        public static Label MakeLabel(string text, Font font = null, Color? color = null)
        {
            return new Label
            {
                Text = text,
                Font = font ?? Theme.FontBody,
                ForeColor = color ?? Theme.TextDark,
                AutoSize = true
            };
        }

        public static TextBox MakeTextBox(int width = 260)
        {
            return new TextBox
            {
                Width = width,
                Height = 28,
                Font = Theme.FontBody,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };
        }

        public static DataGridView MakeGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = Theme.FontBody,
                GridColor = Theme.BorderColor,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.Sidebar;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(5, 0, 0, 0);
            grid.ColumnHeadersHeight = 36;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 240, 243);
            grid.DefaultCellStyle.SelectionForeColor = Theme.TextDark;
            grid.DefaultCellStyle.Padding = new Padding(5, 0, 0, 0);
            grid.RowTemplate.Height = 32;
            grid.EnableHeadersVisualStyles = false;
            return grid;
        }

        public static Panel MakeCard(int width, int height, Color? bg = null)
        {
            return new Panel
            {
                Width = width,
                Height = height,
                BackColor = bg ?? Theme.CardBg,
                Padding = new Padding(16)
            };
        }

        public static void StyleForm(Form form)
        {
            form.BackColor = Theme.Background;
            form.Font = Theme.FontBody;
            form.StartPosition = FormStartPosition.CenterParent;
        }

        public static bool Confirm(string message)
        {
            return MessageBox.Show(message, "Confirm", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Success(string message)
        {
            MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
