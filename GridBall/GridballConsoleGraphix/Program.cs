using GridballCore;
using GridballCore.TurnCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballConsoleGraphix
{
    class Program
    {
        static void Main(string[] args)
        {
            GridballCore.Game game = new GridballCore.Game();

            while(true)
            {
                Console.Clear();
                Console.WriteLine(DrawGame(game));


                var keyPress = Console.ReadKey();
                switch (keyPress.Key) {
                    case ConsoleKey.W:
                        game.ProcessCommands(new MoveTurnCommand(Point.Direction.Up), new NullTurnCommand());
                        break;
                    case ConsoleKey.A:
                        game.ProcessCommands(new MoveTurnCommand(Point.Direction.Left), new NullTurnCommand());

                        break;
                    case ConsoleKey.S:
                        game.ProcessCommands(new MoveTurnCommand(Point.Direction.Down), new NullTurnCommand());

                        break;
                    case ConsoleKey.D:
                        game.ProcessCommands(new MoveTurnCommand(Point.Direction.Right), new NullTurnCommand());

                        break;
                }

                game.FinishTurn();
            }
        }

        static string DrawGame(Game game)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 2 * Game.HALF_ARENA_WIDTH + 3; i++)
                sb.Append("=");

            sb.AppendLine();
            for (int y = Game.HALF_ARENA_HEIGHT; y >= -Game.HALF_ARENA_HEIGHT; y--)
            {
                sb.Append("=");
                for (int x = -Game.HALF_ARENA_WIDTH; x <= Game.HALF_ARENA_WIDTH; x++)                    
                {
                    Point current = new Point(x, y);
                    if (game.playerA.Position.Equals(current))
                    {
                        sb.Append(game.b.carriedBy == game.playerA ? "A" : "a");
                    }
                    else if (game.playerB.Position.Equals(current))
                    {
                        sb.Append(game.b.carriedBy == game.playerB ? "B" : "b");
                    }
                    else if (game.b.Position.Equals(current))
                    {
                        sb.Append("o");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                sb.AppendLine("=");
            }
            for (int i = 0; i < 2 * Game.HALF_ARENA_WIDTH + 3; i++)
                sb.Append("=");

            sb.AppendLine();
            return sb.ToString();
        }
    }
}
