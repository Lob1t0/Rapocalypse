using UnityEngine;

public class Camara : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    [SerializeField] private Transform jugador;

    [Header("Ajustes de seguimiento")]
    [SerializeField] private float suavizado = 0.15f; // 0 = instantáneo
    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Límites de la cámara")]
    [SerializeField] private float limiteMinX = -100f;
    [SerializeField] private float limiteMaxX = 100f;
    [SerializeField] private float limiteMinY = -15f; // 🔑 AHORA EDITABLE DESDE INSPECTOR
    
    [Header("Ajuste altura límite")]
    public float offsetaltura = 0f;

    private Vector3 velocidad = Vector3.zero;
    private Camera camara;

    // ---- SHAKE ----
    [Header("Shake")]
    [SerializeField] private bool limitarYConShake = true;
    private Vector3 shakeOffset = Vector3.zero;
    private Coroutine shakeRoutine;
    private float perlinSeedX;
    private float perlinSeedY;

    private void Awake()
    {
        perlinSeedX = Random.Range(0f, 1000f);
        perlinSeedY = Random.Range(0f, 1000f);
    }

    private void Start()
    {
        camara = Camera.main;
    }

    private void LateUpdate()
    {
        if (jugador == null) return;

        Vector3 posicionDeseada = jugador.position + offset;
        posicionDeseada.x = Mathf.Clamp(posicionDeseada.x, limiteMinX, limiteMaxX);
        posicionDeseada.y = Mathf.Max(posicionDeseada.y, limiteMinY); // ✅ AHORA SIGUE AL JUGADOR

        Vector3 suavizada = Vector3.SmoothDamp(transform.position, posicionDeseada, ref velocidad, suavizado);
        Vector3 final = suavizada + shakeOffset;

        if (limitarYConShake)
            final.y = Mathf.Max(final.y, limiteMinY);

        transform.position = final;
    }

    public void Shake(float duracion = 0.15f, float amplitud = 0.80f, float frecuencia = 45f)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(CoShake(duracion, amplitud, frecuencia));
    }

    private System.Collections.IEnumerator CoShake(float duracion, float amplitud, float frecuencia)
    {
        float t = 0f;
        while (t < duracion)
        {
            float nx = (Mathf.PerlinNoise(perlinSeedX, Time.time * frecuencia) - 0.5f) * 2f;
            float ny = (Mathf.PerlinNoise(perlinSeedY, Time.time * frecuencia) - 0.5f) * 2f;
            shakeOffset = new Vector3(nx * amplitud, ny * amplitud, 0f);
            t += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        shakeRoutine = null;
    }
}
