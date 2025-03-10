using UnityEngine;

public class Env_ResourceRespawnPoint : MonoBehaviour
{
    public GameObject ObjToRespawn;

    GameObject SpawnedObject;

    private void Awake()
    {
        SpawnObj(ObjToRespawn);
    }

    void Update()
    {

        if (GameManager.Instance.DayOrNight == false && SpawnedObject == null)
        {
            SpawnObj(ObjToRespawn);
        }
        
    }

    void SpawnObj(GameObject ObjToRespawn)
    {
        int Rand = Random.Range(-359, 359);
           GameObject ObjInstance = Instantiate(ObjToRespawn, transform.position, Quaternion.identity);
        SpawnedObject = ObjInstance;
    }





}
