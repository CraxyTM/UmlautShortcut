using System.Windows.Forms;

namespace UmlautShortcut
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            KeyHook.Start();
            Application.Run();
            KeyHook.Stop();
        }
    }
}