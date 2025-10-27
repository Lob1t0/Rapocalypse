using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("Disparo")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float proyectilVelocidad = 12f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float escala = 1f;

    [Header("Control de disparo")]
    [SerializeField] private bool esJugador = true;

    [Header("Input System")]
    [SerializeField] private PlayerInput playerInput;

    private float fireCooldown = 0f;
    private InputAction attackAction;

    // Vibración
    [Header("Vibración (máxima potencia)")]
    [SerializeField] private float duracionVibracion = 0.8f;

    private void Awake()
    {
        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        if (playerInput == null) return;

        attackAction = playerInput.actions["Attack"];
        if (attackAction != null)
            attackAction.performed += OnAttack;
    }

    private void OnDisable()
    {
        if (attackAction != null)
            attackAction.performed -= OnAttack;
    }

    private void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        ActualizarPosicionFirePoint();
    }

    private void ActualizarPosicionFirePoint()
    {
        if (firePoint == null) return;

        bool mirandoDerecha = transform.localScale.x > 0;

        firePoint.localPosition = mirandoDerecha
            ? new Vector3(0.9f, 0.25f, 0f)
            : new Vector3(0.9f, 0.25f, 0f);

        firePoint.localRotation = mirandoDerecha
            ? Quaternion.identity
            : Quaternion.Euler(0, 180, 0);
    }

    private void OnAttack(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || !esJugador || fireCooldown > 0f)
            return;

        Disparar();
        fireCooldown = fireRate;
    }

    private void Disparar()
    {
        if (firePoint == null) return;

        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        GameObject bala = ProjectilePool.Instance.GetProjectile(
            firePoint.position,
            Quaternion.identity,
            escala
        );

        bala.GetComponent<Proyectil>().Lanzar(dir, proyectilVelocidad);

        // Vibración
        ActivarVibracionFuerte();
    }

    private async void ActivarVibracionFuerte()
    {
        // Detectar el mando
        var mando = Gamepad.current;

        if (mando == null)
        {
            Debug.LogWarning("⚠ No se detectó ningún Gamepad.");
            return;
        }

        Debug.Log($"🎮 Mando detectado: {mando.displayName}");

        // Activar vibración máxima
        mando.SetMotorSpeeds(1f, 1f);
        Debug.Log("🔥 Vibración forzada al 100%");
        await System.Threading.Tasks.Task.Delay((int)(duracionVibracion * 1000));

        mando.SetMotorSpeeds(0f, 0f);
    }
}
