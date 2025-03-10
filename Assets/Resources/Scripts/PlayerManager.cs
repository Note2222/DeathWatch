using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageableInterface
{
    public float maxHealth = 100;
    private float currentHealth;



    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private static PlayerManager instance;

    public GameObject PlayerObject;

    public GameObject FireObj;

    public GameObject HeldObj;
    public GameObject ObjectHoldPosition;

    void Awake()
    {
        PlayerObject = this.gameObject;
    }
    public static PlayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerManager>();
            }
            return instance;
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);

    }

    public void Update()
    {
        if (HeldObj != null)
            HeldObj.transform.position = ObjectHoldPosition.transform.position;

        Inputs();
    }


    void Inputs()
    {
        if (Input.GetKeyDown(KeyCode.E)) { GrabObject(); }

        if (Input.GetKeyUp(KeyCode.E)) { ReleaseObject(); }
    }

    void GrabObject()
    {
        
        Physics.Raycast(FireObj.transform.position, FireObj.transform.forward, out RaycastHit hitInfo, 20f);

        ResourceManager resourcemanager = hitInfo.collider.gameObject.GetComponent<ResourceManager>();

        if (resourcemanager != null)
        {
            if(resourcemanager.enabled == true)
            resourcemanager.enabled = false;
            HeldObj = resourcemanager.gameObject;
        }
    }

    void ReleaseObject()
    {
        if(HeldObj != null)
        {
            ResourceManager resourcemanager = HeldObj.GetComponent<ResourceManager>();
            if (resourcemanager.enabled == false)
                resourcemanager.enabled = true;
            resourcemanager.BeingCarriedBy = resourcemanager.resourcesScriptObject.PikminNeededToCarry;
            HeldObj = null;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Implement death behavior here
        Destroy(gameObject);
    }
}
