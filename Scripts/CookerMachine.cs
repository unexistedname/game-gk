using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookerMachine : SubMachine
{
    // ... (Variabel Header sama seperti sebelumnya) ...
    [Header("Cooker Settings")]
    public List<CookerRecipe> recipes;
    public CookerInputSlot[] inputSlots;
    public Image outputIconDisplay;

    private ItemData currentResultItem;
    private int currentResultCount;
    private CookerRecipe activeRecipe;

    public ItemData CurrentResult => currentResultItem;
    public int CurrentResultCount => currentResultCount;

    protected override void Start()
    {
        base.Start();
        outputIconDisplay.enabled = false;
    }

    public void CheckRecipes()
    {
        // ... (Sama seperti sebelumnya) ...
        currentResultItem = null;
        currentResultCount = 0;
        activeRecipe = null;
        outputIconDisplay.enabled = false;

        foreach (CookerRecipe recipe in recipes)
        {
            if (IsRecipeMatch(recipe))
            {
                activeRecipe = recipe;
                currentResultItem = recipe.outputItem;
                currentResultCount = recipe.outputCount;

                outputIconDisplay.sprite = currentResultItem.icon;
                outputIconDisplay.enabled = true;
                return;
            }
        }
    }

    bool IsRecipeMatch(CookerRecipe recipe)
    {
        foreach (Ingredient ing in recipe.ingredients)
        {
            bool ingredientFound = false;
            foreach (CookerInputSlot slot in inputSlots)
            {
                if (slot.CurrentItem == ing.item)
                {
                    // [UBAH DI SINI]
                    // Cek 'slot.GetInputValue()' (Angka Input User), BUKAN 'slot.CurrentStackSize'
                    // Ini memastikan resep hanya jalan jika USER MENUNJUKKAN jumlah yang cukup
                    if (slot.GetInputValue() >= (int)ing.amount)
                    {
                        ingredientFound = true;
                        break;
                    }
                }
            }
            if (!ingredientFound) return false;
        }

        int nonEmptySlots = 0;
        foreach (var slot in inputSlots) if (slot.CurrentItem != null) nonEmptySlots++;
        if (nonEmptySlots != recipe.ingredients.Count) return false;

        return true;
    }

    public void CraftItem()
    {
        if (activeRecipe == null) return;

        foreach (CookerInputSlot slot in inputSlots)
        {
            if (slot.CurrentItem == null) continue;

            int amountNeeded = 0;
            foreach (Ingredient ing in activeRecipe.ingredients)
            {
                if (ing.item == slot.CurrentItem)
                {
                    amountNeeded = (int)ing.amount;
                    break;
                }
            }

            if (amountNeeded > 0)
            {
                slot.DecreaseAmount(amountNeeded);
            }
        }
        CheckRecipes();
    }
}
