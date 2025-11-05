using UnityEngine;
using System.Collections;

public class BossVida : MonoBehaviour
{
    [SerializeField] private int vida = 20;

    [Header("Visual")]
    [SerializeField] private Sprite spriteMuerte; // sprite mostrado al morir
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorDañado = Color.red;

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

        // Detener ataques si existían
        if (attackSystem != null)
        {
            attackSystem.DesactivarAtaque();
        }

        // Cambiar sprite y color de muerte
        if (spriteMuerte)
            sr.sprite = spriteMuerte;
        sr.color = colorDañado;

        // Quitar cualquier rutina de movimiento del BossController
        BossController controller = GetComponent<BossController>();
        if (controller != null)
        {
            controller.DetenerJefeAlMorir();
        }

        // Destruir después de 5 segundos (quedará estático)
        Destroy(gameObject, 5f);
    }

    private IEnumerator EfectoDañado()
    {
        if (sr)
        {
            Color original = sr.color;
            sr.color = colorDañado;
            yield return new WaitForSeconds(0.15f);
            sr.color = colorNormal;
        }
    }

    private void SetVulnerableInterno(bool estado) => vulnerable = estado;
    private void SetMovimientoInterno(bool estado) => enMovimiento = estado;
}
