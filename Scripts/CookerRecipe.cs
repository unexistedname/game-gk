using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ingredient
{
    public ItemData item;
    public float amount; // Misal: 10 (ml), 2 (liter)
}

[CreateAssetMenu(fileName = "New Cooker Recipe", menuName = "Machine/Cooker Recipe")]
public class CookerRecipe : ScriptableObject
{
    [Header("Ingredients (Max 3)")]
    public List<Ingredient> ingredients;

    [Header("Result")]
    public ItemData outputItem;
    public int outputCount = 1;
}
