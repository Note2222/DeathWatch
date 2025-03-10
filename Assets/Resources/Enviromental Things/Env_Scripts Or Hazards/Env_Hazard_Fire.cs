using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Env_Hazard_Fire : MonoBehaviour
{
    public string ImmunityName;
    public float FireDamageAmount;
    public float Range;
    private void Update()
    {
        FindNearbyPikmin();
    }

    void FindNearbyPikmin()
    {
        Collider[] CheckHits = Physics.OverlapSphere(transform.position, Range);

        foreach (Collider col in CheckHits)
        {
            if(col.GetComponent<IDamageableInterface>() != null)
            {
                if(!CheckImmunities(col.gameObject))
                    DoDamageToObj(col.gameObject);
            }
        }

    } // Finds the nearest pikmin collider


    void DoDamageToObj(GameObject ObjToInjure)
    {
        if (ObjToInjure.TryGetComponent<IDamageableInterface>(out IDamageableInterface pikmincontroller))
        {
            pikmincontroller.TakeDamage(FireDamageAmount);
            if(ObjToInjure.GetComponent<PikminController>() != null)
                ObjToInjure.GetComponent<PikminController>().state = PikminController.State.PanicState;
        }
        else Debug.Log("Not a pikmin");
    }

    bool CheckImmunities(GameObject ObjToCheck)
    {
        bool Hasimmunity = false;
        PikminController pikmincontroller = ObjToCheck.GetComponent<PikminController>();
        if (pikmincontroller != null)
        {
            for (int i = 0; i < pikmincontroller.Immunities.Length; i++)
            {
                if (pikmincontroller.Immunities[i] == ImmunityName)
                {
                    Hasimmunity = true;
                }
            }
        }
        return Hasimmunity;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
