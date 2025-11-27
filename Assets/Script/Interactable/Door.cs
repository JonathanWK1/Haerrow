using UnityEngine;

public class Door : Interactable
{

    [SerializeField] private Sprite openDoorSprite = null;
    [SerializeField] private Sprite doorSprite = null;

    private SpriteRenderer sr;
    private Collider2D collider2d;
    protected override void Awake()
    {
        base.Awake();
        sr = GetComponent<SpriteRenderer>();
        collider2d = GetComponent<Collider2D>();
    }
    public override void Interact()
    {
        base.Interact();
        sr.sprite = openDoorSprite;
        collider2d.enabled = false;
        base.SetIsInteractable(false);
    }
}
