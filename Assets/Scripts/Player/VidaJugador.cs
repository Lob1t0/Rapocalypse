using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de vida")]
    [SerializeField] public int vidasMax = 4;
    public int vidasActuales;

    [Header("Respawn")]
    [SerializeField] public float limiteY = -8.43f;
    [SerializeField] public Vector3 posicionRespawn = new Vector3(-17.39f, -3.02f, 0f);

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoVida;

    [Header("Daño simultáneo")]
    [Tooltip("Permite recibir daño de varios ataques diferentes a la vez, pero solo una vez por cada ataque.")]
    private HashSet<GameObject> ataquesQueYaDañaron = new HashSet<GameObject>();

    private void Start()
    {
        vidasActuales = vidasMax;
        ActualizarUI();
    }

    private void Update()
    {
        // Detecta si cae fuera del mapa
        if (transform.position.y < limiteY)
        {
            PerderVidaPorCaida();
        }
    }

    // ------------------------------
    //      SISTEMA DE DAÑO
    // ------------------------------

    /// <summary>
    /// Daño estándar (sin referencia de ataque)
    /// </summary>
    public void RecibirDanioEnemigo()
    {
        RecibirDanioEnemigo(null);
    }

    /// <summary>
    /// Daño con referencia al objeto atacante (para evitar daño repetido del mismo)
    /// </summary>
    public void RecibirDanioEnemigo(GameObject atacante)
    {
        // Evita que un mismo ataque cause daño repetido
        if (atacante != null)
        {
            if (ataquesQueYaDañaron.Contains(atacante))
                return; // este ataque ya dañó una vez
            ataquesQueYaDañaron.Add(atacante);
        }

        // Resta vida
        vidasActuales = Mathf.Max(vidasActuales - 1, 0);
        ActualizarUI();

        Debug.Log($"💥 Jugador recibió daño de {atacante?.name ?? "ataque desconocido"}. Vida restante: {vidasActuales}");

        // Si se quedó sin vida, reinicia escena
        if (vidasActuales <= 0)
        {
            ReiniciarEscena();
        }
    }

    // ------------------------------
    //      SISTEMA DE CAÍDA
    // ------------------------------
    private void PerderVidaPorCaida()
    {
        if (vidasActuales <= 1)
        {
            vidasActuales = 0;
            ActualizarUI();
            ReiniciarEscena();
        }
        else
        {
            vidasActuales--;
            ActualizarUI();
            transform.position = posicionRespawn;
        }
    }

    // ------------------------------
    //      FUNCIONES AUXILIARES
    // ------------------------------
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

    // Permite curarse o restaurar vida
    public void RestaurarVida()
    {
        vidasActuales = vidasMax;
        ataquesQueYaDañaron.Clear();
        ActualizarUI();
    }

    public int GetVidaActual() => vidasActuales;
    public int GetVidaMaxima() => vidasMax;
}
