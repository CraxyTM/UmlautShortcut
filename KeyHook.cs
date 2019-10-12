using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace UmlautShortcut
{
    public class KeyHook
    {
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;
        private const int WmSysKeyDown = 0x0104;
        private const int WmKeyUp = 0x0101;
        private const int WmSysKeyUp = 0x0105;

        private static IntPtr _hookId = IntPtr.Zero;

        private static bool _altPressed;
        private static bool _shiftPressed;
        private static bool _hotKeyPressed;

        public static void Start()
        {
            _hookId = SetHook(HookCallback);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr SetHook(KeyboardProc proc)
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                return SetWindowsHookEx(WhKeyboardLl, proc, GetModuleHandle(module?.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Key is pressed
            if (nCode >= 0 && (wParam == (IntPtr) WmKeydown || wParam == (IntPtr) WmSysKeyDown))
            {
                var key = (Keys) Marshal.ReadInt32(lParam);

                //Store and cancel key if activator Key is pressed
                if (key == Keys.RMenu)
                {
                    _altPressed = true;
                    return new IntPtr(-1);
                }

                //Store if shift is pressed
                if (key == Keys.LShiftKey || key == Keys.RShiftKey)
                {
                    _shiftPressed = true;
                }

                if (_altPressed)
                {
                    string sendKey = null;

                    //Check if a shortcut is pressed
                    switch (key)
                    {
                        case Keys.A:
                            sendKey = _shiftPressed ? "Ä" : "ä";
                            break;
                        case Keys.O:
                            sendKey = _shiftPressed ? "Ö" : "ö";
                            break;
                        case Keys.U:
                            sendKey = _shiftPressed ? "Ü" : "ü";
                            break;
                        case Keys.S:
                            sendKey = "ß";
                            break;
                    }

                    //Simulate the key press
                    if (sendKey != null)
                    {
                        SendKeys.SendWait("{" + sendKey + "}");
                        _hotKeyPressed = true;
                        return new IntPtr(-1);
                    }
                }
            }
            //Key is released
            else if (nCode >= 0 && (wParam == (IntPtr) WmKeyUp || wParam == (IntPtr) WmSysKeyUp))
            {
                //Reset alt and shift if they are released
                switch ((Keys) Marshal.ReadInt32(lParam))
                {
                    case Keys.RMenu:
                        if (!_hotKeyPressed)
                        {
                            SendKeys.SendWait("%");
                        }

                        _altPressed = false;
                        _hotKeyPressed = false;

                        return new IntPtr(-1);
                    case Keys.LShiftKey:
                    case Keys.RShiftKey:
                        _shiftPressed = false;
                        break;
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    }
}