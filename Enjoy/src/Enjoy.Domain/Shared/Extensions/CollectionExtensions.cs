namespace Enjoy.Domain.Shared.Extensions;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this ICollection<T> collection) => collection.Count == 0;

    public static bool IsNull<T>(this ICollection<T> collection) => collection is null;

    public static bool IsNullOrEmpty<T>(this ICollection<T> collection) => collection.IsNull() || collection.IsEmpty();

    public static bool IsNullOrEmptyReadOnly<T>(this IReadOnlyCollection<T>? collection) =>
        collection is null || collection.Count <= 0;

    public static bool IsEmptyReadOnly<T>(this IReadOnlyCollection<T> collection) =>
        collection.Count <= 0;
}
