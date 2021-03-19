using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using TestNinja.Mocking;

namespace TestNinja.UnitTests.Mocking
{
    [TestFixture]
    class BookingHelperTests
    {
        private Mock<IBookingStorage> _bookingStorageMock;
        private Booking _existingBooking;

        [SetUp]
        public void SetUp()
        {
            _bookingStorageMock = new Mock<IBookingStorage>();
            _existingBooking = new Booking
            {
                Id = 2,
                ArrivalDate = ArriveOn(2021, 04, 09),
                DepartureDate = DepartOn(2021, 4, 14),
                Reference = "a"
            };
            _bookingStorageMock.Setup(bs => bs.GetActiveBookings(1)).Returns(new List<Booking>
            {
                _existingBooking
            }.AsQueryable());
        }

        [Test]
        public void OverlappingBookingsExist_NewBookingIsCancelled_ReturnsEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.DepartureDate),
                DepartureDate = Before(_existingBooking.DepartureDate, days: 3),
                Status = "Cancelled"
            }, _bookingStorageMock.Object);

            Assert.That(result, Is.EqualTo(string.Empty));
        }

        [Test]
        public void OverlappingBookingsExist_BookingStartsAndFinishesBeforeExistingBooking_ReturnsEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.ArrivalDate, days: 3),
                DepartureDate = Before(_existingBooking.ArrivalDate),
            }, _bookingStorageMock.Object);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void OverlappingBookingsExist_BookingStartsAndFinishesInTheMiddleOfExistingBooking_ReturnsExistingBookingReference()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.ArrivalDate),
                DepartureDate = Before(_existingBooking.DepartureDate),
            }, _bookingStorageMock.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void OverlappingBookingsExist_BookingStartsBeforeAndFinishesAfterExistingBooking_ReturnsExistingBookingReference()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = Before(_existingBooking.ArrivalDate),
                DepartureDate = After(_existingBooking.DepartureDate),
            }, _bookingStorageMock.Object);

            Assert.That(result, Is.EqualTo(_existingBooking.Reference));
        }

        [Test]
        public void OverlappingBookingsExist_BookingStartsAndFinishesAfterExistingBooking_ReturnsEmptyString()
        {
            var result = BookingHelper.OverlappingBookingsExist(new Booking
            {
                Id = 1,
                ArrivalDate = After(_existingBooking.DepartureDate),
                DepartureDate = After(_existingBooking.DepartureDate, days: 3),
            }, _bookingStorageMock.Object);

            Assert.That(result, Is.Empty);
        }

        private DateTime Before(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(-days);
        }

        private DateTime After(DateTime dateTime, int days = 1)
        {
            return dateTime.AddDays(days);
        }

        private DateTime ArriveOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 14, 0, 0);
        }

        private DateTime DepartOn(int year, int month, int day)
        {
            return new DateTime(year, month, day, 10, 0, 0);
        }
    }
}
