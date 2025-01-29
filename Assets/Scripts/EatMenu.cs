using UnityEngine;
using UnityEngine.UI;


public class EatMenu : MonoBehaviour
{
    public GameObject eatMenuContainer;
    public CatStats catStats;
    public Button[] disabledButtons;
    private bool menuVisible = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        eatMenuContainer.SetActive(menuVisible);
    }

    public void ToggleStatsVisibility()
    {
        if (catStats.getStats().GetIsDead() || catStats.getStats().GetIsSleeping()) return;
        menuVisible = !menuVisible;

        eatMenuContainer.SetActive(menuVisible);

        foreach (Button button in disabledButtons)
        {
            button.interactable = !menuVisible;
        }
    }
}
