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



        internal override void Execute(Player p, Game g)
        {
            base.Execute(p, g);


            throw new NotImplementedException();
        }

    }
}
