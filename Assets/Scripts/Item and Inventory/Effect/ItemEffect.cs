using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Item name", menuName = "Data/Item effect")]
public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string itemEffectDescription;
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Execute  Effect");
    }
}
