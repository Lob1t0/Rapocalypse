using UnityEngine;
using System.Collections;

public class BossVida : MonoBehaviour
{
    [SerializeField] private int vida = 20;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorDañado = Color.red;

    private bool vulnerable = false;
    private bool enMovimiento = false;

    private void Awake()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void TomarDaño(float cantidad)
    {
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
        Debug.Log($"💀 {gameObject.name} ha muerto.");
        Destroy(gameObject, 1.5f);
    }

    private IEnumerator EfectoDañado()
    {
        if (spriteRenderer)
        {
            spriteRenderer.color = colorDañado;
            yield return new WaitForSeconds(0.15f);
            spriteRenderer.color = colorNormal;
        }
    }

    // 🔽 llamados internos del BossController
    private void SetVulnerableInterno(bool estado)
    {
        vulnerable = estado;
    }

    private void SetMovimientoInterno(bool estado)
    {
        enMovimiento = estado;
    }
}
