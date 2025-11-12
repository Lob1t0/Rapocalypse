using System.Collections;
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
    [SerializeField] private UIManager uiManager;

    private HashSet<GameObject> ataquesQueYaDañaron = new HashSet<GameObject>();

    private bool muerto = false;
    private Animator animator;

    private void Start()
    {
        vidasActuales = vidasMax;

        if (!uiManager)
            uiManager = FindFirstObjectByType<UIManager>();

        animator = GetComponent<Animator>();

        ActualizarUI();
    }

    private void Update()
    {
        if (muerto) return;

        if (transform.position.y < limiteY)
            PerderVidaPorCaida();
    }

    // ==============================
    // DAÑO
    // ==============================

    public void RecibirDanioEnemigo() => RecibirDanioEnemigo(null);

    public void RecibirDanioEnemigo(GameObject atacante)
    {
        if (muerto) return;

        vidasActuales = Mathf.Max(vidasActuales - 1, 0);
        ActualizarUI();

        // 🔥 ANIMACIÓN DE DAÑO (SOLO SI LE QUEDAN VIDAS)
        if (vidasActuales > 0)
        {
            if (animator)
                animator.SetTrigger("Daño");

            // 🔥 Volver a Idle después de EXACTAMENTE 1 segundo
            StartCoroutine(VolverIdleDespuesDeDanio());
            return;
        }

        // 🔥 SI LLEGÓ A 0 → MUERTE
        MorirJugador();
    }

    private IEnumerator VolverIdleDespuesDeDanio()
    {
        yield return new WaitForSeconds(0.3f);

        // 🔥 VOLVER A IDLE
        if (!muerto && animator)
            animator.Play("Idle");
    }

    // ==============================
    // CAÍDA
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
            ataquesQueYaDañaron.Clear();
        }
    }

    // ==============================
    // MUERTE FINAL
    // ==============================

    private void MorirJugador()
    {
        if (muerto) return;
        muerto = true;

        Debug.Log("☠️ Jugador murió");

        if (animator)
            animator.SetBool("Muerte", true);

        // 🔥 Desactivar scripts del jugador
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var s in scripts)
            if (s != this) s.enabled = false;

        // 🔥 Congelar físicas
        if (TryGetComponent<Rigidbody2D>(out var rb))
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (TryGetComponent<CharacterController>(out var cc))
            cc.enabled = false;

        StartCoroutine(AnimacionMuerteYCongelar());

        // 🔥 Reiniciar escena en EXACTAMENTE 4 segundos
        Invoke(nameof(ReiniciarEscena), 4f);
    }

    private IEnumerator AnimacionMuerteYCongelar()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Muerte"))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        animator.enabled = false; // Congela el sprite
    }

    private void ReiniciarEscena()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ActualizarUI()
    {
        if (textoVida != null)
            textoVida.text = $"Vida: {vidasActuales}/{vidasMax}";
    }

    // ==============================
    // CURACIÓN
    // ==============================

    public int GetVidaActual() => vidasActuales;
    public int GetVidaMaxima() => vidasMax;

    public void RestaurarVida()
    {
        vidasActuales = vidasMax;
        ataquesQueYaDañaron.Clear();
        muerto = false;

        if (animator)
            animator.SetBool("Muerte", false);

        ActualizarUI();
    }

    public void RestaurarVida(int cantidad)
    {
        vidasActuales = Mathf.Min(vidasActuales + cantidad, vidasMax);
        ActualizarUI();
    }

    public void LimpiarAtaques() => ataquesQueYaDañaron.Clear();
}
