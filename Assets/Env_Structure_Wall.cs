using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Env_Structure_Wall : MonoBehaviour, IDamageableInterface
{
    public float maxHealth = 100;
    private float currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    public float TakeDamagePerTick;

    public float timer = 0f;


    public List<GameObject> PikminAttackingWall = new List<GameObject>();


    public ResourcesScriptObject[] ResourceToSpawnWhenBroken;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (PikminAttackingWall.Count != 0)
        {
            PikDamageCollection();
        }


        timer += Time.deltaTime;

        if (timer >= 1)
        {
            TakeDamage(TakeDamagePerTick);
            timer = 0f;

        }
    }

    private void PikDamageCollection()
    {
        float Total = 0;
        foreach (GameObject pikmin in PikminAttackingWall)
        {
            PikminController PikScript = pikmin.GetComponent<PikminController>();

            Total += (PikScript.pikminscriptobject.PikminDamage * PikScript.pikminscriptobject.PikminStructureCoeff);
        }

        TakeDamagePerTick = Total;
    }

    public void Heal(float healamt)
    {

    }

    public void TakeDamage(float Dmgamt)
    {
        
        currentHealth -= Dmgamt;

        if(currentHealth < 0) { DestroyGate(); }

    }

    public void DestroyGate()
    {
        foreach (ResourcesScriptObject i in ResourceToSpawnWhenBroken)
        {
            Instantiate(i.ItemWhenSpawning, transform.position, transform.rotation);
        }

        PikminAttackingWall.Clear();
        Destroy(this.transform.parent.gameObject);
    }



}
