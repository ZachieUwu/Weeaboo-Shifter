using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Runtime.CompilerServices;

public class MovementControl : MonoBehaviour
{
    private Rigidbody2D rb;
    private float moveInput;
    public float speed;
    public float slowSpeed;
    public float jump;
    private Animator anim;

    private bool isDashing = false;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;
    private bool canDash = true;

    [SerializeField] private TrailRenderer tr;

    private bool isAttacking = false;
    public bool canMove = true;

    private CinemachineImpulseSource impulseSource;

    //--Ground--
    private bool isGrounded;
    public Transform feetPos;
    public float checkRad;
    public LayerMask Groundedyes;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        float currentSpeed = isAttacking ? slowSpeed : speed;

        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);

        if (moveInput == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (!canMove) return;

        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRad, Groundedyes);

        if (moveInput > 0.1f)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (moveInput < -0.1f)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("takeOf");
            rb.velocity = Vector2.up * jump;
        }

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
        }
        else
        {
            anim.SetBool("isJumping", true);
        }

        if (Input.GetKeyDown(KeyCode.Q) && canDash)
        {
            CameraShake.instance.CameraShaking(impulseSource);
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(HandleAttack());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RngBox"))
        {
            AbilityManager abilityManager = FindObjectOfType<AbilityManager>();
            if (abilityManager != null)
            {
                abilityManager.CollectRngBox();
            }
        }
    }

    public void ToggleMovement(bool enable)
    {
        canMove = enable;
    }

    IEnumerator HandleAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        anim.SetTrigger("Dash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        float dashDirection = transform.eulerAngles.y == 180 ? 1f : -1f;
        rb.velocity = new Vector2(dashDirection * dashingPower, 0f);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    // Handle player death
    void Die()
    {
        Debug.Log("Player died!");
        canMove = false;
    }
}
