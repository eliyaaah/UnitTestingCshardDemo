using NUnit.Framework;
using System;
using TestNinja.Fundamentals;

namespace TestNinja.UnitTests
{
    [TestFixture]
    class DemeritPointsCalculatorTests
    {
        private DemeritPointsCalculator _calc;

        [SetUp]
        public void SetUp()
        {
            _calc = new DemeritPointsCalculator();
        }

        [Test]
        [TestCase(350)]
        [TestCase(-1)]
        public void CalculateDemeritPoints_SpeedOutOfRange_ThrowsException(int speed)
        {
            Assert.That(() => _calc.CalculateDemeritPoints(speed), Throws.Exception.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCase(50, 0)]
        [TestCase(65, 0)]
        [TestCase(66, 0)]
        [TestCase(70, 1)]
        [TestCase(80, 3)]
        public void CalculateDemeritPoints_SpeedWithinRange_ReturnsNumberOfDemeritPoints(int speed, int expectedResult)
        {
            var result = _calc.CalculateDemeritPoints(speed);

            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
