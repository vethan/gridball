using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace GridBallRealtimeConsole
{
    public class InputHandler 
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        Dictionary<Key, KeyState> keyMaps = new Dictionary<Key, KeyState>();

        public InputHandler(params Key[] keys)
        {
            foreach(var key in keys)
            {
                keyMaps.Add(key, new KeyState());
            }
        }

        public KeyState GetKeyState(Key key)
        {
            return keyMaps[key];
        }

        public void HandleInput()
        {
            foreach(var kvp in keyMaps)
            {
                kvp.Value.UpdateFrame(ApplicationIsActivated() && Keyboard.IsKeyDown(kvp.Key));
            }
        }
        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }


    }
}
