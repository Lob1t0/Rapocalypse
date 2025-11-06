using UnityEngine;

public class Proyectil : MonoBehaviour
{
    [SerializeField] private float daño = 25f; // Cambié a 25 para 4 disparos = muerte
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
        // 🧩 OPCIÓN 1: Verificar si golpea LivingEntity
        LivingEntity entidad = other.GetComponent<LivingEntity>();
        if (entidad != null)
        {
            entidad.TomarDaño(daño);
            ProjectilePool.Instance.ReturnProjectile(gameObject);
            Debug.Log($"🟢 Proyectil dañó a {other.name} ({daño} daño)");
            return;
        }

        // 🧩 OPCIÓN 2: Verificar si golpea EnemyHealth (enemigos simples)
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage((int)daño);
            ProjectilePool.Instance.ReturnProjectile(gameObject);
            Debug.Log($"🟢 Proyectil dañó a ENEMIGO {other.name} ({daño} daño)");
            return;
        }

        // 🧠 OPCIÓN 3: Verificar si golpea a un jefe (BossVida)
        BossVida boss = other.GetComponent<BossVida>();
        if (boss != null)
        {
            boss.TomarDaño(daño);
            ProjectilePool.Instance.ReturnProjectile(gameObject);
            Debug.Log($"🔥 Proyectil dañó al JEFE {other.name} ({daño} daño)");
            return;
        }

        // Si golpea cualquier otra cosa, simplemente retorna el proyectil
        ProjectilePool.Instance.ReturnProjectile(gameObject);
    }
}
