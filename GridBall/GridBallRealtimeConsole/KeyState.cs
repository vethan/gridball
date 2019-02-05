using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridBallRealtimeConsole
{
    public class KeyState
    {
        public bool pressed { get { return !lastFrameState && isDown; } }
        public bool released { get { return lastFrameState && !isDown; } }

        private bool lastFrameState;
        public bool isDown { get; private set; }

        public void UpdateFrame(bool isDown)
        {
            lastFrameState = this.isDown;
            this.isDown = isDown;
        }
    }
}
