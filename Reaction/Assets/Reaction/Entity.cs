using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reaction
{
    public class Entity
    {
        private readonly Dictionary<global::System.Type, IComponent> components = new Dictionary<global::System.Type, IComponent>();

        internal void AssignComponent<T>(T component) where T : Component<T>
        {
            var componentType = typeof(T);
            if (components.ContainsKey(componentType)) return;
            components.Add(componentType, component);
            component.Entity = this;
        }

        public T GetComponent<T>() where T : Component<T> {
            IComponent component;
            return components.TryGetValue(typeof(T), out component) ? (T)component : null;
        }

        internal void UnassignComponent<T>() where T : Component<T> {
            var componentType = typeof(T);
            if (!components.ContainsKey(componentType)) return;
            components.Remove(componentType);
        }

        internal bool HasAllComponents(List<global::System.Type> componentTypes) {
            foreach (var componentType in componentTypes) {
                if (!components.ContainsKey(componentType)) return false;
            }
            return true;
        }

        internal bool Empty {
            get {
                return components.Count == 0;
            }
        }

    }
}