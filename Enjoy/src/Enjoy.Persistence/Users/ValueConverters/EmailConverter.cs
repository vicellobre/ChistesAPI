using Enjoy.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Users.ValueConverters;

public sealed class EmailConverter : ValueConverter<Email, string>
{
    public EmailConverter()
        : base(
            email => email.Value,
            value => Email.Create(value).Value)
    {
    }
}
