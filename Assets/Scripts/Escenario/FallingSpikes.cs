using UnityEngine;

/// <summary>
/// Script para picos que caen desde el cielo cuando el jugador pasa debajo.
/// Detecta proximidad del jugador y comienza caída después de delay.
/// Causa daño al impactar con el jugador.
/// </summary>
public class FallingSpikes : MonoBehaviour
{
    [Header("Configuración de Caída")]
    [SerializeField] private float velocidadCaida = 10f;
    [SerializeField] private float delayAntesDeCaer = 0.5f;
    [SerializeField] private float alturaReaparicion = 5f;

    [Header("Detección")]
    [SerializeField] private float distanciaDeteccion = 3f;
    private Transform jugador;

    [Header("Daño")]
    [SerializeField] private int danoCausado = 1;

    private Vector3 posicionInicial;
    private bool cayendo = false;
    private bool detectado = false;
    private float tiempoDeteccion = 0f;
    private Rigidbody2D rb;
    private Collider2D col;

    private void Start()
    {
        posicionInicial = transform.position;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Buscar al jugador automáticamente
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
        else
            Debug.LogWarning($"⚠ {gameObject.name}: No se encontró el jugador con tag 'Player'");

        // Asegurar que el Rigidbody sea cinemático inicialmente
        if (rb != null)
            rb.isKinematic = true;
    }

    private void Update()
    {
        if (jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Detectar cuando el jugador está debajo (distancia horizontal cercana)
        if (distancia < distanciaDeteccion && !cayendo && !detectado)
        {
            detectado = true;
            tiempoDeteccion = 0f;
            Debug.Log($"🎯 Picos {gameObject.name} detectaron al jugador. Caerán en {delayAntesDeCaer}s");
        }

        // Esperar antes de caer (efecto de "aviso")
        if (detectado && !cayendo)
        {
            tiempoDeteccion += Time.deltaTime;
            
            // Cambiar color como aviso visual
            if (GetComponent<SpriteRenderer>() != null)
            {
                float lerp = tiempoDeteccion / delayAntesDeCaer;
                Color color = Color.Lerp(Color.white, Color.red, lerp);
                GetComponent<SpriteRenderer>().color = color;
            }

            if (tiempoDeteccion >= delayAntesDeCaer)
            {
                ComenzarCaida();
            }
        }

        // Resetear si cae demasiado
        if (cayendo && transform.position.y < posicionInicial.y - alturaReaparicion)
        {
            Resetear();
        }
    }

    /// <summary>
    /// Inicia la caída de los picos
    /// </summary>
    private void ComenzarCaida()
    {
        cayendo = true;
        Debug.Log($"⬇️ Picos {gameObject.name} cayendo");

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.down * velocidadCaida;
            rb.gravityScale = 0; // Control manual de velocidad
        }
    }

    /// <summary>
    /// Detecta colisión con el jugador y causa daño
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && cayendo)
        {
            Debug.Log($"💥 Picos {gameObject.name} impactaron al jugador");

            // Causar daño
            VidaJugador vidaJugador = collision.GetComponent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
            }
            else
            {
                Debug.LogWarning($"⚠ No se encontró VidaJugador en {collision.gameObject.name}");
            }

            // Resetear inmediatamente después de golpear
            Resetear();
        }
    }

    /// <summary>
    /// Restaura los picos a su posición y estado original
    /// </summary>
    private void Resetear()
    {
        cayendo = false;
        detectado = false;
        tiempoDeteccion = 0f;

        transform.position = posicionInicial;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        Debug.Log($"✅ Picos {gameObject.name} reaparecieron");
    }

    /// <summary>
    /// Visualiza la zona de detección en el editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * alturaReaparicion);
    }
}
