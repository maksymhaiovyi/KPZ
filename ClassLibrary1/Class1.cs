using System;
using System.Collections.Generic;
using System.Linq;
using Robot.Common;

namespace ClassLibrary1
{
    public class Class1 : IRobotAlgorithm
    {
        protected static int COLLECT_ENERGY_DISTANCE = 1;
        protected static int MAX_COUNT_ROBOTS_NEAR_STATION = 5;
        protected static int MAX_RADIUS = 7;
        protected static int CREATE_ROBOT_START_ENERGE = 100;


        public string Author => "Shpytchuk Vasyl test";

        public int Round { get; set; }

        public int RobotCount { get; set; }

        public Class1()
        {
            Round = 0;
            RobotCount = 10;
            Logger.OnLogRound += Logger_OnLogRound;
        }

        private void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            this.Round = e.Number;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var robot = robots[robotToMoveIndex];

            if (CanCollectEnergy(robot, map))
            {
                if (CanCreateNewRobot(robot, map, robots))
                {
                    RobotCount++;
                    return new CreateNewRobotCommand()
                    {
                        NewRobotEnergy = CREATE_ROBOT_START_ENERGE,
                    };
                }

                return new CollectEnergyCommand();
            }


            return new MoveCommand()
            {
                NewPosition = GoToNearestStation(robot, map, robots)
            };
        }

        protected bool CanCollectEnergy(Robot.Common.Robot currentRobot, Map map)
        {
            var res = map.GetNearbyResources(currentRobot.Position, COLLECT_ENERGY_DISTANCE);
            if (Round <= 40)
                return res.Count > 0 && res.Any(station => station.Energy > 0);
            return res.Count > 0;
        }

        protected bool CanCreateNewRobot(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var station = map.GetNearbyResources(currentRobot.Position, COLLECT_ENERGY_DISTANCE)[0];

            if (Round > 40 || RobotCount >= 100)
                return false;

            if (currentRobot.Energy < 100 + CREATE_ROBOT_START_ENERGE)
                return false;


            /* if (GetNearestRobots(robots, station.Position, COLLECT_ENERGY_DISTANCE).Count > MAX_COUNT_ROBOTS_NEAR_STATION 
               //  && station.Energy < MAX_COUNT_ROBOTS_NEAR_STATION * 40
                 )
                 return false;*/

            return true;
        }

        protected Position GoToNearestStation(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            for (int i = MAX_RADIUS; i < 100; i++)
            {
                var stations = map.Stations
                    .OrderBy(s => Helper.FindDistance(s.Position, currentRobot.Position))
                    .ToList();

                if (stations.Count <= 0) continue;

                foreach (var station in stations)
                {
                    if ((GetNearestRobots(robots, station.Position, COLLECT_ENERGY_DISTANCE).Count >
                            MAX_COUNT_ROBOTS_NEAR_STATION && station.Energy < MAX_COUNT_ROBOTS_NEAR_STATION * 40) || station.Energy <= 0) continue;

                    var to = map.FindFreeCell(station.Position, robots);
                    var stepAndRadius = Helper.GetOptimalRadius(currentRobot.Position, to, currentRobot.Energy, MAX_RADIUS);

                    return stepAndRadius.CountStep + Round >= 51 ? currentRobot.Position : Helper.GetIntermediatePosition(currentRobot.Position, to, stepAndRadius.Radius);
                }
            }

            return currentRobot.Position;
        }


        protected IList<Robot.Common.Robot> GetNearestRobots(IList<Robot.Common.Robot> robots, Position position, int radius)
        {
            return robots
                .Where(robot => Helper.IsWithinRadius(robot.Position, position, radius))
                .ToList();
        }
    }

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
