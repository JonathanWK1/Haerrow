using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DialogueStart", menuName = "Game/Dialogue Start")]
public class DialogueStart : ScriptableObject
{
    [Header("Info")]
    public string dialogueStartName;
    public DialogueNode startNode;
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
    public string textKey;
    public DialogueNode nextNode;
    [Header("On Dialogue Option Event")]
    public UnityEvent<string> onEvent;
    public string eventParam;
}
