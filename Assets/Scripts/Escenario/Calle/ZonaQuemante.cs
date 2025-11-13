using UnityEngine;

public class ZonaQuemante : MonoBehaviour
{
    [Header("Ciclo de Activación")]
    [SerializeField] private float tiempoInactivo = 3f; // Tiempo seguro
    [SerializeField] private float tiempoActivo = 2f; // Tiempo peligroso
    
    [Header("Visuales")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorInactivo = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    [SerializeField] private Color colorActivo = new Color(1f, 0.2f, 0f, 1f); // Rojo/naranja
    [SerializeField] private ParticleSystem particulasFuego; // Opcional
    
    [Header("Daño")]
    [SerializeField] private string tagJugador = "Player";
    
    private bool zonaActiva = false;
    private float tiempoTranscurrido = 0f;
    private VidaJugador vidaJugador;
    private bool jugadorEnZona = false;

    private void Start()
    {
        vidaJugador = FindFirstObjectByType<VidaJugador>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        ActualizarVisuales();
    }

    private void Update()
    {
        tiempoTranscurrido += Time.deltaTime;
        
        // Cambiar entre activo e inactivo
        float tiempoLimite = zonaActiva ? tiempoActivo : tiempoInactivo;
        
        if (tiempoTranscurrido >= tiempoLimite)
        {
            zonaActiva = !zonaActiva;
            tiempoTranscurrido = 0f;
            ActualizarVisuales();
            
            // Dañar si el jugador está en la zona cuando se activa
            if (zonaActiva && jugadorEnZona && vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
                Debug.Log("🔥 Zona quemante activa - Jugador dañado");
            }
        }
    }

    private void ActualizarVisuales()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = zonaActiva ? colorActivo : colorInactivo;
        }
        
        if (particulasFuego != null)
        {
            if (zonaActiva)
                particulasFuego.Play();
            else
                particulasFuego.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            jugadorEnZona = true;
            
            // Si ya está activa, dañar inmediatamente
            if (zonaActiva && vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador) && zonaActiva && vidaJugador != null)
        {
            // Daño continuo cada segundo (ajustable)
            vidaJugador.RecibirDanioEnemigo(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            jugadorEnZona = false;
        }
    }
}
