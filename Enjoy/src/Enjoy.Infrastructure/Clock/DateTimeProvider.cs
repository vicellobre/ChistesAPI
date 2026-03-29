using Enjoy.Application.Abstractions.Clock;

namespace Enjoy.Infrastructure.Clock;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
