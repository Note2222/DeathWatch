using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public InventoryDictionary InventoryDictionary;
    public bool DayOrNight;

    // Start is called before the first frame update
    void Awake()
    {
        DayOrNight = true;
        InventoryDictionary = GetComponentInParent<InventoryDictionary>();
    }

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    /// <summary>
    /// Changes the current day or night state to what it is now. Day is true, night is false.
    /// </summary>
    public void ChangeDayOrNight()
    {
        if (DayOrNight == true) DayOrNight = false; // Changes from DAY to NIGHT.

        if (DayOrNight == false) DayOrNight = true; // Changes from NIGHT to DAY.
    }

}
