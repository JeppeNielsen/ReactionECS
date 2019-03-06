using System;
using System.Collections.Generic;

namespace Reaction
{
    public abstract class SystemBase
    {
        private readonly List<Entity> entities = new List<Entity>();
        private readonly List<global::System.Type> componentTypes = new List<global::System.Type>();

        public IEnumerable<Entity> Entities
        {
            get { return entities; }
        }

        public List<global::System.Type> ComponentTypes
        {
            get
            {
                return componentTypes;
            }
        }

        protected abstract void AddComponentTypes(List<global::System.Type> componentTypes);

        public SystemBase() {
            AddComponentTypes(componentTypes);
        }

        public void AddEntity(Entity entity) {
            if (!entity.HasAllComponents(componentTypes)) return;
            if (entities.Contains(entity)) return;
            entities.Add(entity);
            EntityAdded(entity);
        }

        public void RemoveEntity(Entity entity) {
            if (!entity.HasAllComponents(componentTypes)) return;
            if (!entities.Contains(entity)) return;
            EntityRemoved(entity);
            entities.Remove(entity);
        }

        public virtual void EntityAdded(Entity entity) {}
        public virtual void EntityRemoved(Entity entity) { }
        public virtual void Update(float dt) { }
    }

    //in lack of variadic templates, :/
    public abstract class System<Component> : SystemBase {
        sealed protected override void AddComponentTypes(List<Type> componentTypes) {
            componentTypes.Add(typeof(Component));
        }
    }

    public abstract class System<Component1, Component2> : SystemBase
    {
        sealed protected override void AddComponentTypes(List<Type> componentTypes)
        {
            componentTypes.Add(typeof(Component1));
            componentTypes.Add(typeof(Component2));
        }
    }

    public abstract class System<Component1, Component2, Component3> : SystemBase
    {
        sealed protected override void AddComponentTypes(List<Type> componentTypes)
        {
            componentTypes.Add(typeof(Component1));
            componentTypes.Add(typeof(Component2));
            componentTypes.Add(typeof(Component3));
        }
    }

    public abstract class System<Component1, Component2, Component3, Component4> : SystemBase
    {
        sealed protected override void AddComponentTypes(List<Type> componentTypes)
        {
            componentTypes.Add(typeof(Component1));
            componentTypes.Add(typeof(Component2));
            componentTypes.Add(typeof(Component3));
            componentTypes.Add(typeof(Component4));
        }
    }

}
