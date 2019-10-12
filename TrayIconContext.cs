using System;
using System.Windows.Forms;
using UmlautShortcut.Properties;

namespace UmlautShortcut
{
    public class TrayIconContext : ApplicationContext
    {
        private readonly NotifyIcon _trayIcon;

        public TrayIconContext()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = Resources.AppIcon,
                ContextMenu = new ContextMenu(new[]
                {
                    new MenuItem("Exit", Exit)
                }),
                Visible = true,
                Text = "UmlautShortcut"
            };
        }

        private void Exit(object eventSender, EventArgs eventArgs)
        {
            _trayIcon.Visible = false;
            Application.Exit();
        }
    }
}