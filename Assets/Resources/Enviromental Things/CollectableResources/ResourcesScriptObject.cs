using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/ResourcesScriptObject", order = 1)]
public class ResourcesScriptObject : ScriptableObject
{
    public GameObject ItemWhenSpawning;
    public bool Collectable = true;
    public int PikminNeededToCarry;
    public int ResourceAmountProvided;
    public string ResourceNameIndex;

    public Image ResourceUIImage;
}

