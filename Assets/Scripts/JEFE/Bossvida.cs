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
    /// Recibe daño cuando es vulnerable y no está en movimiento.
    /// El UIManager se encargará de mostrar la barra actualizada automáticamente.
    /// </summary>
    public virtual void TomarDaño(float cantidad)
    {
        if (muerto) return;
        if (!vulnerable || enMovimiento) return;

        vida -= Mathf.RoundToInt(cantidad);

        Debug.Log($"{gameObject.name} recibió {cantidad} de daño. Vida restante: {vida}");

        // Iniciar efecto visual de daño
        StartCoroutine(EfectoDañado());

        // Verificar si murió
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

        // Detener movimiento físico
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Detener ataques
        if (attackSystem != null)
        {
            attackSystem.DesactivarAtaque();
        }

        // Cambiar sprite y color de muerte
        if (spriteMuerte)
            sr.sprite = spriteMuerte;
        sr.color = colorDañado;

        // Detener movimiento del controlador
        BossController controller = GetComponent<BossController>();
        if (controller != null)
        {
            controller.DetenerJefeAlMorir();
        }

        // Destruir después de 5 segundos (quedará estático)
        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// Efecto visual de flash rojo cuando recibe daño.
    /// </summary>
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

    /// <summary>
    /// Método interno para establecer si es vulnerable.
    /// Llamado desde BossController.
    /// </summary>
    private void SetVulnerableInterno(bool estado) => vulnerable = estado;

    /// <summary>
    /// Método interno para establecer si está en movimiento.
    /// Llamado desde BossController.
    /// </summary>
    private void SetMovimientoInterno(bool estado) => enMovimiento = estado;

    // ==============================
    // GETTERS (para UIManager)
    // ==============================

    public int GetVida() => vida;
    public int GetVidaMaxima() => 20;
    public bool EsVulnerable() => vulnerable;
    public bool EstaEnMovimiento() => enMovimiento;
    public bool EstaMuerto() => muerto;

    /// <summary>
    /// Restaura la vida del jefe (útil para pruebas o power-ups del enemigo).
    /// </summary>
    public void RestaurarVida(int cantidad)
    {
        if (muerto) return;
        vida = Mathf.Min(vida + cantidad, 20);
        Debug.Log($"{gameObject.name} restauró {cantidad} de vida. Vida actual: {vida}");
    }

    /// <summary>
    /// Restaura toda la vida del jefe.
    /// </summary>
    public void RestaurarVidaCompleta()
    {
        if (muerto) return;
        vida = 20;
        muerto = false;
        sr.color = colorNormal;
        Debug.Log($"{gameObject.name} vida restaurada completamente.");
    }
}