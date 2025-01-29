using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CatStats : MonoBehaviour, IPointerDownHandler
{
    // Stats que necesitan persistencia
    private CatStatsData stats;

    // Otras stats
    private bool recentlySlept = false;
    private bool refusedLastAction = false;
    private string refusedActionType = "";
    private float overlayTransparencyValue = 0.7f;

    // Referencias externas
    public Button btnCall;
    public GameObject darkOverlay;
    public GameObject bubbles;
    public AudioClip meowSound;
    public StatsDisplay statsDisplay;
    public EatMenu eatMenu;
    public GameObject eggObject;
    public Egg eggScript;

    private SpriteRenderer darkOverlayRenderer;
    private Animator animator;
    private AudioSource audioSource;
    private BubblesManager bubblesManager;
    private PoopManager poopManager;

    public void Awake()
    {
        darkOverlayRenderer = darkOverlay.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        bubblesManager = bubbles.GetComponent<BubblesManager>();
        poopManager = GetComponent<PoopManager>();
    }

    private void Start()
    {
        Debug.Log("Iniciando Tamagotchi...");        

        if (stats.GetIsDead() && !stats.GetEgg())
        {
            eggObject.SetActive(true);
            MoveCatOutOfView();
        } else
        {
            eggObject.SetActive(false);
        }

        btnCall.interactable = false;
        GetComponent<SpriteRenderer>().enabled = true;

        if (stats.GetIsSleeping())
        {
            Sleep();
        }

        if (!stats.GetLightOn())
        {
            SetOverlayTransparency(overlayTransparencyValue);
        }

        for (int i = 0; i < stats.GetPoopAmount(); i++)
        {
            MakeDirty();
        }

        System.DateTime lastCheckTime = System.DateTime.Parse(stats.GetLastCheckTime());
        float timeSinceLastCheck = (float)(System.DateTime.Now - lastCheckTime).TotalSeconds;
        float timeToNextCheck = Mathf.Max(stats.GetCheckInterval() - timeSinceLastCheck, 0);

        InvokeRepeating(nameof(RepeatedCheckState), timeToNextCheck, stats.GetCheckInterval());
    }

    private void RepeatedCheckState()
    {
        System.DateTime currentCheckTime = System.DateTime.Now;
        CheckState(currentCheckTime);
    }

    // Acciones del jugador
    public void EatMeal()
    {
        if (IsActionBlocked()) return;
        
        if (RefuseAction("EatMeal") && stats.GetHunger() < stats.GetMaxHunger())
        {
            ActionRefused("EatMeal");
            bubblesManager.PlayBubble("sad");
            Debug.Log("El Tamagotchi rechazó la comida sin motivo.");
        }
        else if (stats.GetHunger() < stats.GetMaxHunger())
        {
            ResetRefuse();
            stats.IncreaseHunger(1);
            stats.IncreaseWeight(1);
            ResetCall();
            bubblesManager.PlayBubble("heart");
            Debug.Log($"El Tamagotchi comió una comida. Hambre: {stats.GetHunger()}, Peso: {stats.GetWeight()}");
        }
        else
        {
            bubblesManager.PlayBubble("questionmark");
            Debug.Log("El Tamagotchi ya está lleno y no necesita comer.");
        }

        eatMenu.ToggleStatsVisibility();
    }

    public void EatSnack()
    {
        if (IsActionBlocked()) return;
        ResetRefuse();

        if (stats.GetHappiness() < stats.GetMaxHappiness())
        {
            stats.IncreaseHappiness(1);
            stats.IncreaseWeight(2);
            ResetCall();
            Debug.Log($"El Tamagotchi comió un snack. Felicidad: {stats.GetHappiness()}, Peso: {stats.GetWeight()}");
            bubblesManager.PlayBubble("heart");
        }
        else
        {
            Debug.Log("La felicidad del Tamagotchi ya está al máximo.");
            bubblesManager.PlayBubble("questionmark");
        }

        eatMenu.ToggleStatsVisibility();
    }

    public void TurnLight()
    {
        stats.SetLightOn(!stats.GetLightOn());
        Debug.Log($"La luz se ha {(stats.GetLightOn() ? "encendido" : "apagado")}.");

        if (stats.GetLightOn())
        {
            SetOverlayTransparency(0f);
            if (!stats.GetIsSleeping())
                bubblesManager.PlayBubble("surprise");
        }
        else
            SetOverlayTransparency(overlayTransparencyValue);
    }

    public void Play()
    {
        if (IsActionBlocked()) return;
        ResetRefuse();

        if (stats.GetWeight() > 7)
        {
            if (RefuseAction("Play") && stats.GetHappiness() < stats.GetMaxHappiness())
            {
                ActionRefused("Play");
                Debug.Log("El Tamagotchi rechazó jugar sin motivo.");
                bubblesManager.PlayBubble("sad");
            }
            else
            {
                if (stats.GetHappiness() < stats.GetMaxHappiness())
                    stats.IncreaseHappiness(1);
                stats.DecreaseWeight(1);
                ResetCall();
                Debug.Log($"El Tamagotchi jugó. Felicidad: {stats.GetHappiness()}, Peso: {stats.GetWeight()}");
                bubblesManager.PlayBubble("heart");
            }
        }
        else
        {
            Debug.Log("El Tamagotchi no pesa lo suficiente para jugar.");
            bubblesManager.PlayBubble("questionmark");
        }
    }

    public void Medicine()
    {
        if (IsActionBlocked()) return;
        ResetRefuse();

        if (stats.GetIsSick())
        {
            if (RefuseAction("Medicine"))
            {
                ActionRefused("Medicine");
                Debug.Log("El Tamagotchi rechazó la medicina estando enfermo.");
                bubblesManager.PlayBubble("sad");
            }
            else if (Random.value > 0.5f) // Probabilidad del 50% de curarse
            {
                stats.SetIsSick(false);
                stats.SetSickTime(0f); // Reseteamos el contador de enfermedad
                ResetCall();
                Debug.Log("El Tamagotchi ha sido curado con éxito.");
                bubblesManager.SetHealed();
                bubblesManager.PlayBubble("heart");
            }
            else
            {
                Debug.Log("El Tamagotchi sigue enfermo. Intenta de nuevo.");
                bubblesManager.PlayBubble("silence");
            }
        }
        else
        {
            Debug.Log("El Tamagotchi no está enfermo y no necesita medicina.");
            bubblesManager.PlayBubble("questionmark");
        }
    }

    public void Clean()
    {
        if (stats.GetIsDead())
        {
            Debug.Log("No puedes interactuar con un Tamagotchi muerto.");
            return;
        }

        if (stats.GetPoopAmount() > 0)
        {
            stats.SetPoopAmount(0);
            Debug.Log("El Tamagotchi y su habitación ahora están limpios.");
            if (!stats.GetIsSleeping())
                bubblesManager.PlayBubble("happy");
            poopManager.ClearAllPoops();
            ResetRefuse();
        }
        else if (!stats.GetIsSleeping())
        {
            Debug.Log("El Tamagotchi ya está limpio.");
            if (stats.GetPoopAmount() == 0)
                bubblesManager.PlayBubble("questionmark");
        }
    }

    public void Discipline()
    {
        if (IsActionBlocked()) return;

        // Caso 1: llamada activa y el tamagotchi no necesita atención
        if (btnCall.interactable && !NeedsAttention())
        {
            stats.IncreaseDiscipline(1);
            Debug.Log($"El Tamagotchi ha sido disciplinado por llamar sin razón. Disciplina: {stats.GetDiscipline()}");
            btnCall.interactable = false;
            bubblesManager.PlayBubble("surprise");
            return;
        }

        // Caso 2: rechaza una acción sin razón válida
        if (refusedLastAction)
        {
            if (refusedActionType == "EatMeal" && stats.GetHunger() < stats.GetMaxHunger())
            {
                stats.IncreaseDiscipline(1);
                Debug.Log($"El Tamagotchi ha sido disciplinado por negarse a comer sin motivo válido. Disciplina: {stats.GetDiscipline()}");
                ResetCall();
                bubblesManager.PlayBubble("surprise");
            }
            else if (refusedActionType == "Play" && stats.GetHappiness() < stats.GetMaxHappiness())
            {
                stats.IncreaseDiscipline(1);
                Debug.Log($"El Tamagotchi ha sido disciplinado por negarse a jugar sin motivo válido. Disciplina: {stats.GetDiscipline()}");
                ResetCall();
                bubblesManager.PlayBubble("surprise");
            }
            else if (refusedActionType == "Medicine" && stats.GetIsSick())
            {
                stats.IncreaseDiscipline(1);
                Debug.Log($"El Tamagotchi ha sido disciplinado por negarse a tomar medicina estando enfermo. Disciplina: {stats.GetDiscipline()}");
                ResetCall();
                bubblesManager.PlayBubble("surprise");
            }
            else
            {
                Debug.Log("El Tamagotchi no fue disciplinado porque la acción no ameritaba disciplina.");
                bubblesManager.PlayBubble("questionmark");
            }

            ResetRefuse();
            return;
        }

        Debug.Log("No es momento de disciplinar al Tamagotchi.");
        bubblesManager.PlayBubble("questionmark");
    }

    // Otros
    private void ResetRefuse()
    {
        refusedLastAction = false;
        refusedActionType = "";
    }

    private void ResetCall()
    {
        if (!NeedsAttention())
            btnCall.interactable = false;
    }


    private void CheckState(System.DateTime checkTime)
    {
        stats.SetLastCheckTime(checkTime.ToString("o"));

        if (stats.GetIsDead() || CheckIfDead()) return;

        float elapsedTime = Time.time - stats.GetLastTimeAged();

        // Verificar si han pasado 24 horas (86,400 segundos) para que envejezca
        if (elapsedTime >= 86400)
        {
            stats.IncreaseAge(1);
            stats.SetLastTimeAged(Time.time);
        }

        if (CheckIfSleeping(checkTime)) return;

        int randomEvent = Random.Range(0, 5);
        switch (randomEvent)
        {
            case 0: TrySleep(); break;
            case 1: GetSick(); break;
            case 2: MakeDirty(); break;
            case 3: ReduceHunger(); break;
            case 4: ReduceHappiness(); break;
        }
        UpdateCallButton();
    }

    private bool CheckIfDead()
    {
        if (stats.GetHungerTime() >= 86400f || stats.GetHappinessTime() >= 86400f || stats.GetSickTime() >= 86400f)
        {
            if (!stats.GetEgg())
                SpawnEgg();

            Debug.Log("El Tamagotchi ha muerto por negligencia.");
            CancelInvoke(); // Detenemos los checks
            return true;
        }

        if (stats.GetAge() > 20 && Random.value < (stats.GetAge() - 20) * 0.01f)
        {
            if (!stats.GetEgg())
                SpawnEgg();

            Debug.Log("El Tamagotchi ha muerto de vejez.");
            CancelInvoke();
            return true;
        }

        return false;
    }

    private bool CheckIfSleeping(System.DateTime checkTime)
    {
        int currentHour = checkTime.Hour;
        //if (currentHour >= 20 || currentHour < 8)
        if (currentHour >= stats.GetSleepStartHour() || currentHour < stats.GetSleepEndHour())
        {
            if (!stats.GetIsSleeping())
            {
                Sleep();
            }
            return true;
        } else
        {
            if (stats.GetIsSleeping())
            {
                bubblesManager.PlayBubble("day");
                animator.Play("cat1_idle", 0, 0f);
                stats.SetIsSleeping(false);
                if (!stats.GetLightOn())
                {
                    SetOverlayTransparency(0f);
                    stats.SetLightOn(true);
                }                    
            } else
            {
                stats.SetIsSleeping(false);
                animator.Play("cat1_idle", 0, 0f);
            }            
        }

        return false;
    }

    private void Sleep()
    {
        stats.SetIsSleeping(true);
        recentlySlept = true;
        animator.Play("cat1_sleeping2");
        bubblesManager.PlayBubble("sleep");
        Debug.Log("El Tamagotchi se fue a dormir porque es de noche.");
    }

    private void TrySleep()
    {
        if (!recentlySlept)
        {
            Sleep();
        }
    }

    private void GetSick()
    {
        float chance = 0.1f;

        if (stats.GetPoopAmount() > 0)
        {
            chance += 0.05f * stats.GetPoopAmount(); // Cada caca incrementa un 5% la probabilidad
        }

        if (stats.GetAge() > 20)
        {
            chance += 0.2f;
        }

        if (stats.GetHunger() == 0)
        {
            chance += 0.2f;
        }

        if (stats.GetHappiness() == 0)
        {
            chance += 0.2f;
        }

        if (stats.GetWeight() < 5f || stats.GetWeight() > 30f)
        {
            chance += 0.15f; // Incremento del 15% si el peso es malo
        }

        if (Random.value < chance)
        {
            stats.SetIsSick(true);
            recentlySlept = false;
            bubblesManager.PlayBubble("sick");
            bubblesManager.SetSick();
            Debug.Log("El Tamagotchi se enfermó.");
        }
    }


    private void MakeDirty()
    {
        if (stats.GetPoopAmount() < 9)
            stats.IncreasePoopAmount();
        recentlySlept = false;
        poopManager.SpawnPoop();
        Debug.Log("El Tamagotchi se ensució.");
    }

    private void ReduceHunger()
    {
        if (stats.GetHunger() > 0)
        {
            stats.DecreaseHunger(1);
            recentlySlept = false;
            Debug.Log($"El hambre del Tamagotchi disminuyó. Hambre: {stats.GetHunger()}");
        }
    }

    private void ReduceHappiness()
    {
        if (stats.GetHappiness() > 0)
        {
            stats.DecreaseHappiness(1);
            recentlySlept = false;
            Debug.Log($"La felicidad del Tamagotchi disminuyó. Felicidad: {stats.GetHappiness()}");
        }
    }

    private void UpdateCallButton()
    {
        const float MAX_CALL_CHANCE = 0.1f; // 10%
        const float MIN_CALL_CHANCE = 0.05f; // 5%

        float callChance = MAX_CALL_CHANCE - ((stats.GetDiscipline() / (float)stats.GetMaxDiscipline()) * (MAX_CALL_CHANCE - MIN_CALL_CHANCE));

        // Determina si debe llamar por necesidad o sin motivo
        bool shouldCall = (stats.GetHunger() == 0 || stats.GetHappiness() == 0 || stats.GetIsSick()) || Random.value < callChance;

        btnCall.interactable = shouldCall;

        if (shouldCall)
        {
            Debug.Log("El Tamagotchi está llamando al jugador.");
            animator.Play("cat1_meow");
        }
        else
        {
            Debug.Log("El Tamagotchi no está llamando al jugador.");
        }
    }

    private bool RefuseAction(string actionType)
    {
        if (!NeedsAttention()) return false;

        const float MAX_REFUSAL_CHANCE = 0.2f; // 20%
        const float MIN_REFUSAL_CHANCE = 0.05f; // 5%

        float refusalChance = MAX_REFUSAL_CHANCE - ((stats.GetDiscipline() / (float)stats.GetMaxDiscipline()) * (MAX_REFUSAL_CHANCE - MIN_REFUSAL_CHANCE));

        if (Random.value < refusalChance)
        {
            return true;
        }

        return false;
    }

    private void ActionRefused(string actionType)
    {
        refusedLastAction = true;
        refusedActionType = actionType;
        Debug.Log($"El Tamagotchi rechazó la acción {actionType}. Disciplina: {stats.GetDiscipline()}");
    }

    private bool IsActionBlocked()
    {
        if (stats.GetIsDead())
        {
            Debug.Log("No puedes interactuar con un Tamagotchi muerto.");
            return true;
        }

        if (stats.GetIsSleeping())
        {
            Debug.Log("El Tamagotchi está durmiendo y no puede realizar esta acción.");
            return true;
        }

        return false;
    }

    private bool NeedsAttention()
    {
        return (stats.GetHunger() == 0 || stats.GetHappiness() == 0 || stats.GetIsSick());
    }

    private void SpawnEgg()
    {
        stats.SetEgg(true);
        MoveCatOutOfView();
        eggObject.SetActive(true);
        Debug.Log("Huevo generado.");
    }

    public void SetOverlayTransparency(float alpha)
    {
        if (darkOverlayRenderer != null)
        {
            Color color = darkOverlayRenderer.color;
            color.a = Mathf.Clamp01(alpha);
            darkOverlayRenderer.color = color;
        }
    }

    public void PlayMeow()
    {
        audioSource.PlayOneShot(meowSound);
    }

    public void MoveCatOutOfView()
    {
        float offset = 6f;
        Vector3 newPosition = new Vector3(this.transform.position.x + offset, this.transform.position.y, this.transform.position.z);
        this.transform.position = newPosition;
    }

    public void MoveCatInView()
    {
        Vector3 newPosition = new Vector3(0, this.transform.position.y, this.transform.position.z);
        this.transform.position = newPosition;
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(stats);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/catStats.json", json);
        Debug.Log("Datos guardados en: " + Application.persistentDataPath + "/catStats.json");
    }

    public void LoadData()
    {
        string path = Application.persistentDataPath + "/catStats.json";

        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            stats = JsonUtility.FromJson<CatStatsData>(json);
            Debug.Log("Datos cargados correctamente.");

            int selectedAnimatorControllerIndex = stats.GetSelectedAnimatorControllerIndex();
            if (animator != null && eggScript.GetAnimatorControllers().Length > 0)
            {
                animator.runtimeAnimatorController = eggScript.GetAnimatorControllers()[selectedAnimatorControllerIndex];
            }

            if (!string.IsNullOrEmpty(stats.GetLastCheckTime()) &&
            System.DateTime.TryParse(stats.GetLastCheckTime(), out System.DateTime lastCheckTime))
            {
                System.TimeSpan timeSinceLastCheck = System.DateTime.Now - lastCheckTime;

                // Calcula cuántos checks han ocurrido durante el tiempo desconectado
                int checksToSimulate = Mathf.FloorToInt((float)(timeSinceLastCheck.TotalSeconds / stats.GetCheckInterval()));
                Debug.Log($"Se deben simular {checksToSimulate} checks.");

                // Simula cada check no realizado
                for (int i = 0; i < checksToSimulate; i++)
                {
                    System.DateTime checkTime = lastCheckTime.AddSeconds(i * stats.GetCheckInterval());
                    SimulateCheck(checkTime);
                }

                // Actualizar el tiempo del último check procesado con delay
                stats.SetLastCheckTime(lastCheckTime.AddSeconds(checksToSimulate * stats.GetCheckInterval()).ToString("o"));
            }
            else
            {
                Debug.LogWarning("El campo LastCheckTime no es válido o no está disponible.");
                stats.SetLastCheckTime(System.DateTime.Now.ToString("o"));
            }
        }
        else
        {
            Debug.LogWarning("No se encontró ningún archivo de datos para cargar.");
            InitializeNewStats();
            SaveData();
        }
    }

    private void SimulateCheck(System.DateTime checkTime)
    {
        int hour = checkTime.Hour;
        bool wasSleepTime = (hour >= stats.GetSleepStartHour() && hour < stats.GetSleepEndHour());

        if (wasSleepTime)
        {
            Debug.Log($"El Tamagotchi estaba dormido a las {checkTime}. No se realiza el check.");
            return;
        }

        CheckState(checkTime);
    }

    public void ResetStats()
    {
        InitializeNewStats();
        poopManager.ClearAllPoops();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!stats.GetIsDead() && !stats.GetIsSleeping())
            animator.Play("cat1_meow");
    }

    public CatStatsData getStats()
    {
        return stats;
    }

    private void InitializeNewStats()
    {
        stats = new CatStatsData();
        stats.SetLastCheckTime(System.DateTime.Now.ToString("o"));
        stats.SetLastTimeAged(Time.time);

        SaveData();
    }
}
