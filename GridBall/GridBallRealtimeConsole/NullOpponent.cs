using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridballCore.TurnCommands;

namespace GridBallRealtimeConsole
{
    class NullOpponent : IOpponent
    {
        public bool localIsPlayerOne => true;

        public void HandleInput()
        {
            
        }

        public TurnCommand HandleOpponentUpdate(byte frame, TurnCommand myTurnCommand)
        {
            return new NullTurnCommand();
        }

        public void SetupOpponent()
        {
        }
    }
}
