using Enjoy.Domain.Topics.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enjoy.Persistence.Topics.ValueConverters;

public sealed class TopicNameConverter : ValueConverter<TopicName, string>
{
    public TopicNameConverter()
        : base(
            topicName => topicName.Value,
            value => TopicName.Create(value).Value)
    {
    }
}
