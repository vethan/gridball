﻿using GridballCore.TurnCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridballCore
{
    public class Game
    {
        public const int HALF_ARENA_WIDTH = 8;
        public const int HALF_ARENA_HEIGHT = 8;
        int cycleLength = 3;
        int ballCarrierMoves = 2;
        public Player playerA;
        public Player playerB;
        public Ball b;
        public readonly static Random random = new Random();

        public Game()
        {
            playerA = new Player();
            playerA.Position = new Point(-2, 0);

            playerB = new Player();
            playerB.Position = new Point(2, 0);

            b = new Ball();
        }

        public void ProcessCommands(TurnCommand playerACommand, TurnCommand playerBCommand)
        {
            switch(playerACommand.CompareTo(playerACommand))
            {
                case -1:
                    playerBCommand.Execute(playerB, this);
                    playerACommand.Execute(playerA, this);
                    break;
                default:
                    playerACommand.Execute(playerA, this);
                    playerBCommand.Execute(playerB, this);
                    break;
            }

            if(b.carriedBy != null)
            {
                b.Position = b.carriedBy.Position;
            }
        }

        Point BumpBall()
        {
            Point aboveBall = b.Position + Point.FromDirection(Point.Direction.Up);
            Point belowBall = b.Position + Point.FromDirection(Point.Direction.Down);
            bool aboveBallTaken = playerA.Position.Equals(aboveBall) || playerB.Position.Equals(aboveBall) || aboveBall.x > HALF_ARENA_HEIGHT; ;
            bool belowBallTaken = playerA.Position.Equals(belowBall) || playerB.Position.Equals(belowBall) || belowBall.x < -HALF_ARENA_HEIGHT; ;

            if (aboveBallTaken && !belowBallTaken) return belowBall;            
            
            if (!aboveBallTaken && belowBallTaken) return aboveBall;            

            if (!(aboveBallTaken && belowBallTaken)) return random.NextDouble() > 0.5 ? belowBall : aboveBall;

            //At this point, we have to choose a horizontal

            Point leftOfBall = b.Position + Point.FromDirection(Point.Direction.Left);
            Point rightOfBall = b.Position + Point.FromDirection(Point.Direction.Right);
            bool leftTaken = playerA.Position.Equals(leftOfBall) || playerB.Position.Equals(leftOfBall) || leftOfBall.x < -HALF_ARENA_WIDTH;
            bool rightTaken = playerA.Position.Equals(rightOfBall) || playerB.Position.Equals(rightOfBall) || rightOfBall.x  > HALF_ARENA_WIDTH;

            if (leftTaken && !rightTaken) return rightOfBall;

            if (!leftTaken && rightTaken) return leftOfBall;

            if (!(leftTaken && rightTaken)) return random.NextDouble() > 0.5 ? rightOfBall : leftOfBall;

            //If everywhere is blocked, move diagonally towards the center
            return b.Position - new Point(Math.Sign(b.Position.x), Math.Sign(b.Position.y));
        }

        Point FindBargePosition(Player barger, Player bargee)
        {
            if (!barger.HasMovedThisTurn)
                return bargee.pastPosition;

            Point bargePosition = bargee.Position + barger.Position - barger.pastPosition;

            if(Math.Abs(bargePosition.x) > HALF_ARENA_WIDTH)
            {
                bargePosition.x = bargee.Position.x;
                bargePosition.y = bargePosition.y ==0 ? 
                    (random.NextDouble() > 0.5 ? 1 : -1) :
                    Math.Sign(bargePosition.y) * (Math.Abs(bargePosition.y) - 1);                
            }

            if (Math.Abs(bargePosition.y) > HALF_ARENA_HEIGHT)
            {
                bargePosition.y = bargee.Position.y;
                bargePosition.x = bargePosition.x == 0 ?
                    (random.NextDouble() > 0.5 ? 1 : -1) :
                    Math.Sign(bargePosition.x) * (Math.Abs(bargePosition.x) - 1);
            }

            return bargePosition;
        }

        public void HandleTackle()
        {
            Player tacklee = b.carriedBy;
            Player tackler = tacklee == playerA ? playerB : playerA;

            b.carriedBy = tackler;

            tacklee.Position = FindBargePosition(tackler, tacklee);
        }

        public void FinishTurn()
        {
            if(playerA.Position.Equals(playerB.Position))
            {
                //If they have the ball and it's a tackle
                if(b.carriedBy != null)
                {
                    HandleTackle();
                }
                else if(!playerA.HasMovedThisTurn)
                {
                        //Barge playerA
                        playerA.Position = FindBargePosition(playerB, playerA);
                }
                else if(!playerB.HasMovedThisTurn)
                {
                        //BargePlayerB
                        playerB.Position = FindBargePosition(playerA, playerB);
                    }
                else
                {
                    bool bumpBall = b.Position.Equals(playerA.Position);
        
                    //Bounce Back and bump ball if it was there
                    playerB.Position = playerB.pastPosition;
                    playerA.Position = playerA.pastPosition;

                    if(bumpBall)
                    {
                        b.Position = BumpBall();
                    }
                }
            }

            if(b.carriedBy == null)
            {
                if(playerA.Position.Equals(b.Position))
                {
                    b.carriedBy = playerA;
                }

                if (playerB.Position.Equals(b.Position))
                {
                    b.carriedBy = playerB;
                }
            }
        }

    }
}
