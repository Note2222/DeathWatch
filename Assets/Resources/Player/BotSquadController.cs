using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
public class BotSquadController : MonoBehaviour
{
    public GameObject PlayerInstance;

    public float PikminCountMax = 100;
    public float CurrentPikminCount = 0;

    public GameObject PikminGatherSpot;

    public KeyCode DisperseKey = KeyCode.X;
    public KeyCode ThrowKey = KeyCode.Mouse0;
    public KeyCode WhistleKey = KeyCode.Mouse1;

    public static BotSquadController instance;

    public float WhistleSize;
    public float PikminThrowRange;
    public float GetNearbyPikminRange;

    public LayerMask RayCastLayer;
    private void Awake()
    {
        PlayerInstance = PlayerManager.Instance.PlayerObject;
    }

    private void Update()
    {

        MyInput(); // Gets key inputs
        HandleSquad(); // Handles squad on update
    }
    public void MyInput()
    {
        if (Input.GetKey(WhistleKey))
        {
            Whistle(); // Whistles for the pikmin
        }

        if (Input.GetKeyDown(DisperseKey))
        {
            Disperse(); // Sets all pikmin on the map to idle
        }

        if (Input.GetKeyDown(ThrowKey))
        {
            ThrowPikmin(); // Throws a pikmin that is close to you
        }
    }

    public void Whistle()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ray from cam to mouse pos
        RaycastHit hit; // stores hit
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) // fires ray to infinity (change later to max range on throwing)
        {
            Collider[] WhistleSphere = Physics.OverlapSphere(hit.point, WhistleSize); // collider of all in whistle range
            foreach (Collider pik in WhistleSphere) // for each
            {
                PikminController pikcontroller = pik.gameObject.GetComponent<PikminController>(); // get pikmin controllers
                if (pikcontroller != null) // if its not null
                {
                    if (pikcontroller.state != PikminController.State.Follow) // if its not follow, set it to be follow.
                    {
                        pikcontroller.state = PikminController.State.Follow;
                    }
                }
            }
        }
    }



    public void ThrowPikmin() // Throws pikmin
    {
        Collider nearestCollider = FindNearbyPikmin(); // Calls on find nearby pikmin which then returns the nearest pikmin

        // set the AI's destination to the position of the nearest collider
        if (nearestCollider != null) // if its not null
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // rays to mouse pos and stores
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, PikminThrowRange, RayCastLayer)) // if it hit something
            {

                GameObject SelectedPikmin = nearestCollider.gameObject; // selected pikmin is equal to the nearest collider's gameobject
                PikminController SelectedPikminController = SelectedPikmin.GetComponent<PikminController>(); // then the pikmin controller afterwards
                if (SelectedPikminController.state == PikminController.State.Follow) // if its follow
                {
                    SelectedPikminController.state = PikminController.State.Jump; // make it jump

                    SelectedPikmin.transform.DOJump(hit.point + new Vector3(0, SelectedPikmin.transform.localScale.y, 0), SelectedPikminController.pikminscriptobject.PikminJumpHeight, 1, 1.1f).OnComplete(() => //dotween a jump to vector pos
                    SelectedPikminController.agent.Warp(hit.point));
                    SelectedPikminController.state = PikminController.State.Idle; // then set to idle
                }
            }
        }
    }


    Collider FindNearbyPikmin()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, GetNearbyPikminRange);
        // find the nearest collider
        Collider nearestCollider = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<PikminController>(out PikminController pikmincontroller))
            {
                // find the distance between the collider and the AI gameobject
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCollider = collider;
                }
            }
        }
        return nearestCollider;
    } // Finds the nearest pikmin collider


    public void HandleSquad() // Handles the squad
    {
       


        float TotalSquaddedCount = 0f; // Total squad is 0 by default
        GameObject[] Pikmin = GameObject.FindGameObjectsWithTag("Pikmin"); // finds all objects with pikmin
        foreach (GameObject pik in Pikmin) // for each
        {
            if (pik.TryGetComponent<PikminController>(out PikminController pikmincontroller)) // tries to get all of their components
            {
                if (pikmincontroller.state == PikminController.State.Follow) // if they are following
                {
                    NavMeshAgent Agent = pik.GetComponent<PikminController>().agent; // set their destination to be the captain
                    if(Agent.enabled)
                    Agent.SetDestination(PikminGatherSpot.transform.position);
                    TotalSquaddedCount += 1f; //increases squad count
                }
            }
        }
        CurrentPikminCount = TotalSquaddedCount; // current pikmin is equal to squad count.
    }

    public void Disperse()
    {
        GameObject[] Pikmin = GetPikminOnTheMapTotal();

        foreach (GameObject pik in Pikmin)
        {
            if (pik.TryGetComponent<PikminController>(out PikminController pikmincontroller))
            {
                if (pikmincontroller.state == PikminController.State.Idle) continue; // if the pikmins state is not idle or interacting, then it sets it to idle.


                if (pikmincontroller.state == PikminController.State.Interact) continue;

                CurrentPikminCount--;
                pikmincontroller.state = PikminController.State.Idle;
            }
        }
    }

    public GameObject[] GetPikminOnTheMapTotal()
    {
        GameObject[] Pikmin = GameObject.FindGameObjectsWithTag("Pikmin");
        return Pikmin;
    }



}
