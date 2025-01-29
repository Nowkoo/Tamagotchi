using UnityEngine;

public class BubblesManager : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    private void TurnOffBubbles()
    {
        spriteRenderer.enabled = false;
    }

    private void TurnOnBubbles()
    {
        spriteRenderer.enabled = true;
    }

    public void PlayBubble(string bubbleName)
    {
        spriteRenderer.enabled = true;
        animator.Play(bubbleName);
    }

    public void SetSick()
    {
        animator.SetBool("isSick", true);
    }

    public void SetHealed()
    {
        animator.SetBool("isSick", false);
    }
}
