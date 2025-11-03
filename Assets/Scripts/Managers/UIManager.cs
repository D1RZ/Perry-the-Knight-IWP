using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerData playerData;

    [SerializeField] Image HealthBar;

    [SerializeField] Image YellowBar; // behind health bar

    [SerializeField] TextMeshProUGUI HealthText;

    [SerializeField] private float yellowBarSpeed = 5f; // lerp speed

    private float targetYellowWidth;

    [SerializeField] private PostProcessController postProcessController;
    
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

        float targetRedWidth = 0.77f * (enemy.health / enemy.entityData.MaxHealth) * 100;

        // instantly set red bar
        var redSize = enemyRedBar.rectTransform.sizeDelta;
        redSize.x = targetRedWidth;
        enemyRedBar.rectTransform.sizeDelta = redSize;

        // start coroutine to lerp grey bar down
        StartCoroutine(LerpEnemyGreyBar(enemyGreyBar, targetRedWidth, 0.5f));
    }
    
    private IEnumerator LerpEnemyGreyBar(Image greyBar, float targetWidth, float speed)
    {
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
        Vector2 finalSize = greyBar.rectTransform.sizeDelta;
        finalSize.x = targetWidth;
        greyBar.rectTransform.sizeDelta = finalSize;
    }

    private void UpdatePlayerHealthUI(float currentHealth)
    {
        HealthBar.rectTransform.sizeDelta = new Vector2(1.54f * playerData.HealthData, 17);

          // Update the yellow bar target (it will lerp in Update)
        targetYellowWidth = 1.54f * playerData.HealthData;

        if (playerData.HealthData > 0) HealthText.text = playerData.HealthData + "/" + playerData.MaxHealth;
        else HealthText.text = 0 + "/" + playerData.MaxHealth;

        if(playerData.HealthData <= 40)
        {
            Debug.Log("Health Low!");
            postProcessController.EnableVignette();
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
        playerData.HealthData = 100;
        UpdatePlayerHealthUI(playerData.HealthData);
        targetYellowWidth = YellowBar.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        // Smoothly lerp the yellow bar's width towards the target
        Vector2 yellowSize = YellowBar.rectTransform.sizeDelta;
        yellowSize.x = Mathf.Lerp(yellowSize.x, targetYellowWidth, Time.deltaTime * yellowBarSpeed);
        YellowBar.rectTransform.sizeDelta = yellowSize;
    }
}
