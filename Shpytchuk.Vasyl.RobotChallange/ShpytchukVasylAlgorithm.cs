using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Robot.Common;

namespace Shpytchuk.Vasyl.RobotChallange
{
    public class ShpytchukVasylAlgorithm : IRobotAlgorithm
    {
        protected static int COLLECT_ENERGY_DISTANCE = 1;
        protected static int MAX_COUNT_ROBOTS_NEAR_STATION = 2;
        protected static int MAX_RADIUS = 9;
        protected static int CREATE_ROBOT_START_ENERGE = 100;


        public string Author => "Shpytchuk Vasyl";

        public int Round { get; set; }

        public int RobotCount { get; set; }
        public int Counter { get; set; }


        public ShpytchukVasylAlgorithm()
        {
            Round = 0;
            Counter = 0;
            RobotCount = 10;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            CountRound();

            var robot = robots[robotToMoveIndex];

            if (CanCollectEnergy(robot, map, robots))
            {
                if (CanCreateNewRobot(robot, map, robots))
                {
                    RobotCount++;
                    Counter++;
                    return new CreateNewRobotCommand()
                    {
                        NewRobotEnergy = CREATE_ROBOT_START_ENERGE,
                    };
                }

                return new CollectEnergyCommand();
            }

            var newPosition = GoToNearestStation(robot, map, robots);
            if (newPosition == null) return new CollectEnergyCommand();

            return new MoveCommand()
            {
                NewPosition = newPosition
            };
        }

        protected bool CanCollectEnergy(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var res = map.GetNearbyResources(currentRobot.Position, COLLECT_ENERGY_DISTANCE);
            if (Round <= 45)
                return res.Count > 0 && (res.Any(station => station.Energy > 0) || GetNearestRobots(robots, currentRobot.Position, COLLECT_ENERGY_DISTANCE)
                    .Select(rob => rob.OwnerName == Author).Count() < MAX_COUNT_ROBOTS_NEAR_STATION);
            return res.Count > 0;
        }

        protected bool CanCreateNewRobot(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            if (Round > 45 || RobotCount >= 100)
                return false;

            if (currentRobot.Energy < 100 + CREATE_ROBOT_START_ENERGE)
                return false;

            return true;
        }

        protected Position GoToNearestStation(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var stations = map.Stations
                .OrderBy(s => Helper.FindDistance(s.Position, currentRobot.Position))
                .ToList();


            foreach (var station in stations)
            {
                if ((GetNearestRobots(robots, station.Position, COLLECT_ENERGY_DISTANCE).Count >
                        MAX_COUNT_ROBOTS_NEAR_STATION && station.Energy < MAX_COUNT_ROBOTS_NEAR_STATION * 40) || station.Energy <= 0) continue;

                var to = map.FindFreeCell(station.Position, robots);
                var stepAndRadius = Helper.GetOptimalRadius(currentRobot.Position, to, currentRobot.Energy, MAX_RADIUS);

                return stepAndRadius.CountStep + Round >= 51 ? null : Helper.GetIntermediatePosition(currentRobot.Position, to, stepAndRadius.Radius);
            }

            return null;
        }


        protected IList<Robot.Common.Robot> GetNearestRobots(IList<Robot.Common.Robot> robots, Position position, int radius)
        {
            return robots
                .Where(robot => Helper.IsWithinRadius(robot.Position, position, radius))
                .ToList();
        }

        protected void CountRound()
        {
            Counter++;
            if (Counter <= RobotCount) return;
            Round++;
            Counter = 0;
        }

    }


    public class Helper
    {

        public static bool IsWithinRadius(Position center, Position point, int radius)
        {
            return Math.Abs(center.X - point.X) <= radius && Math.Abs(center.Y - point.Y) <= radius;
        }

        private static int Min2D(int x1, int x2)
        {
            return ((IEnumerable<int>)new int[3]
            {
                (int) Math.Pow((double) (x1 - x2), 2.0),
                (int) Math.Pow((double) (x1 - x2 + 100), 2.0),
                (int) Math.Pow((double) (x1 - x2 - 100), 2.0)
            }).Min();
        }

        public static double FindDistance(Position a, Position b)
        {
            return Math.Sqrt(Min2D(a.X, b.X) + Min2D(a.Y, b.Y));
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
                        CountStep = countStep,
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