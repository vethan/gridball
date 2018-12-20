using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore.TurnCommands
{
    public abstract class TurnCommand : IComparable<TurnCommand>
    {
        public int CompareTo(TurnCommand other)
        {
            return Priority.CompareTo(other.Priority);
        }

        virtual internal void Execute(Player p, Game g)
        {
            p.StorePastPosition();
        }

        protected abstract int Priority
        {
            get;
        }
    }
}
