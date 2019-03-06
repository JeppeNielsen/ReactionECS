using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Reaction
{
    public interface IWorld {
        void AddSystem(SystemBase system);
        void UpdateLoop(float dt);
        void AddComponent<T>(int gameObjectId, T component) where T : Component<T>;
        void RemoveComponent<T>(int gameObjectId, T component) where T : Component<T>;
    }

    [ExecuteInEditMode]
    public class World : MonoBehaviour, IWorld
    {
        public static event Action<World> WorldCreated;

        private static World instance;
        public static IWorld Instance {
            get {
                if (instance == null) {

                    instance = FindObjectOfType<World>();
                    // fallback, might not be necessary.
                    if (instance == null)
                    {
                        instance = new GameObject(typeof(World).Name).AddComponent<World>();
                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(instance.gameObject);
                        }
                        instance.Initialize();

                        if (WorldCreated != null)
                        {
                            WorldCreated(instance);
                        }
                    }
                }
                return instance;
            }
        }

        public static bool IsWorldValid {
            get { return instance != null; }
        }

        private readonly Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        private readonly List<SystemBase> systems = new List<SystemBase>();
        private readonly List<Action> addComponentActions = new List<Action>();
        private readonly List<Action> removeComponentActions = new List<Action>();
        private readonly Dictionary<Type, List<SystemBase>> systemsWithComponent = new Dictionary<Type, List<SystemBase>>();

        private void Initialize() {

        }

        public void AddSystem(SystemBase system) {
            systems.Add(system);

            foreach (var componentType in system.ComponentTypes)
            {
                if (!systemsWithComponent.TryGetValue(componentType, out List<SystemBase> systems)) {
                    systems = new List<SystemBase>();
                    systemsWithComponent.Add(componentType, systems);
                }
                systems.Add(system);
            }
        }

        public void AddComponent<T>(int gameObjectId, T component) where T : Component<T> 
        {
            if (!entities.TryGetValue(gameObjectId, out Entity entity))
            {
                entity = new Entity();
                entities.Add(gameObjectId, entity);
            }

            entity.AssignComponent<T>(component);
            addComponentActions.Add(() =>
            {
                if (systemsWithComponent.TryGetValue(typeof(T), out List<SystemBase> systemsWithComponentType))
                {
                    foreach (var system in systemsWithComponentType)
                    {
                        system.AddEntity(entity);
                    }
                }
            });
        }

        public void RemoveComponent<T>(int gameObjectId, T component) where T : Component<T>
        {

            if (!entities.TryGetValue(gameObjectId, out Entity entity))
            {
                entity = new Entity();
                entities.Add(gameObjectId, entity);
            }

            removeComponentActions.Add(() =>
            {
                if (systemsWithComponent.TryGetValue(typeof(T), out List<SystemBase> systemsWithComponentType))
                {
                    foreach (var system in systemsWithComponentType)
                    {
                        system.RemoveEntity(entity);
                    }
                }

                entity.UnassignComponent<T>();
                if (entity.Empty) {
                    entities.Remove(gameObjectId);
                }
            });
        }

        private void Update()
        {
            UpdateLoop(Time.deltaTime);
        }

        private void DoActions(List<Action> actions) {
            foreach (var action in actions)
            {
                action();
            }
            actions.Clear();
        }

        public void UpdateLoop(float dt) {
            DoActions(addComponentActions);
            DoActions(removeComponentActions);
            foreach (var system in systems)
            {
                system.Update(dt);
            }
        }

    }
}