using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private BarraVidaFlotante barraVida;
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        barraVida = GetComponentInChildren<BarraVidaFlotante>();
        
        if (barraVida != null)
        {
            barraVida.SetMaxHealth(maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        if (barraVida != null)
        {
            barraVida.UpdateBar(currentHealth, maxHealth);
        }

        StartCoroutine(EfectoDano());

        Debug.Log(gameObject.name + " recibió " + damage + " de daño. Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator EfectoDano()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = colorOriginal;
        }
    }

    void Die()
    {
        isDead = true;

        if (barraVida != null)
        {
            Destroy(barraVida.gameObject);
        }

        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
