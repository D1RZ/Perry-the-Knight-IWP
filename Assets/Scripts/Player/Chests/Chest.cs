using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Loot Settings")]
    [SerializeField] private List<LootItem> lootTable;
    [SerializeField] private int minLoot = 1;
    [SerializeField] private int maxLoot = 3;
    [SerializeField] private Transform itemSpawn;

    [Header("Throw Settings")]
    [SerializeField] private float upwardForce = 5f;
    [SerializeField] private float horizontalForce = 2f;

    private bool hasBeenOpened = false;

    private Animator animator;

    private bool playerInRange = false;

    [SerializeField] private TextMeshProUGUI chestOpenText;

    private Coroutine fadeRoutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        chestOpenText.alpha = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if(!hasBeenOpened) FadeText(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (!hasBeenOpened) FadeText(false);
        }
    }

    private void Update()
    {
        if (playerInRange && !hasBeenOpened && Input.GetKeyDown(KeyCode.E))
        {
            hasBeenOpened = true;
            animator.SetTrigger("Open");
            FadeText(false);
        }
    }

    private void FadeText(bool show)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeTextRoutine(show));
    }

    private IEnumerator FadeTextRoutine(bool show)
    {
        float start = chestOpenText.alpha;
        float target = show ? 1f : 0f;
        float speed = 6f;

        while (!Mathf.Approximately(chestOpenText.alpha, target))
        {
            chestOpenText.alpha = Mathf.MoveTowards(
                chestOpenText.alpha,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }

    public void SpawnLoot()
    {
        int spawnCount = Random.Range(minLoot, maxLoot + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            LootItem loot = GetRandomLoot();

            if (loot == null) continue;

            GameObject item = Instantiate(
                loot.prefab,
                itemSpawn.position,
                Quaternion.identity
            );

            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 force = new Vector2(
                         Random.Range(-horizontalForce, horizontalForce),
                         upwardForce
                     );

                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    private LootItem GetRandomLoot()
    {
        float totalWeight = 0f;
        foreach (var loot in lootTable)
            totalWeight += loot.dropChance;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var loot in lootTable)
        {
            cumulative += loot.dropChance;
            if (roll <= cumulative)
                return loot;
        }

        return null;
    }

}
