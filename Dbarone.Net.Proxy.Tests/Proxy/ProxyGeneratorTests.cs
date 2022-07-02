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
public class TestInterceptor<T>
{
    public int MethodsIntercepted = 0;
    public int ExceptionsIntercepted = 0;

    public bool CancelBefore { get; set; }
    public bool CancelAfter { get; set; }
    public bool CancelException { get; set; }
    public bool DuckInterceptor { get; set; }
    public void Interceptor(InterceptorArgs<T> interceptorArgs)
    {
        MethodsIntercepted++;
        if (interceptorArgs.Exception != null)
        {
            ExceptionsIntercepted++;
        }

        if (interceptorArgs.BoundaryType == BoundaryType.Before && CancelBefore)
        {
            interceptorArgs.Continue = false;
        }
        else if (interceptorArgs.BoundaryType == BoundaryType.After && CancelAfter)
        {
            interceptorArgs.Continue = false;
        }
        else if (interceptorArgs.BoundaryType == BoundaryType.After && interceptorArgs.Exception != null && CancelException)
        {
            interceptorArgs.Continue = false;
        } else if (DuckInterceptor && interceptorArgs.BoundaryType==BoundaryType.After) {
            interceptorArgs.Result = "quack";
        }
    }
}

public class ProxyGeneratorTests
{

    [Fact]
    public void ProxyGenerator_Method()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        var generator = new ProxyGenerator<IAnimal>();
        generator.Interceptor = interceptor.Interceptor;
        var proxy = generator.Decorate(dog);
        proxy.MakeSound();          // will call before + after method call.
        Assert.Equal(2, interceptor.MethodsIntercepted);
    }

    [Fact]
    public void ProxyGenerator_Property()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        var generator = new ProxyGenerator<IAnimal>();
        generator.Interceptor = interceptor.Interceptor;
        var proxy = generator.Decorate(dog);
        proxy.Name = "Rover";          // will call before + after method call.
        Assert.Equal(2, interceptor.MethodsIntercepted);
        Assert.Equal("Rover", proxy.Name);
    }

    [Fact]
    public void ProxyGenerator_NoInterceptor()
    {
        var dog = new Dog("Fido");
        var generator = new ProxyGenerator<IAnimal>();
        var proxy = generator.Decorate(dog);
        proxy.Name = "Rover";          // will call before + after method call.
        Assert.Equal("Rover", proxy.Name);
    }

    [Fact]
    public void ProxyGenerator_CancelBefore()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        interceptor.CancelBefore = true;
        var generator = new ProxyGenerator<IAnimal>();
        var proxy = generator.Decorate(dog, interceptor.Interceptor);
        proxy.Name = "Rover";          // will call Before, but then interceptor will cancel further invocations.
        Assert.Equal(1, interceptor.MethodsIntercepted);    // only Before interceptor will be executed.
        Assert.Equal(null, proxy.Name);   // set_Name is not called - Name defaults to default value for type.
    }

    [Fact]
    public void ProxyGenerator_Exception()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        var generator = new ProxyGenerator<IAnimal>();
        var proxy = generator.Decorate(dog, interceptor.Interceptor);

        // Exception in target invocation should still be thrown, but we should be able to intercept.
        var ex = Assert.Throws<Exception>(() =>
        {
            proxy.Fly();          // should throw exception
            Assert.Equal(2, interceptor.MethodsIntercepted);
            Assert.Equal(1, interceptor.ExceptionsIntercepted);
        });
        Assert.Equal("Dogs can't fly!", ex.Message);
    }

    [Fact]
    public void ProxyGenerator_CancelException()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        interceptor.CancelException = true;
        var generator = new ProxyGenerator<IAnimal>();
        var proxy = generator.Decorate(dog, interceptor.Interceptor);

        // Exception in target invocation should be swallowed.
        proxy.Fly();          // should throw exception
        Assert.Equal(2, interceptor.MethodsIntercepted);
        Assert.Equal(1, interceptor.ExceptionsIntercepted);
    }

   [Fact]
    public void ProxyGenerator_ChangeResult()
    {
        var dog = new Dog("Fido");
        var interceptor = new TestInterceptor<IAnimal>();
        interceptor.DuckInterceptor = true;
        var generator = new ProxyGenerator<IAnimal>();
        var proxy = generator.Decorate(dog, interceptor.Interceptor);
        var sound = proxy.MakeSound();
        Assert.Equal("quack", sound);
    }    
}