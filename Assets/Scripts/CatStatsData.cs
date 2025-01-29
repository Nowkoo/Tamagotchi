using UnityEngine;

[System.Serializable]
public class CatStatsData
{
    [SerializeField] private int hunger;
    [SerializeField] private int happiness;
    [SerializeField] private int discipline;
    [SerializeField] private int weight;
    [SerializeField] private int age;
    [SerializeField] private bool isSick;
    [SerializeField] private int poopAmount;
    [SerializeField] private bool egg;
    [SerializeField] private float lastTimeAged;
    [SerializeField] private bool isSleeping;
    [SerializeField] private bool lightOn;
    [SerializeField] private float sickTime;
    [SerializeField] private float hungerTime;
    [SerializeField] private float happinessTime;
    [SerializeField] private bool isDead;
    [SerializeField] private string lastCheckTime;
    [SerializeField] private int selectedAnimatorControllerIndex;

    private readonly int MAX_HUNGER = 4;
    private readonly int MAX_HAPPINESS = 4;
    private readonly int MAX_DISCIPLINE = 10;
    private readonly int MIN_WEIGHT = 5;
    private readonly int MAX_WEIGHT = 99;
    private readonly float CHECK_INTERVAL = 300f;
    private readonly int SLEEP_START_HOUR = 20;
    private readonly int SLEEP_END_HOUR = 8;

    public CatStatsData()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        hunger = 0;
        happiness = 0;
        discipline = 0;
        weight = 1;
        age = 0;
        isSick = false;
        poopAmount = 0;
        egg = false;
        lastTimeAged = 0f;
        isSleeping = false;
        lightOn = true;
        sickTime = 0f;
        hungerTime = 0f;
        happinessTime = 0f;
        isDead = true;
        lastCheckTime = System.DateTime.Now.ToString("o");
    }


    // Métodos para manejar las stats

    public void IncreaseHunger(int amount) => hunger = Mathf.Clamp(hunger + amount, 0, MAX_HUNGER);
    public void DecreaseHunger(int amount) => hunger = Mathf.Clamp(hunger - amount, 0, MAX_HUNGER);
    public int GetHunger() => hunger;
    public void SetHunger(int value) => hunger = Mathf.Clamp(value, 0, 100);

    public void IncreaseHappiness(int amount) => happiness = Mathf.Clamp(happiness + amount, 0, MAX_HAPPINESS);
    public void DecreaseHappiness(int amount) => happiness = Mathf.Clamp(happiness - amount, 0, MAX_HAPPINESS);
    public int GetHappiness() => happiness;
    public void SetHappiness(int value) => happiness = Mathf.Clamp(value, 0, 100);

    public void IncreaseDiscipline(int amount) => discipline = Mathf.Clamp(discipline + amount, 0, MAX_DISCIPLINE);
    public void DecreaseDiscipline(int amount) => discipline = Mathf.Clamp(discipline - amount, 0, MAX_DISCIPLINE);
    public int GetDiscipline() => discipline;
    public void SetDiscipline(int value) => discipline = Mathf.Clamp(value, 0, 100);

    public void IncreaseWeight(int amount) => weight = Mathf.Clamp(weight + amount, 0, MAX_WEIGHT);
    public void DecreaseWeight(int amount) => weight = Mathf.Clamp(weight - amount, 0, MAX_WEIGHT);
    public int GetWeight() => weight;
    public void SetWeight(int value) => weight = Mathf.Max(value, 1);

    public void IncreaseAge(int amount) => age = Mathf.Clamp(age + amount, 0, 99);
    public int GetAge() => age;
    public void SetAge(int value) => age = Mathf.Max(value, 0);

    public bool GetIsSick() => isSick;
    public void SetIsSick(bool value) => isSick = value;

    public void IncreasePoopAmount() => poopAmount = Mathf.Clamp(poopAmount + 1, 0, 9);
        
    public int GetPoopAmount() => poopAmount;
    public void SetPoopAmount(int value) => poopAmount = Mathf.Max(value, 0);

    public bool GetEgg() => egg;
    public void SetEgg(bool value) => egg = value;

    public float GetLastTimeAged() => lastTimeAged;
    public void SetLastTimeAged(float value) => lastTimeAged = Mathf.Max(value, 0f);

    public bool GetIsSleeping() => isSleeping;
    public void SetIsSleeping(bool value) => isSleeping = value;

    public bool GetLightOn() => lightOn;
    public void SetLightOn(bool value) => lightOn = value;

    public float GetSickTime() => sickTime;
    public void SetSickTime(float value) => sickTime = Mathf.Max(value, 0f);

    public float GetHungerTime() => hungerTime;
    public void SetHungerTime(float value) => hungerTime = Mathf.Max(value, 0f);

    public float GetHappinessTime() => happinessTime;
    public void SetHappinessTime(float value) => happinessTime = Mathf.Max(value, 0f);

    public bool GetIsDead() => isDead;
    public void SetIsDead(bool value) => isDead = value;

    public string GetLastCheckTime() => lastCheckTime;
    public void SetLastCheckTime(string value) => lastCheckTime = value;

    public int GetSelectedAnimatorControllerIndex() => selectedAnimatorControllerIndex;
    public void SetSelectedAnimatorControllerIndex(int index) => selectedAnimatorControllerIndex = index;


    // Getters para constantes (si es necesario)
    public int GetMaxHunger() => MAX_HUNGER;
    public int GetMaxHappiness() => MAX_HAPPINESS;
    public int GetMaxDiscipline() => MAX_DISCIPLINE;
    public int GetMinWeight() => MIN_WEIGHT;
    public int GetMaxWeight() => MAX_WEIGHT;
    public float GetCheckInterval() => CHECK_INTERVAL;
    public int GetSleepStartHour() => SLEEP_START_HOUR;
    public int GetSleepEndHour() => SLEEP_END_HOUR;
}
