using UnityEngine;

public class PlataformaHorizontal : MonoBehaviour
{
    [Header("Movimiento Horizontal")]
    [SerializeField] private float distanciaEnTiles = 4f;  // Cuántos tiles se mueve hacia la derecha
    [SerializeField] private float velocidad = 2f;          // Unidades por segundo
    
    private Vector3 posicionA;
    private Vector3 posicionB;
    private Vector3 destinoActual;

    void Start()
    {
        // Punto A es la posición inicial
        posicionA = transform.position;
        // Punto B es la posición inicial + distancia en el eje X
        posicionB = posicionA + new Vector3(distanciaEnTiles, 0, 0);
        destinoActual = posicionB;
    }

    void Update()
    {
        // Mueve suavemente entre ambos puntos
        transform.position = Vector3.MoveTowards(
            transform.position, 
            destinoActual, 
            velocidad * Time.deltaTime
        );

        // Al llegar al destino, cambia de dirección
        if (Vector3.Distance(transform.position, destinoActual) < 0.01f)
        {
            destinoActual = (destinoActual == posicionA) ? posicionB : posicionA;
        }
    }
}
