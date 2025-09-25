using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMax = 100f;
    private float vidaActual;

    private void Awake()
    {
        vidaActual = vidaMax;
    }

    public void TomarDaño(float cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log($"{name} recibió {cantidad} de daño. Vida restante: {vidaActual}");

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    protected virtual void Morir()
    {
        Debug.Log($"{name} murió.");
        gameObject.SetActive(false); // desactivar por ahora, se puede mejorar
    }

    public void RestaurarVida(float cantidad)
    {
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMax);
    }
}