using UnityEngine;
using UnityEngine.SceneManagement;

public class TpIndustria : MonoBehaviour
{
    [Header("Configuración de Teletransporte")]
    [Tooltip("Nombre exacto de la escena a cargar")]
    [SerializeField] private string nombreEscenaDestino = "JefeFinallindustrial";

    [Tooltip("Tiempo de espera antes de cargar la nueva escena (en segundos)")]
    [SerializeField] private float retrasoCarga = 0.5f;

    [Header("Requerimientos")]
    [Tooltip("Etiqueta del jugador que activa el TP")]
    [SerializeField] private string tagJugador = "Player";

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Evita múltiples activaciones
        if (activado) return;

        // Verifica que el jugador sea quien entra en el TP
        if (other.CompareTag(tagJugador))
        {
            activado = true;
            Debug.Log("🔮 Jugador detectado — Teletransportando a 'JefeInfierno'...");
            Invoke(nameof(CargarEscena), retrasoCarga);
        }
    }

    private void CargarEscena()
    {
        if (!string.IsNullOrEmpty(nombreEscenaDestino))
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.LogError("⚠ No se ha asignado el nombre de la escena destino en el inspector.");
        }
    }
}

