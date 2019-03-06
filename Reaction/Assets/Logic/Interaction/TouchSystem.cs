using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchSystem : Reaction.System<MeshComponent, Touchable>
{
    private readonly InputSystem inputSystem;
    private readonly CameraSystem cameraSystem;

    private static readonly List<Touchable.TouchInfo> getTouches = new List<Touchable.TouchInfo>();
    private readonly List<Touchable.TouchInfo>[] touches = new List<Touchable.TouchInfo>[InputSystem.MAX_TOUCHES];

    private readonly List<Touchable.TouchInfo> downs = new List<Touchable.TouchInfo>();
    private readonly List<Touchable.TouchInfo> ups = new List<Touchable.TouchInfo>();
    private readonly List<Touchable.TouchInfo> clicks = new List<Touchable.TouchInfo>();

    public TouchSystem(InputSystem inputSystem, CameraSystem cameraSystem)
    {
        this.inputSystem = inputSystem;
        this.cameraSystem = cameraSystem;

        this.inputSystem.Down += Input_Down;
        this.inputSystem.Up += Input_Up;

        for (int i = 0; i < touches.Length; i++)
        {
            touches[i] = new List<Touchable.TouchInfo>();
        }
    }

    void Input_Down(int touchIndex, Vector2 position)
    {
        var list = touches[touchIndex];
        GetTouchesAtPosition(position, touchIndex, list);
        foreach (var item in list)
        {
            downs.Add(item);
        }
    }

    void Input_Up(int touchIndex, Vector2 position)
    {
        var list = touches[touchIndex];
        List<Touchable.TouchInfo> touching = new List<Touchable.TouchInfo>();
        GetTouchesAtPosition(position, touchIndex, touching);

        foreach (var item in list)
        {
            if (TouchListContainsTouchable(touching, item.touchable)) {
                clicks.Add(item);
            }
            ups.Add(item);
        }
    }

    private bool TouchListContainsTouchable(List<Touchable.TouchInfo> touchList, Touchable touchable) {
        foreach (var touch in touchList)
        {
            if (touch.touchable == touchable) return true;
        }
        return false;
    }

    private void GetTouchesAtPosition(Vector2 touchPosition, int touchIndex, List<Touchable.TouchInfo> outputList) {
        getTouches.Clear();
        foreach (var cameraEntity in cameraSystem.Entities)
        {
            FindTouchablesAtTouchLocation(touchPosition, cameraEntity.GetComponent<CameraComponent>(), getTouches);
        }

        getTouches.Sort((a, b) =>
        {
            return a.hitInfo.pickDistance.CompareTo(b.hitInfo.pickDistance);
        });

        outputList.Clear();
        foreach (var item in getTouches)
        {
            var copy = item;
            copy.touchIndex = touchIndex;
            outputList.Add(copy);
            if (!item.touchable.ClickThrough) break;
        }
    }

    private void FindTouchablesAtTouchLocation(Vector2 screenPosition, CameraComponent camera, List<Touchable.TouchInfo> touchInfos) {
        Ray ray = camera.Camera.ScreenPointToRay(screenPosition);

        foreach (var entity in Entities)
        {
            MeshComponent meshComponent = entity.GetComponent<MeshComponent>();
            if (meshComponent.Mesh == null) continue;
            Bounds bounds = meshComponent.Mesh.bounds;
            Transform transform = meshComponent.transform;
            Ray localRay = new Ray(transform.InverseTransformPoint(ray.origin), transform.InverseTransformVector(ray.direction));
            if (!bounds.IntersectRay(localRay)) continue;
            MeshComponent.RayCastInfo info;
            if (meshComponent.IntersectsRay(localRay, out info)) {
                touchInfos.Add(new Touchable.TouchInfo
                {
                    hitInfo = info,
                    touchable = entity.GetComponent<Touchable>(),
                    camera = camera,
                    screenPosition = screenPosition,
                    worldPosition = screenPosition
                });
            }
        }
    }


    public override void Update(float dt)
    {
        foreach (var down in downs)
        {
            down.touchable.InvokeDown(down);
        }
        downs.Clear();

        foreach (var click in clicks)
        {
            click.touchable.InvokeClick(click);
        }
        clicks.Clear();

        foreach (var up in ups)
        {
            up.touchable.InvokeUp(up);
        }
        ups.Clear();
    }

}
