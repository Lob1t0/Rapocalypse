using UnityEngine;

public class ObjetoMovimientoCruz : MonoBehaviour
{
    [Header("Movimiento en Cruz")]
    [SerializeField] private float distanciaMovimiento = 4f;
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private float tiempoPausaEnEsquinas = 0.5f;
    
    [Header("Daño")]
    [SerializeField] private string tagJugador = "Player";
    
    private Vector3 posicionInicial;
    private int direccionActual = 0; // 0=derecha, 1=arriba, 2=izquierda, 3=abajo
    private Vector3 objetivoActual;
    private float tiempoPausa = 0f;
    private bool enPausa = false;
    private VidaJugador vidaJugador;

    private void Start()
    {
        posicionInicial = transform.position;
        CalcularSiguienteObjetivo();
        vidaJugador = FindFirstObjectByType<VidaJugador>();
    }

    private void Update()
    {
        if (enPausa)
        {
            tiempoPausa += Time.deltaTime;
            if (tiempoPausa >= tiempoPausaEnEsquinas)
            {
                enPausa = false;
                tiempoPausa = 0f;
                direccionActual = (direccionActual + 1) % 4; // Siguiente dirección
                CalcularSiguienteObjetivo();
            }
            return;
        }

        // Mover hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, objetivoActual, velocidad * Time.deltaTime);
        
        // Si llegó al objetivo, pausar
        if (Vector3.Distance(transform.position, objetivoActual) < 0.1f)
        {
            enPausa = true;
        }
    }

    private void CalcularSiguienteObjetivo()
    {
        switch (direccionActual)
        {
            case 0: // Derecha
                objetivoActual = posicionInicial + Vector3.right * distanciaMovimiento;
                break;
            case 1: // Arriba
                objetivoActual = posicionInicial + Vector3.up * distanciaMovimiento;
                break;
            case 2: // Izquierda
                objetivoActual = posicionInicial + Vector3.left * distanciaMovimiento;
                break;
            case 3: // Abajo
                objetivoActual = posicionInicial + Vector3.down * distanciaMovimiento;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador) && vidaJugador != null)
        {
            vidaJugador.RecibirDanioEnemigo(gameObject);
            Debug.Log("⚡ Objeto en cruz dañó al jugador");
        }
    }
}
