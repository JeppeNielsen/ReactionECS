using System.Collections;
using System.Collections.Generic;
using Reaction;
using UnityEngine;

public class SpriteSystem : Reaction.System<Sprite, MeshComponent, Sizable>
{
    private readonly HashSet<Entity> dirtySprites = new HashSet<Entity>();

    public override void EntityAdded(Entity entity)
    {
        var sizable = entity.GetComponent<Sizable>();
        sizable.SizeChanged += Handle_SizeChanged;
        Handle_SizeChanged(sizable);
    }

    public override void EntityRemoved(Entity entity)
    {
        entity.GetComponent<Sizable>().SizeChanged -= Handle_SizeChanged;
        dirtySprites.Remove(entity);
    }

    void Handle_SizeChanged(Sizable sizable)
    {
        dirtySprites.Add(sizable.Entity);
    }

    public override void Update(float dt)
    {
        foreach (var e in dirtySprites)
        {
            CreateSprite(e);
        }
        dirtySprites.Clear();
    }

    private void CreateSprite(Entity e) {

        MeshComponent meshComponent = e.GetComponent<MeshComponent>();
        Sizable sizable = e.GetComponent<Sizable>();

        Mesh mesh = meshComponent.Mesh;

        if (mesh == null) {
            mesh = new Mesh();
            meshComponent.Mesh = mesh;
        }

        mesh.vertices = new Vector3[] {
            new Vector3(0,0,0),
            new Vector3(0,sizable.Size.y,0),
            new Vector3(sizable.Size.x, sizable.Size.y, 0),
            new Vector3(sizable.Size.x, 0, 0)
        };

        mesh.triangles = new int[] {
            0,1,2,0,2,3
        };

        mesh.RecalculateBounds();
    }


}
