using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    public class HousekeeperHelperTests
    {
        private Mock<IHousekeeperHelperRepository> _housekeeperHelperRepositoryMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private DateTime _statementDate = new DateTime(2021, 03, 25, 10, 0, 0);
        private Housekeeper _housekeeper = new Housekeeper
        {
            Email = "test@test.com",
            Oid = 1,
            FullName = "FullName",
            StatementEmailBody = "body"
        };
        private string _statementFilename;

        [SetUp]
        public void SetUp()
        {
            _housekeeperHelperRepositoryMock = new Mock<IHousekeeperHelperRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _statementFilename = "fileName";
            _housekeeperHelperRepositoryMock
              .Setup(r => r.SaveStatement(1, "FullName", _statementDate))
              .Returns(() => _statementFilename);

            _unitOfWorkMock.Setup(uow => uow.Query<Housekeeper>()).Returns(new List<Housekeeper>
            {
               _housekeeper
            }.AsQueryable());
        }

        [Test]
        public void SendStatementEmails_WhenCalled_GenerateStatements()
        {
            _housekeeper.Email = "test@test.com";

            HousekeeperHelper.SendStatementEmails(_statementDate,
                _unitOfWorkMock.Object, _housekeeperHelperRepositoryMock.Object);

            _housekeeperHelperRepositoryMock.Verify(hhr => hhr.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, _statementDate));
        }

        [Test]
        [TestCase(null)]
        [TestCase(" ")]
        public void SendStatementEmails_HousekeepersEmailIsNullOrWhitespace_ShouldNotGenerateStatement(string email)
        {
            _housekeeper.Email = email;

            HousekeeperHelper.SendStatementEmails(_statementDate,
                _unitOfWorkMock.Object, _housekeeperHelperRepositoryMock.Object);

            _housekeeperHelperRepositoryMock.Verify(hhr => hhr.SaveStatement(_housekeeper.Oid, _housekeeper.FullName, _statementDate), Times.Never);
        }

        [Test]
        public void SendStatementEmails_WhenCalled_EmailTheStatement()
        {
            _housekeeper.Email = "test@test.com";

            HousekeeperHelper.SendStatementEmails(_statementDate,
                _unitOfWorkMock.Object, _housekeeperHelperRepositoryMock.Object);

            _housekeeperHelperRepositoryMock.Verify(hhr => hhr.EmailFile(_housekeeper.Email, _housekeeper.StatementEmailBody,
                _statementFilename, It.IsAny<string>()));
        }

        [Test]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public void SendStatementEmails_StatementFilenameIsEmptyOrWhiteSpaceOrNull_EmailTheStatement(string filename)
        {
            _housekeeper.Email = "test@test.com";

            _statementFilename = filename;

            HousekeeperHelper.SendStatementEmails(_statementDate,
                _unitOfWorkMock.Object, _housekeeperHelperRepositoryMock.Object);

            _housekeeperHelperRepositoryMock.Verify(hhr => hhr.EmailFile(It.IsAny<string>(), It.IsAny<string>(),
                _statementFilename, It.IsAny<string>()), Times.Never);
        }
    }
}
