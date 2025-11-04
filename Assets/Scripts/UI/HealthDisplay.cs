using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [Header("Sprites de corazones")]
    public Sprite emptyHeart;
    public Sprite fullHeart;

    [Header("Referencias")]
    public Image[] hearts;
    public VidaJugador VidaJugador;

    void Update()
    {
        int health = VidaJugador.vidasActuales;
        int maxHealth = VidaJugador.vidasMax;

        for (int i = 0; i < hearts.Length; i++)
        {
            // Si el jugador aún tiene esta vida, muestra corazón lleno
            if (i < health)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;

            // Activa o desactiva el corazón según la cantidad máxima
            hearts[i].enabled = (i < maxHealth);
        }
    }
}
