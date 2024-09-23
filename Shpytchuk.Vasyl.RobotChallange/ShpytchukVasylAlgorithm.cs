using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Robot.Common;

namespace Shpytchuk.Vasyl.RobotChallange
{
    public class ShpytchukVasylAlgorithm : IRobotAlgorithm
    {
        protected static int COLLECT_ENERGY_DISTANCE = 2;
        protected static int MAX_COUNT_ROBOTS_NEAR_STATION = 3;
        protected static int MAX_RADIUS = 9;
        protected static int CREATE_ROBOT_START_ENERGY = 100;


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
                        NewRobotEnergy = CREATE_ROBOT_START_ENERGY,
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

        public bool CanCollectEnergy(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var res = map.GetNearbyResources(currentRobot.Position, COLLECT_ENERGY_DISTANCE);
            Console.WriteLine($"Energy Station Available: {res.Any(station => station.Energy > 0)}");
            if (Round <= 45)
                return res.Count > 0 && (res.Any(station => station.Energy > 0) || GetNearestRobots(robots, currentRobot.Position, COLLECT_ENERGY_DISTANCE)
                    .Select(rob => rob.OwnerName == Author).Count() < MAX_COUNT_ROBOTS_NEAR_STATION);
            return res.Count > 0;
        }

        protected bool CanCreateNewRobot(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            if (Round > 45 || RobotCount >= 100)
                return false;

            if (currentRobot.Energy < 100 + CREATE_ROBOT_START_ENERGY)
                return false;

            return true;
        }

        public Position GoToNearestStation(Robot.Common.Robot currentRobot, Map map, IList<Robot.Common.Robot> robots)
        {
            var stations = map.Stations
                .OrderBy(s => Helper.FindDistanceBetweenPositions(s.Position, currentRobot.Position))
                .ToList();


            foreach (var station in stations)
            {
                if ((GetNearestRobots(robots, station.Position, COLLECT_ENERGY_DISTANCE).Count >
                        MAX_COUNT_ROBOTS_NEAR_STATION && station.Energy < MAX_COUNT_ROBOTS_NEAR_STATION * 40) || station.Energy <= 0) continue;

                var to = map.FindFreeCell(station.Position, robots);
                var moveParams = Helper.GetOptimalMoveParams(currentRobot.Position, to, currentRobot.Energy, MAX_RADIUS);

                return moveParams.StepsNeeded + Round >= 51 ? null : Helper.GetNextPosition(currentRobot.Position, to, moveParams.RadiusOfStep);
            }

            return null;
        }


        protected IList<Robot.Common.Robot> GetNearestRobots(IList<Robot.Common.Robot> robots, Position position, int radius)
        {
            return robots
                .Where(robot => Helper.IsPositionWithinRadius(robot.Position, position, radius))
                .ToList();
        }

        protected void CountRound()
        {
            Counter++;
            if (Counter <= RobotCount) return;//if all robots have moved
            Round++;
            Counter = 0;
        }

    }


    public class Helper
    {

        public static bool IsPositionWithinRadius(Position center, Position point, int radius)
        {
            return Math.Abs(center.X - point.X) <= radius && Math.Abs(center.Y - point.Y) <= radius;
        }

        public static int FindDistanceBetweenPoints(int x1, int x2)
        {
            return ((IEnumerable<int>)new int[3]
            {
                (int) Math.Pow((double) (x1 - x2), 2.0),
                (int) Math.Pow((double) (x1 - x2 + 100), 2.0),//wraps around the map
                (int) Math.Pow((double) (x1 - x2 - 100), 2.0)
            }).Min();
        }

        public static double FindDistanceBetweenPositions(Position a, Position b)
        {
            return Math.Sqrt(FindDistanceBetweenPoints(a.X, b.X) + FindDistanceBetweenPoints(a.Y, b.Y));
        }

        public static int FindDirectionWithWrap(int x1, int x2)
        {
            int directDiff = x2 - x1;
            int wrapPositive = (x2 - x1 + 100);
            int wrapNegative = (x2 - x1 - 100);

            int[] options = { directDiff, wrapPositive, wrapNegative };

            return options.OrderBy(Math.Abs).First();
        }

        public static Position GetNextPosition(Position start, Position destination, int energyRadius)
        {
            var distance = FindDistanceBetweenPositions(start, destination);

            if (Math.Round(distance) <= energyRadius)
            {
                return destination;
            }

            var dx = FindDirectionWithWrap(start.X, destination.X);
            var dy = FindDirectionWithWrap(start.Y, destination.Y);
            var scale = energyRadius / distance;

            int newX = start.X + (int)(dx * scale);
            int newY = start.Y + (int)(dy * scale);
            newX = (newX + 100) % 100;
            newY = (newY + 100) % 100;

            return new Position
            {
                X = newX,
                Y = newY
            };
        }


        public static MoveParams GetOptimalMoveParams(Position start, Position destination, int robotEnergy, int maxRadius)
        {
            var distance = FindDistanceBetweenPositions(start, destination);

            for (int i = maxRadius; i > 0; i--)
            {
                var countStep = (int)Math.Round(distance / i);
                if (Math.Pow(i, 2) * countStep < robotEnergy)
                    return new MoveParams(countStep,i);
            }

            return new MoveParams(Int32.MaxValue,-1);
        }

    }

    public class MoveParams
    {
        public int StepsNeeded { get; set; }
        public int RadiusOfStep { get; set; }

        public MoveParams(int steps, int radius)
        {
            StepsNeeded = steps;
            RadiusOfStep = radius;
        }
    }
}