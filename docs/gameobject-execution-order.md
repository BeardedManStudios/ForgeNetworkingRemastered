# GameObject Execution Order
You may be use to the standard method execution order in Unity and this can cause some confusion when it comes to when you are able to access the network object on a client. You see, when you create a network object **on the client** it takes some time to get to the server, validate, and return back to the client. Because of this, the `networkObject` you are use to using may be null during the `Start` and `Awake` methods of your MonoBehaviour.

## NetworkStart
To resolve the issue with starting things once the networkObject has been setup, we have created a `protected` method you can `override` that will be called once the networkObject has been setup. You should do any initialization logic that is required for the network object in this method **don't forget to call the `base.NetworkStart()` method**.

```csharp
protected override void NetworkStart()
{
    base.NetworkStart();
    // TODO:  Your initialization code that relies on network setup for this object goes here
}
```

## Update and FixedUpdate
If the network is a little slow, it is very possible that the `networkObject` is not setup by the time the first call to the `Update` or `FixedUpdate` happens in Unity. So make sure to do your `null` checks or set some-kind of flag for when the object is ready to be used.

```csharp
private void Update()
{
    if (networkObject != null)
    {
        // TODO:  Whatever you need to do with the networkObject
    }
}
```