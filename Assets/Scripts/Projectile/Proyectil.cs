using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [SerializeField] private float daño = 1f;
    [SerializeField] private float tiempoVida = 3f;

    private Vector2 direccion;
    private float velocidad;
    private float vidaTimer;

    public void Lanzar(Vector2 dir, float vel)
    {
        direccion = dir.normalized;
        velocidad = vel;
        vidaTimer = tiempoVida;
    }

    private void Update()
    {
        // Movimiento
        transform.Translate(direccion * velocidad * Time.deltaTime);

        // Tiempo de vida
        vidaTimer -= Time.deltaTime;
        if (vidaTimer <= 0f)
        {
            ProjectilePool.Instance.ReturnProjectile(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificar si el objeto golpeado tiene vida
        LivingEntity entidad = other.GetComponent<LivingEntity>();
        if (entidad != null)
        {
            entidad.TomarDaño(daño);
        }

        // Devuelve el proyectil al pool
        ProjectilePool.Instance.ReturnProjectile(gameObject);
    }
}