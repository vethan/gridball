using System;
using System.Collections.Generic;
using System.Windows.Input;
using GridballCore.TurnCommands;
using SharpDX.XInput;

namespace GridBallRealtimeConsole
{
    class XInputOpponent  : IOpponent
    {
        public bool localIsPlayerOne => true;
        Controller controller;
        Dictionary<GamepadButtonFlags, TurnCommand> commandMap;
        Dictionary<GamepadButtonFlags, KeyState> keyMaps = new Dictionary<GamepadButtonFlags, KeyState>();

        private TurnCommand GenerateTurnCommand()
        {
            foreach (var kvp in commandMap)
            {
                if (keyMaps[kvp.Key].pressed)
                    return kvp.Value;
            }
            return new NullTurnCommand();
        }

        public TurnCommand HandleOpponentUpdate(byte frame, TurnCommand myTurnCommand)
        {
            controller.GetState().Gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
            TurnCommand result = current;
            current = new NullTurnCommand();
            return result;
        }

        TurnCommand current = new NullTurnCommand();
        public void HandleInput()
        {
            foreach (var kvp in keyMaps)
            {
                kvp.Value.UpdateFrame(controller.GetState().Gamepad.Buttons.HasFlag(kvp.Key));
            }

            if (current is NullTurnCommand)
            {
                current = GenerateTurnCommand();
            }
        }

        public void SetupOpponent()
        {
            controller = new Controller(UserIndex.One);
            Console.WriteLine("Waiting for controller 1 to be connected");
            while (!controller.IsConnected)
            {
                
            }
            keyMaps.Add(GamepadButtonFlags.DPadUp, new KeyState());
            keyMaps.Add(GamepadButtonFlags.DPadDown, new KeyState());
            keyMaps.Add(GamepadButtonFlags.DPadLeft, new KeyState());
            keyMaps.Add(GamepadButtonFlags.DPadRight, new KeyState());

            keyMaps.Add(GamepadButtonFlags.A, new KeyState());
            keyMaps.Add(GamepadButtonFlags.B, new KeyState());
            keyMaps.Add(GamepadButtonFlags.X, new KeyState());
            keyMaps.Add(GamepadButtonFlags.Y, new KeyState());

             commandMap = new Dictionary<GamepadButtonFlags, TurnCommand>
            {
                [GamepadButtonFlags.DPadUp] = new MoveTurnCommand(GridballCore.Point.Direction.Up),
                [GamepadButtonFlags.DPadLeft] = new MoveTurnCommand(GridballCore.Point.Direction.Left),
                [GamepadButtonFlags.DPadDown] = new MoveTurnCommand(GridballCore.Point.Direction.Down),
                [GamepadButtonFlags.DPadRight] = new MoveTurnCommand(GridballCore.Point.Direction.Right),

                [GamepadButtonFlags.A] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Down),
                [GamepadButtonFlags.B] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Right),
                [GamepadButtonFlags.X] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Left),
                [GamepadButtonFlags.Y] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Up)
            };
        }
    }
}
