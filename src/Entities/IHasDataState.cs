namespace AuroraScienceHub.Framework.Entities;

/// <summary>
/// Domain entity with data state
/// </summary>
public interface IHasDataState<out TDataState>
    where TDataState : Enum
{
    /// <summary>
    /// Record's data state
    /// </summary>
    public TDataState DataState { get; }
}
