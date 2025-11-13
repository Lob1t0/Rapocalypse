using UnityEngine;

public class EnemigoOscilanteIndustrial : MonoBehaviour
{
    [Header("Movimiento Vertical")]
    [SerializeField] private float alturaOscilacion = 3f; // Distancia arriba-abajo
    [SerializeField] private float velocidadOscilacion = 2f;
    
    [Header("Disparo")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private float intervaloDisparo = 2f; // Cada 2 segundos dispara
    [SerializeField] private Transform puntoDisparoArriba;
    [SerializeField] private Transform puntoDisparoAbajo;
    [SerializeField] private float velocidadProyectil = 5f;
    
    private Vector3 posicionInicial;
    private float tiempoDisparo = 0f;
    private bool disparandoArriba = true;

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        // Oscilación vertical
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * velocidadOscilacion) * alturaOscilacion;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
        
        // Sistema de disparo
        tiempoDisparo += Time.deltaTime;
        if (tiempoDisparo >= intervaloDisparo)
        {
            Disparar();
            tiempoDisparo = 0f;
            disparandoArriba = !disparandoArriba; // Alterna entre arriba y abajo
        }
    }

    private void Disparar()
    {
        Transform puntoDisparo = disparandoArriba ? puntoDisparoArriba : puntoDisparoAbajo;
        Vector2 direccion = disparandoArriba ? Vector2.up : Vector2.down;
        
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        Rigidbody2D rb = proyectil.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direccion * velocidadProyectil;
        }
        
        Debug.Log($"💥 Enemigo dispara hacia {(disparandoArriba ? "ARRIBA" : "ABAJO")}");
    }
}
