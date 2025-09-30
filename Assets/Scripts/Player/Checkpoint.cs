using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Checkpoint : MonoBehaviour
{
    [Header("Jugador")]
    public Transform player;

    [Header("Configuración")]
    public float limiteY = -8.43f;  // límite de caída
    private Vector3 ultimaPosicion;

    [Header("Vida del jugador")]
    public int vidasMax = 4;
    private int vidasActuales;

    [Header("UI")]
    public TextMeshProUGUI textoVida; // arrastra el TMP desde el Canvas

    void Start()
    {
        if (player != null)
            ultimaPosicion = player.position;

        vidasActuales = vidasMax;
        ActualizarUI();
    }

    void Update()
    {
        if (player == null) return;

        // Si el jugador cae por debajo del límite
        if (player.position.y < limiteY)
        {
            PerderVida();
        }
    }

    private void PerderVida()
    {
        vidasActuales--;

        if (vidasActuales > 0)
        {
            RespawnearJugador();
        }
        else
        {
            ReiniciarEscena();
        }

        ActualizarUI();
    }

    private void RespawnearJugador()
    {
        player.position = ultimaPosicion;
        Debug.Log("Jugador respawneado en: " + ultimaPosicion);
    }

    private void ReiniciarEscena()
    {
        Debug.Log("Jugador sin vidas. Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActualizarUI()
    {
        if (textoVida != null)
        {
            textoVida.text = "Vida: " + vidasActuales + "/" + vidasMax;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            ultimaPosicion = other.transform.position;
            Debug.Log("Checkpoint actualizado en: " + ultimaPosicion);
        }
    }
}
