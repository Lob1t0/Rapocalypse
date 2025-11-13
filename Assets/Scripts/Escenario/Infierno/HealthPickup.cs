using UnityEngine;

/// <summary>
/// Script para items de vida / curables que el jugador puede recolectar.
/// Restaura vida y desaparece después de ser recolectado.
/// Incluye efecto visual de parpadeo para destacar.
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [Header("Cura")]
    [SerializeField] private int cantidadCura = 1;

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoCura;

    [Header("Visual")]
    [SerializeField] private float frecuenciaParpadeo = 2f;
    [SerializeField] private float escalaOscilacion = 0.1f;

    private bool recolectado = false;
    private SpriteRenderer spriteRenderer;
    private Vector3 posicionInicial;
    private float tiempoCreacion;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        posicionInicial = transform.position;
        tiempoCreacion = Time.time;
    }

    private void Update()
    {
        // Efecto visual: bobbing (subir/bajar ligeramente)
        float offsetY = Mathf.Sin(Time.time * frecuenciaParpadeo) * escalaOscilacion;
        transform.position = posicionInicial + Vector3.up * offsetY;

        // Parpadeo suave
        if (spriteRenderer != null)
        {
            float alpha = Mathf.Sin(Time.time * frecuenciaParpadeo) * 0.5f + 0.5f;
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
    }

    /// <summary>
    /// Se activa cuando el jugador toca el item
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !recolectado)
        {
            recolectado = true;

            // Obtener sistema de vida del jugador
            VidaJugador playerHealth = collision.GetComponent<VidaJugador>();
            if (playerHealth != null)
            {
                playerHealth.RestaurarVida(cantidadCura);
                Debug.Log($"🩹 Jugador curado {cantidadCura} punto(s). Vida: {playerHealth.GetVidaActual()}/{playerHealth.GetVidaMaxima()}");
            }
            else
            {
                Debug.LogWarning($"⚠ No se encontró VidaJugador en {collision.gameObject.name}");
            }

            // Reproducir sonido
            if (sonidoCura != null)
            {
                AudioSource.PlayClipAtPoint(sonidoCura, transform.position);
            }
            else
            {
                Debug.LogWarning("⚠ No se asignó sonido de cura");
            }

            // Destruir item
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Visualiza el item en el editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.3f);

        // Etiqueta
        Gizmos.color = Color.white;
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"🩹 Cura +{cantidadCura}");
    }
}
