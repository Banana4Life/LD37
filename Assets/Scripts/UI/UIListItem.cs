using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public abstract class UIListItem : MonoBehaviour
{
    public abstract bool Highlighted { get; set; }
}
