using System.Collections.Generic;
using System.Linq;

namespace TestNinja.Mocking
{
    public interface IBookingStorage
    {
        IQueryable<Booking> GetActiveBookings(int? excludedBookingId = null);
    }

    public class BookingStorage : IBookingStorage
    {
        public IQueryable<Booking> GetActiveBookings(int? excludedBookingId = null)
        {

            var unitOfWork = new UnitOfWork();
            var bookings =
                unitOfWork.Query<Booking>()
                    .Where(
                        b => b.Status != "Cancelled");
            if (excludedBookingId.HasValue)
                bookings = bookings.Where(b => b.Id != excludedBookingId.Value);

            return bookings;
        }

        public class UnitOfWork
        {
            public IQueryable<T> Query<T>()
            {
                return new List<T>().AsQueryable();
            }
        }
    }
}
