using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touchable : Reaction.Component<Touchable> {

    public struct TouchInfo {
        public int touchIndex;
        public Touchable touchable;
        public MeshComponent.RayCastInfo hitInfo;
        public Vector2 screenPosition;
        public Vector3 worldPosition;
        public CameraComponent camera;
    }

    public event System.Action<TouchInfo> Down;
    public event System.Action<TouchInfo> Up;
    public event System.Action<TouchInfo> Click;

    public bool ClickThrough;

    public void InvokeDown(TouchInfo info)
    {
        if (Down != null)
        {
            Down(info);
        }
    }

    public void InvokeUp(TouchInfo info)
    {
        if (Up != null)
        {
            Up(info);
        }
    }

    public void InvokeClick(TouchInfo info)
    {
        if (Click != null)
        {
            Click(info);
        }
    }

}
