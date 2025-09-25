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
    [SerializeField] private float largoRay = 0.12f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Paredes")]
    [SerializeField] private Transform controladorPared;
    [SerializeField] private Vector2 dimensionesCajaPared = new Vector2(0.2f, 1f);
    [SerializeField] private LayerMask capaPared;

    [Header("Calidad de salto")]
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Slide en suelo")]
    [SerializeField] private float slideDuracion = 0.25f;
    [SerializeField] private float slideFuerza = 12f;
    [SerializeField] private float slideCooldown = 0.5f;
    [SerializeField] private bool ajustarColliderEnSlide = true;

    [Header("Wall Slide / Wall Jump")]
    [SerializeField] private float velocidadDeslizar = 2f;
    [SerializeField] private float fuerzaSaltoParedX = 8f;
    [SerializeField] private float fuerzaSaltoParedY = 12f;
    private bool enPared;
    private bool deslizando;
    private bool saltandoPared;

    [Header("Animator (Triggers)")]
    public Animator animator;

    [Header("Animator (Estado de caída opcional)")]
    [SerializeField] private bool usarEstadoFall = true;
    [SerializeField] private string nombreEstadoFall = "Fall";
    [SerializeField] private float umbralCaida = -0.05f;

    [Header("Transiciones rápidas")]
    [SerializeField] private float snapBlend = 0.02f;

    [Header("Botones mando (PS)")]
    [SerializeField] private KeyCode botonGamepadSaltar = KeyCode.JoystickButton1;
    [SerializeField] private KeyCode botonGamepadSlide = KeyCode.JoystickButton0;

    // --- privados ---
    private Rigidbody2D rb;
    private BoxCollider2D boxCol;

    private float inputX;
    private Vector2 velRef = Vector2.zero;
    private bool mirandoDerecha = true;

    // Timers
    private float tDesdeSuelo = 0f;
    private float jumpPressedRemember = 0f;

    // Aire / locomoción
    private bool enAire = false;
    private bool estabaMov = false;

    // Slide en suelo
    private bool pidoSlide = false;
    private bool haciendoSlide = false;
    private float slideTimer = 0f;
    private float slideCooldownTimer = 0f;

    // Collider backup
    private Vector2 colSizeOri, colOffsetOri;

    // Animator hashes
    private int stIdle, stWalk, stJump, stSlide, stFall;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 1f;

        boxCol = GetComponent<BoxCollider2D>();
        if (boxCol) { colSizeOri = boxCol.size; colOffsetOri = boxCol.offset; }

        if (!animator) animator = GetComponent<Animator>();

        stIdle = Animator.StringToHash("Idle");
        stWalk = Animator.StringToHash("Walk");
        stJump = Animator.StringToHash("Jump");
        stSlide = Animator.StringToHash("Slide");
        stFall = Animator.StringToHash(nombreEstadoFall);
    }

    private void Update()
    {
        inputX = LeerHorizontal();

        if (JumpPressed())
            jumpPressedRemember = jumpBuffer;

        if (SlidePressed())
            pidoSlide = true;

        tDesdeSuelo += Time.deltaTime;
        if (jumpPressedRemember > 0) jumpPressedRemember -= Time.deltaTime;
        if (slideCooldownTimer > 0f) slideCooldownTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        bool enSuelo = TocaSueloRaycast();
        if (enSuelo) tDesdeSuelo = 0f;

        // Detectar pared
        enPared = Physics2D.OverlapBox(controladorPared.position, dimensionesCajaPared, 0f, capaPared);

        // Wall Slide
        deslizando = !enSuelo
                     && enPared
                     && Mathf.Abs(inputX) > 0.01f
                     && Mathf.Sign(inputX) == (mirandoDerecha ? 1 : -1)
                     && rb.linearVelocity.y < -0.1f;

        if (animator) animator.SetBool("WallSlide", deslizando);

        if (deslizando)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -velocidadDeslizar, float.MaxValue));

        // Slide en suelo
        if (!haciendoSlide && pidoSlide && enSuelo && slideCooldownTimer <= 0f)
            IniciarSlide();
        pidoSlide = false;

        if (haciendoSlide)
        {
            slideTimer -= Time.fixedDeltaTime;
            if (slideTimer <= 0f) TerminarSlide();
        }

        // Movimiento (activo incluso en aire o wall jump)
        if (!haciendoSlide)
        {
            Vector2 velObjetivo = new Vector2(inputX * velocidad, rb.linearVelocity.y);
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, velObjetivo, ref velRef, suavizado);
        }

        // Salto normal
        if (!haciendoSlide && !enAire && tDesdeSuelo <= coyoteTime && jumpPressedRemember > 0)
        {
            enAire = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

            DispararTrigger("Jump");
            SnapTo(stJump);

            jumpPressedRemember = 0;
        }

        // Wall Jump
        if (jumpPressedRemember > 0 && deslizando)
        {
            WallJump();
            jumpPressedRemember = 0;
        }

        // Estado de caída
        if (usarEstadoFall && enAire && rb.linearVelocity.y < umbralCaida)
            SnapTo(stFall);

        // Aterrizaje
        if (enAire && enSuelo && rb.linearVelocity.y <= 0.01f)
        {
            enAire = false;
            saltandoPared = false;

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

        // Flip (deshabilitado durante slide en suelo)
        if (!haciendoSlide) 
        {
            if (inputX > 0.01f && !mirandoDerecha) Girar();
            else if (inputX < -0.01f && mirandoDerecha) Girar();
        }


        // Idle/Walk
        if (!enAire && !haciendoSlide)
        {
            bool mov = Mathf.Abs(inputX) > 0.01f && enSuelo;
            if (mov && !estabaMov) { DispararTrigger("Walk"); SnapTo(stWalk); estabaMov = true; }
            else if (!mov && estabaMov) { SnapIdle(); estabaMov = false; }
        }

        if (enSuelo && Mathf.Abs(inputX) < 0.01f && !haciendoSlide && !enAire)
            SnapIdleOnNotIdle();
    }

    // Wall Jump
    private void WallJump()
    {
        float dir = mirandoDerecha ? -1f : 1f;
        rb.linearVelocity = new Vector2(fuerzaSaltoParedX * dir, fuerzaSaltoParedY);
        saltandoPared = true;
        enAire = true;

        DispararTrigger("Jump");
        SnapTo(stJump);
    }

    // Entradas
    private float LeerHorizontal()
    {
        float kb = 0f;
        if (Input.GetKey(KeyCode.A)) kb -= 1f;
        if (Input.GetKey(KeyCode.D)) kb += 1f;

        float stick = Input.GetAxis("Horizontal");
        return Mathf.Abs(kb) > 0.01f ? kb : stick;
    }

    private bool JumpPressed()
    {
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        if (Input.GetKeyDown(botonGamepadSaltar)) return true;
        return false;
    }

    private bool SlidePressed()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) return true;
        if (Input.GetKeyDown(botonGamepadSlide)) return true;
        return false;
    }

    // Slide en suelo
    private void IniciarSlide()
    {
        haciendoSlide = true;
        slideTimer = slideDuracion;
        slideCooldownTimer = slideCooldown;

        float dir = mirandoDerecha ? 1f : -1f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * slideFuerza, 0f), ForceMode2D.Impulse);

        DispararTrigger("Slide");
        SnapTo(stSlide);

        if (ajustarColliderEnSlide && boxCol)
        {
            boxCol.size = new Vector2(boxCol.size.x, 0.75f);
            boxCol.offset = new Vector2(colOffsetOri.x, -0.35f);
        }
    }

    private void TerminarSlide()
    {
        haciendoSlide = false;

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

    // Suelo
    private bool TocaSueloRaycast()
    {
        Vector2 origen = (Vector2)transform.position + offsetRay;
        RaycastHit2D hit = Physics2D.Raycast(origen, Vector2.down, largoRay, capaSuelo);
        Debug.DrawRay(origen, Vector2.down * largoRay, hit ? Color.green : Color.red);
        return hit.collider != null;
    }

    // Anim Utils
    private bool HasController() => animator && animator.runtimeAnimatorController != null;

    private void DispararTrigger(string nombre)
    {
        if (!HasController()) return;
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Walk");
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Slide");
        animator.SetTrigger(nombre);
    }

    private void SnapTo(int stateHash)
    {
        if (!HasController()) return;
        animator.CrossFadeInFixedTime(stateHash, snapBlend, 0, 0f);
    }

    private void SnapIdle()
    {
        DispararTrigger("Idle");
        SnapTo(stIdle);
    }

    private void SnapIdleOnNotIdle()
    {
        if (!HasController()) return;
        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.shortNameHash != stIdle)
            SnapIdle();
    }

    // Varios
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        var s = transform.localScale; s.x *= -1f; transform.localScale = s;
    }

    private void OnDrawGizmosSelected()
    {
        // Suelo
        Gizmos.color = Color.cyan;
        Vector3 o = transform.position + (Vector3)offsetRay;
        Gizmos.DrawLine(o, o + Vector3.down * largoRay);

        // Pared
        if (controladorPared != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(controladorPared.position, dimensionesCajaPared);
        }
    }
}