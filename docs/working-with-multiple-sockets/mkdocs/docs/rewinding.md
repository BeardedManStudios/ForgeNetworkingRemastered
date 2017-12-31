# Rewinding
Rewinding is a method in which you can go back in "time" and look at what the value of a variable was at/around a specific timestep. The `Rewind` data structure is a pretty simple structure that allows you to do rewinding however you would like. Since you are able to get the Timestep at any point in your code through the use of `NetworkManager.Instance.NetWorker.Time.Timestep`, you can easily store anything into the structure that you need. Below is an example showing you how to get started with rewinding in a practical way.

1. Create a C# file in your project
2. Name the file `RewindTest.cs`
3. Open the file and paste the code following these steps
4. Create a new scene
5. Place a cube in the scene
6. Attach the `RewindTeste.cs` script to the cube
7. Attach a `Rigidbody` component to the cube
8. Save the scene as **RewindTest**
9. Open the Multiplayer Menu sample scene
10. Open the build settings
11. Click the **Player Settings...** button
12. Make sure **Run in Background** is turned on
13. Add the Multiplayer Menu Scene as scene (index 0) to the build settings
14. Add the **RewindTest** scene as the second scene (index 1) to the build settings
15. Press play
16. Host a game
17. Wait a second or two
18. Press the spacebar
19. Check the **GET** message and what time it was at
20. Check the **SET** message with the closest time to that seen in the get message and you will notice they are a match

```csharp
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using UnityEngine;

public class RewindTest : MonoBehaviour
{
    private Rewind<Vector3> rewind;

    private void Start()
    {
        rewind = new Rewind<Vector3>(5000);
        StartCoroutine(Store());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var timestep = NetworkManager.Instance.Networker.Time.Timestep - 1000;
            Debug.Log("GET: " + timestep + " | " + rewind.Get(timestep));
            UnityEditor.EditorApplication.isPaused = true;
        }
    }

    private IEnumerator Store()
    {
        for (;;)
        {
            var timestep = NetworkManager.Instance.Networker.Time.Timestep;
            rewind.Register(transform.position, timestep);
            Debug.Log("SET: " + timestep + " | " + transform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
```

## Deeper Dive
You will notice in the above example we do `rewind = new Rewind<Vector3>(5000);`. The `5000` in this case is the number of milliseconds to track data. Anything older than 5 seconds from now will be removed for memory constraint purposes. You can put whatever time frame you want here, 5000 may be a bit much (because 5000ms is 5 seconds which is a very long time). The `<Vector3>` is the type of data that we are storing into this rewinding structure.

Next you will notice the `rewind.Register(transform.position, timestep);` call. This will save the value of `transform.position` to the rewind history at the provided `ulong` **timestep**. This adds to the pool of stored data for this specific verable for you to pull from. **Note** It is not good practice to `Register` different `Vector3` addresses, so if we are using `rewind` for `transform.position` we should only pass `transform.position` into this variable's `Register` method.

Lastly we come to the most important action we can do `rewind.Get(timestep)`. There are many different overloads for getting the values near and around a timestep however the simplest method to call is `Get`. So let's say you `Register` a value **A** at timestep **3** and a value **B** at timestep **20**. If you were to call `Get(10)` then you will get value **A** returned, however if you called `Get(19)` you would get the value **B**. This is because the value closest to the sent timestamp would be returned.

## Other Get Methods
`List<T> Get(ulong timestep, out T lower, out T upper)` - If we were following the above example and we passed `Get(10, out lower, out upper)` into this method, then `lower` would equal **3**, `upper` would equal **20** and the returned value will be **3**. This method is useful for if you want to calculate a median or where the value might possibly be between the two values.

`List<T> GetRange(ulong timestep, int count)` - This method will act exactly like the first `Get` method we learned about in the **Deeper Dive** section above; however, it also will return **n** values before the given timestep. So if we passed in **3** for the `count` then we would get a list of values back where the first element is the target value for that timestep and every element after that is the value stored at the last timestep registered.

`T GetRange(ulong timestepMin, ulong timestepMax)` - This method allows you to get all values between two timesteps (including the min and the max).