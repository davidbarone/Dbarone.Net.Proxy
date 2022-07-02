namespace Dbarone.Net.Proxy;
using System;
using System.Reflection;

/// <summary>
/// Provides proxy functionality using DispatchProxy. Can be used for AOP use cases.
/// Instances of ProxyGenerator are able to generate proxies using the `Decorate` method
/// with an optional `Interceptor` providing decorator or proxy behaviour.
/// https://www.c-sharpcorner.com/article/aspect-oriented-programming-in-c-sharp-using-dispatchproxy/
/// https://devblogs.microsoft.com/dotnet/migrating-realproxy-usage-to-dispatchproxy/
/// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy?view=net-6.0
/// </summary>
public class ProxyGenerator<T> : DispatchProxy
{
    public InterceptDelegate? Interceptor { get; set; }

    /// <summary>
    /// Expose the target object as a read-only property.
    /// </summary>
    public T? Target { get; private set; }

    public T Decorate(T target)
    {
        var proxy = Create<T, ProxyGenerator<T>>();
        (proxy as ProxyGenerator<T>)!.Target = target;
        (proxy as ProxyGenerator<T>)!.Interceptor = Interceptor;
        return proxy;
    }

    public T Decorate(T target, InterceptDelegate interceptor)
    {
        var proxy = Create<T, ProxyGenerator<T>>();
        (proxy as ProxyGenerator<T>)!.Target = target;
        (proxy as ProxyGenerator<T>)!.Interceptor = interceptor;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod != null)
        {
            try
            {
                InterceptorArgs beforeArgs = new InterceptorArgs()
                {
                    BoundaryType = BoundaryType.Before,
                    TargetMethod = targetMethod,
                    Args = args,
                    Result = null,
                    Exception = null,
                    Continue = true
                };

                // before
                if (Interceptor != null)
                {
                    Interceptor.Invoke(beforeArgs);
                }

                // invoke
                if (beforeArgs.Continue || Interceptor == null)
                {
                    var result = targetMethod.Invoke(Target, args);

                    InterceptorArgs afterArgs = new InterceptorArgs()
                    {
                        BoundaryType = BoundaryType.After,
                        TargetMethod = targetMethod,
                        Args = args,
                        Result = result,
                        Exception = null,
                        Continue = true
                    };

                    // after
                    if (Interceptor != null)
                    {
                        Interceptor.Invoke(afterArgs);
                    }
                    return afterArgs.Result;
                }

                // If get here, the has not called target method. Return default.
                return default;
            }
            catch (Exception ex)
            {
                InterceptorArgs exceptionArgs = new InterceptorArgs()
                {
                    BoundaryType = BoundaryType.After,
                    TargetMethod = targetMethod,
                    Args = args,
                    Result = null,
                    Exception = ex.InnerException ?? ex,
                    Continue = true
                };

                if (Interceptor != null)
                {
                    Interceptor.Invoke(exceptionArgs);
                }
                if (exceptionArgs.Continue)
                {
                    // If interceptor returns true, rethrow the error - otherwise, error will be swallowed.
                    throw ex.InnerException ?? ex;
                }
                return default;
            }
        }
        throw new ArgumentException(nameof(targetMethod));
    }
}