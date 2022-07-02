# Dbarone.Net.Proxy


>## T:Dbarone.Net.Proxy.BoundaryType

 Defines the interception event. 

---
### F:Dbarone.Net.Proxy.BoundaryType.Before
 Interceptor call before the target method is invoked. 

---
### F:Dbarone.Net.Proxy.BoundaryType.After
 Interceptor call after the target method is invoked. 

---


>## T:Dbarone.Net.Proxy.InterceptDelegate`1

 Defines the method signature of an interceptor. 
|interceptionArgs: ||

---


>## T:Dbarone.Net.Proxy.InterceptorArgs`1

 Defines the arguments passed to an interceptor. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.BoundaryType
 The interceptor event type. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.TargetMethod
 The method on the target object being invoked. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.Target
 The target object being wrapped in a proxy. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.Args
 Arguments passed to the target method being invoked. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.Result
 The result returned by the target invocation. Can be overriden in the 'After' interception handler. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.Exception
 The exception object if the target invocation throws an exception. Can be inspected in the 'After' interception handler. 

---
### P:Dbarone.Net.Proxy.InterceptorArgs`1.Continue
 If set to false in the 'Before' interception handler, the target method will not be invoked. IF set to false when an Exception is handled in an invocation, the error will be swallowed. 

---


>## T:Dbarone.Net.Proxy.ProxyGenerator`1

 Provides proxy functionality using DispatchProxy. Can be used for AOP use cases. Instances of ProxyGenerator are able to generate proxies using the `Decorate` method with an optional `Interceptor` providing decorator or proxy behaviour. https://www.c-sharpcorner.com/article/aspect-oriented-programming-in-c-sharp-using-dispatchproxy/ https://devblogs.microsoft.com/dotnet/migrating-realproxy-usage-to-dispatchproxy/ https://docs.microsoft.com/en-us/dotnet/api/system.reflection.dispatchproxy?view=net-6.0 

---
### P:Dbarone.Net.Proxy.ProxyGenerator`1.Interceptor
 Sets or gets the proxy interceptor. The interceptor can intercept before / after method invocations on the target object. 

---
### P:Dbarone.Net.Proxy.ProxyGenerator`1.Target
 Expose the target object as a read-only property. 

---
### M:Dbarone.Net.Proxy.ProxyGenerator`1.Decorate(`0)
 Returns a proxy of the target object, which can be intercepted. 
|Name | Description |
|-----|------|
|target: |The target object.|

---
### M:Dbarone.Net.Proxy.ProxyGenerator`1.Decorate(`0,Dbarone.Net.Proxy.InterceptDelegate{`0})
 Returns a proxy of the target object, which can be intercepted. 
|Name | Description |
|-----|------|
|target: |The target object.|
|interceptor: |The interceptor to use.|

---
### M:Dbarone.Net.Proxy.ProxyGenerator`1.Invoke(System.Reflection.MethodInfo,System.Object[])
 Called whenever a target method is invoked. 
|Name | Description |
|-----|------|
|targetMethod: |The target method.|
|args: |The args used in the target invocation.|

Exception thrown: [T:System.ArgumentException](#T:System.ArgumentException): 

---
