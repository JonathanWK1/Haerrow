using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueGroup", menuName = "Game/Dialogue Group")]
public class DialogueGroup : ScriptableObject
{
    [Header("Group Info")]
    public string groupName;
    public DialogueNode startNode;
    public List<DialogueNode> allNodes;
}
[CreateAssetMenu(fileName = "DialogueNode", menuName = "Game/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("Speaker Info")]
    public string speakerNameKey;
    public Sprite speakerSprite;

    public string dialogueKey;

    [Header("Dialogue Options")]
    public List<DialogueOption> options;

    [Header("Next (auto-continue)")]
    public DialogueNode nextNode;

    [Header("On Dialogue Event")]
    public UnityEvent<string> onEvent;

    public string eventParam;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    public DialogueNode nextNode;
    [Header("On Dialogue Option Event")]
    public UnityEvent<string> onEvent;
}
