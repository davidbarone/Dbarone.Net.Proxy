# Dbarone.Net.Proxy
A .NET Proxy / Decorator generator using interceptors.

Although .NET Core does not include remoting functionality or the RealProxy class found in .NET Framework, it does include a DispatchProxy class in the `System.Reflection` namespace which can be used for basic aspect oriented programming (AOP) use cases like logging, caching, and security.

The main class in this library is the `ProxyGenerator` class. This class can be used to generate proxy objects that wrap the original object, and provide 'Before' and 'After' interception hooks denoted by the `BoundaryType` enum:

| Interception Hook | Invoked                             |
| ----------------- | ----------------------------------- |
| Before            | Before the target method is called. |
| After             | After the method is called.         |

## Interceptors
The target object is decorated using an interceptor method. The interceptor method must accept an `InterceptorArgs` parameter which has the following properties:

| Parameter    | Type         | Description                                                                                                                  |
| ------------ | ------------ | ---------------------------------------------------------------------------------------------------------------------------- |
| BoundaryType | BoundaryType | Contains the interception hook (Before, After, Exception).                                                                   |
| TargetMethod | MethodInfo   | The target method being invoked.                                                                                             |
| Target       | T            | The target object being wrapped.                                                                                             |
| Args         | object?[]?   | The parameters used for the target method invocation.                                                                        |
| Result       | object?      | The result of the target method invocation.                                                                                  |
| Continue     | bool         | Can be set to false to stop further execution of the method. Set to false when handling exceptions to swallow the exception. |
| Exception    | Exception    | Any exception if thrown from the target method invocation.                                                                   |

## Exceptions
If an exception is thrown in the target method invocation, the 'After' interception handler is guaranteed to be called, and the Exception object will be passed into the interception handler via the `Exception` property. If the `Continue` interception property is set to false, the exception will be swallowed. Otherwise, the original method will continue to throw the error after the interception handler completes.

## Cancelling
In the `Before` interception handler, setting the `Continue` property to false will stop the target method being invoked. In the `After` interception handler, setting the `Continue` property to false when an exception is thrown in the target method will result in the exception being swallowed.

## Sample Code
The following code snippet shows how the `ProxyGenerator` can be used:

``` c#
namespace Dbarone.Net.Proxy.Tests;
using Dbarone.Net.Proxy;
using System.Reflection;

public interface IAnimal
{
    public string MakeSound();
    public void Fly();
    public string Name { get; set; }
}

public class Dog : IAnimal
{
    public string MakeSound() { return "woof"; }
    public string Name { get; set; }

    public void Fly()
    {
        throw new Exception("Dogs can't fly!");
    }

    public Dog(string name)
    {
        Name = name;
    }
}

/// <summary>
/// Simple interceptor class.
/// </summary>
public class DuckInterceptor<T>
{
    public void Interceptor(InterceptorArgs<T> interceptorArgs)
    {
        // Change MakeSound behaviour on all animals
        if (interceptorArgs.BoundaryType==BoundaryType.After && interceptorArgs.TargetMethod.Name=="MakeSound" ) {
            interceptorArgs.Result = "quack";
        }
    }
}

public class Program
{
    public void Main()
    {
        var dog = new Dog("Fido");
        var interceptor = new DuckInterceptor<IAnimal>();
        var generator = new ProxyGenerator<IAnimal>();
        generator.Interceptor = interceptor.Interceptor;
        var proxy = generator.Decorate(dog);
        proxy.MakeSound();          // "quack"
    }
}
```

## Documentation
For full details of the library, please refer to the [documentation](https://github.com/davidbarone/Dbarone.Net.Proxy/blob/main/Documentation.md).