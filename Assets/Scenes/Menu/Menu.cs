using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Botones del Menú")]
    [SerializeField] private Button botonJugar;
    [SerializeField] private Button botonControles;
    [SerializeField] private Button botonSalir;

    [Header("Configuración Visual")]
    [SerializeField] private Color colorSeleccionado = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private float cambioRetardo = 0.25f;

    private int indiceActual = 0;
    private Button[] botones;
    private float tiempoUltimoCambio;

    private void Start()
    {
        botones = new Button[] { botonJugar, botonControles, botonSalir };
        ActualizarSeleccionVisual();
    }

    private void Update()
    {
        if (Keyboard.current == null && Gamepad.current == null) return;

        bool moverAbajo = false;
        bool moverArriba = false;

        // --- TECLADO ---
        if (Keyboard.current != null)
        {
            moverAbajo = Keyboard.current.downArrowKey.wasPressedThisFrame;
            moverArriba = Keyboard.current.upArrowKey.wasPressedThisFrame;
        }

        // --- MANDO ---
        if (Gamepad.current != null)
        {
            moverAbajo |= Gamepad.current.dpad.down.wasPressedThisFrame;
            moverArriba |= Gamepad.current.dpad.up.wasPressedThisFrame;

            if (Gamepad.current.leftStick.ReadValue().y < -0.5f && Time.time - tiempoUltimoCambio > cambioRetardo)
                moverAbajo = true;

            if (Gamepad.current.leftStick.ReadValue().y > 0.5f && Time.time - tiempoUltimoCambio > cambioRetardo)
                moverArriba = true;
        }

        // --- CAMBIO DE OPCIÓN ---
        if (moverAbajo && Time.time - tiempoUltimoCambio > cambioRetardo)
        {
            indiceActual = (indiceActual + 1) % botones.Length;
            ActualizarSeleccionVisual();
            tiempoUltimoCambio = Time.time;
        }

        if (moverArriba && Time.time - tiempoUltimoCambio > cambioRetardo)
        {
            indiceActual = (indiceActual - 1 + botones.Length) % botones.Length;
            ActualizarSeleccionVisual();
            tiempoUltimoCambio = Time.time;
        }

        // --- CONFIRMAR ---
        if ((Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame))
        {
            botones[indiceActual].onClick.Invoke();
        }
    }

    private void ActualizarSeleccionVisual()
    {
        for (int i = 0; i < botones.Length; i++)
        {
            var text = botones[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            bool seleccionado = (i == indiceActual);
            text.color = seleccionado ? colorSeleccionado : colorNormal;
            text.fontSize = seleccionado ? 90 : 80;
        }

        botones[indiceActual].Select();
    }

    // === ACCIONES DE LOS BOTONES ===
    public void Jugar()
    {
        SceneManager.LoadScene("Infierno");
    }

    public void Controles()
    {
        SceneManager.LoadScene("Controles");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}
