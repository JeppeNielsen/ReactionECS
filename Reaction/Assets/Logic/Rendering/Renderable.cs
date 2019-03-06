using UnityEngine;

public class Renderable : Reaction.Component<Renderable>
{
    private Renderer _renderer;

    public Renderer Renderer {
        get {
            if (_renderer == null) {
                _renderer = GetComponent<Renderer>();
            }
            return _renderer;
        }
    }
}
