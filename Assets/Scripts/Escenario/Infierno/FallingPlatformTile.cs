using UnityEngine;

/// <summary>
/// Script para plataformas individuales que se caen cuando el jugador las toca.
/// Incluye fase de advertencia (parpadeo) antes de caer.
/// Asigna a cada tile o GameObject de plataforma que debe caer.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class FallingPlatformTile : MonoBehaviour
{
    [Header("Tiempos")]
    [SerializeField] private float delayAntesDeCaer = 1.0f;
    [SerializeField] private float duracionAdvertencia = 0.5f;
    [SerializeField] private float tiempoHastaRespawn = 3.0f;

    [Header("Componentes")]
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2D;
    private Color colorOriginal;

    private float tiempoActual = 0f;
    private bool activado = false;
    private bool cayendo = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();

        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;
    }

    /// <summary>
    /// Se activa cuando el jugador toca la plataforma
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !activado && !cayendo)
        {
            activado = true;
            tiempoActual = 0f;
            Debug.Log($"⚠ Plataforma {gameObject.name} activada. Caerá en {delayAntesDeCaer}s");
        }
    }

    private void Update()
    {
        if (!activado || cayendo) return;

        tiempoActual += Time.deltaTime;

        // Fase de advertencia: parpadeo rojo
        if (tiempoActual < duracionAdvertencia)
        {
            float flicker = Mathf.Sin(tiempoActual * 10f) > 0 ? 0.5f : 1f;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(
                    colorOriginal.r * flicker,
                    colorOriginal.g * flicker * 0.5f,
                    colorOriginal.b * flicker * 0.5f,
                    colorOriginal.a
                );
            }
        }

        // Caída efectiva
        if (tiempoActual >= delayAntesDeCaer)
        {
            Caer();
        }
    }

    /// <summary>
    /// Hace que la plataforma desaparezca (visualmente y por colisiones)
    /// </summary>
    private void Caer()
    {
        cayendo = true;

        // Desaparecer visualmente
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        // Desactivar colisiones
        if (collider2D != null)
            collider2D.enabled = false;

        Debug.Log($"💨 Plataforma {gameObject.name} cayó. Reaparecerá en {tiempoHastaRespawn}s");

        // Resetear después de N segundos
        Invoke(nameof(Resetear), tiempoHastaRespawn);
    }

    /// <summary>
    /// Restaura la plataforma a su estado original
    /// </summary>
    private void Resetear()
    {
        activado = false;
        cayendo = false;
        tiempoActual = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = colorOriginal;
        }

        if (collider2D != null)
            collider2D.enabled = true;

        Debug.Log($"✅ Plataforma {gameObject.name} reaparició");
    }
}
