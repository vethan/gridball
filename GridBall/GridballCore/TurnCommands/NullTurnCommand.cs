using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore.TurnCommands
{
    public class NullTurnCommand : TurnCommand
    {
        protected override int Priority => 3;
    }
}
