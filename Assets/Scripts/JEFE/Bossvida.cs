using UnityEngine;
using System.Collections;

/// <summary>
/// Sistema de vida del jefe mejorado para integrarse con UIManager.
/// Mantiene compatibilidad con el sistema anterior.
/// </summary>
public class BossVida : MonoBehaviour
{
    [SerializeField] private int vida = 20;

    [Header("Visual")]
    [SerializeField] private Sprite spriteMuerte; // sprite mostrado al morir
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorDañado = Color.red;

    [Header("Efecto de daño")]
    [SerializeField] private float duracionFlashDaño = 0.15f;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoMuerte;
    [SerializeField] private float volumenMuerte = 1.5f;
    [SerializeField] private float rango3D = 500f;        // ← ALCANCE SUPER AMPLIO

    private bool vulnerable = false;
    private bool enMovimiento = false;
    private bool muerto = false;

    private SpriteRenderer sr;
    private Sprite spriteInicial;
    private Rigidbody2D rb;
    private BossAttackSystem attackSystem;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        attackSystem = GetComponent<BossAttackSystem>();
        spriteInicial = sr.sprite;
        sr.color = colorNormal;
    }

    /// <summary>
    /// Recibe daño cuando es vulnerable.
    /// </summary>
    public virtual void TomarDaño(float cantidad)
    {
        if (muerto) return;
        if (!vulnerable || enMovimiento) return;

        vida -= Mathf.RoundToInt(cantidad);

        Debug.Log($"{gameObject.name} recibió {cantidad} de daño. Vida restante: {vida}");

        StartCoroutine(EfectoDañado());

        if (vida <= 0)
        {
            Muerte();
        }
    }

    /// <summary>
    /// Lógica de muerte del jefe.
    /// </summary>
    protected virtual void Muerte()
    {
        if (muerto) return;

        muerto = true;
        Debug.Log($"💀 {gameObject.name} ha muerto.");
        SceneManager.LoadScene("industrial");

        ReproducirSonidoMuerte3D();  // ← NUEVO SISTEMA 3D

        // Detener movimiento físico
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Detener ataques
        if (attackSystem != null)
            attackSystem.DesactivarAtaque();

        // Cambiar sprite/colores
        if (spriteMuerte)
            sr.sprite = spriteMuerte;

        sr.color = colorDañado;

        // Detener AI del boss
        BossController controller = GetComponent<BossController>();
        if (controller != null)
            controller.DetenerJefeAlMorir();

        // Destruir luego
        Destroy(gameObject, 5f);
    }

    /// 🔊 SONIDO DE MUERTE 3D - SUPER AMPLIO
    private void ReproducirSonidoMuerte3D()
    {
        if (sonidoMuerte == null) return;

        // Crear fuente de audio temporal en la escena
        GameObject go = new GameObject("BossDeathSound");
        go.transform.position = transform.position;

        AudioSource a = go.AddComponent<AudioSource>();
        a.clip = sonidoMuerte;
        a.volume = volumenMuerte;

        a.spatialBlend = 1f;           // 100% 3D
        a.minDistance = 3f;
        a.maxDistance = rango3D;        // ← SUPER RANGE: 500f
        a.rolloffMode = AudioRolloffMode.Linear;

        a.Play();

        // destruir cuando acabe
        Destroy(go, sonidoMuerte.length + 0.5f);
    }

    private IEnumerator EfectoDañado()
    {
        if (sr)
        {
            Color original = sr.color;
            sr.color = colorDañado;
            yield return new WaitForSeconds(duracionFlashDaño);
            sr.color = original;
        }
    }

    private void SetVulnerableInterno(bool estado) => vulnerable = estado;
    private void SetMovimientoInterno(bool estado) => enMovimiento = estado;

    // ----- GETTERS -----
    public int GetVida() => vida;
    public int GetVidaMaxima() => 20;
    public bool EsVulnerable() => vulnerable;
    public bool EstaEnMovimiento() => enMovimiento;
    public bool EstaMuerto() => muerto;

    public void RestaurarVida(int cantidad)
    {
        if (muerto) return;
        vida = Mathf.Min(vida + cantidad, 20);
        Debug.Log($"{gameObject.name} restauró {cantidad} de vida. Vida actual: {vida}");
    }

    public void RestaurarVidaCompleta()
    {
        if (muerto) return;
        vida = 20;
        muerto = false;
        sr.color = colorNormal;
        Debug.Log($"{gameObject.name} vida restaurada completamente.");
    }
}
