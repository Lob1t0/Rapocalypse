using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BossVida bossVida;
    [SerializeField] private BossAttackSystem attackSystem;

    [Header("Posiciones del jefe")]
    [SerializeField] private Transform posicionFondo;
    [SerializeField] private Transform posicionLateral;

    [Header("Tiempos")]
    [SerializeField] private float duracionAtaqueFondo = 3f;
    [SerializeField] private float tiempoEnLateral = 5f;

    [Header("Movimiento")]
    [SerializeField] private float velocidadTransicion = 3f;

    [Header("Visual")]
    [SerializeField] private Sprite spriteNormal;
    [SerializeField] private Sprite spriteLateral;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorVulnerable = new Color(1f, 0.7f, 0.7f);

    private bool vulnerable = false;
    private bool enMovimiento = false;
    private bool jefeMuerto = false;
    private SpriteRenderer sr;

    private Coroutine cicloAtaques;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!bossVida) bossVida = GetComponent<BossVida>();
        if (!attackSystem) attackSystem = GetComponent<BossAttackSystem>();

        if (spriteNormal) sr.sprite = spriteNormal;
        sr.color = colorNormal;

        cicloAtaques = StartCoroutine(CicloDeAtaques());
    }

    private IEnumerator CicloDeAtaques()
    {
        while (!jefeMuerto && bossVida != null)
        {
            // Fase fondo (invulnerable)
            vulnerable = false;
            attackSystem?.ActivarAtaque();
            yield return StartCoroutine(MoverA(posicionFondo.position));

            if (spriteNormal) sr.sprite = spriteNormal;
            sr.color = colorNormal;
            Debug.Log("⚔ El jefe ataca desde el fondo...");
            yield return new WaitForSeconds(duracionAtaqueFondo);

            // Fase lateral (vulnerable)
            attackSystem?.DesactivarAtaque();
            yield return StartCoroutine(MoverA(posicionLateral.position));
            vulnerable = true;

            if (spriteLateral) sr.sprite = spriteLateral;
            sr.color = colorVulnerable;
            Debug.Log("🩸 El jefe ahora es vulnerable.");

            yield return new WaitForSeconds(tiempoEnLateral);
        }
    }

    private IEnumerator MoverA(Vector3 destino)
    {
        enMovimiento = true;
        while (!jefeMuerto && Vector3.Distance(transform.position, destino) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadTransicion * Time.deltaTime);
            yield return null;
        }
        enMovimiento = false;
    }

    private void Update()
    {
        if (jefeMuerto) return;

        if (bossVida != null)
        {
            bossVida.SendMessage("SetVulnerableInterno", vulnerable, SendMessageOptions.DontRequireReceiver);
            bossVida.SendMessage("SetMovimientoInterno", enMovimiento, SendMessageOptions.DontRequireReceiver);
        }
    }

    // 🔹 Llamado por BossVida cuando muere
    public void DetenerJefeAlMorir()
    {
        jefeMuerto = true;
        StopAllCoroutines();
        attackSystem?.DesactivarAtaque();
        Debug.Log("🧊 El jefe ha muerto, movimiento detenido.");
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
