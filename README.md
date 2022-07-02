# Dbarone.Net.Proxy
A .NET Proxy / Decorator generator.

Although .NET Core does not including remoting functionality or the RealProxy class found in .NET Framework, it does include a DispatchProxy class in the `System.Reflection` namespace which can be used for basic aspect oriented programming (AOP) use cases like logging, caching, and security.

The main class is the `ProxyGenerator` class. This class can be used to generate proxy objects that wrap the original object, and provide Before, After, and Exception interception hooks denoted by the `BoundaryType` enum:

| Interception Hook | Invoked                             |
| ----------------- | ----------------------------------- |
| Before            | Before the target method is called. |
| After             | After the method is called.         |

## Interceptors
The target object is decorated using an interceptor method. The interceptor method must take the following parameters, and return a void:

| Parameter    | Type         | Description                                                |
| ------------ | ------------ | ---------------------------------------------------------- |
| boundaryType | BoundaryType | Contains the interception hook (Before, After, Exception). |
| targetMethod | MethodInfo   | The target method being invoked.                           |
| args         | object?[]?   | The parameters used for the target method invocation.      |
| exception    | Exception    | Any exception if thrown from the target method invocation. |

## Exceptions
If an exception is thrown in the target method invocation, the 'After' interception hook is guaranteed to be called, and the Exception object will be passed into the interception handler. The original method will continue to throw the error after the interception handler completes.

## Cancelling

