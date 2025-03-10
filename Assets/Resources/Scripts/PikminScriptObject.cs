using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "Pikmin", menuName = "ScriptableObjects/PikminScriptableObject", order = 1)]
public class PikminScriptObject : ScriptableObject
{
    public GameObject Pikmintospawn;
    public float PikminDamage;
    public float PikminStructureCoeff = 1f;
    public float PikminJumpHeight;
    public float PikminMoveSpeed;
    public int PikminCarryWeight;

    public int[] ResourceCount;
    public string[] ResourceID;
    public Sprite PikminUIImage;
}
