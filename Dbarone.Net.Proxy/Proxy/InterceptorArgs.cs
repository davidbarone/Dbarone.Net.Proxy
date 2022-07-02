namespace Dbarone.Net.Proxy;
using System.Reflection;

public class InterceptorArgs
{
    public BoundaryType BoundaryType { get; set; }
    public MethodInfo TargetMethod { get; set; } = default!;
    public object?[]? Args { get; set; }
    public object? Result { get; set; }
    public Exception? Exception { get; set; } = default!;
    public bool Continue { get; set; } = true;
}