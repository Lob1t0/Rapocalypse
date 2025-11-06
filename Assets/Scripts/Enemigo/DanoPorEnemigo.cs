using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DañoPorEnemigo : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform player;

    [Header("Configuración de daño")]
    [Tooltip("Distancia mínima para que el jugador reciba daño del enemigo")]
    [SerializeField] private float distanciaDaño = 1.2f;
    [Tooltip("Tiempo de invulnerabilidad después de recibir daño (en segundos)")]
    [SerializeField] private float tiempoInvulnerable = 1.0f;
    private float temporizadorInvulnerable = 0f;

    [Header("Vida del jugador")]
    [SerializeField] public int vidasMax = 4;
    public int vidasActuales;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoVida;

    [Header("Configuración enemigos")]
    [Tooltip("Etiqueta que deben tener los enemigos")]
    [SerializeField] private string tagEnemigo = "Enemigo";

    void Start()
    {
        vidasActuales = vidasMax;
        ActualizarUI();
    }

    void Update()
    {
        if (player == null) return;

        // Contador de invulnerabilidad
        if (temporizadorInvulnerable > 0f)
            temporizadorInvulnerable -= Time.deltaTime;

        // Buscar todos los enemigos
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag(tagEnemigo);

        foreach (GameObject enemigo in enemigos)
        {
            float distancia = Vector2.Distance(player.position, enemigo.transform.position);

            // Si el jugador está lo suficientemente cerca del enemigo
            if (distancia <= distanciaDaño && temporizadorInvulnerable <= 0f)
            {
                RecibirDaño();
                break; // evita recibir daño de múltiples enemigos al mismo frame
            }
        }
    }

    private void RecibirDaño()
    {
        vidasActuales--;
        temporizadorInvulnerable = tiempoInvulnerable;
        ActualizarUI();

        Debug.Log($"Jugador recibió daño de enemigo. Vidas restantes: {vidasActuales}/{vidasMax}");

        if (vidasActuales <= 0)
        {
            ReiniciarEscena();
        }
    }

    private void ReiniciarEscena()
    {
        Debug.Log("☠️ Jugador sin vidas. Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActualizarUI()
    {
        if (textoVida != null)
            textoVida.text = $"Vida: {vidasActuales}/{vidasMax}";
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja la distancia de daño como círculo de referencia
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player != null ? player.position : transform.position, distanciaDaño);
    }
}
