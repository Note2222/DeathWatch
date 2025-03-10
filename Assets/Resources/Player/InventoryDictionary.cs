using System.Collections.Generic;
using UnityEngine;

public class InventoryDictionary : MonoBehaviour
{
    public Dictionary<string, int> PlayerResources = new Dictionary<string, int>();

    // Add a resource with a specified quantity to the inventory
    public void AddResource(string resourceName, int quantity)
    {
        if (PlayerResources.ContainsKey(resourceName))
        {
            PlayerResources[resourceName] += quantity;
        }
        else
        {
            PlayerResources.Add(resourceName, quantity);
        }
    }

    // Check if multiple resources with specific quantities are available
    public bool HasEnoughResources(string ResourceId, int ResourceQuantity)
    {

            if (!PlayerResources.ContainsKey(ResourceId) || PlayerResources[ResourceId] < ResourceQuantity)
            {
                return false;
            }
        
        return true;
    }

    // Consume multiple resources with specific quantities from the inventory
    public bool ConsumeResources(string ResourceId, int ResourceQuantity)
    {
        if (HasEnoughResources(ResourceId, ResourceQuantity))
        {

                PlayerResources[ResourceId] -= ResourceQuantity;

                if (PlayerResources[ResourceId] <= 0)
                {
                    PlayerResources.Remove(ResourceId);
                }
            
            return true; // Successfully consumed resources
        }
        else
        {
            return false; // Not enough resources to consume
        }
    }

    public int GetResourceCount(string resourceName)
    {
        if (PlayerResources.ContainsKey(resourceName))
        {
            return PlayerResources[resourceName];
        }
        else
        {
            return 0; // Resource not found, return 0
        }
    }

    public void FixedUpdate()
    {
        Debug.Log("Resources in inventory:");
        foreach (KeyValuePair<string, int> kvp in PlayerResources)
        {
            string resource = kvp.Key;
            int quantity = kvp.Value;
            Debug.Log(resource + ": " + quantity);
        }
    }
}