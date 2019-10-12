using System.Windows.Forms;

namespace UmlautShortcut
{
    public class Program
    {
        private static void Main(string[] args)
        {
            KeyHook.Start();
            Application.Run(new TrayIconContext());
            KeyHook.Stop();
        }
    }
}