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

        internal Point pastPosition { get; private set; }
        public int movedWithBall { get; private set; } = 0;
        public void StorePastPosition()
        {
            pastPosition = Position;
        }

        public void Reset()
        {
            movedWithBall = 0;
        }

        public bool HasMovedThisTurn
        {
            get {
                return !pastPosition.Equals(Position);
               }
        }

        public void NewCycle(Ball b)
        {
            if(HasMovedThisTurn && b.carriedBy == this)
            {
                movedWithBall++;
            }
            else
            {
                movedWithBall = 0;
            }
        }
    }
}
