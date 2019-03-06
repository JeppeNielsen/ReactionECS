using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sizable : Reaction.Component<Sizable>{

    [GetSet("Size")]
    [SerializeField]
    private Vector2 size;

    public Vector2 Size
    {
        get { return size; }
        set { size = value;

            if (SizeChanged!=null) {
                SizeChanged(this);
            }
        }
    }

    public event System.Action<Sizable> SizeChanged;

}
