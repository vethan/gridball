using GridballCore.TurnCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GridballCore.Point;

namespace GridballCore.AI
{
    public class GridballAIAgent
    {
        Random r = new Random();
        public Game game { get; }
        Player player { get; }
        Direction offensiveDirection;
        public GridballAIAgent(Game g, Player p, Direction goalDirection)
        {
            game = g;
            player = p;
            offensiveDirection = goalDirection;
        }

        Player GetOpponent()
        {
            
            return game.playerA == player ? game.playerB : game.playerA;
        }

        TurnCommand HandleCarryingBallMove()
        {
            //Exploit:  Be directly above and move in and you'll be able to tackle the ball
            //If they're far in front, or out of alignment move forward
            if ((Math.Abs(player.Position.x - GetOpponent().Position.x) > 2 
                || Math.Abs(player.Position.y - GetOpponent().Position.y) >=1) &&
                Point.TurnDistance(player.Position + FromDirection(offensiveDirection),GetOpponent().Position) > 1) {
                return new MoveTurnCommand(offensiveDirection);
            }

            //Handle opponent is close by and in-line. Dodge up or down, depending on the position Y
            //if at top or bottom, dodge opposite;
            if(player.Position.y == Game.HALF_ARENA_HEIGHT)
            {
                return new MoveTurnCommand(Direction.Down);
            }
            if (player.Position.y == -Game.HALF_ARENA_HEIGHT)
            {
                return new MoveTurnCommand(Direction.Up);
            }


            float yDistance = player.Position.y + Game.HALF_ARENA_HEIGHT;
            yDistance /= 2 * Game.HALF_ARENA_HEIGHT;
            return r.NextDouble() > yDistance ? new MoveTurnCommand(Direction.Up) : new MoveTurnCommand(Direction.Down);
        }

        TurnCommand ApproachBallBlindly()
        {
            if (game.b.Position.y > player.Position.y)
            {
                return new MoveTurnCommand(Direction.Up);
            }

            if (game.b.Position.y < player.Position.y)
            {
                return new MoveTurnCommand(Direction.Down);
            }

            if (game.b.Position.x < player.Position.x)
            {
                return new MoveTurnCommand(Direction.Left);
            }

            if (game.b.Position.x > player.Position.x)
            {
                return new MoveTurnCommand(Direction.Right);
            }

            return new NullTurnCommand();
        }

        TurnCommand HandleNoBallMove()
        {
            if(game.b.carriedBy == GetOpponent())
            {
                //Approach opponent
            }
            return ApproachBallBlindly();
        }

        public TurnCommand DoMove()
        {
            if (game.b.carriedBy == player)
            {
                return HandleCarryingBallMove();
            }
            return HandleNoBallMove();
        }
    }
}
