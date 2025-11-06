using UnityEngine;

public class DamoAlJugador : MonoBehaviour
{
    [Header("Daño al jugador")]
    [SerializeField] private float tiempoEntreDaños = 1.5f; // Cooldown entre daños
    
    [Header("Referencias")]
    [SerializeField] private string tagJugador = "Player";
    
    private VidaJugador vidaJugador;
    private float temporizadorDaño = 0f;

    private void Start()
    {
        // Buscar el script de vida del jugador
        vidaJugador = FindFirstObjectByType<VidaJugador>();
        
        if (vidaJugador == null)
        {
            Debug.LogError("❌ No se encontró VidaJugador en la escena");
        }
    }

    private void Update()
    {
        // Reducir el cooldown
        if (temporizadorDaño > 0f)
        {
            temporizadorDaño -= Time.deltaTime;
        }
    }

    // ✅ SE LLAMA CUANDO EL JUGADOR ENTRA EN CONTACTO
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            DañarJugador();
        }
    }

    // ✅ SE LLAMA CADA FRAME MIENTRAS ESTÉ EN CONTACTO
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador) && temporizadorDaño <= 0f)
        {
            DañarJugador();
        }
    }

    // ✅ SE LLAMA CUANDO EL JUGADOR SALE DEL CONTACTO
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(tagJugador))
        {
            temporizadorDaño = 0f; // 🔑 ESTO RESETEA EL COOLDOWN
        }
    }

    // ✅ MÉTODO PARA DAÑAR
    private void DañarJugador()
    {
        if (vidaJugador == null) return;

        vidaJugador.RecibirDanioEnemigo(gameObject);
        temporizadorDaño = tiempoEntreDaños;
        
        Debug.Log($"💥 {gameObject.name} atacó al jugador. Corazones: {vidaJugador.vidasActuales}");
    }
}
