using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolverMenu : MonoBehaviour
{
    private Button boton;

    private void Start()
    {
        // Obtiene el botón del mismo objeto
        boton = GetComponent<Button>();

        // Asigna la función al evento OnClick del botón
        boton.onClick.AddListener(LoadMenu);
    }

    private void Update()
    {
        // Detecta botón X del control o Enter del teclado
        if ((Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame) ||
            (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame))
        {
            LoadMenu();
        }
    }

    // 👇 Esta es la función pública que aparecerá en el OnClick()
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

