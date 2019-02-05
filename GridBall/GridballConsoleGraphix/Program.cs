using GridballCore;
using GridballCore.TurnCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballConsoleGraphix
{
    public class Program
    {
        static void Main(string[] args)
        {
            GridballCore.Game game = new GridballCore.Game();
            bool gameSetup = false;
            bool singlePlayer = false;
            while(!gameSetup)
            {
                Console.WriteLine("1: Single-player  2: Multi-player");
                var keyPress = Console.ReadKey();
                switch (keyPress.Key)
                {
                    case ConsoleKey.D1:
                        gameSetup = true;
                        singlePlayer = true;
                        break;
                    case ConsoleKey.D2:
                        gameSetup = true;
                        break;
                }
                Console.Clear();
                if(!gameSetup)
                {
                    Console.WriteLine("Press 1 or 2");
                }
            }


            while (true)
            {
                GameToConsole(game);
                game.ProcessCommands(
                    GetPlayerMove("A",game.playerA,game),
                    singlePlayer ? GetAIMove() : GetPlayerMove("B", game.playerB, game)
                    );               
            }
        }

        static TurnCommand GetAIMove()
        {
            return new NullTurnCommand();
        }

        public static void GameToConsole(Game game)
        {
            Console.Clear();
            Console.WriteLine(DrawGame(game));
        }

        static Point.Direction SelectDirection(Game g)
        {
            while (true) {
                GameToConsole(g);
                Console.WriteLine("Select direction using WASD");
                var keyPress = Console.ReadKey();
                switch (keyPress.Key)
                {
                    case ConsoleKey.W:
                        return Point.Direction.Up;
                    case ConsoleKey.A:
                        return (Point.Direction.Left);
                    case ConsoleKey.S:
                        return (Point.Direction.Down);
                    case ConsoleKey.D:
                        return Point.Direction.Right;
                }
            }
        }

        static TurnCommand HandleThrowCommand(Game g)
        {
            GameToConsole(g);
            Console.WriteLine("Select throw distance (1 (default) or 2)");
            int throwDistance = 1;
            var keyPress = Console.ReadKey();
            switch (keyPress.Key)
            {
                case ConsoleKey.D2:
                    throwDistance = 2;
                    break;
            }

            return new ThrowTurnCommand(throwDistance,SelectDirection(g));

        }

        static TurnCommand GetPlayerMove(String playerId, Player player, Game game)
        {
            while (true)
            {
                GameToConsole(game);
                Console.WriteLine("Player {0}: Select your action", playerId);
                ConsoleKeyInfo keyPress;
                if (game.b.carriedBy == player)
                {
                    if (player.movedWithBall >= Game.ballCarrierMoves)
                    {

                        Console.WriteLine("1: Throw  2: Nothing");
                        keyPress = Console.ReadKey();
                        switch (keyPress.Key)
                        {
                            case ConsoleKey.D1:
                                return HandleThrowCommand(game);
                            case ConsoleKey.D2:
                                return new NullTurnCommand();
                        }
                    }
                    else
                    {
                        Console.WriteLine("1: Move 2: Throw  3: Nothing");
                        keyPress = Console.ReadKey();
                        switch (keyPress.Key)
                        {
                            case ConsoleKey.D1:
                                return new MoveTurnCommand(SelectDirection(game));
                            case ConsoleKey.D2:
                                return HandleThrowCommand(game);
                            case ConsoleKey.D3:
                                return new NullTurnCommand();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("1: Move  2: Nothing");
                    keyPress = Console.ReadKey();
                    switch (keyPress.Key)
                    {
                        case ConsoleKey.D1:
                            return new MoveTurnCommand(SelectDirection(game));
                        case ConsoleKey.D2:
                            return new NullTurnCommand();
                    }
                }
            }
        }

        public static string DrawGame(Game game)
        {
            
            StringBuilder sb = new StringBuilder();
            sb.Append(game.secondHalf ? "Second Half." : "First Half.");
            if(game.turnsLeft == 0)
            {
                sb.AppendFormat(" Last Turn\n  A: {0} - {1} :B", game.aScore, game.bScore);
            } else if(game.turnsLeft == 1)
            {
                sb.AppendFormat(" 1 Turn Left\n  A: {0} - {1} :B", game.aScore, game.bScore);
            } else
            {
                sb.AppendFormat(" {0} Turns Left\n  A: {1} - {2} :B",game.turnsLeft,game.aScore,game.bScore);
            }

            sb.Append("\n");
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
