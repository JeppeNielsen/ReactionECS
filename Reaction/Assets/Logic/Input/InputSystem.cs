using UnityEngine;
using System.Collections;
using Reaction;
using System;
using System.Collections.Generic;

public class InputSystem : SystemBase
{
    public const int MAX_TOUCHES = 3;

    public delegate void TouchEvent(int touchIndex, Vector2 position);

    public event TouchEvent Down;

    public event TouchEvent Up;


    private struct TouchInfo {
        public int index;
        public Vector2 position;
        public bool isDown;
    }

    private readonly TouchInfo[] touches = new TouchInfo[MAX_TOUCHES];
    private readonly TouchInfo[] prevTouches = new TouchInfo[MAX_TOUCHES];

    public Vector2 GetTouchPosition(int touchIndex)
    {
        if (touchIndex < 0) return Vector2.zero;
        if (touchIndex >= touches.Length) return Vector2.zero;
        return touches[touchIndex].position;
    }

    protected override void AddComponentTypes(List<Type> componentTypes) { }

    public InputSystem()
    {
        Input.simulateMouseWithTouches = true;
    }

    public override void Update(float dt)
    {
        for (int i = 0; i < touches.Length; i++)
        {
            touches[i].isDown = i==0 ? Input.GetMouseButton(0) : false;
            touches[i].position = Input.mousePosition;
        }

        int numTouches = Input.touchCount;

        for (int i = 0; i < numTouches; i++)
        {
            var touch = Input.GetTouch(i);
            if (touch.fingerId < 0) continue;
            if (touch.fingerId >= touches.Length) continue;
            touches[touch.fingerId].isDown = true;
            touches[touch.fingerId].position = touch.position;
        }

        if (Down!=null) {
            for (int i = 0; i < touches.Length; i++)
            {
                if (touches[i].isDown && !prevTouches[i].isDown) {
                    Down(i, touches[i].position);
                }
            }
        }

        if (Up != null)
        {
            for (int i = 0; i < touches.Length; i++)
            {
                if (!touches[i].isDown && prevTouches[i].isDown)
                {
                    Up(i, touches[i].position);
                }
            }
        }

        for (int i = 0; i < touches.Length; i++)
        {
            prevTouches[i] = touches[i];
        }
    }

}
