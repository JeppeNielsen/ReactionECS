using UnityEngine;
using System.Collections;
using Reaction;

public class DraggableSystem : Reaction.System<Touchable, Draggable>
{
    public override void EntityAdded(Entity entity)
    {
        entity.GetComponent<Touchable>().Down += Handle_Down;
        entity.GetComponent<Touchable>().Up += Handle_Up;
    }

    public override void EntityRemoved(Entity entity)
    {
        entity.GetComponent<Touchable>().Down -= Handle_Down;
        entity.GetComponent<Touchable>().Up -= Handle_Up;
    }

    void Handle_Down(Touchable.TouchInfo obj)
    {
        Debug.LogError("Down : " + obj.touchable.ToString() +" : " + obj.worldPosition + " normal : " + obj.hitInfo.normal);
    }

    void Handle_Up(Touchable.TouchInfo obj)
    {
        Debug.LogError("Up : " + obj.touchable.ToString() + " : " + obj.worldPosition + " normal : " + obj.hitInfo.normal);
    }
}
