using UnityEngine;
using TMPro;

public class VidaJugador : MonoBehaviour
{
    [Header("Vida del jugador")]
    [SerializeField] private int vidaMax = 4; // Máximo 4 golpes
    private int vidaActual;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoVida;

    private void Start()
    {
        vidaActual = vidaMax;
        ActualizarUI();
    }

    public void RecibirDaño(int daño)
    {
        vidaActual -= daño;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMax);

        ActualizarUI();

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void ActualizarUI()
    {
        textoVida.text = "Vida: " + vidaActual + "/" + vidaMax;
    }

    private void Morir()
    {
        Debug.Log("El jugador murió.");
        // Aquí puedes poner animación de muerte, reinicio de nivel, etc.
    }
}
