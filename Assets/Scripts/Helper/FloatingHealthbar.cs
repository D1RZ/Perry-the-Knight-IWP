using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private D_Entity EnemyData;

    float maxHealth;

    float currentHealth;

    public void UpdateHealthBar()
    {
        maxHealth = EnemyData.MaxHealth;

        if(transform.root.gameObject.tag == "Explosive")
        {
            //currentHealth = transform.root.gameObject.GetComponent<SpearEnemy>().Health;
        }
        else if(transform.root.gameObject.tag == "Flying")
        {
            //currentHealth = transform.root.gameObject.GetComponent<Enemy2>().Health;
        }

        //slider.value = currentHealth / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }

}
