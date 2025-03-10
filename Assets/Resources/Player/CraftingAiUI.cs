using UnityEngine;
using UnityEngine.UI;

public class CraftingAiUI : MonoBehaviour
{


    public PikminScriptObject PikminScriptObject;


    public void CraftPikmin(PikminScriptObject pikmin)
    {
        GameObject Player = this.GetComponentInParent<PlayerUiController>().Player;
        bool CreateEntity = false;
        int ValidationCheck = 0;
        if (CreateEntity == false)
        {
            for (int i = 0; i < pikmin.ResourceID.Length; i++)
            {
                if (GameManager.Instance.InventoryDictionary.HasEnoughResources(pikmin.ResourceID[i], pikmin.ResourceCount[i]))
                    ValidationCheck++;
                Debug.Log("Crafting Validation Check Stage:" + " " + ValidationCheck);
            }

            if (ValidationCheck == pikmin.ResourceID.Length)
                for (int i = 0; i < pikmin.ResourceID.Length; i++)
                {
                    if (GameManager.Instance.InventoryDictionary.ConsumeResources(pikmin.ResourceID[i], pikmin.ResourceCount[i]))
                        CreateEntity = true;
                }
        }

        if (CreateEntity == true)
        {
            Instantiate(pikmin.Pikmintospawn, Player.transform.position, Player.transform.rotation);
            ValidationCheck = 0;
            CreateEntity = false;
        }
    }
}
