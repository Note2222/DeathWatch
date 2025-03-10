using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResourceManager : MonoBehaviour
{
    public ResourcesScriptObject resourcesScriptObject;
    public LayerMask pikminlayer;
    public NavMeshAgent Agent;
    public int BeingCarriedBy;
    public Transform Transportpoint;

    public bool collected = false;


    public float radius = 1;
    public GameObject CarryObjectBase;
    public List<GameObject> CarryingPoints = new List<GameObject>();


    public List<GameObject> CarryingBots = new List<GameObject>();


    public Vector3 CollectionScale;

    public Text CarryingText;
    private void Awake()
    {
        radius = this.gameObject.transform.localScale.x;

        for (int i = 0; i < CarryingBots.Count; i++)
            CarryingBots.Add(null);


        CarryingPoints.Capacity = resourcesScriptObject.PikminNeededToCarry + 3;

        CarryingBots.Capacity = resourcesScriptObject.PikminNeededToCarry + 3;


        for (int i = 0; i < resourcesScriptObject.PikminNeededToCarry + 3; i++)
        {
            GameObject CarryPoint = Instantiate(CarryObjectBase, new Vector3(0,0,0), Quaternion.identity, this.transform);
            CarryingBots.Add(CarryObjectBase);
        }



        for (int i = 0; i < 360; i += 360 / CarryingPoints.Capacity)
        {
            // Calculate the radians for the current angle
            float angleInRadians = i * Mathf.Deg2Rad;

            // Calculate the position on the circle
            float x = Mathf.Cos(angleInRadians) * radius;
            float z = Mathf.Sin(angleInRadians) * radius;

            // Create a new position vector
            Vector3 position = new Vector3(this.transform.position.x + x, this.transform.position.y, this.transform.position.z + z); // Assuming you want the objects on the XZ plane

            // Instantiate the object at the calculated position with no rotation
            GameObject CarryPoint = Instantiate(CarryObjectBase, position, Quaternion.identity, this.transform);
            CarryingPoints.Add(CarryPoint);
        }

        Transportpoint = FindTransportPoint();


        //for (int i = 0; i < CarryingBots.Capacity; i++)
        //{
        //    CarryingBots.Add(null);
        //}
    }
    private void FixedUpdate()
    {

        Transportpoint = FindTransportPoint();
    }

    void Update()
    {


        BeingCarriedBy = CarriedByCheck();


        if (BeingCarriedBy > 1) CarryingText.text = BeingCarriedBy.ToString() + " " + "/" + " " + resourcesScriptObject.PikminNeededToCarry.ToString();
        else if (BeingCarriedBy < 1) CarryingText.text = "";

        setspeedofcarrying();

        //NearbyPikminCheck();

        if (collected == true)
        {
            Collect(Transportpoint.position);
        }

        if (BeingCarriedBy >= resourcesScriptObject.PikminNeededToCarry)
            BeginMoving();
        else Agent.isStopped = true;

    }

    private void setspeedofcarrying()
    {
        if (BeingCarriedBy != resourcesScriptObject.PikminNeededToCarry * 2)
            Agent.speed = resourcesScriptObject.PikminNeededToCarry * (BeingCarriedBy * 1.05f);
        else Agent.speed = resourcesScriptObject.PikminNeededToCarry * (resourcesScriptObject.PikminNeededToCarry * 1.05f);
    }

    //private void NearbyPikminCheck()
    //{
    //    Collider[] CollisionBox = Physics.OverlapBox(transform.position, CollectionScale, Quaternion.identity, pikminlayer);
    //    foreach (Collider pik in CollisionBox)
    //    {
    //        GameObject pikobject = pik.gameObject;

    //        pik.TryGetComponent<PikminController>(out PikminController pikmincontroller);
    //        if (pikmincontroller != null)
    //        {
    //            if (pikmincontroller.state == PikminController.State.Idle && pik.GetComponent<FixedJoint>() != null)
    //            {

    //                pik.transform.DOKill();
    //                pikmincontroller.state = PikminController.State.Interact;
    //            }
    //        }
    //    }
    //}

    int CarriedByCheck()
    {
        int total = 0;

        for (int i = 0; i < CarryingBots.Count; i++)
        {
            if (CarryingBots[i].GetComponent<PikminController>() == null) continue;
            if (CarryingBots[i].GetComponent<PikminController>().state != PikminController.State.Interact)
            {
                CarryingBots.Remove(CarryingBots[i]);
                CarryingBots.Insert(i, CarryObjectBase);
            }


            if (CarryingBots[i].GetComponent<PikminController>() == null) continue;
               total += CarryingBots[i].GetComponent<PikminController>().pikminscriptobject.PikminCarryWeight;
        }
    

        
        return total;
    }

    //(GameObject Carried in CarryingBots)
    //{
    //    if (Carried.GetComponent<PikminController>().state == PikminController.State.Interact)
    //    {
    //        if (Carried != null)
    //            total += Carried.GetComponent<PikminController>().pikminscriptobject.PikminCarryWeight;
    //    }
    //    else CarryingBots.Remove(Carried);
    //}
    //GameObject[] Pikmin = GameObject.FindGameObjectsWithTag("Pikmin");
    //foreach (var item in Pikmin)
    //{
    //    FixedJoint PikminJoints = item.GetComponent<FixedJoint>();
    //    if (PikminJoints != null)
    //        if (PikminJoints.connectedBody == this.GetComponent<Rigidbody>())
    //        {

    //            total += item.GetComponent<PikminController>().pikminscriptobject.PikminCarryWeight;
    //        }
    //}

    //return total;

    //private void PikminCarryCheck()
    //{
    //    int Total = 0;
    //    PikminController[] pikmincontroller = GetComponentsInChildren<PikminController>();
    //    foreach(PikminController pikcon in pikmincontroller)
    //    {
    //        Total += pikcon.pikminscriptobject.PikminCarryWeight;
    //    }

    //    BeingcarriedBy = Total;
    //}

    Transform FindTransportPoint()
    {
        GameObject[] Transportpoints = GameObject.FindGameObjectsWithTag("CarryPoint"); // Find all carry points on the map
        // find the nearest collider
        GameObject nearestPoint = null;
        float nearestDistance = float.MaxValue;
        ///
        /// Return the closest one found on the map.
        ///
        foreach (GameObject Transportpoint in Transportpoints)
        {
            // find the distance between the collider and the AI gameobject
            float distance = Vector3.Distance(transform.position, Transportpoint.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPoint = Transportpoint;
            }
        }
        return nearestPoint.transform;
    }

    void BeginMoving()
    {
        Agent.SetDestination(Transportpoint.position);
        Agent.isStopped = false;
    }

    public bool IsCarryPointFree(int i)
    {
        bool isCarryPointFree = true;

        if(CarryingBots[i] == CarryObjectBase)
            isCarryPointFree = true;

        else if (CarryingBots[i] != CarryObjectBase)
            isCarryPointFree = false;

        return isCarryPointFree;

    }


    public void Collect(Vector3 CollectionPoint)
    {
        PikminController[] pikmincontroller = GetComponentsInChildren<PikminController>();
        if (collected == false)
        {
            foreach (PikminController child in pikmincontroller)
            {

                var Joint = child.GetComponent<Joint>();
                Destroy(Joint);
                child.state = PikminController.State.Idle;
            }
            Invoke("AddResourcesToMemory", 0);
        }
        collected = true;
        if (collected == true)
        {
            foreach (PikminController child in pikmincontroller)
            {

                var Joint = child.GetComponent<Joint>();
                Destroy(Joint);

                child.state = PikminController.State.Idle;
            }
            //Capture Animation
            float time = 1.6f;
            transform.DOMove(CollectionPoint, time).SetEase(Ease.InQuint);
            transform.DOScale(0, time).SetEase(Ease.InQuint).OnComplete(() => Destroy(this));
        }




    }

    void AddResourcesToMemory()
    {
        GameManager.Instance.InventoryDictionary.AddResource(resourcesScriptObject.ResourceNameIndex, resourcesScriptObject.ResourceAmountProvided);
    }

    public Vector3 GetPosition()
    {
        float angle = BeingCarriedBy * Mathf.PI * 2f / resourcesScriptObject.PikminNeededToCarry;
        return transform.position + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawWireCube(transform.position, CollectionScale);
    }

}
