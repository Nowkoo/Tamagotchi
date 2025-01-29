using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;


public class StatsDisplay : MonoBehaviour
{
    public GameObject happinessContainer;
    public GameObject hungerContainer;
    public GameObject disciplineContainer;
    public GameObject tilemapObject;
    public GameObject statsContainer;
    public TextMeshProUGUI edadText;
    public TextMeshProUGUI pesoText;
    public CatStats catStats;

    public Button btnEat;
    public Button btnLight;
    public Button btnPlay;
    public Button btnMedicine;
    public Button btnClean;
    public Button btnDiscipline;

    private bool statsVisible = false;

    public void Start()
    {
        SetStatsVisibility(false);
    }

    public void UpdateStats()
    {
        happinessContainer.GetComponent<HealthDisplay>().UpdateHealthDisplay(catStats.getStats().GetHappiness(), catStats.getStats().GetMaxHappiness());
        hungerContainer.GetComponent<HealthDisplay>().UpdateHealthDisplay(catStats.getStats().GetHunger(), catStats.getStats().GetMaxHunger());
        disciplineContainer.GetComponent<HealthDisplay>().UpdateHealthDisplay(catStats.getStats().GetDiscipline(), catStats.getStats().GetMaxDiscipline());
        edadText.text = "Age:" + catStats.getStats().GetAge().ToString() + " yrs";
        pesoText.text = "Wt:" + catStats.getStats().GetWeight().ToString("F0") + " lb";
    }

    public void ToggleStatsVisibility()
    {
        if (catStats.getStats().GetIsDead())
            return;
        statsVisible = !statsVisible;
        SetStatsVisibility(statsVisible);
        SetButtonsVisibility(!statsVisible);
    }

    private void SetStatsVisibility(bool isVisible)
    {
        tilemapObject.SetActive(isVisible);
        statsContainer.SetActive(isVisible);
        edadText.enabled = isVisible;
        pesoText.enabled = isVisible;

        if (isVisible)
        {
            UpdateStats();
        }
    }

    private void SetButtonsVisibility(bool isVisible)
    {
        btnEat.interactable = isVisible;
        btnLight.interactable = isVisible;
        btnPlay.interactable = isVisible;
        btnMedicine.interactable = isVisible;
        btnClean.interactable = isVisible;
        btnDiscipline.interactable = isVisible;
    }
}
