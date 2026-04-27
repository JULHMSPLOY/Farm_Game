using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantItem : MonoBehaviour
{
    public PlantObject plant;

    public TMP_Text nameTxt;
    public TMP_Text priceTxt;
    public Image icon;

    public Image btnImage;
    public TMP_Text btnTxt;

    FarmManager fm;
    
    void Start()
    {
        fm = FindFirstObjectByType<FarmManager>();
        InitializeUI();
    }

    public void BuyPlant()
    {
        Debug.Log("Bought " + plant.plantName);
        fm.SelectPlant(this);
    }

    void InitializeUI()
    {
        nameTxt.text = plant.plantName;
        priceTxt.text = "$" + plant.buyPrice.ToString();
        icon.sprite = plant.icon;
    }
}
