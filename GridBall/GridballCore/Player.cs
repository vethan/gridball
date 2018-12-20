using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore
{
    public class Player : PositionableObject
    {
        public enum Team { A, B }

        Team team;

        bool hadBallThisCycle = false;
        internal Point pastPosition { get; private set; }

        public void StorePastPosition()
        {
            pastPosition = Position;
        }



        public bool HasMovedThisTurn
        {
            get {
                return !pastPosition.Equals(Position);
               }
        }

        public void NewCycle()
        {
            hadBallThisCycle = false;
        }
    }
}
