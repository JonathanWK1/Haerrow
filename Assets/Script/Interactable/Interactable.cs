using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public UnityEvent onInteract;

    [Header("Highlight Settings")]
    public Color highlightColor = Color.yellow;
    [Range(0f, 1f)] public float highlightStrength = 0.2f;
    public bool IsInteractable{ get; private set; }

    private SpriteRenderer spriteRenderer;

    private MaterialPropertyBlock propBlock;
    private bool isHighlighted = false;
    protected virtual void Awake()
    {
        IsInteractable = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public virtual void Interact()
    {
        Debug.Log("Lets go");
        onInteract?.Invoke();
    }

    public virtual void SetIsInteractable(bool isInteractable)
    {
        IsInteractable = isInteractable;
        if (!IsInteractable)
        {
            SetHighlight(false);
        }
    }

    public void SetHighlight(bool enable)
    {
        if (isHighlighted == enable) return;
        isHighlighted = enable;

        spriteRenderer.GetPropertyBlock(propBlock);

        if (enable)
        {
            propBlock.SetColor("_HighlightColor", highlightColor);
            propBlock.SetFloat("_HighlightStrength", highlightStrength);
        }
        else
        {
            propBlock.SetFloat("_HighlightStrength", 0f);
        }

        spriteRenderer.SetPropertyBlock(propBlock);
    }
}
