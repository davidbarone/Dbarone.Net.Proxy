namespace Dbarone.Net.Proxy;

/// <summary>
/// Defines the method signature of an interceptor.
/// </summary>
/// <param name="interceptionArgs"></param>
public delegate void InterceptDelegate<T>(InterceptorArgs<T> interceptionArgs);