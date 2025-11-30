using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private List<DialogueStart> dialogues = new List<DialogueStart>();
    [SerializeField] private Player player;
    private DialogueStart currentDialogueStart;
    private DialogueNode currentNode;

    private InputAction nextAction;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        nextAction = InputSystem.actions.FindAction("UI/NextStory");
        nextAction.performed += _ => SetNextDialogue();
    }

    private void Start()
    {
        StartDialogue("Intro");
    }

    // ✅ Start dialogue by group name
    public void StartDialogue(string groupName)
    {
        currentDialogueStart = dialogues.Find(g => g.dialogueStartName == groupName);
        if (currentDialogueStart == null)
        {
            Debug.LogError($"DialogueGroup '{groupName}' not found!");
            return;
        }

        currentNode = currentDialogueStart.startNode;
        if (currentNode == null)
        {
            Debug.LogError($"DialogueGroup '{groupName}' has no start node!");
            return;
        }
        player.DisabledInput(true);
        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        // send node to UI
        dialogueUI.SetDialogue(currentNode);
    }

    private void SetNextDialogue()
    {
        if (currentDialogueStart == null && currentNode == null)
        {
            return;
        }
        if (!dialogueUI.canContinue)
        {
            dialogueUI.SkipDialogue();
            return;
        }
        if (currentNode.options != null && currentNode.options.Count > 0)
        {
            return;
        }

        currentNode = currentNode.nextNode;

        ShowCurrentNode();
    }    

    public void SelectOption(DialogueOption option)
    {
        option.onEvent?.Invoke(option.eventParam);
        currentNode = option.nextNode;
        ShowCurrentNode();
    }

    public void EndDialogue()
    {
        player.DisabledInput(false);
        dialogueUI.Hide();
        currentDialogueStart = null;
        currentNode = null;
    }
}
