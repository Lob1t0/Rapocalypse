using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Sistema de vida del jugador mejorado para integrarse con UIManager.
/// Mantiene compatibilidad con el sistema anterior de texto.
/// </summary>
public class VidaJugador : MonoBehaviour
{
    [Header("Configuración de vida")]
    [SerializeField] public int vidasMax = 4;
    public int vidasActuales;

    [Header("Respawn")]
    [SerializeField] public float limiteY = -8.43f;
    [SerializeField] public Vector3 posicionRespawn = new Vector3(-17.39f, -3.02f, 0f);

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoVida; // Para compatibilidad si lo deseas
    [SerializeField] private UIManager uiManager; // Nueva referencia al gestor de UI

    [Header("Daño simultáneo")]
    [Tooltip("Permite recibir daño de varios ataques diferentes a la vez, pero solo una vez por cada ataque.")]
    private HashSet<GameObject> ataquesQueYaDañaron = new HashSet<GameObject>();

    private bool muerto = false;

    private void Start()
    {
        vidasActuales = vidasMax;
        
        // Buscar UIManager si no está asignado
        if (!uiManager)
            uiManager = FindFirstObjectByType<UIManager>();

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

    // ==============================
    // SISTEMA DE DAÑO
    // ==============================

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
        if (muerto) return;

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
            MorirJugador();
        }
    }

    // ==============================
    // SISTEMA DE CAÍDA
    // ==============================

    private void PerderVidaPorCaida()
    {
        if (muerto) return;

        if (vidasActuales <= 1)
        {
            vidasActuales = 0;
            ActualizarUI();
            MorirJugador();
        }
        else
        {
            vidasActuales--;
            ActualizarUI();
            transform.position = posicionRespawn;

            // Limpiar ataques después de respawn
            ataquesQueYaDañaron.Clear();

            Debug.Log($"⬇️ El jugador cayó. Vidas restantes: {vidasActuales}");
        }
    }

    // ==============================
    // FUNCIONES AUXILIARES
    // ==============================

    private void MorirJugador()
    {
        if (muerto) return;
        
        muerto = true;
        Debug.Log("☠️ Jugador sin vidas. Reiniciando escena...");
        
        // Pequeño delay para que se vea la animación final
        Invoke(nameof(ReiniciarEscena), 0.5f);
    }

    private void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActualizarUI()
    {
        // Mantener compatibilidad con texto si existe
        if (textoVida != null)
            textoVida.text = $"Vida: {vidasActuales}/{vidasMax}";

        // El UIManager se encargará de actualizar los corazones automáticamente en su Update()
    }

    // ==============================
    // GETTERS Y SETTERS
    // ==============================

    public int GetVidaActual() => vidasActuales;
    public int GetVidaMaxima() => vidasMax;

    /// <summary>
    /// Permite curarse o restaurar vida (útil para power-ups)
    /// </summary>
    public void RestaurarVida()
    {
        vidasActuales = vidasMax;
        ataquesQueYaDañaron.Clear();
        muerto = false;
        ActualizarUI();
    }

    /// <summary>
    /// Restaura una cantidad específica de vida
    /// </summary>
    public void RestaurarVida(int cantidad)
    {
        vidasActuales = Mathf.Min(vidasActuales + cantidad, vidasMax);
        ActualizarUI();
    }

    /// <summary>
    /// Limpia el registro de ataques (útil después de cambiar de escena o fase)
    /// </summary>
    public void LimpiarAtaques()
    {
        ataquesQueYaDañaron.Clear();
    }
}