using UnityEngine;

public class Camara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    [SerializeField] private Transform jugador;

    [Header("Ajustes de seguimiento")]
    [SerializeField] private float suavizado = 0.15f; // 0 = seguimiento instantáneo
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Límites de la cámara")]
    [SerializeField] private float limiteMinX = -100f; // Límite izquierdo del mapa
    [SerializeField] private float limiteMaxX = 100f;  // Límite derecho del mapa
    private float limiteMinY;                          // Límite inferior calculado

    public float offsetaltura=0f;

    private Vector3 velocidad = Vector3.zero;
    private Camera camara;

    private void Start()
    {
        camara = Camera.main;

        // Calculamos el límite mínimo de Y dinámicamente
        // La cámara nunca bajará más allá de este punto
        float alturaVisible = camara.orthographicSize;
        limiteMinY = transform.position.y - alturaVisible +offsetaltura; 
    }

    private void LateUpdate()
    {
        if (jugador == null) return;

        // Calculamos la posición deseada de la cámara
        Vector3 posicionDeseada = jugador.position + offset;

        // Limitamos el eje X entre los bordes del mapa
        posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, limiteMinX, limiteMaxX);

        // Limitamos el eje Y para que la cámara nunca muestre el vacío inferior
        posicionDeseada.y = Mathf.Max(posicionDeseada.y, limiteMinY);

        // Aplicamos el movimiento suave
        transform.position = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
    }
}
