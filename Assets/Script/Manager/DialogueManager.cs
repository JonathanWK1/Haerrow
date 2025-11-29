using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private List<DialogueGroup> dialogueGroups = new List<DialogueGroup>();

    private DialogueGroup currentGroup;
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
        currentGroup = dialogueGroups.Find(g => g.groupName == groupName);
        if (currentGroup == null)
        {
            Debug.LogError($"DialogueGroup '{groupName}' not found!");
            return;
        }

        currentNode = currentGroup.startNode;
        if (currentNode == null)
        {
            Debug.LogError($"DialogueGroup '{groupName}' has no start node!");
            return;
        }

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
        dialogueUI.Hide();
        currentGroup = null;
        currentNode = null;
    }
}
