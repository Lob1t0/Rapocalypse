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

    [Header("Invulnerabilidad")]
    [SerializeField] public float tiempoInvulnerable = 1.2f;
    private bool invulnerable = false;

    void Start()
    {
        vidasActuales = vidasMax;
        ActualizarUI();
    }

    void Update()
    {
        // Detecta caída fuera del mapa
        if (transform.position.y < limiteY)
        {
            PerderVidaPorCaida();
        }
    }

    // ------------------------------
    //     SISTEMA DE DAÑO
    // ------------------------------

    public void RecibirDanioEnemigo()
    {
        if (invulnerable) return;

        vidasActuales = Mathf.Max(vidasActuales - 1, 0);
        ActualizarUI();

        if (vidasActuales <= 0)
        {
            ReiniciarEscena();
            return;
        }

        StartCoroutine(InvulnerabilidadTemporal());
    }

    private void PerderVidaPorCaida()
    {
        if (invulnerable) return;

        // Si ya no le quedan vidas, reinicia
        if (vidasActuales <= 1)
        {
            vidasActuales = 0;
            ActualizarUI();
            ReiniciarEscena();
        }
        else
        {
            vidasActuales = Mathf.Max(vidasActuales - 1, 0);
            ActualizarUI();
            transform.position = posicionRespawn;
            StartCoroutine(InvulnerabilidadTemporal());
        }
    }

    private void ReiniciarEscena()
    {
        Debug.Log("Jugador sin vidas. Reiniciando escena...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActualizarUI()
    {
        if (textoVida != null)
            textoVida.text = $"Vida: {vidasActuales}/{vidasMax}";
    }

    private System.Collections.IEnumerator InvulnerabilidadTemporal()
    {
        invulnerable = true;
        yield return new WaitForSeconds(tiempoInvulnerable);
        invulnerable = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            RecibirDanioEnemigo();
        }
    }
}
