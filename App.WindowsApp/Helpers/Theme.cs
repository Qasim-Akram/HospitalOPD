using System.Drawing;

namespace App.WindowsApp.Helpers
{
    public static class Theme
    {
        public static readonly Color Sidebar = Color.FromArgb(27, 42, 74);
        public static readonly Color SidebarHover = Color.FromArgb(38, 57, 97);
        public static readonly Color SidebarActive = Color.FromArgb(15, 163, 177);
        public static readonly Color Accent = Color.FromArgb(15, 163, 177);
        public static readonly Color AccentDark = Color.FromArgb(10, 130, 142);
        public static readonly Color Background = Color.FromArgb(245, 247, 250);
        public static readonly Color CardBg = Color.White;
        public static readonly Color TextDark = Color.FromArgb(30, 40, 55);
        public static readonly Color TextMuted = Color.FromArgb(120, 130, 145);
        public static readonly Color BorderColor = Color.FromArgb(220, 225, 232);
        public static readonly Color DangerRed = Color.FromArgb(220, 53, 69);
        public static readonly Color SuccessGreen = Color.FromArgb(40, 167, 69);
        public static readonly Color WarningAmber = Color.FromArgb(255, 165, 0);

        public static readonly Font FontTitle = new Font("Segoe UI", 22f, FontStyle.Bold);
        public static readonly Font FontSubtitle = new Font("Segoe UI", 11f, FontStyle.Regular);
        public static readonly Font FontNav = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontNavBold = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontBody = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FontSmall = new Font("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font FontCardValue = new Font("Segoe UI", 26f, FontStyle.Bold);
        public static readonly Font FontCardLabel = new Font("Segoe UI", 9f, FontStyle.Regular);
    }
}
