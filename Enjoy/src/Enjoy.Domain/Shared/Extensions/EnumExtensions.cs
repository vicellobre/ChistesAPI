using System.Globalization;

namespace Enjoy.Domain.Shared.Extensions;

public static class EnumExtensions
{
    public static int ToInt(this Enum enumValue)
    {
        return Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
    }
}
