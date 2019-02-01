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
            if (g.b.carriedBy == p)
            {
                if (p.Position.x < -Game.HALF_ARENA_WIDTH)
                {
                    g.Score(false);
                    return;
                }
                if (p.Position.x > Game.HALF_ARENA_WIDTH)
                {
                    g.Score(true);
                    return;
                }


                p.Position = p.Position.ClampTo(new Point(-Game.HALF_ARENA_WIDTH, -Game.HALF_ARENA_HEIGHT),
    new Point(Game.HALF_ARENA_WIDTH, Game.HALF_ARENA_HEIGHT));
            }
        }


        public Point.Direction direction { get; private set; }

    }
}
