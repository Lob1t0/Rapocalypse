using UnityEngine;
using UnityEngine.UI;

public class BarraVidaFlotante : MonoBehaviour
{
    [SerializeField] private Image barraLlena;
    [SerializeField] private Color colorSano = Color.green;
    [SerializeField] private Color colorDanado = Color.red;
    
    private float maxHealth = 100f;

    public void SetMaxHealth(float max)
    {
        maxHealth = max;
    }

    public void UpdateBar(float currentHealth, float maxHealth)
    {
        if (barraLlena == null) return;

        float percentage = currentHealth / maxHealth;
        barraLlena.fillAmount = percentage;

        if (percentage > 0.5f)
            barraLlena.color = colorSano;
        else if (percentage > 0.25f)
            barraLlena.color = Color.yellow;
        else
            barraLlena.color = colorDanado;
    }
}
