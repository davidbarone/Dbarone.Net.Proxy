namespace Dbarone.Net.Proxy;
using System.Reflection;

/// <summary>
/// Defines the arguments passed to an interceptor.
/// </summary>
public class InterceptorArgs<T>
{
    /// <summary>
    /// The interceptor event type.
    /// </summary>
    public BoundaryType BoundaryType { get; set; }
    
    /// <summary>
    /// The method on the target object being invoked.
    /// </summary>
    public MethodInfo TargetMethod { get; set; } = default!;

    /// <summary>
    /// The target object being wrapped in a proxy.
    /// </summary>
    public T? Target { get; set; } = default!;

    /// <summary>
    /// Arguments passed to the target method being invoked.
    /// </summary>
    public object?[]? Args { get; set; }
    
    /// <summary>
    /// The result returned by the target invocation. Can be overriden in the 'After' interception handler.
    /// </summary>
    public object? Result { get; set; }
    
    /// <summary>
    /// The exception object if the target invocation throws an exception. Can be inspected in the 'After' interception handler.
    /// </summary>
    public Exception? Exception { get; set; } = default!;
    
    /// <summary>
    /// If set to false in the 'Before' interception handler, the target method will not be invoked. IF set to false when an Exception is handled in an invocation, the error will be swallowed.
    /// </summary>
    public bool Continue { get; set; } = true;
}