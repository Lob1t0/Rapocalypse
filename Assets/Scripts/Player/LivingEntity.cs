using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] protected float vidaMax = 100f;
    protected float vidaActual;

    [Header("Animator")]
    [SerializeField] protected Animator animator;

    protected bool estaMuerto = false;

    protected virtual void Awake()
    {
        vidaActual = vidaMax;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // 🔹 Método virtual para permitir override en clases hijas (Enemy, Player, etc.)
    public virtual void TomarDaño(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        Debug.Log($"{name} recibió {cantidad} de daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    // 🔹 También lo marcamos virtual para poder personalizarlo en Enemy
    protected virtual void Morir()
    {
        estaMuerto = true;
        Debug.Log($"{name} murió.");

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            DesactivarObjeto();
        }
    }

    public void RestaurarVida(float cantidad)
    {
        if (estaMuerto) return;
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMax);
    }

    // Llamado al final de la animación "Dead"
    public void DesactivarObjeto()
    {
        gameObject.SetActive(false);
    }
}
