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
    /// <summary>
    /// Sets or gets the proxy interceptor. The interceptor can intercept before / after method invocations on the target object.
    /// </summary>
    public InterceptDelegate<T>? Interceptor { get; set; }

    /// <summary>
    /// Expose the target object as a read-only property.
    /// </summary>
    public T? Target { get; private set; }

    /// <summary>
    /// Returns a proxy of the target object, which can be intercepted.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <returns>A proxy of the target object, which can be intercepted.</returns>
    public T Decorate(T target)
    {
        var proxy = Create<T, ProxyGenerator<T>>();
        (proxy as ProxyGenerator<T>)!.Target = target;
        (proxy as ProxyGenerator<T>)!.Interceptor = Interceptor;
        return proxy;
    }

    /// <summary>
    /// Returns a proxy of the target object, which can be intercepted.
    /// </summary>
    /// <param name="target">The target object.</param>
    /// <param name="interceptor">The interceptor to use.</param>
    /// <returns>A proxy of the target object, which can be intercepted.</returns>
    public T Decorate(T target, InterceptDelegate<T> interceptor)
    {
        var proxy = Create<T, ProxyGenerator<T>>();
        (proxy as ProxyGenerator<T>)!.Target = target;
        (proxy as ProxyGenerator<T>)!.Interceptor = interceptor;
        return proxy;
    }

    /// <summary>
    /// Called whenever a target method is invoked.
    /// </summary>
    /// <param name="targetMethod">The target method.</param>
    /// <param name="args">The args used in the target invocation.</param>
    /// <returns>Returns the result of the invocation including any changed made by the interceptor.</returns>
    /// <exception cref="ArgumentException"></exception>
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod != null)
        {
            try
            {
                InterceptorArgs<T> beforeArgs = new InterceptorArgs<T>()
                {
                    BoundaryType = BoundaryType.Before,
                    TargetMethod = targetMethod,
                    Target = this.Target,
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

                    InterceptorArgs<T> afterArgs = new InterceptorArgs<T>()
                    {
                        BoundaryType = BoundaryType.After,
                        TargetMethod = targetMethod,
                        Target = this.Target,
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
                InterceptorArgs<T> exceptionArgs = new InterceptorArgs<T>()
                {
                    BoundaryType = BoundaryType.After,
                    TargetMethod = targetMethod,
                    Target = this.Target,
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