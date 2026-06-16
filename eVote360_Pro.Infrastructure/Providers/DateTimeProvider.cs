using eVote360_Pro.Domain.Interfaces.Providers;

namespace eVote360_Pro.Infrastructure.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
