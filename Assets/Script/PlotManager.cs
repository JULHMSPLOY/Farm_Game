using UnityEngine;

public class PlotManager : MonoBehaviour
{
    [Header("Settings & State")]
    public bool isBought = true;
    bool isPlanted = false;
    bool isDry = true;

    int plantStage = 0;
    float timer;
    float speed = 1f;

    [Header("References")]
    private SpriteRenderer plot;
    private SpriteRenderer plant;
    private BoxCollider2D plantCollider;

    private PlantObject selectedPlant;
    private FarmManager fm;

    [Header("Sprites")]
    public Sprite drySprite;
    public Sprite normalSprite;
    public Sprite unavailableSprite;

    [Header("Feedback Colors")]
    public Color availableColor = Color.green;
    public Color unavailableColor = Color.red;

    [Header("Drop Items")]
    public GameObject coinPrefab;

    void Start()
    {
        fm = FarmManager.Instance;
        plot = GetComponent<SpriteRenderer>();
        plant = transform.GetChild(0).GetComponent<SpriteRenderer>();
        plantCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();

        if (isBought) plot.sprite = drySprite;
        else plot.sprite = unavailableSprite;
        
        plant.gameObject.SetActive(false);
    }

    void Update()
    {
        // พืชจะโตต่อเมื่อ: ปลูกแล้ว และ ดินไม่แห้ง
        if (!isPlanted || isDry) return;

        // ตรวจสอบว่าพืชยังโตไม่สุด
        if (plantStage < selectedPlant.plantStages.Length - 1)
        {
            timer -= speed * Time.deltaTime;

            if (timer < 0)
            {
                plantStage++;
                timer = selectedPlant.timeBtwStages;
                UpdatePlant();
            }
        }
    }

    private void OnMouseDown()
    {
        if (isPlanted && plantStage == selectedPlant.plantStages.Length - 1)
        {
            if (!fm.isSelecting && !fm.isPlanting) // เก็บได้เมื่อไม่ได้ถือเครื่องมืออื่น
            {
                Harvest();
                return;
            }
        }
        
        if (!isPlanted && fm.isPlanting && isBought)
        {
            if (fm.money >= fm.selectPlant.plant.buyPrice)
            {
                Plant(fm.selectPlant.plant);
            }
            return;
        }

        // 2. การใช้เครื่องมือ (Tools)
        if (fm.isSelecting)
        {
            switch (fm.selectedTool)
            {
                case 1: // บัวรดน้ำ
                    if (isBought && isDry)
                    {
                        isDry = false;
                        plot.sprite = normalSprite;
                        if (isPlanted) UpdatePlant();
                    }
                    break;

                case 2: // ปุ๋ย
                    if (isBought && isPlanted && fm.money >= 10 && speed < 2f)
                    {
                        fm.Transaction(-10);
                        speed += 0.2f;
                    }
                    break;

                case 3: // ซื้อที่ดิน
                    if (!isBought && fm.money >= 100)
                    {
                        fm.Transaction(-100);
                        isBought = true;
                        plot.sprite = drySprite;
                    }
                    break;
            }
        }
    }

    private void OnMouseOver()
    {
        if (isPlanted && plantStage == selectedPlant.plantStages.Length - 1 && !fm.isSelecting && !fm.isPlanting)
        {
            plot.color = Color.yellow; 
            return; // จบการทำงานไม่ต้องไปเช็คเงื่อนไขอื่น
        }

        if (fm.isPlanting)
        {
            bool canPlant = !isPlanted && isBought && fm.money >= fm.selectPlant.plant.buyPrice;
            plot.color = canPlant ? availableColor : unavailableColor;
        }
        else if (fm.isSelecting)
        {
            bool canUseTool = false;
            if (fm.selectedTool == 1) canUseTool = isBought && isDry; 
            if (fm.selectedTool == 2) canUseTool = isBought && isPlanted && fm.money >= 10; 
            if (fm.selectedTool == 3) canUseTool = !isBought && fm.money >= 100; 

            plot.color = canUseTool ? availableColor : unavailableColor;
        }
    }

    private void OnMouseExit() => plot.color = Color.white;

    void Plant(PlantObject newPlant)
    {
        selectedPlant = newPlant;
        isPlanted = true;
        fm.Transaction(-selectedPlant.buyPrice);

        plantStage = 0;
        timer = selectedPlant.timeBtwStages;

        plant.gameObject.SetActive(true);
        UpdatePlant();
    }

    void Harvest()
    {
        if (selectedPlant == null) return;
        int moneyToGain = selectedPlant.sellPrice;

        // เหรียญโผล่ออกมาแทนที่
        if (coinPrefab != null)
        {
            GameObject coin = Instantiate(coinPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(coin, 1f); 
        }

        fm.Transaction(moneyToGain);
        ResetPlot();
    }

    void ResetPlot()
    {
        isPlanted = false;
        isDry = true;
        speed = 1f;
        plantStage = 0;
        selectedPlant = null;

        plant.gameObject.SetActive(false); 
        plot.sprite = drySprite;           
    }

    void UpdatePlant()
    {
        if (selectedPlant == null) return;
        plant.sprite = isDry ? selectedPlant.dryPlanted : selectedPlant.plantStages[plantStage];

        if (plant.sprite != null)
        {
            plantCollider.size = plant.sprite.bounds.size;
            plantCollider.offset = new Vector2(0, plant.sprite.bounds.size.y / 2);
        }
    }
}