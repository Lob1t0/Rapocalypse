using UnityEngine;
using TMPro;
using System.Collections;

public class DialogoTrigger2D : MonoBehaviour
{
    [Header("Líneas de diálogo")]
    [TextArea(2, 5)]
    [SerializeField] private string[] lineasDialogo;

    [Header("Referencias UI")]
    [SerializeField] private GameObject panelDialogo;
    [SerializeField] private TextMeshProUGUI textoDialogo;
    [SerializeField] private GameObject textoInteraccion; // Texto "Presiona E"

    [Header("Configuración")]
    [SerializeField] private float tiempoEntreLetras = 0.03f;
    [SerializeField] private float tiempoAntesDeCerrar = 2f;
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;

    [Header("Botones mando (PS)")]
    [SerializeField] private KeyCode botonGamepadInteraccion = KeyCode.JoystickButton0;
    // 🔹 PS: Cuadrado = Button2   |  (Button0 = X, Button1 = Círculo, Button2 = Cuadrado, Button3 = Triángulo)

    private int indiceLinea = 0;
    private bool jugadorDentro = false;
    private bool mostrando = false;

    private void Update()
    {
        // ✅ Detectar interacción por teclado o por mando
        bool presionoInteraccion =
            Input.GetKeyDown(teclaInteraccion) || Input.GetKeyDown(botonGamepadInteraccion);

        if (jugadorDentro && presionoInteraccion)
        {
            if (!panelDialogo.activeSelf && !mostrando)
            {
                if (textoInteraccion != null)
                    textoInteraccion.SetActive(false);

                panelDialogo.SetActive(true);
                StartCoroutine(MostrarDialogo());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            if (!mostrando && textoInteraccion != null)
                textoInteraccion.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = false;

            // ⚠️ No cerramos el panel si el diálogo está activo
            if (!mostrando)
            {
                if (textoInteraccion != null)
                    textoInteraccion.SetActive(false);

                if (panelDialogo != null)
                    panelDialogo.SetActive(false);

                if (textoDialogo != null)
                    textoDialogo.text = "";
            }
        }
    }

    private IEnumerator MostrarDialogo()
    {
        mostrando = true;

        if (panelDialogo == null || textoDialogo == null)
            yield break;

        textoDialogo.text = "";

        // Efecto de escritura
        foreach (char letra in lineasDialogo[indiceLinea])
        {
            if (textoDialogo == null) yield break;
            textoDialogo.text += letra;
            yield return new WaitForSeconds(tiempoEntreLetras);
        }

        // Esperar unos segundos después de escribir
        yield return new WaitForSeconds(tiempoAntesDeCerrar);

        // Cerrar el panel de forma segura
        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (textoDialogo != null)
            textoDialogo.text = "";

        indiceLinea = 0;
        mostrando = false;

        // Si el jugador sigue dentro, vuelve a mostrar el texto "Presiona E"
        if (jugadorDentro && textoInteraccion != null)
            textoInteraccion.SetActive(true);
    }
}
