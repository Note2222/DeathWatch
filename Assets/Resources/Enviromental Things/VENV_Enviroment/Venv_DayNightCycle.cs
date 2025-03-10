using DG.Tweening;
using UnityEngine;
public class Venv_DayNightCycle : MonoBehaviour
{

    private static Venv_DayNightCycle instance;
    public float timer = 0f;
    public float TotalDayNightLength;

    public static Venv_DayNightCycle Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Venv_DayNightCycle>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalRotate(new Vector3(360, 0, 0), TotalDayNightLength, RotateMode.LocalAxisAdd).OnComplete(() =>
        transform.DORestart());
    }



    private void Update()
    {

        timer += Time.deltaTime;

        if (timer >= (TotalDayNightLength / 2))
        {
            GameManager.Instance.DayOrNight = false;
        }

        if(timer >= TotalDayNightLength)
        {
            GameManager.Instance.DayOrNight = true;
            timer = 0f;
        }
    }

}
