using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMax = 100f;
    private float vidaActual;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    private bool estaMuerto = false;

    private void Awake()
    {
        vidaActual = vidaMax;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TomarDaño(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual -= cantidad;
        Debug.Log($"{name} recibió {cantidad} de daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    protected virtual void Morir()
    {
        estaMuerto = true;
        Debug.Log($"{name} murió.");

        // Disparar animación de muerte
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        else
        {
            // Si no tiene animator, desaparecer directo
            DesactivarObjeto();
        }
    }

    public void RestaurarVida(float cantidad)
    {
        if (estaMuerto) return;

        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMax);
    }

    // 🔹 Método llamado por un Animation Event al final del clip "Dead"
    public void DesactivarObjeto()
    {
        gameObject.SetActive(false);
    }
}
