using Xunit;
using Shpytchuk.Vasyl.RobotChallange;
using Robot.Common;
using static Shpytchuk.Vasyl.RobotChallange.Helper;

namespace VasylShpytchukAlgorithmTest
{
    public class HelperTests
    {
        #region IsPositionWithinRadius

        [Fact]
        public void IsPositionWithinRadius_WhenWithinRadius_ReturnsTrue()
        {
            // Arrange
            var center = new Position(0, 0);
            var point = new Position(1, 1);
            int radius = 2;

            // Act
            bool calculatedResult = IsPositionWithinRadius(center, point, radius);

            // Assert
            Assert.True(calculatedResult);
        }

        [Fact]
        public void IsPositionWithinRadius_WhenOutsideRadius_ReturnsFalse()
        {
            // Arrange
            var center = new Position(0, 0);
            var point = new Position(3, 3);
            int radius = 2;

            // Act
            bool calculatedResult = IsPositionWithinRadius(center, point, radius);

            // Assert
            Assert.False(calculatedResult);
        }

        [Fact]
        public void IsWithinRadius_WhenOnTheEdgeOfRadius_ReturnsTrue()
        {
            // Arrange
            var center = new Position(0, 0);
            var point = new Position(2, 0);
            int radius = 2;

            // Act
            bool calculatedResult = IsPositionWithinRadius(center, point, radius);

            // Assert
            Assert.True(calculatedResult);
        }
        #endregion

        #region FindDistanceBetweenPoints

        [Fact]
        public void FindDistanceBetweenPoints_WhenNumbersAreEqual_ReturnsZero()
        {
            // Arrange
            int x1 = 50;
            int x2 = 50;

            // Act
            int calculatedDistance = FindDistanceBetweenPoints(x1, x2);

            // Assert
            Assert.Equal(0, calculatedDistance);
        }

        [Fact]
        public void FindDistanceBetweenPoints_WhenNumbersAreDifferent_ReturnsCorrectValue()
        {
            // Arrange
            int x1 = 30;
            int x2 = 50;
            int expectedDistance = 400;
            // Act
            int calculatedDistance = FindDistanceBetweenPoints(x1, x2);

            // Assert
            Assert.Equal(expectedDistance, calculatedDistance);
        }

        [Fact]
        public void FindDistanceBetweenPoints_WhenNumbersAreOnDifferentEnds_ReturnsZero()
        {
            // Arrange
            int x1 = 0;
            int x2 = 100;

            // Act
            int calculatedDistance = FindDistanceBetweenPoints(x1, x2);

            // Assert
            Assert.Equal(0, calculatedDistance);
        }
        #endregion

        #region FindDistanceBetweenPositions
        [Fact]
        public void FindDistance_WhenPositionsAreEqual_ReturnsZero()
        {
            // Arrange
            Position a = new Position(50, 50);
            Position b = new Position(50, 50);

            // Act
            double calculatedDistance = FindDistanceBetweenPositions(a, b);

            // Assert
            Assert.Equal(0, calculatedDistance);
        }

        
        [Fact]
        public void FindDistance_WhenPositionsDifferInX_CalculatesCorrectDistance()
        {
            // Arrange
            Position a = new Position(0, 50);
            Position b = new Position(50, 50);
            double expectedDistance = 50;

            // Act
            double calculatedDistance = FindDistanceBetweenPositions(a, b);

            // Assert
            Assert.Equal(expectedDistance, calculatedDistance); 
        }

        [Fact]
        public void FindDistance_WhenPositionsDifferInY_CalculatesCorrectDistance()
        {
            // Arrange
            Position a = new Position(50, 0);
            Position b = new Position(50, 75);
            double expectedDistance = 25;//wraps up

            // Act
            double calculatedDistance = FindDistanceBetweenPositions(a, b);

            // Assert
            Assert.Equal(expectedDistance, calculatedDistance); 
        }

        [Fact]
        public void FindDistance_WhenPositionsAreDifferent_CalculatesCorrectDistance()
        {
            // Arrange
            Position a = new Position(10, 10);
            Position b = new Position(13, 14);
            double expectedDistance = 5;
            // Act
            double result = FindDistanceBetweenPositions(a, b);

            // Assert
         
            Assert.Equal(expectedDistance, result);
        }

        #endregion

        #region FindDirectionWithWrap
        [Fact]
        public void FindDirectionWithWrap_WhenDirectMovement_ReturnsPositive()
        {
            // Arrange
            int x1 = 10;
            int x2 = 20;
            int expectedDirection = 10;

            // Act
            var calculatedDirection = FindDirectionWithWrap(x1, x2);

            // Assert
            Assert.Equal(expectedDirection, calculatedDirection);
        }

        [Fact]
        public void FindDirectionWithWrap_WhenBackwardMovement_ReturnsNegative()
        {
            // Arrange
            int x1 = 10;
            int x2 = 5;
            int expectedDirection = -5;

            // Act
            var calculatedDirection = FindDirectionWithWrap(x1, x2);

            // Assert
            Assert.Equal(expectedDirection, calculatedDirection);
        }

        [Fact]
        public void FindDirectionWithWrap_WhenPositiveWrap_ShouldReturnPositive()
        {
            // Arrange
            int x1 = 95;
            int x2 = 5;
            int expectedDirection = 10;

            // Act
            var calculatedDirection = FindDirectionWithWrap(x1, x2);

            // Assert
            Assert.Equal(expectedDirection, calculatedDirection);
        }

