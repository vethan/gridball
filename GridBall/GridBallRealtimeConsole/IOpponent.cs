using GridballCore.TurnCommands;

namespace GridBallRealtimeConsole
{
    interface IOpponent 
    {
        bool localIsPlayerOne { get; }
   
        TurnCommand HandleOpponentUpdate(byte frame, TurnCommand myTurnCommand);
        void SetupOpponent();
        void HandleInput();
    }
}