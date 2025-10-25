using UnityEngine;
using UnityEngine.Tilemaps; // 👈 NECESARIO para usar Tilemap y TilemapCollider2D

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class VerticalMovingPlatform : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    [Tooltip("Distancia total que sube y baja (en unidades del mundo, no en píxeles)")]
    public float distanciaMovimiento = 2f;

    [Tooltip("Velocidad de movimiento vertical")]
    public float velocidad = 2f;

    [Tooltip("Tiempo que espera antes de cambiar de dirección")]
    public float tiempoDeEspera = 0.5f;

    private Vector3 posicionInicial;
    private Vector3 objetivo;
    private bool subiendo = true;
    private float tiempoDeEsperaActual = 0f;
    private Rigidbody2D rb;

    void Start()
    {
        posicionInicial = transform.position;
        objetivo = posicionInicial + Vector3.up * distanciaMovimiento;

        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void FixedUpdate()
    {
        if (tiempoDeEsperaActual > 0)
        {
            tiempoDeEsperaActual -= Time.fixedDeltaTime;
            return;
        }

        Vector3 nuevaPosicion = Vector3.MoveTowards(transform.position, objetivo, velocidad * Time.fixedDeltaTime);
        rb.MovePosition(nuevaPosicion);

        if (Vector3.Distance(transform.position, objetivo) < 0.05f)
        {
            subiendo = !subiendo;
            objetivo = subiendo ? posicionInicial + Vector3.up * distanciaMovimiento : posicionInicial;
            tiempoDeEsperaActual = tiempoDeEspera;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * distanciaMovimiento);
    }
}
