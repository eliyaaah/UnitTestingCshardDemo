using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class EmployeeControllerTests
    {
        private Mock<IEmployeeStorage> _employeeStorageMock;
        private EmployeeController _employeeController;

        [SetUp]
        public void SetUp()
        {
            _employeeStorageMock = new Mock<IEmployeeStorage>();
            _employeeController = new EmployeeController(_employeeStorageMock.Object);
        }

        [Test]
        public void DeleteEmployee_WhenCalled_DeleteTheEmployeeFromDb()
        {
            _employeeController.DeleteEmployee(1);

            _employeeStorageMock.Verify(es => es.DeleteEmployee(1));
        }

        [Test]
        public void DeleteEmployee_WhenCalled_ReturnsRedirectResult()
        {
            var result = _employeeController.DeleteEmployee(1);

            Assert.That(result, Is.TypeOf<RedirectResult>());
        }
    }
}
