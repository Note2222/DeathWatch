using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class PlayerUiController : MonoBehaviour
{
    public KeyCode Menukey;

    public GameObject CraftingMenu;
    public GameObject OverworldMenu;
    public GameObject Player;
    public float transitionSpeed;
    public Vector3 Offset;
    public TextMeshProUGUI BotsInSquadText;
    public TextMeshProUGUI BotsOnTheFieldText;
    public TextMeshProUGUI BotsStoredInTheShipText;
    public SpriteRenderer CurrentPikminDisplay;
    public Slider DaySlider;

    bool CameraToggle;

    public GameObject MainCam;
    public CinemachineVirtualCamera BuildingCam;

    private void Awake()
    {

    }
    void Start()
    {
        //Transform StartCampos = Cam.transform;
        Player = PlayerManager.Instance.PlayerObject;
        BuildingCam.Priority = 0;
        MainCam.SetActive(true);
    }

    private void Update()
    {
        SetUiImage();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        HandleDaySlider();

        if (Input.GetKeyDown(KeyCode.F))
            HandleBuildingMenu();

        //if(BuildingCam.Priority == 0 && MainCam.activeInHierarchy == false)
        //{
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 100))
        //    {
        //        Debug.Log(hit.transform.position);
        //        Debug.Log("hit");
        //    }

        //}

        HandleSquadCount();

        if (Input.GetKeyDown(Menukey))
            HandleCraftingMenu();
    }

    void HandleCraftingMenu()
    {
        if (OverworldMenu.activeInHierarchy == true)
        {
            OverworldMenu.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Player.GetComponent<CharacterController>().enabled = false;
            CraftingMenu.SetActive(true);
        }
        else
        {
            OverworldMenu.SetActive(true);
            Debug.Log("toggledmenu");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Player.GetComponent<CharacterController>().enabled = true;
            CraftingMenu.SetActive(false);
        }
    }

    void SetUiImage()
    {
        GameObject[] Pikmin = Player.GetComponent<BotSquadController>().GetPikminOnTheMapTotal();
        BotsOnTheFieldText.text = Pikmin.Length.ToString();

        Collider nearestCollider = FindNearbyPikmin();
        if (nearestCollider != null)
        {
            GameObject nearestpikmin = nearestCollider.gameObject;

            CurrentPikminDisplay.sprite = nearestpikmin.GetComponentInParent<PikminController>().pikminscriptobject.PikminUIImage;
        }
        else
            CurrentPikminDisplay.sprite = null;
    }
    void HandleSquadCount() // Handles the squad
    {
        float TotalSquaddedCount = 0f; // Total squad is 0 by default
        GameObject[] Pikmin = GameObject.FindGameObjectsWithTag("Pikmin"); // finds all objects with pikmin
        foreach (GameObject pik in Pikmin) // for each
        {
            if (pik.TryGetComponent<PikminController>(out PikminController pikmincontroller)) // tries to get all of their components
            {
                if (pikmincontroller.state == PikminController.State.Follow) // if they are following
                {
                    TotalSquaddedCount += 1f; //increases squad count
                }
            }
        }
        BotsInSquadText.text = TotalSquaddedCount.ToString(); // current pikmin is equal to squad count.
    }

    void HandleBuildingMenu()
    {
        if (CameraToggle == true)
        {
            BuildingCam.Priority = 1000;

            OverworldMenu.SetActive(false);
            this.GetComponent<PlayerInput>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            ////Player.GetComponent<CharacterController>().enabled = false;

            CameraToggle = false;
        }
        else if (CameraToggle == false)
        {
            BuildingCam.Priority = 0;
            this.GetComponent<PlayerInput>().enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            ////Player.GetComponent<CharacterController>().enabled = true;
            OverworldMenu.SetActive(true);

            CameraToggle = true;
        }
    }

    void HandleDaySlider()
    {
        DaySlider.maxValue = Venv_DayNightCycle.Instance.TotalDayNightLength;
        DaySlider.value = Venv_DayNightCycle.Instance.timer;
    }

    Collider FindNearbyPikmin()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 3);
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
}
