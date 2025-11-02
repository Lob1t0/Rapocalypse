using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BossVida bossVida; // Script de vida del jefe

    [Header("Posiciones del jefe")]
    [SerializeField] private Transform posicionFondo;   // Donde inicia o ataca desde el fondo
    [SerializeField] private Transform posicionLateral; // Donde es vulnerable

    [Header("Tiempos")]
    [SerializeField] private float duracionAtaqueFondo = 3f;   // Tiempo atacando en el fondo
    [SerializeField] private float tiempoEnLateral = 5f;       // Tiempo vulnerable

    [Header("Movimiento")]
    [SerializeField] private float velocidadTransicion = 3f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorVulnerable = new Color(1f, 0.7f, 0.7f);

    private bool vulnerable = false;
    private bool enMovimiento = false;

    private void Start()
    {
        if (!sprite) sprite = GetComponent<SpriteRenderer>();
        if (!bossVida) bossVida = GetComponent<BossVida>();

        // 🔄 Inicializamos el ciclo del jefe
        StartCoroutine(CicloDeAtaques());
    }

    private IEnumerator CicloDeAtaques()
    {
        while (bossVida != null) // Mientras siga con vida
        {
            // 1️⃣ Fase en el fondo (atacando, invulnerable)
            vulnerable = false;
            bossVida.enabled = true; // sigue activo, pero no se le puede dañar
            yield return StartCoroutine(MoverA(posicionFondo.position));
            sprite.color = colorNormal;

            Debug.Log("⚔ El jefe ataca desde el fondo...");
            yield return new WaitForSeconds(duracionAtaqueFondo);

            // 2️⃣ Fase vulnerable (se mueve a lateral y puede recibir daño)
            yield return StartCoroutine(MoverA(posicionLateral.position));
            vulnerable = true;
            sprite.color = colorVulnerable;

            Debug.Log("🩸 El jefe ahora es vulnerable.");
            yield return new WaitForSeconds(tiempoEnLateral);
        }

        // Si se destruye, salir del ciclo
        yield break;
    }

    private IEnumerator MoverA(Vector3 destino)
    {
        enMovimiento = true;
        while (Vector3.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadTransicion * Time.deltaTime);
            yield return null;
        }
        enMovimiento = false;
    }

    private void Update()
    {
        // 👇 Actualiza el estado del jefe en su script de vida (para que no reciba daño si se mueve)
        if (bossVida != null)
        {
            bossVida.enabled = true;
            // el daño solo aplica si está vulnerable y no en movimiento
            bossVida.SendMessage("SetVulnerableInterno", vulnerable, SendMessageOptions.DontRequireReceiver);
            bossVida.SendMessage("SetMovimientoInterno", enMovimiento, SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnDrawGizmos()
    {
        if (posicionFondo)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(posicionFondo.position, 0.2f);
        }
        if (posicionLateral)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(posicionLateral.position, 0.2f);
        }
    }
}
