using UnityEngine;

public class ShipController : MonoBehaviour
{

    public Vector3 CollectionScale;
    public Transform CollectionPoint;
    public Transform CollectionPeak;

    // Update is called once per frame
    void Update()
    {
        Collider[] colliderArray = Physics.OverlapBox(CollectionPoint.position, CollectionScale);
        // find the nearest collider
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<ResourceManager>(out ResourceManager Resourcemanager))
            {
                if (Resourcemanager.BeingCarriedBy >= Resourcemanager.resourcesScriptObject.PikminNeededToCarry)
                {
                    Resourcemanager.Collect(CollectionPeak.transform.position);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(CollectionPeak.position, 0.1f);
        Gizmos.DrawWireCube(CollectionPoint.position, CollectionScale);
    }
}
