using UnityEngine;
using System.Collections;

public class CatAnimationManager : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private CatStats catStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        catStats = GetComponent<CatStats>();

        while(true)
        {
            yield return new WaitForSeconds(10);

            if (Random.Range(0f, 1f) > 0.50f)
            {
                if (!catStats.getStats().GetIsSleeping())
                    spriteRenderer.flipX = !spriteRenderer.flipX;
            }

            anim.SetInteger("RandomIndex", Random.Range(0, 5));
            anim.SetTrigger("Random");
        }
    }

    void FlipSprite()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}
