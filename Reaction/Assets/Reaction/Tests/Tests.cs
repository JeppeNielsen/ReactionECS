using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Reaction;

namespace Reaction
{
    public class Tests
    {
        public class Position : Reaction.Component<Position> {
            public Vector3 pos;
        }

        public class Velocity : Reaction.Component<Velocity>
        {
            public Vector3 vel;
        }

        public class VelocitySystem : System<Position, Velocity> {

            public System.Action<Entity> entityAdded;
            public System.Action<Entity> entityRemoved;


            public override void EntityAdded(Entity entity)
            {
                if (entityAdded != null)
                {
                    entityAdded(entity);
                }
            }

            public override void EntityRemoved(Entity entity)
            {
                if (entityRemoved != null)
                {
                    entityRemoved(entity);
                }
            }


            public override void Update(float dt)
            {
                foreach (var entity in Entities)
                {
                    var p = entity.GetComponent<Position>();
                    var v = entity.GetComponent<Velocity>();
                    p.pos += v.vel * dt;
                }
            }

        }

        private void NewScene() {
            UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void WorldInstantiated()
        {
            NewScene();
            bool wasInstantiated = false;

            void worldCreated(World w)
            {
                World.WorldCreated -= worldCreated;
                wasInstantiated = true;
            }

            World.WorldCreated += worldCreated;

            GameObject go = new GameObject();
            go.AddComponent<Velocity>();

 
            Assert.That(wasInstantiated);
        }

        [Test]
        public void System_EntityAdded()
        {
            NewScene();

            bool entityAdded = false;

            VelocitySystem velocitySystem = new VelocitySystem();

            World.Instance.AddSystem(velocitySystem);

            GameObject go = new GameObject();
            go.AddComponent<Position>();
            go.AddComponent<Velocity>();

            velocitySystem.entityAdded = (e) =>
            {
                entityAdded = true;
            };

            World.Instance.UpdateLoop(0.0f);
            Assert.That(entityAdded);
        }

        [Test]
        public void System_EntityRemoved()
        {
            NewScene();

            bool entityRemoved = false;

            VelocitySystem velocitySystem = new VelocitySystem();

            World.Instance.AddSystem(velocitySystem);

            GameObject go = new GameObject();

            go.AddComponent<Velocity>();
            go.AddComponent<Position>();

            velocitySystem.entityRemoved = (e) =>
            {
                entityRemoved = true;
            };

            Object.DestroyImmediate(go);

            World.Instance.UpdateLoop(0.0f);
            Assert.That(entityRemoved);
        }

        [Test]
        public void System_EntityRemovedByComponentDisabled()
        {
            NewScene();

            bool entityRemoved = false;

            VelocitySystem velocitySystem = new VelocitySystem();

            World.Instance.AddSystem(velocitySystem);

            GameObject go = new GameObject();
            go.AddComponent<Position>();
            go.AddComponent<Velocity>().enabled = false;

            velocitySystem.entityRemoved = (e) =>
            {
                entityRemoved = true;
            };

            World.Instance.UpdateLoop(0.0f);
            Assert.That(entityRemoved);
        }

    }
}
