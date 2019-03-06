using UnityEngine;
using System.Collections;

namespace Reaction
{
    public interface IComponent {
    }

    [ExecuteInEditMode]
    public abstract class Component<T> : MonoBehaviour, IComponent where T : Component<T>
    {
        private int owner;

        private int Owner {
            get {
                if (owner == 0) {
                    owner = gameObject.GetInstanceID();
                }
                return owner;
            }
        }

        private void OnEnable()
        {
            World.Instance.AddComponent(Owner, (T)this);
        }

        private void OnDisable()
        {
            if (!World.IsWorldValid) return;
            World.Instance.RemoveComponent(Owner, (T)this);
        }

        public Entity Entity
        {
            get;
            set;
        }
    }
}
