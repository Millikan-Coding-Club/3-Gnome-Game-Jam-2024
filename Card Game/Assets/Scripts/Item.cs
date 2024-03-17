using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    public string Description;
    public int Price;
}
