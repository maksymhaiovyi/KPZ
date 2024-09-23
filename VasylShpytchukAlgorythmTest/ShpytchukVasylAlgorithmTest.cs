using Xunit;
using Shpytchuk.Vasyl.RobotChallange;
using Robot.Common;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace VasylShpytchukRobotChallangeTest
{
    public class ShpytchukVasylAlgorithmTest
    {
        #region GoToNearestStation
        [Fact]
        public void GoToNearestStation_WhenStationAvailable_ShouldReturnNextPosition()
        {
            // Arrange
            var map = new Map();
            map.MaxPozition = new Position { X = 100, Y = 100 };
            var stationPosition = new Position { X = 10, Y = 10 };
            int radius = 3;
            var energyStation = new EnergyStation
            {
                Position = stationPosition,
                Energy = 100,
                RecoveryRate = 10
            };
            map.Stations.Add(energyStation);

            var robot = new Robot.Common.Robot { Position = new Position { X = 5, Y = 5 }, Energy = 100 };
            var algorithm = new ShpytchukVasylAlgorithm();

            // Act
            var resultPosition = algorithm.GoToNearestStation(robot, map, new List<Robot.Common.Robot> { robot });
            bool isWithinStation = Helper.IsPositionWithinRadius(stationPosition, resultPosition, radius);
            
            // Assert
            Assert.True(isWithinStation);
        }

        [Fact]
        public void GoToNearestStation_WhenStationOvercrowded_ShouldReturnNull()
        {
            // Arrange
            var algorithm = new ShpytchukVasylAlgorithm();
            var robot = new Robot.Common.Robot
            {
                Position = new Position(0, 0),
                Energy = 200
            };

            var map = new Map();
            var stationPosition = new Position(5, 5);
            var energyStation = new EnergyStation
            {
                Position = stationPosition,
                Energy = 100
            };
            map.Stations.Add(energyStation);

            var robots = new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot { Position = stationPosition },
                new Robot.Common.Robot { Position = new Position(6, 5) },
                new Robot.Common.Robot { Position = new Position(5, 6) },
                new Robot.Common.Robot { Position = new Position(4, 5) }
            };

            // Act
            var result = algorithm.GoToNearestStation(robot, map, robots);

            // Assert
            Assert.Null(result); 
        }

        [Fact]
        public void GoToNearestStation_WhenStationEnergyDepleted_ShouldReturnNull()
        {
            // Arrange
            var algorithm = new ShpytchukVasylAlgorithm();
            var robot = new Robot.Common.Robot
            {
                Position = new Position(0, 0),
                Energy = 200
            };

            var map = new Map();
            var energyStation = new EnergyStation
            {
                Position = new Position(5, 5),
                Energy = 0 
            };
            map.Stations.Add(energyStation);

            var robots = new List<Robot.Common.Robot> { robot };

            // Act
            var result = algorithm.GoToNearestStation(robot, map, robots);

            // Assert
            Assert.Null(result); 
        }

        [Fact]
        public void GoToNearestStation_WhenRobotCantReachStation_ShouldReturnNull()
        {
            // Arrange
            var algorithm = new ShpytchukVasylAlgorithm();
            algorithm.Round = 50; 

            var robot = new Robot.Common.Robot
            {
                Position = new Position(0, 0),
                Energy = 200
            };

            var map = new Map();
            map.MaxPozition = new Position { X = 100, Y = 100 };
            var energyStation = new EnergyStation
            {
                Position = new Position(50, 50),
                Energy = 100
            };
            map.Stations.Add(energyStation);

            var robots = new List<Robot.Common.Robot> { robot };

            // Act
            var result = algorithm.GoToNearestStation(robot, map, robots);

            // Assert
            Assert.Null(result); 
        }

        #endregion

        #region CanCollectEnergy
        [Fact]
        public void CanCollectEnergy_WhenResourcesAvailable_ShouldReturnTrue()
        {
            // Arrange
            var algorithm = new ShpytchukVasylAlgorithm();
            var robot = new Robot.Common.Robot
            {
                Position = new Position(0, 0),
                Energy = 100,
                OwnerName = algorithm.Author 
            };

            var map = new Map();
            var energyStation = new EnergyStation
            {
                Position = new Position(1, 1),
                Energy = 100,
                RecoveryRate = 10
            };
            map.Stations.Add(energyStation);

            var robots = new List<Robot.Common.Robot> { robot };

            // Act
            var result = algorithm.CanCollectEnergy(robot, map, robots);

            // Assert
            Assert.True(result); 
        }

        [Fact]
        public void CanCollectEnergy_WhenNoResources_ShouldReturnFalse()
        {
            // Arrange
            var algorithm = new ShpytchukVasylAlgorithm();
            var robot = new Robot.Common.Robot
            {
                Position = new Position(0, 0),
                Energy = 100,
                OwnerName = algorithm.Author 
            };

            var map = new Map(); 

            var robots = new List<Robot.Common.Robot> { robot };

            // Act
            var result = algorithm.CanCollectEnergy(robot, map, robots);

            // Assert
            Assert.False(result); 
        }
        #endregion

        #region DoStep

        [Fact]
        public void DoStep_WhenCannotCreateNewRobot_ReturnCollectEnergyCommand()
        {
            // Arrange
            var robot = new Robot.Common.Robot { Energy = 100 };
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map();

            // Act
            var algorithm = new ShpytchukVasylAlgorithm();
            var result = algorithm.DoStep(robots, 0, map);

            // Assert
            Assert.IsType<CollectEnergyCommand>(result);
        }

        [Fact]
        public void DoStep_WhenNoValidNewPosition_ReturnsCollectEnergyCommand()
        {
            // Arrange
            var robot = new Robot.Common.Robot { Energy = 50 };
            var robots = new List<Robot.Common.Robot> { robot };
            var map = new Map();

            var algorithm = new ShpytchukVasylAlgorithm();

            // Act
            var result = algorithm.DoStep(robots, 0, map);

            // Assert
            Assert.IsType<CollectEnergyCommand>(result);
        }
        #endregion
    }
}
