# Destroying the Network Object

One of the most common actions that you may want to do on the network is to destroy the various network objects that you create. Any object that has a **networkObject** or that derives from one of the generated classes made by the Network Contract Wizard (NCW) can be destroyed on the network. If you have a reference to the object you wish to destroy then it is just a matter of calling one function:

```csharp
networkObject.Destroy();```

This will not only destroy the Network Object, but it will also call the UnityEngine.GameObject::Destroy method on the gameObject that the networkObject is attached to. If you wish to know when an object is destroyed you can register to the NetworkObject::onDestroy event.

```csharp
networkObject.onDestroy += MyMethod;```