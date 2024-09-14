using Robot.Common;
using System;
using System.CodeDom;


namespace Shpytchuk.Vasyl.RobotChallange
{
    public class Helper
    {

        public static bool IsWithinRadius(Position center, Position point, int radius)
        {
            return Math.Abs(center.X - point.X) <= radius && Math.Abs(center.Y - point.Y) <= radius;
        }

        public static double FindDistance(Position a, Position b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static Position GetIntermediatePosition(Position from, Position to, int radius)
        {
            var distance = FindDistance(from, to);

            if (Math.Round(distance) <= radius)
            {
                return to;
            }

            var dx = to.X - from.X;
            var dy = to.Y - from.Y;
            var scale = radius / distance;

            return new Position
            {
                X = from.X + (int)(dx * scale),
                Y = from.Y + (int)(dy * scale)
            };
        }

        public static StepAndRadius GetOptimalRadius(Position from, Position to, int robotEnergy, int maxRadius)  
        {
            var distance = FindDistance(from, to);
            
            for (int i = maxRadius; i > 0; i--)
            {
                var countStep = (int)Math.Round(distance / i);
                if (Math.Pow(i, 2) * countStep < robotEnergy)
                    return new StepAndRadius()
                    {
                        CountStep =    countStep,
                        Radius = i
                    };
            }

            return new StepAndRadius()
            {
                CountStep = Int32.MaxValue,
                Radius = -1
            };
        }

    }

    public class StepAndRadius
    {
        public int CountStep { get; set; }
        public int Radius { get; set; }
    }


}
