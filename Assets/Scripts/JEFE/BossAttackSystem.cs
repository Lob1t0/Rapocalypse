using UnityEngine;
using System.Collections;

public class BossAttackSystem : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform firePointNormal;
    [SerializeField] private Transform firePointTeledirigidoA;
    [SerializeField] private Transform firePointTeledirigidoB;
    [SerializeField] private GameObject proyejefe;
    [SerializeField] private GameObject ataqueSueloPrefab;
    [SerializeField] private Transform player;

    [Header("Ataques generales")]
    [SerializeField] private float velocidadDisparo = 6f;
    [SerializeField] private float intervaloAtaques = 2.5f;
    [SerializeField] private int rafagasPorAtaque = 2;

    private Coroutine rutinaAtaque;
    private bool activo = false;

    private void Start()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void ActivarAtaque()
    {
        if (activo) return;
        activo = true;
        rutinaAtaque = StartCoroutine(CicloAtaques());
    }

    public void DesactivarAtaque()
    {
        if (!activo) return;
        activo = false;
        if (rutinaAtaque != null)
            StopCoroutine(rutinaAtaque);
    }

    private IEnumerator CicloAtaques()
    {
        yield return new WaitForSeconds(0.8f);
        while (activo)
        {
            if (firePointNormal) DispararRecto();
            yield return new WaitForSeconds(1f);

            DispararTeledirigidoDoble();
            yield return new WaitForSeconds(2f);

            LanzarCapsulaDesdeArriba();
            yield return new WaitForSeconds(intervaloAtaques);
        }
    }

    // -------------------- ATAQUE 1 --------------------
    private void DispararRecto()
    {
        if (!proyejefe || !firePointNormal) return;
        GameObject nuevo = Instantiate(proyejefe, firePointNormal.position, firePointNormal.rotation);
        Rigidbody2D rb = nuevo.GetComponent<Rigidbody2D>();
        if (rb)
            rb.linearVelocity = firePointNormal.right * velocidadDisparo;
    }

    // -------------------- ATAQUE 2 --------------------
    private void DispararTeledirigidoDoble()
    {
        if (!proyejefe || !player) return;
        CrearProyectilTeledirigido(firePointTeledirigidoA);
        CrearProyectilTeledirigido(firePointTeledirigidoB);
    }

    private void CrearProyectilTeledirigido(Transform firePoint)
    {
        if (!firePoint) return;

        GameObject nuevo = Instantiate(proyejefe, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = nuevo.GetComponent<Rigidbody2D>();
        if (rb)
            rb.linearVelocity = (player.position - firePoint.position).normalized * (velocidadDisparo * 0.8f);

        ProyeJefe seguidor = nuevo.GetComponent<ProyeJefe>();
        if (seguidor != null)
        {
            seguidor.HabilitarSeguimiento(true);
            seguidor.SetDuracion(2f);
        }
    }

    // -------------------- ATAQUE 3 --------------------
    private void LanzarCapsulaDesdeArriba()
    {
        if (!ataqueSueloPrefab || !player) return;

        Vector3 posicionInicio = new Vector3(player.position.x, player.position.y + 8f, 0f);
        Instantiate(ataqueSueloPrefab, posicionInicio, Quaternion.identity);
        Debug.Log("☄️ Cápsula lanzada desde arriba!");
    }
}
