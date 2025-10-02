using UnityEngine;

public class EnemigoPatrulla : MonoBehaviour
{
    [Header("Puntos de patrulla")]
    public Transform puntoA;
    public Transform puntoB;

    [Header("Configuración")]
    public float velocidad = 2f;

    [Header("Animator")]
    public Animator animator;

    private Transform objetivoActual;
    private bool mirandoIzquierda = true; // 👈 ahora decimos que por defecto el sprite mira a la izquierda

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        objetivoActual = puntoB;
    }

    void Update()
    {
        if (objetivoActual == null) return;

        // Mover hacia el objetivo
        transform.position = Vector2.MoveTowards(transform.position, objetivoActual.position, velocidad * Time.deltaTime);

        // Activar animación walk mientras se mueve
        bool estaCaminando = Vector2.Distance(transform.position, objetivoActual.position) > 0.1f;
        animator.SetBool("isWalking", estaCaminando);

        // Cambiar al otro punto al llegar
        if (!estaCaminando)
        {
            objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
        }

        // Flip según dirección
        if (objetivoActual != null)
        {
            if (objetivoActual.position.x > transform.position.x && mirandoIzquierda)
            {
                Girar();
            }
            else if (objetivoActual.position.x < transform.position.x && !mirandoIzquierda)
            {
                Girar();
            }
        }
    }

    private void Girar()
    {
        mirandoIzquierda = !mirandoIzquierda;
        Vector3 escala = transform.localScale;
        escala.x *= -1; // invertimos el sprite
        transform.localScale = escala;
    }
}

