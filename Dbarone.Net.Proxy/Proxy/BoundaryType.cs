namespace Dbarone.Net.Proxy;

/// <summary>
/// Defines the interception event.
/// </summary>
public enum BoundaryType
{
    /// <summary>
    /// Interceptor call before the target method is invoked.
    /// </summary>
    Before,

    /// <summary>
    /// Interceptor call after the target method is invoked.
    /// </summary>
    After
}
