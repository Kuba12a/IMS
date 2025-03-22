namespace Common.Utils;

public static class Clock
{
    [ThreadStatic] private static DateTimeOffset? _customNow;

    public static DateTimeOffset Now => DateTimeOffset.UtcNow;

    public static void Set(DateTimeOffset customNow) => _customNow = customNow;

    public static void Reset() => _customNow = null;
}
