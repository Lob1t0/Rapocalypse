using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TpEMINEM : MonoBehaviour
{
    [Header("Jefes de la fase")]
    public BossVida[] bosses;

    [Header("Configuración")]
    public string escenaDestino = "industrial";
    public float delay = 3f;

    private bool escenaDisparada = false;

    void Update()
    {
        if (escenaDisparada) return;

        bool todosMuertos = true;

        foreach (BossVida boss in bosses)
        {
            if (boss != null && !boss.EstaMuerto())
            {
                todosMuertos = false;
                break;
            }
        }

        if (todosMuertos)
        {
            escenaDisparada = true;
            StartCoroutine(CambiarEscena());
        }
    }

    private IEnumerator CambiarEscena()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("industrial");
    }
}
