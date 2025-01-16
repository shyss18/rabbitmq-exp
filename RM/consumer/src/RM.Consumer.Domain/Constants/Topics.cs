namespace RM.Consumer.Domain.Constants;

public static class Topics
{
    public static string[] Values => new[]
    {
        "kern.*",
        "*.critical"
    };
}