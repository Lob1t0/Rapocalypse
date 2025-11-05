using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class AtaqueSuelo : MonoBehaviour
{
    [Header("Configuración del ataque")]
    [SerializeField] private float daño = 1f;
    [SerializeField] private float velocidadCaida = 8f;
    [SerializeField] private float tiempoVida = 8f;
    [SerializeField] private bool destruirAlImpactar = true;
    [SerializeField] private string tagJugador = "Player";

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.down * velocidadCaida;

        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            VidaJugador vidaJugador = other.GetComponent<VidaJugador>() ?? other.GetComponentInParent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
                Debug.Log("💥 Jugador dañado por ataque de suelo.");
            }

            if (destruirAlImpactar)
                Destroy(gameObject);
        }
    }
}
