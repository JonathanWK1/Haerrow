using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
public class DialogueUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;   
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private Transform optionsContainer;

    [Header("Prefabs")]
    [SerializeField] private GameObject optionButtonPrefab;

    private List<GameObject> spawnedOptions = new List<GameObject>();
    private bool isVisible = false;
    private StringTable dialogueTable;
    //private StringTable dialogueOptionTable;
    private void Awake()
    {
        dialogueTable = LocalizationSettings.StringDatabase.GetTable("DIalogue");
        // dialogueOptionTable = LocalizationSettings.StringDatabase.GetTable("DialogueOptions");
        Hide(); // start hidden
    }

    private string GetDialogue(string key)
    {
        string translatedText =  dialogueTable.GetEntry(key)?.GetLocalizedString() ?? key;
        translatedText.Replace("{player}", Player.Instance.playerName);
        return translatedText;
    }

    private string GetDialogueOptions(string key)
    {
        string translatedText = dialogueTable.GetEntry(key)?.GetLocalizedString() ?? key;
        translatedText.Replace("{player}", Player.Instance.playerName);
        return translatedText;
    }
    // ✅ Call this to show a dialogue node on screen
    public void SetDialogue(DialogueNode node)
    {
        if (node == null)
        {
            ClearOptions();
            Hide();
            return;
        }

        speakerNameText.text = GetDialogue(node.speakerNameKey);
        dialogueText.text = GetDialogue(node.dialogueKey);

        if (speakerPortrait != null && node.speakerSprite != null)
            speakerPortrait.sprite = node.speakerSprite;

        SetDialogueOption(node.options);
        Show();

        node.onEvent?.Invoke(node.eventParam);
    }

    private void ClearOptions()
    {
        foreach (var obj in spawnedOptions)
            Destroy(obj);
        spawnedOptions.Clear();
    }

    public void SetDialogueOption(List<DialogueOption> options)
    {
        ClearOptions();

        if (options == null || options.Count == 0)
            return;

        foreach (DialogueOption option in options)
        {
            GameObject newButtonObj = Instantiate(optionButtonPrefab, optionsContainer);
            spawnedOptions.Add(newButtonObj);

            Button btn = newButtonObj.GetComponent<Button>();
            TextMeshProUGUI textComponent = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
                textComponent.text = GetDialogueOptions(option.textKey); // or localized text

            btn.onClick.AddListener(() =>
            {
                DialogueManager.Instance.SelectOption(option);
                ClearOptions();
            });
        }
    }
    public void Show()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(true);
        }

        isVisible = true;
    }

    // ✅ Hide (fade-out optional)
    public void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
        isVisible = false;
    }

    public bool IsVisible() => isVisible;
}
