using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Sonido de muerte (arrastrar WAV/MP3)")]
    [SerializeField] private AudioClip sonidoMuerte;

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
        if (isDead) return;
        isDead = true;

        // 🔊 REPRODUCIR SONIDO (garantizado)
        if (sonidoMuerte != null)
        {
            AudioSource.PlayClipAtPoint(sonidoMuerte, transform.position, 10f);
        }

        // eliminar barra
        if (barraVida != null)
        {
            Destroy(barraVida.gameObject);
        }

        // destruir enemigo
        Destroy(gameObject, 0.2f);
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
