using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrierMachine : MachineBase
{
    [Header("Configuration")]
    public List<DrierRecipes> recipes;

    [Header("UI Controls")]
    public Slider tempSlider;
    public TextMeshProUGUI tempText;
    public Slider durationSlider;
    public TextMeshProUGUI durationText;
    public Button startButton;
    public TextMeshProUGUI etaText;

    [Header("Batch Control")]
    public Slider batchQuantitySlider;
    public TextMeshProUGUI batchQuantityText;

    [Header("Slots UI")]
    public Image inputIcon;
    public Image outputIcon;

    // Internal Data
    private ItemData currentInputItem;
    private InventoryItem currentOutputData; // Wrapper Data

    private int currentInputStock;
    private int quantityToProcess; // Jumlah yang sedang/akan diproses

    private DrierRecipes validRecipe;
    private InventoryUI sourceInventorySlot;
    private bool isProcessing = false;
    private float lastProcessQuality = 0f;

    protected override void Start()
    {
        base.Start();

        tempSlider.onValueChanged.AddListener(delegate { UpdateUITexts(); });
        durationSlider.onValueChanged.AddListener(delegate { UpdateUITexts(); });
        batchQuantitySlider.onValueChanged.AddListener(delegate { OnBatchSliderChanged(); });

        startButton.onClick.AddListener(StartBatchProcess);

        ResetUI();
    }

    // --- INPUT ---
    public void TryInsertItem(ItemData item, int totalQty, InventoryUI sourceSlot)
    {
        if (isProcessing) return;
        if (currentOutputData != null) return;

        DrierRecipes match = recipes.Find(r => r.inputItem == item);
        if (match == null) return;

        validRecipe = match;
        currentInputItem = item;
        currentInputStock = totalQty;
        sourceInventorySlot = sourceSlot;

        inputIcon.gameObject.SetActive(true);
        inputIcon.sprite = item.icon;
        inputIcon.color = Color.white;
        inputIcon.rectTransform.localScale = Vector3.one;

        batchQuantitySlider.maxValue = totalQty;
        batchQuantitySlider.value = 1;
        quantityToProcess = 1;

        ToggleControls(true);
        UpdateUITexts();

        if (etaText) etaText.text = "READY";
    }

    public void CancelInput()
    {
        if (isProcessing) return;
        ResetUI();
    }

    // --- PROSES ---
    void StartBatchProcess()
    {
        if (currentInputItem == null) return;

        if (sourceInventorySlot != null)
        {
            if (sourceInventorySlot.CurrentItem == currentInputItem && sourceInventorySlot.Quantity >= quantityToProcess)
            {
                sourceInventorySlot.AddCount(-quantityToProcess);
            }
            else
            {
                ResetUI();
                return;
            }
        }
        StartCoroutine(ProcessRoutine());
    }

    IEnumerator ProcessRoutine()
    {
        isProcessing = true;
        ToggleControls(false);

        float settingTemp = tempSlider.value;
        float settingTime = durationSlider.value;
        float totalBatchDuration = settingTime;

        ItemData resultBaseItem = validRecipe.outputItem;

        float startTime = Time.time;
        float endTime = startTime + totalBatchDuration;

        while (Time.time < endTime)
        {
            float timeRemaining = endTime - Time.time;
            if (etaText) etaText.text = $"{timeRemaining:0.0}s";
            yield return null;
        }

        isProcessing = false;
        if (etaText) etaText.text = "DONE";

        lastProcessQuality = CalculateQuality(settingTemp, settingTime);

        // [FIX BUG 1] Output Quantity sekarang mengikuti quantityToProcess (bukan hardcoded 1 lagi)
        currentOutputData = new InventoryItem(resultBaseItem, quantityToProcess, lastProcessQuality, true);

        ShowOutput();

        currentInputItem = null;
        inputIcon.gameObject.SetActive(false);

        startButton.interactable = false;
        ToggleControls(false);
    }

    float CalculateQuality(float userTemp, float userTime)
    {
        float diffTemp = Mathf.Abs(userTemp - validRecipe.optimalTemperature);
        float diffTime = Mathf.Abs(userTime - validRecipe.optimalDuration);
        float penalty = (diffTemp * 1.0f) + (diffTime * 2.0f);
        return Mathf.Clamp(100f - penalty, 0f, 100f);
    }

    // --- OUTPUT ---
    void ShowOutput()
    {
        outputIcon.gameObject.SetActive(true);
        outputIcon.sprite = currentOutputData.data.icon;
        outputIcon.color = Color.white;
        outputIcon.rectTransform.localScale = Vector3.one;

        // [FIX BUG 2] Paksa Raycast Target nyala agar bisa didrag
        if (outputIcon.GetComponent<Image>() != null)
            outputIcon.GetComponent<Image>().raycastTarget = true;
    }

    public InventoryItem TakeOutputItem()
    {
        InventoryItem temp = currentOutputData;
        ClearOutput();
        return temp;
    }

    public void ClearOutput()
    {
        currentOutputData = null;

        outputIcon.gameObject.SetActive(false);
        outputIcon.sprite = null;

        ResetUI();
    }

    // --- UI UPDATE ---

    void UpdateUITexts()
    {
        if (tempText != null) tempText.text = $"{tempSlider.value:0} °C";
        if (durationText != null) durationText.text = $"{durationSlider.value:0} s";
    }

    void OnBatchSliderChanged()
    {
        quantityToProcess = Mathf.RoundToInt(batchQuantitySlider.value);
        if (batchQuantityText)
            batchQuantityText.text = $"Process: {quantityToProcess} / {currentInputStock}";
    }

    void ToggleControls(bool active)
    {
        tempSlider.interactable = active;
        durationSlider.interactable = active;
        batchQuantitySlider.interactable = active;
        startButton.interactable = active;
    }

    void ResetUI()
    {
        currentInputItem = null;
        if (inputIcon) inputIcon.gameObject.SetActive(false);

        if (currentOutputData == null)
        {
            if (outputIcon) outputIcon.gameObject.SetActive(false);
        }

        if (etaText) etaText.text = "IDLE";

        batchQuantitySlider.value = 1;
        tempSlider.value = tempSlider.minValue;
        durationSlider.value = durationSlider.minValue;

        if (batchQuantityText) batchQuantityText.text = "Process: 0";
        UpdateUITexts();

        ToggleControls(false);
    }
}
