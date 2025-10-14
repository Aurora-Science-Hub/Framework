namespace AuroraScienceHub.Framework.Exceptions;

/// <summary>
/// Validation exception reason
/// </summary>
public sealed class ValidationExceptionReason : Dictionary<string, List<string>>
{
    public ValidationExceptionReason()
    {
    }

    public ValidationExceptionReason(string name, string reason)
    {
        AddReason(name, reason);
    }

    public ValidationExceptionReason AddReason(string name, string reason)
    {
        if (!TryGetValue(name, out var reasons))
        {
            reasons = new List<string>();
            Add(name, reasons);
        }

        reasons.Add(reason);

        return this;
    }
}
