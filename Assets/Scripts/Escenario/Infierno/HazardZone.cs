using UnityEngine;

/// <summary>
/// Script para zonas de daño estáticas (picos fijos, lava, punjas, etc).
/// Causa daño repetido mientras el jugador está en contacto.
/// Con cooldown entre daños para evitar daño excesivo.
/// </summary>
public class HazardZone : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int danoPorToque = 1;
    [SerializeField] private float cooldownEntreDanos = 0.5f;

    [Header("Visual (opcional)")]
    [SerializeField] private bool parpadearAlDañar = true;
    [SerializeField] private float duracionParpadeo = 0.2f;

    private float ultimoDanoTiempo = 0f;
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;
    }

    /// <summary>
    /// Se ejecuta mientras el jugador está dentro del trigger
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        float tiempoDesdeUltimoDano = Time.time - ultimoDanoTiempo;

        // Verificar cooldown
        if (tiempoDesdeUltimoDano >= cooldownEntreDanos)
        {
            VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
                ultimoDanoTiempo = Time.time;

                Debug.Log($"💀 Zona de daño {gameObject.name} causó {danoPorToque} daño");

                // Efecto visual
                if (parpadearAlDañar && spriteRenderer != null)
                {
                    StartCoroutine(ParpadeoAlDano());
                }
            }
        }
    }

    /// <summary>
    /// Corrutina para hacer parpadear la zona cuando daña
    /// </summary>
    private System.Collections.IEnumerator ParpadeoAlDano()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(duracionParpadeo);
            spriteRenderer.color = colorOriginal;
        }
    }

    /// <summary>
    /// Visualiza la zona de daño en el editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Bounds bounds = col.bounds;
            Gizmos.DrawCube(bounds.center, bounds.size);
        }
    }
}
