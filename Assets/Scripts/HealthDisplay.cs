using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public GameObject heartPrefab;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    public void UpdateHealthDisplay(int current, int max)
    {
        // Elimina todos los corazones actuales
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Crea los corazones según la salud actual
        for (int i = 0; i < max; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);
            Image heartImage = heart.GetComponent<Image>();

            if (i < current)
            {
                heartImage.sprite = fullHeartSprite;
            }
            else
            {
                heartImage.sprite = emptyHeartSprite;
            }
        }
    }
}
