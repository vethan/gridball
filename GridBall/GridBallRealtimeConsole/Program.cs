using GridballCore;
using GridballCore.TurnCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GridBallRealtimeConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //IOpponent ni = new NetworkOpponent();
            IOpponent ni = new XInputOpponent();
            ni.SetupOpponent();

            Console.Clear();
            InputHandler ih = new InputHandler(Key.W, Key.A, Key.S, Key.D, Key.J, Key.K, Key.L, Key.I, Key.D1, Key.D2);
            Stopwatch stopWatch = new Stopwatch();
            TurnCommand current = new NullTurnCommand();
            Game g = new Game();
            Dictionary<Key, TurnCommand> commandMap = new Dictionary<Key, TurnCommand>
            {
                [Key.W] = new MoveTurnCommand(GridballCore.Point.Direction.Up),
                [Key.A] = new MoveTurnCommand(GridballCore.Point.Direction.Left),
                [Key.S] = new MoveTurnCommand(GridballCore.Point.Direction.Down),
                [Key.D] = new MoveTurnCommand(GridballCore.Point.Direction.Right),

                [Key.I] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Up),
                [Key.J] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Left),
                [Key.K] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Down),
                [Key.L] = new ThrowTurnCommand(2, GridballCore.Point.Direction.Right)
            };

            double frameLength = 500;
            byte frameCounter = 0;
            double timePassed = 0;
            //Core Event Loop;
            while (true)
            {
                var deltaTime = stopWatch.Elapsed.TotalMilliseconds;
                stopWatch.Restart();
                ih.HandleInput();
                ni.HandleInput();
                if(current is NullTurnCommand)
                {
                    current = GenerateTurnCommand(ih, commandMap);
                }
                timePassed += deltaTime;
                if(timePassed >= frameLength)
                {
                    timePassed -= frameLength;
                    Console.WriteLine("Waiting for opponent data....");
                    TurnCommand opponentCommand = ni.HandleOpponentUpdate(frameCounter, current);
                    frameCounter++;
                    g.ProcessCommands(ni.localIsPlayerOne ? current : opponentCommand, ni.localIsPlayerOne ? opponentCommand : current);
                    //GridballConsoleGraphix.Program.GameToConsole(g);
                    current = new NullTurnCommand();
                    Console.Clear();
                }
                Draw(g, current, frameLength-timePassed);

                //Console.WriteLine(deltaTime);
                //HandleInput();
                Thread.Sleep(33);
            }
        }

        private static void Draw(Game g, TurnCommand command, double ms)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(GridballConsoleGraphix.Program.DrawGame(g));
            if(command is NullTurnCommand)
            {
                Console.WriteLine("You are not planning any move this turn                   ");
            }
            else if (command is MoveTurnCommand)
            {
                Console.WriteLine("You are planning to move " + ((MoveTurnCommand)command).direction + "                ");
            }
            if (command is ThrowTurnCommand)
            {
                Console.WriteLine("You are planning to throw " + ((ThrowTurnCommand)command).direction + "                 ");
            }

            Console.WriteLine("ms Left: " + (int)ms + "                       ");
        }

        private static TurnCommand GenerateTurnCommand(InputHandler ih, Dictionary<Key, TurnCommand> commandMap)
        {
            foreach(var kvp in commandMap)
            {
                if (ih.GetKeyState(kvp.Key).pressed)
                    return kvp.Value;
            }
            return new NullTurnCommand();
        }
    }
}
