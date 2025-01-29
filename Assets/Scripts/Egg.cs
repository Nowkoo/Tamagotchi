using UnityEngine;
using UnityEngine.EventSystems;

public class Egg : MonoBehaviour, IPointerDownHandler
{
    public CatStats tamagotchi;
    public RuntimeAnimatorController[] animatorControllers;

    public void OnPointerDown(PointerEventData eventData)
    {
        tamagotchi.ResetStats();
        tamagotchi.getStats().SetIsDead(false);

        // Asignar un AnimatorController aleatorio
        Animator animator = tamagotchi.GetComponent<Animator>();
        if (animator != null && animatorControllers.Length > 0)
        {
            int randomIndex = Random.Range(0, animatorControllers.Length);
            animator.runtimeAnimatorController = animatorControllers[randomIndex];

            tamagotchi.getStats().SetSelectedAnimatorControllerIndex(randomIndex);
        }

        Debug.Log("Nuevo Tamagotchi nacido.");

        tamagotchi.MoveCatInView();
        gameObject.SetActive(false);
    }

    public RuntimeAnimatorController[] GetAnimatorControllers()
    {
        return animatorControllers;
    }
}
