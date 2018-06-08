# Running Unity specific code on the main thread

We have created a helper class for you to be able to offload any logic to the main thread from a separate thread. This helper class is called **MainThreadManager** and there are 2 main ways that you can use this class. The entry point for both methods of use is the static method **Run** ; see the code snippets below for practical uses. The examples below emulate a RPC method's contents.

### MainThreadManager.Run Function Pointer
```csharp
public override void MyCustomRPC(object[] args)
{
	// Register the private function within this class to be called on the main thread
	MainThreadManager.Run(OtherFunction);
}

private void OtherFunction()
{
	Debug.Log("Hello World!");
}
```

You can see that the above example requires another accessible method in order to pass it into the main thread manager's Run method.

### MainThreadManager.Run Lambda Expression
```csharp
public override void MyCustomRPC(object[] args)
{
	// Setup a temporary method call (lambda expression) to be executed on the main thread
	MainThreadManager.Run(() => { Debug.Log("Hello World!"); });
}
```

**The above is the preferred**  **method**

The lambda expression is a native C# feature that allows you to essentially create an inline function at runtime. Please see [this website](http://www.dotnetperls.com/lambda) or the [offi](https://msdn.microsoft.com/en-us/library/bb397687.aspx) [cial documentation](https://msdn.microsoft.com/en-us/library/bb397687.aspx) for more information on lambda expressions.

**What does the Main Thread Manager do?**

The Main Thread Manager is actually a pretty small and simple singleton class. When you send a method pointer or inline expression into the Run method it will be added to a queue. The Main Thread Manager is a Unity Game Object and will automatically create itself if one is not created already. Every **FixedUpdate** for this object, it will check to see if there are any pending methods in the queue, if so it will run them and remove them from the queue. By running these methods in the **FixedUpdate** they are automatically ran on the main thread.
