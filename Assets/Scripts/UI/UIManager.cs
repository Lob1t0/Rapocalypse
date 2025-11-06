using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Gestor centralizado de la UI del juego.
/// Controla tanto la vida del jugador (4 corazones) como la barra de vida del jefe (0-20).
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private VidaJugador vidaJugador;
    [SerializeField] private BossVida bossVida;

    [Header("UI Jugador - Corazones")]
    [SerializeField] private Image[] corazones = new Image[4];
    [SerializeField] private Color colorCorazonActivo = Color.red;
    [SerializeField] private Color colorCorazonInactivo = Color.black;

    [Header("UI Jefe - Barra de Vida")]
    [SerializeField] private Image barraVidaJefe;
    [SerializeField] private TextMeshProUGUI textoVidaJefe;
    [SerializeField] private float velocidadAnimacionBarra = 2f;

    private int vidaJugadorAnterior;
    private float vidaJefeAnterior;
    private Coroutine animacionBarra;

    private void Start()
    {
        if (!vidaJugador)
            vidaJugador = FindFirstObjectByType<VidaJugador>();
        
        if (!bossVida)
            bossVida = FindFirstObjectByType<BossVida>();

        // Inicializar UI del jugador
        ActualizarCorazones();
        vidaJugadorAnterior = vidaJugador.GetVidaActual();

        // Inicializar UI del jefe
        ActualizarBarraVidaJefe();
        vidaJefeAnterior = ObtenerVidaJefe();
    }

    private void Update()
    {
        // Verificar cambios en la vida del jugador
        if (vidaJugador.GetVidaActual() != vidaJugadorAnterior)
        {
            vidaJugadorAnterior = vidaJugador.GetVidaActual();
            ActualizarCorazones();
        }

        // Verificar cambios en la vida del jefe
        float vidaJefeActual = ObtenerVidaJefe();
        if (vidaJefeActual != vidaJefeAnterior)
        {
            vidaJefeAnterior = vidaJefeActual;
            ActualizarBarraVidaJefe();
        }
    }

    /// <summary>
    /// Actualiza los 4 corazones según la vida del jugador.
    /// Rojo = vida activa, Negro = vida perdida.
    /// </summary>
    private void ActualizarCorazones()
    {
        int vidasActuales = vidaJugador.GetVidaActual();
        int vidasMaximas = vidaJugador.GetVidaMaxima();

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;

            // Si el índice está dentro de las vidas actuales, es rojo; si no, es negro
            if (i < vidasActuales)
            {
                corazones[i].color = colorCorazonActivo;
            }
            else
            {
                corazones[i].color = colorCorazonInactivo;
            }
        }

        Debug.Log($"💗 Corazones actualizados: {vidasActuales}/{vidasMaximas}");
    }

    /// <summary>
    /// Obtiene la vida actual del jefe mediante reflexión.
    /// </summary>
    private float ObtenerVidaJefe()
    {
        if (bossVida == null) return 0f;

        // Acceder al campo privado "vida" del script BossVida
        var campo = typeof(BossVida).GetField("vida", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (campo != null)
        {
            return (int)campo.GetValue(bossVida);
        }

        return 0f;
    }

    /// <summary>
    /// Actualiza la barra de vida del jefe con animación suave.
    /// </summary>
    private void ActualizarBarraVidaJefe()
    {
        if (bossVida == null) return;

        float vidaActual = ObtenerVidaJefe();
        float vidaMaxima = 20f; // La vida máxima del jefe es 20

        // Detener animación anterior si existe
        if (animacionBarra != null)
            StopCoroutine(animacionBarra);

        // Iniciar nueva animación
        animacionBarra = StartCoroutine(AnimarBarraVida(vidaActual / vidaMaxima));

        // Actualizar texto de vida
        if (textoVidaJefe != null)
            textoVidaJefe.text = $"{Mathf.RoundToInt(vidaActual)}/20";

        Debug.Log($"⚔ Vida del jefe: {vidaActual}/20");
    }

    /// <summary>
    /// Anima la barra de vida del jefe de forma suave.
    /// </summary>
    private IEnumerator AnimarBarraVida(float targetFillAmount)
    {
        if (barraVidaJefe == null) yield break;

        float tiempoTranscurrido = 0f;
        float valorInicial = barraVidaJefe.fillAmount;

        while (tiempoTranscurrido < 1f)
        {
            tiempoTranscurrido += Time.deltaTime * velocidadAnimacionBarra;
            barraVidaJefe.fillAmount = Mathf.Lerp(valorInicial, targetFillAmount, tiempoTranscurrido);
            yield return null;
        }

        barraVidaJefe.fillAmount = targetFillAmount;
    }

    /// <summary>
    /// Reinicia la UI cuando se reinicia la escena.
    /// Llamar esto desde VidaJugador antes de recargar la escena.
    /// </summary>
    public void ReiniciarUI()
    {
        vidaJugadorAnterior = vidaJugador.GetVidaMaxima();
        vidaJefeAnterior = 20f;
        ActualizarCorazones();
        ActualizarBarraVidaJefe();
    }
}
