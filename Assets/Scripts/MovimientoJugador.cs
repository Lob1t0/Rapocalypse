using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoJugador2D : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 6f;
    [SerializeField] private float suavizado = 0.05f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 12f;

    [Header("Raycast de suelo")]
    [SerializeField] private Vector2 offsetRay = new Vector2(0f, -0.5f);
    [SerializeField] private float largoRay = 0.1f;
    [SerializeField] private LayerMask capaSuelo = ~0;

    [Header("Calidad de salto")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Slide")]
    [SerializeField] private float slideDuracion = 0.25f;
    [SerializeField] private float slideFuerza = 12f;
    [SerializeField] private float slideCooldown = 0.5f;
    [SerializeField] private bool ajustarColliderEnSlide = true;
    [SerializeField] private Vector2 slideColliderSize = new Vector2(0.7f, 0.6f);
    [SerializeField] private Vector2 slideColliderOffset = new Vector2(0f, -0.2f);

    [Header("Animator (Triggers)")]
    public Animator animator; // "Idle", "Walk", "Jump", "Slide"

    [Header("Transiciones rápidas")]
    [SerializeField] private float snapBlend = 0.02f;

    [Header("Botones mando (PS)")]
    [Tooltip("Saltar (PS: X/Cross). Por defecto Button1 porque en tu setup X disparaba el 1.")]
    [SerializeField] private KeyCode botonGamepadSaltar = KeyCode.JoystickButton1;
    [Tooltip("Slide (PS: Square). Por defecto Button0 porque en tu setup Square disparaba el 0.")]
    [SerializeField] private KeyCode botonGamepadSlide  = KeyCode.JoystickButton0;

    // --- privados ---
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    private float inputX;
    private Vector2 velRef = Vector2.zero;
    private bool mirandoDerecha = true;

    // Timers
    private float tDesdeSuelo = 0f;
    private float tDesdeJump  = 999f;

    // Aire / locomoción
    private bool enAire = false;
    private bool bloquearLocomocion = false;
    private bool estabaMov = false;

    // Slide
    private bool pidoSlide = false;
    private bool haciendoSlide = false;
    private float slideTimer = 0f;
    private float slideCooldownTimer = 0f;

    // Collider backup
    private Vector2 colSizeOri, colOffsetOri;

    // Animator hashes
    private int stIdle, stWalk, stJump, stSlide;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 1f;

        boxCol = GetComponent<BoxCollider2D>();
        if (boxCol) { colSizeOri = boxCol.size; colOffsetOri = boxCol.offset; }

        stIdle  = Animator.StringToHash("Idle");
        stWalk  = Animator.StringToHash("Walk");
        stJump  = Animator.StringToHash("Jump");
        stSlide = Animator.StringToHash("Slide");
    }

    private void Update()
    {
        // AD + stick izquierdo (eje "Horizontal" del Input Manager)
        inputX = LeerHorizontal();

        // SALTO: Space o X (Cross) del mando
        if (JumpPressed())
            tDesdeJump = 0f;

        // SLIDE: Shift o Square del mando
        if (SlidePressed())
            pidoSlide = true;

        // timers
        tDesdeSuelo += Time.deltaTime;
        tDesdeJump  += Time.deltaTime;
        if (slideCooldownTimer > 0f) slideCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        bool enSuelo = TocaSueloRaycast();
        if (enSuelo) tDesdeSuelo = 0f;

        // ---------- SLIDE ----------
        if (!haciendoSlide && pidoSlide && enSuelo && slideCooldownTimer <= 0f)
            IniciarSlide();
        pidoSlide = false;

        if (haciendoSlide)
        {
            slideTimer -= Time.fixedDeltaTime;
            if (slideTimer <= 0f) TerminarSlide();
        }

        // ---------- MOVIMIENTO ----------
        if (!haciendoSlide)
        {
            Vector2 velObjetivo = new Vector2(inputX * velocidad, rb.linearVelocity.y);
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, velObjetivo, ref velRef, suavizado);
        }

        // ---------- SALTO ----------
        if (!haciendoSlide && !enAire && tDesdeSuelo <= coyoteTime && tDesdeJump <= jumpBuffer)
        {
            enAire = true;                 // bloquea Walk/Idle
            bloquearLocomocion = true;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            DispararTrigger("Jump");
            SnapTo(stJump);

            tDesdeJump = 999f;
        }

        // ---------- ATERRIZAJE ----------
        if (enAire && enSuelo && rb.linearVelocity.y <= 0.01f)
        {
            enAire = false;
            bloquearLocomocion = false;

            if (Mathf.Abs(inputX) > 0.01f)
            {
                DispararTrigger("Walk");
                SnapTo(stWalk);
                estabaMov = true;
            }
            else
            {
                SnapIdle();
                estabaMov = false;
            }
        }

        // ---------- FLIP ----------
        if (inputX > 0.01f && !mirandoDerecha) Girar();
        else if (inputX < -0.01f && mirandoDerecha) Girar();

        // ---------- WALK / IDLE ----------
        if (!bloquearLocomocion && !enAire && !haciendoSlide)
        {
            bool mov = Mathf.Abs(inputX) > 0.01f && enSuelo;
            if (mov && !estabaMov) { DispararTrigger("Walk"); SnapTo(stWalk); estabaMov = true; }
            else if (!mov && estabaMov) { SnapIdle(); estabaMov = false; }
        }

        // Fallback a Idle
        if (enSuelo && Mathf.Abs(inputX) < 0.01f && !haciendoSlide && !enAire)
            SnapIdleOnNotIdle();
    }

    // ----------------- ENTRADAS -----------------
    private float LeerHorizontal()
    {
        // teclado A/D
        float kb = 0f;
        if (Input.GetKey(KeyCode.A)) kb -= 1f;
        if (Input.GetKey(KeyCode.D)) kb += 1f;

        // stick izquierdo
        float stick = Input.GetAxis("Horizontal"); // -1..1

        // si hay teclado, priorízalo
        return Mathf.Abs(kb) > 0.01f ? kb : stick;
    }

    private bool JumpPressed()
    {
        // Teclado: Space (si quieres quitarlo, borra esta línea)
        if (Input.GetKeyDown(KeyCode.Space))
            return true;

        // Mando: X / Cross (South) -> por defecto JoystickButton1 en tu setup
        if (Input.GetKeyDown(botonGamepadSaltar))
            return true;

        return false;
    }

    private bool SlidePressed()
    {
        // Teclado: Shift (si no lo quieres, borra esta línea)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            return true;

        // Mando: Square (West) -> por defecto JoystickButton0 en tu setup
        if (Input.GetKeyDown(botonGamepadSlide))
            return true;

        return false;
    }

    // ----------------- SLIDE -----------------
    private void IniciarSlide()
    {
        haciendoSlide = true;
        slideTimer = slideDuracion;
        slideCooldownTimer = slideCooldown;
        bloquearLocomocion = true;

        float dir = mirandoDerecha ? 1f : -1f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * slideFuerza, 0f), ForceMode2D.Impulse);

        DispararTrigger("Slide");
        SnapTo(stSlide);

        if (ajustarColliderEnSlide && boxCol)
        {
            boxCol.size = slideColliderSize;
            boxCol.offset = slideColliderOffset;
        }
    }

    private void TerminarSlide()
    {
        haciendoSlide = false;
        bloquearLocomocion = false;

        if (ajustarColliderEnSlide && boxCol)
        {
            boxCol.size = colSizeOri;
            boxCol.offset = colOffsetOri;
        }

        if (Mathf.Abs(inputX) > 0.01f && TocaSueloRaycast())
        {
            DispararTrigger("Walk");
            SnapTo(stWalk);
            estabaMov = true;
        }
        else
        {
            SnapIdle();
            estabaMov = false;
        }
    }

    // ----------------- SUELO -----------------
    private bool TocaSueloRaycast()
    {
        Vector2 origen = (Vector2)transform.position + offsetRay;
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.down, largoRay, capaSuelo);
        Debug.DrawRay(origen, Vector2.down * largoRay, hit ? Color.green : Color.red);
        return hit.collider != null;
    }

    // ----------------- ANIM UTILS -----------------
    private void DispararTrigger(string nombre)
    {
        if (!animator) return;
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Slide");
        animator.SetTrigger(nombre);
    }

    private void SnapTo(int stateHash)
    {
        if (!animator) return;
        animator.CrossFadeInFixedTime(stateHash, snapBlend, 0, 0f);
    }

    private void SnapIdle()
    {
        DispararTrigger("Idle");
        SnapTo(stIdle);
    }

    private void SnapIdleOnNotIdle()
    {
        if (!animator) return;
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.shortNameHash != stIdle)
            SnapIdle();
    }

    // ----------------- VARIOS -----------------
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        var s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 o = transform.position + (Vector3)offsetRay;
        Gizmos.DrawLine(o, o + Vector3.down * largoRay);
    }
}
