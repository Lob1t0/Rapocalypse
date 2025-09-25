using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float proyectilVelocidad = 12f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float escala = 1f;

    [Header("Control de disparo")]
    [SerializeField] private bool esJugador = true;

    private float fireCooldown = 0f;

    private void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        if (esJugador)
        {
            // Click izquierdo (Mouse0) o Fire1 (definido en Input Manager)
            if (Input.GetButtonDown("Fire1") && fireCooldown <= 0f)
            {
                Disparar();
                fireCooldown = fireRate;
            }
        }
        // Si es enemigo, el disparo se controla desde otro script (IA, timer, etc.)
    }

    public void Disparar(Vector2? direccionForzada = null)
    {
        // Dirección
        Vector2 dir;
        if (direccionForzada != null)
            dir = direccionForzada.Value.normalized;
        else
            dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Obtener proyectil del pool
        GameObject bala = ProjectilePool.Instance.GetProjectile(
            firePoint.position,
            Quaternion.identity,
            escala
        );

        // Lanzar el proyectil
        bala.GetComponent<Proyectil>().Lanzar(dir, proyectilVelocidad);
    }
}