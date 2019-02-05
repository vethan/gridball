using System;

namespace GridballCore.TurnCommands
{
    [Serializable]
    public class ThrowTurnCommand : TurnCommand
    {
        public Point.Direction direction { get; private set; }
        public int distance;

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
            if (g.b.Position.x < -Game.HALF_ARENA_WIDTH)
            {
                g.Score(false);
                return;
            }
            if (g.b.Position.x > Game.HALF_ARENA_WIDTH)
            {
                g.Score(true);
                return;
            }

            g.b.Position = g.b.Position.ClampTo(new Point(-Game.HALF_ARENA_WIDTH, -Game.HALF_ARENA_HEIGHT),
                new Point(Game.HALF_ARENA_WIDTH, Game.HALF_ARENA_HEIGHT));
            g.b.carriedBy = null;
        }

    }
}
