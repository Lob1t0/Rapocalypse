using UnityEngine;
using UnityEngine.SceneManagement;

public class VolverMenuP : MonoBehaviour
{
    [Header("Tiempo antes de regresar")]
    [SerializeField] private float delay = 10f; // segundos de espera

    private void Start()
    {
        Invoke(nameof(CargarMenu), delay);
    }

    private void CargarMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
