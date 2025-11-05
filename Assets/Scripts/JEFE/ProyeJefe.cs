using UnityEngine;

public class ProyeJefe : MonoBehaviour
{
    [Header("Configuración del proyectil")]
    [SerializeField] private float daño = 1f;
    [SerializeField] private float tiempoVida = 5f;
    [SerializeField] private bool destruirAlImpactar = true;
    [SerializeField] private bool seguirJugador = false;
    [SerializeField] private float fuerzaGiro = 3f;

    private Transform objetivo;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        objetivo = GameObject.FindGameObjectWithTag("Player")?.transform;
        Destroy(gameObject, tiempoVida);
    }

    private void FixedUpdate()
    {
        if (seguirJugador && objetivo != null && rb != null)
        {
            Vector2 direccion = ((Vector2)objetivo.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, direccion * rb.linearVelocity.magnitude, Time.fixedDeltaTime * fuerzaGiro);
        }
    }

    public void HabilitarSeguimiento(bool estado)
    {
        seguirJugador = estado;
    }

    public void SetDuracion(float segundos)
    {
        tiempoVida = segundos;
        Destroy(gameObject, tiempoVida);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            VidaJugador vidaJugador = other.GetComponent<VidaJugador>() ?? other.GetComponentInParent<VidaJugador>();
            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanioEnemigo(gameObject);
                Debug.Log("💥 Jugador dañado por proyectil del jefe.");
            }

            if (destruirAlImpactar)
                Destroy(gameObject);
        }
    }
}