        [Fact]
        public void FindDirectionWithWrap_WhenNegativeWrap_ShouldReturnNegative()
        {
            // Arrange
            int x1 = 5;
            int x2 = 95;
            int expectedDirection = -10;

            // Act
            var calculatedDirection = FindDirectionWithWrap(x1, x2);

            // Assert
            Assert.Equal(expectedDirection, calculatedDirection);
        }
        #endregion

        #region GetNextPosition
        [Fact]
        public void GetNextPosition_WhenDestinationWithinEnergyRadius_ReturnsDestination()
        {
            // Arrange
            var start = new Position { X = 10, Y = 10 };
            var destination = new Position { X = 15, Y = 15 };
            int energyRadius = 10;  
            var expectedPosition = destination;

            // Act
            var calculatedPosition = GetNextPosition(start, destination, energyRadius);

            // Assert
            Assert.Equal(expectedPosition.X, calculatedPosition.X);
            Assert.Equal(expectedPosition.Y, calculatedPosition.Y);
        }

        [Fact]
        public void GetNextPosition_WhenMovingDirectlyWithinMap_ReturnsNextPosition()
        {
            // Arrange
            var start = new Position { X = 10, Y = 10 };
            var destination = new Position { X = 20, Y = 10 };
            int energyRadius = 5; // Not enough to reach the destination
            var expectedPosition = new Position { X = 15, Y = 10 }; // Partial move

            // Act
            var calculatedPosition = GetNextPosition(start, destination, energyRadius);

            // Assert
            Assert.Equal(expectedPosition.X, calculatedPosition.X);
            Assert.Equal(expectedPosition.Y, calculatedPosition.Y);
        }

        [Fact]
        public void GetNextPosition_WhenPositiveWrapAround_ShouldWrapCorrectly()
        {
            // Arrange
            var start = new Position { X = 95, Y = 10 };
            var destination = new Position { X = 5, Y = 10 };
            int energyRadius = 5; 
            var expectedPosition = new Position { X = 0, Y = 10 }; 

            // Act
            var calculatedPosition = GetNextPosition(start, destination, energyRadius);

            // Assert
            Assert.Equal(expectedPosition.X, calculatedPosition.X);
            Assert.Equal(expectedPosition.Y, calculatedPosition.Y);
        }

        [Fact]
        public void GetNextPosition_WhenNegativeWrapAround_ShouldWrapCorrectly()
        {
            // Arrange
            var start = new Position { X = 5, Y = 10 };
            var destination = new Position { X = 95, Y = 10 };
            int energyRadius = 8; 
            var expectedPosition = new Position { X = 97, Y = 10 }; 

            // Act
            var calculatedPosition = GetNextPosition(start, destination, energyRadius);

            // Assert
            Assert.Equal(expectedPosition.X, calculatedPosition.X);
            Assert.Equal(expectedPosition.Y, calculatedPosition.Y);
        }

        [Fact]
        public void GetNextPosition_MoveDiagonally_ReturnsNextPosition()
        {
            // Arrange
            var start = new Position { X = 10, Y = 10 };
            var destination = new Position { X = 20, Y = 20 };
            int energyRadius = 5; 
            var expectedPosition = new Position { X = 13, Y = 13 }; 

            // Act
            var calculatedPosition = GetNextPosition(start, destination, energyRadius);

            // Assert
            Assert.Equal(expectedPosition.X, calculatedPosition.X);
            Assert.Equal(expectedPosition.Y, calculatedPosition.Y);
        }

        #endregion

        #region GetOptimalMoveParams
        [Fact]
        public void GetOptimalMoveParams_WhenEnergyIsSufficient_ReturnsOptimalParams()
        {
            // Arrange
            var start = new Position { X = 0, Y = 0 };
            var destination = new Position { X = 50, Y = 0 };
            int robotEnergy = 100;
            int maxRadius = 10;

            // Act
            MoveParams result = GetOptimalMoveParams(start, destination, robotEnergy, maxRadius);

            //Assert
            Assert.True(result.RadiusOfStep > 0);
        }

        [Fact]
        public void GetOptimalMoveParams_WhenNotEnoughEnergy_ReturnsNegativeRadius()
        {
            // Arrange
            var start = new Position { X = 0, Y = 0 };
            var destination = new Position { X = 100, Y = 0 };
            int robotEnergy = 0;  
            int maxRadius = 10;

            // Act
            MoveParams result = GetOptimalMoveParams(start, destination, robotEnergy, maxRadius);

            // Assert
            Assert.True(result.RadiusOfStep < 0);
        }

        [Fact]
        public void GetOptimalMoveParams_WithMaxRadiusZero_ReturnsNegativeStepsNeeded()
        {
            // Arrange
            var start = new Position { X = 0, Y = 0 };
            var destination = new Position { X = 10, Y = 0 };
            int robotEnergy = 10;
            int maxRadius = 0; 

            // Act
            MoveParams result = GetOptimalMoveParams(start, destination, robotEnergy, maxRadius);

            // Assert
            Assert.True(result.RadiusOfStep < 0); 
        }

        [Fact]
        public void GetOptimalMoveParams_WhenDistanceIsLessThanRadius_ReturnsOptimalParams()
        {
            // Arrange
            var start = new Position { X = 0, Y = 0 };
            var destination = new Position { X = 5, Y = 5 };
            int robotEnergy = 100;
            int maxRadius = 5;
            int expectedRadius = maxRadius;
            int expectedStepsNeeded = 1;
            // Act
            var result = GetOptimalMoveParams(start, destination, robotEnergy, maxRadius);

            // Assert
            Assert.Equal(expectedStepsNeeded, result.StepsNeeded);  
            Assert.Equal(expectedRadius, result.RadiusOfStep); 
        }
        #endregion
    }
}