using UnityEngine;

public class Env_Hazard_Electricity : MonoBehaviour
{
    public string ImmunityName;
    public float ShockDamageAmount;

    private void OnTriggerEnter(Collider other)
    {
        GameObject ObjInQuestion = other.gameObject;
        Debug.Log("Pikmin touched");
        if (!CheckImmunities(ObjInQuestion))
        {
            DoDamageToObj(ObjInQuestion);
        }
    }


    void DoDamageToObj(GameObject ObjToInjure)
    {
        if (ObjToInjure.TryGetComponent<PikminController>(out PikminController pikmincontroller))
        {
            pikmincontroller.TakeDamage(ShockDamageAmount);
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
}
