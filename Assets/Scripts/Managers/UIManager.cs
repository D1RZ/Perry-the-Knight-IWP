using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    [SerializeField] Image HealthBar;

    [SerializeField] Image YellowBar; // behind health bar

    [SerializeField] Image BlackBar; // behind yellow bar its health bar border basically

    [SerializeField] TextMeshProUGUI HealthText;

    [SerializeField] TextMeshProUGUI HealthPotionText;

    [SerializeField] private float yellowBarSpeed = 5f; // lerp speed

    private float targetYellowWidth;

    [SerializeField] private PostProcessController postProcessController;

    private bool respawnTrigger = false;

    private Coroutine lastDamageCoroutine = null;

    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if(_instance == null) Debug.Log("UIManager is null");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerHit += UpdatePlayerHealthUI;
        Enemy.OnEnemyHit += UpdateEnemyHealthUI;
    }

    private void UpdateEnemyHealthUI(Enemy enemy)
    {
        // get references
        var enemyRedBar = enemy.HealthBar.transform.GetChild(2).GetComponent<Image>();
        var enemyGreyBar = enemy.HealthBar.transform.GetChild(1).GetComponent<Image>();
        
        float targetRedWidth = enemy.healthBar1PercentWidth * (enemy.health / enemy.entityData.MaxHealth) * 100;

        // instantly set red bar
        var redSize = enemyRedBar.rectTransform.sizeDelta;
        redSize.x = targetRedWidth;
        enemyRedBar.rectTransform.sizeDelta = redSize;

        // start coroutine to lerp grey bar down
        StartCoroutine(LerpEnemyGreyBar(enemyGreyBar, targetRedWidth, 0.5f));
    }
    
    private IEnumerator LerpEnemyGreyBar(Image greyBar, float targetWidth, float speed)
    {
        if (greyBar == null) yield return null;

        yield return new WaitForSeconds(0.15f); // optional small delay for "damage lag" effect

        float currentWidth = greyBar.rectTransform.sizeDelta.x;
        while (Mathf.Abs(currentWidth - targetWidth) > 0.1f)
        {
            if (greyBar.rectTransform == null) { yield break; }
            currentWidth = Mathf.Lerp(currentWidth, targetWidth, Time.deltaTime * (speed * 10f));
            Vector2 size = greyBar.rectTransform.sizeDelta;
            size.x = currentWidth;
            greyBar.rectTransform.sizeDelta = size;
            yield return null;
        }

        // ensure final width matches target
        if (greyBar != null)
        {
            Vector2 finalSize = greyBar.rectTransform.sizeDelta;
            finalSize.x = targetWidth;
            greyBar.rectTransform.sizeDelta = finalSize;
        }
    }
    
    private void UpdatePlayerHealthUI(float currentHealth)
    {
        HealthBar.rectTransform.sizeDelta = new Vector2(1.54f * playerData.HealthData, 17);

        if (lastDamageCoroutine != null) StopCoroutine(lastDamageCoroutine);
        lastDamageCoroutine = StartCoroutine(LerpOnDamage(200));

        if (playerData.HealthData > 0) HealthText.text = playerData.HealthData + "/" + playerData.MaxHealth;
        else HealthText.text = 0 + "/" + playerData.MaxHealth;

        if(playerData.HealthData <= 40)
        {
            Debug.Log("Health Low!");
            postProcessController.EnableVignette();
        }
        else
        {
            if(postProcessController.vignetteActive) postProcessController.DisableVignette();
        }
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerHit -= UpdatePlayerHealthUI;
        Enemy.OnEnemyHit -= UpdateEnemyHealthUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerData.MaxHealth = 100;
        playerData.HealthData = 100;
        UpdatePlayerHealthUI(playerData.HealthData);
        targetYellowWidth = YellowBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (PlayerController.Instance._PlayerData.HealthData <= 0)
        {
            if(Input.GetKeyDown(KeyCode.R) && !respawnTrigger)
            {
                respawnTrigger = true;
                RoomManager.Instance.EnterRoom(CheckpointManager.Instance.GetRespawnRoom(), true);
            }
        }
        else
        {
            respawnTrigger = false;
        }
    }

    public void AnimateHealthIncrease(float newMaxHealth)
    {
        StopAllCoroutines();
        playerData.MaxHealth = newMaxHealth;
        StartCoroutine(LerpHealthBars(newMaxHealth));
    }

    public void OnPlayerHeal(float newHealth)
    {
        StopAllCoroutines();
        HealthPotionText.text = PlayerController.Instance.HealthPotionsCollected.ToString();
        StartCoroutine(LerpOnHeal(200, newHealth));
    }

    private IEnumerator LerpHealthBars(float targetHealth)
    {
        float speed = 200f; // pixels per second (tweak this)
        
        float targetGreenWidth = 1.54f * targetHealth;
        float targetYellowWidth = targetGreenWidth;
        float targetBlackWidth = 1.63f * targetHealth;
        float targetTextParentWidth = 1.54f * targetHealth;
    
        RectTransform textParent = HealthText.transform.parent.GetComponent<RectTransform>();
    
        while (true)
        {
            bool done = true;
    
            // GREEN
            float newGreen = Mathf.MoveTowards(
                HealthBar.rectTransform.sizeDelta.x,
                targetGreenWidth,
                speed * Time.deltaTime
            );
    
            // YELLOW
            float newYellow = Mathf.MoveTowards(
                YellowBar.rectTransform.sizeDelta.x,
                targetYellowWidth,
                speed * Time.deltaTime
            );
    
            // BLACK
            float newBlack = Mathf.MoveTowards(
                BlackBar.rectTransform.sizeDelta.x,
                targetBlackWidth,
                speed * Time.deltaTime
            );

            float newTextGO = Mathf.MoveTowards(
                textParent.sizeDelta.x,
                targetTextParentWidth,
                speed * Time.deltaTime
            );
    
            // Apply
            HealthBar.rectTransform.sizeDelta = new Vector2(newGreen, HealthBar.rectTransform.sizeDelta.y);
            YellowBar.rectTransform.sizeDelta = new Vector2(newYellow, YellowBar.rectTransform.sizeDelta.y);
            BlackBar.rectTransform.sizeDelta = new Vector2(newBlack, BlackBar.rectTransform.sizeDelta.y);
            textParent.sizeDelta = new Vector2(newTextGO, textParent.sizeDelta.y);
    
            // Text
            float displayedHealth = Mathf.Lerp(playerData.HealthData, targetHealth, 0.2f);
            playerData.HealthData = displayedHealth;
            HealthText.text = Mathf.RoundToInt(displayedHealth) + "/" + targetHealth;
    
            // Check completion
            if (Mathf.Abs(newGreen - targetGreenWidth) < 0.1f &&
                Mathf.Abs(newYellow - targetYellowWidth) < 0.1f &&
                Mathf.Abs(newBlack - targetBlackWidth) < 0.1f)
            {
                break;
            }
    
            yield return null;
        }
    
        // Snap final values
        playerData.HealthData = targetHealth;
    }

    private IEnumerator LerpOnDamage(float speed)
    {
        // Target widths based on new max HP
        float targetYellowWidth = 1.54f * playerData.HealthData;

        while(true)
        {
            float newYellow = Mathf.MoveTowards(
                YellowBar.rectTransform.sizeDelta.x,
                targetYellowWidth,
                speed * Time.deltaTime
            );

            YellowBar.rectTransform.sizeDelta = new Vector2(newYellow, YellowBar.rectTransform.sizeDelta.y);

            if (Mathf.Abs(newYellow - targetYellowWidth) < 0.1f)
            {
                break;
            }

            yield return null;
        }

        YellowBar.rectTransform.sizeDelta = new Vector2(targetYellowWidth, YellowBar.rectTransform.sizeDelta.y);
    }

    private IEnumerator LerpOnHeal(float speed,float targetHealth)
    {
        // Target widths based on new max HP
        float targetYellowWidth = 1.54f * targetHealth;
        float targetGreenWidth = targetYellowWidth;

        while (true)
        {
            float newGreen = Mathf.MoveTowards(
                HealthBar.rectTransform.sizeDelta.x,
                targetGreenWidth,
                speed * Time.deltaTime);

            float newYellow = Mathf.MoveTowards(
                YellowBar.rectTransform.sizeDelta.x,
                targetYellowWidth,
                speed * Time.deltaTime
            );

            HealthBar.rectTransform.sizeDelta = new Vector2(newGreen, HealthBar.rectTransform.sizeDelta.y);
            YellowBar.rectTransform.sizeDelta = new Vector2(newYellow, YellowBar.rectTransform.sizeDelta.y);

            float displayedHealth = Mathf.Lerp(playerData.HealthData, targetHealth, 0.2f);
            playerData.HealthData = displayedHealth;
            HealthText.text = Mathf.RoundToInt(displayedHealth) + "/" + playerData.MaxHealth;

            if (Mathf.Abs(newYellow - targetYellowWidth) < 0.1f)
            {
                break;
            }

            yield return null;
        }

        HealthBar.rectTransform.sizeDelta = new Vector2(targetGreenWidth, HealthBar.rectTransform.sizeDelta.y);
        YellowBar.rectTransform.sizeDelta = new Vector2(targetYellowWidth, YellowBar.rectTransform.sizeDelta.y);
        playerData.HealthData = targetHealth;
    }

    public void UpdateHealthPotion()
    {
        PlayerController.Instance.HealthPotionsCollected += 1;
        HealthPotionText.text = PlayerController.Instance.HealthPotionsCollected.ToString();
    }
    
}
