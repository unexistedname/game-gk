using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mixer Recipe", menuName = "Machine/Mixer Recipe")]
public class MixerRecipe : ScriptableObject
{
    [Header("Ingredients (Max 2)")]
    public List<Ingredient> ingredients; // Class Ingredient numpang punya CookerRecipe tadi (tidak perlu buat lagi)

    [Header("Result")]
    public ItemData outputItem;
    public int outputCount = 1;
}
