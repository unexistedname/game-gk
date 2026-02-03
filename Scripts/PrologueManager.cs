using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

[System.Serializable]
public class StoryStep
{
    [TextArea(3, 5)]
    public string dialogue;
    public string speakerName;
    public Sprite characterSprite;
    public Sprite backgroundImage;
}

public class PrologueManager : MonoBehaviour
{
    [Header("Settings")]
    public float typingSpeed = 0.05f;
    public int gameSceneIndex = 2; // Sesuai index build kamu

    [Header("UI References")]
    public Image backgroundDisplay;
    public Image characterDisplay;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public Button skipButton;

    [Header("Story Data")]
    public List<StoryStep> storySteps;

    private int currentStepIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        // [FIX 1] Reset TimeScale agar coroutine jalan (PENTING SETELAH PAUSE MENU)
        Time.timeScale = 1f;

        // [FIX 2] Cek error jika lupa isi data
        if (storySteps == null || storySteps.Count == 0)
        {
            Debug.LogError("ERROR: Story Steps KOSONG! Harap isi di Inspector.");
            return;
        }

        currentStepIndex = 0;
        UpdateUI();

        if (skipButton) skipButton.onClick.AddListener(SkipPrologue);
    }

    void Update()
    {
        // Safety check biar ga error kalau list kosong
        if (storySteps.Count == 0) return;

        // Deteksi Input (Mouse Kiri ATAU Spasi)
        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isTyping)
            {
                CompleteTyping();
            }
            else
            {
                AdvanceStory();
            }
        }
    }

    void AdvanceStory()
    {
        currentStepIndex++;

        if (currentStepIndex >= storySteps.Count)
        {
            EndPrologue();
        }
        else
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // [FIX 3] Safety check object UI
        if (dialogueText == null) { Debug.LogError("Lupa assign Dialogue Text!"); return; }

        StoryStep currentData = storySteps[currentStepIndex];

        if (nameText) nameText.text = currentData.speakerName;

        if (currentData.backgroundImage != null && backgroundDisplay != null)
            backgroundDisplay.sprite = currentData.backgroundImage;

        if (characterDisplay != null)
        {
            if (currentData.characterSprite != null)
            {
                characterDisplay.sprite = currentData.characterSprite;
                characterDisplay.enabled = true;
            }
            // Opsional: Matikan gambar jika sprite kosong (biar tidak ada kotak putih)
            // else { characterDisplay.enabled = false; }
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(currentData.dialogue));
    }

    IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in textToType.ToCharArray())
        {
            dialogueText.text += letter;
            // Ini yang bikin macet kalau TimeScale = 0
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void CompleteTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.text = storySteps[currentStepIndex].dialogue;
        isTyping = false;
    }

    public void SkipPrologue()
    {
        EndPrologue();
    }

    void EndPrologue()
    {
        SceneManager.LoadScene(2);
    }
}
