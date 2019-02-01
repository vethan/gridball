using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
namespace GridBallRealtimeConsole
{
    class InputHandler
    {
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
                kvp.Value.UpdateFrame(Keyboard.IsKeyDown(kvp.Key));
            }
        }
    }
}
