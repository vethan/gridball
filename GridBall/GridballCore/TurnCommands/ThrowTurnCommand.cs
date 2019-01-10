using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore.TurnCommands
{
    public class ThrowTurnCommand : TurnCommand
    {
        Point.Direction direction;
        int distance;

        protected override int Priority => 1;
        public ThrowTurnCommand(int distance, Point.Direction direction)
        {
            this.direction = direction;
            this.distance = distance;
        }


        internal override void Execute(Player p, Game g)
        {
            base.Execute(p, g);

            if (!(g.b.carriedBy == p))
                return;

            
            g.b.Position += Point.FromDirection(direction) * distance;
            g.b.Position = g.b.Position.ClampTo(new Point(-Game.HALF_ARENA_WIDTH, -Game.HALF_ARENA_HEIGHT),
                new Point(Game.HALF_ARENA_WIDTH, Game.HALF_ARENA_HEIGHT));
            g.b.carriedBy = null;
        }

    }
}
