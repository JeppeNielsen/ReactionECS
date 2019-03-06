using System.Collections;
using System.Collections.Generic;
using Reaction;
using UnityEngine;

public class WorldInitializer : MonoBehaviour
{
    void Start()
    {
        var input = new InputSystem();
        var cameras = new CameraSystem();

        World.Instance.AddSystem(new SpriteSystem());
        World.Instance.AddSystem(input);
        World.Instance.AddSystem(cameras);
        World.Instance.AddSystem(new TouchSystem(input, cameras));
        World.Instance.AddSystem(new DraggableSystem());

    }
}
