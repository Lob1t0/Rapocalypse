using UnityEngine;

public class EnemigoMovimiento : MonoBehaviour
{
    [Header("Puntos de Patrulla")]
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float tiempoEspera = 1f;

    [Header("Componentes")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Transform objetivoActual;
    private bool esperando = false;
    private float tiempoActualEspera = 0f;

    void Start()
    {
        // Establecer el primer destino
        objetivoActual = puntoB;

        // Obtener componentes si no fueron asignados
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (puntoA == null || puntoB == null) return;

        // Control del tiempo de espera
        if (esperando)
        {
            tiempoActualEspera -= Time.deltaTime;
            if (tiempoActualEspera <= 0)
                esperando = false;
            else
                return;
        }

        // Movimiento hacia el objetivo
        Vector2 direccion = (objetivoActual.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, objetivoActual.position, velocidad * Time.deltaTime);

        // Activar animación de caminar
        if (animator != null)
            animator.SetBool("isWalking", true);

        // ---- 🔁 CORRECCIÓN: Flip según dirección real ----
        if (spriteRenderer != null)
        {
            if (direccion.x > 0.01f)
                spriteRenderer.flipX = true;  // mira a la derecha
            else if (direccion.x < -0.01f)
                spriteRenderer.flipX = false;   // mira a la izquierda
        }
        // --------------------------------------------------

        // Si llega al destino
        if (Vector2.Distance(transform.position, objetivoActual.position) < 0.05f)
        {
            // Cambiar destino
            objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;

            // Esperar antes de continuar
            esperando = true;
            tiempoActualEspera = tiempoEspera;

            // Cambiar animación a Idle
            if (animator != null)
                animator.SetBool("isWalking", false);
        }
    }

    // Visualización en el editor
    private void OnDrawGizmosSelected()
    {
        if (puntoA != null && puntoB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(puntoA.position, puntoB.position);
            Gizmos.DrawWireSphere(puntoA.position, 0.1f);
            Gizmos.DrawWireSphere(puntoB.position, 0.1f);
        }
    }
}
