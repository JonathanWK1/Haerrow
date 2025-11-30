using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [SerializeField] private float typeSpeed = 0.03f;
    private DialogueNode currentNode;

    private List<GameObject> spawnedOptions = new List<GameObject>();
    private Coroutine dialogueCoroutine;

    private bool isVisible = false;
    public bool canContinue = false;
    private StringTable dialogueTable;
    //private StringTable dialogueOptionTable;
    private void Awake()
    {
        dialogueTable = LocalizationSettings.StringDatabase.GetTable("DIalogue");

        // dialogueOptionTable = LocalizationSettings.StringDatabase.GetTable("DialogueOptions");
        Hide(); // start hidden
    }

    private void NextDialogue()
    {

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
        currentNode = node;
        if (node == null)
        {
            ClearOptions();
            Hide();
            return;
        }

        Show();
        speakerNameText.text = GetDialogue(node.speakerNameKey);

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }
        dialogueCoroutine = StartCoroutine(DisplayLine());

        if (speakerPortrait != null && node.speakerSprite != null)
            speakerPortrait.sprite = node.speakerSprite;


        node.onEvent?.Invoke(node.eventParam);
    }

    public void SkipDialogue()
    {
        if (currentNode == null)
            return;
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        dialogueText.text = GetDialogue(currentNode.dialogueKey);
        canContinue = true;
        optionsContainer.gameObject.SetActive(true);
        SetDialogueOption(currentNode.options);
    }
    private IEnumerator DisplayLine()
    {
        string fullText = GetDialogue(currentNode.dialogueKey);
        dialogueText.text = "";
        canContinue = false;
        optionsContainer.gameObject.SetActive(false);

        foreach (char c in fullText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        canContinue = true;
        optionsContainer.gameObject.SetActive(true);
        SetDialogueOption(currentNode.options);
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

}
