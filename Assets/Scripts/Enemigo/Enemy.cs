using System;
using UnityEngine;

public class Enemy : LivingEntity
{
    public static event Action<Enemy> OnEnemyKilled;

    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    private Transform objetivoActual;

    [Header("Barra de vida flotante")]
    [SerializeField] private FloatingHealthBar healthBarPrefab;
    private FloatingHealthBar healthBarInstance;

    private Rigidbody2D rb;
    private bool esperando;
    private float tiempoEspera = 1f;
    private float contadorEspera;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        objetivoActual = puntoB;

        if (healthBarPrefab == null)
        {
            Debug.LogWarning($"⚠ Enemy ({name}): Prefab de barra no asignado.");
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning($"⚠ Enemy ({name}): No se encontró un Canvas activo en la escena.");
            return;
        }

        // Instanciar la barra dentro del Canvas sin definir posición aún
        healthBarInstance = Instantiate(healthBarPrefab, canvas.transform);
        healthBarInstance.AsignarObjetivo(transform);
        healthBarInstance.SetMaxHealth((int)vidaMax);
    }

    private void Update()
    {
        if (estaMuerto) return;

        if (esperando)
        {
            contadorEspera -= Time.deltaTime;
            if (contadorEspera <= 0)
                esperando = false;
            else
                return;
        }

        // Movimiento entre puntos
        transform.position = Vector2.MoveTowards(transform.position, objetivoActual.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, objetivoActual.position) < 0.05f)
        {
            objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
            esperando = true;
            contadorEspera = tiempoEspera;
        }
    }

    public override void TomarDaño(float cantidad)
    {
        base.TomarDaño(cantidad);

        if (healthBarInstance != null)
            healthBarInstance.UpdateHealthBar((int)vidaActual);
    }

    protected override void Morir()
    {
        base.Morir();

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        OnEnemyKilled?.Invoke(this);
    }
}
