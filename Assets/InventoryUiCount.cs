using UnityEngine;
using UnityEngine.UI;
public class InventoryUiCount : MonoBehaviour
{
    public ResourcesScriptObject Resourcescriptobj;
    public Text Text;

    public Image Artwork;

    public void Start()
    {
        Artwork.sprite = Resourcescriptobj.ResourceUIImage.sprite;
    }

    public void Update()
    {
        Text.text = GameManager.Instance.InventoryDictionary.GetResourceCount(Resourcescriptobj.ResourceNameIndex).ToString();
    }
}
