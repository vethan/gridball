using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore.TurnCommands
{
    public class MoveTurnCommand : TurnCommand
    {

        protected override int Priority => 2;
        public MoveTurnCommand(Point.Direction d)
        {
            direction = d; 
        }
        internal override void Execute(Player p, Game g)
        {
            base.Execute(p, g);
            p.Position += Point.FromDirection(direction);
            p.Position= p.Position.ClampTo(new Point(-Game.HALF_ARENA_WIDTH, -Game.HALF_ARENA_HEIGHT),
                new Point(Game.HALF_ARENA_WIDTH, Game.HALF_ARENA_HEIGHT));
        }



        Point.Direction direction;

    }
}
