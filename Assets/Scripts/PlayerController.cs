using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 600f;
    public float gravity = 9.8f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;
    public float slowdownDuration = 1f;
    public float meleeAttackDuration = 0.5f;
    private bool isMeleeAttacking = false;

    private CharacterController controller;
    private Animator animator;
    private float dashTimer = 0f;

    private bool canInput = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canInput)
        {
            HandleMovementInput();
            HandleDashInput();
            HandleMeleeAttackInput();
            UpdateDashTimer();
        }
    }

    void HandleMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveVector = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveVector * moveSpeed * Time.deltaTime);

            animator.SetFloat("Speed", moveSpeed);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
    void HandleDashInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && dashTimer <= 0f && canInput)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        animator.SetTrigger("Roll");

        dashTimer = dashCooldown;

        float initialSpeed = moveSpeed;
        moveSpeed = dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        animator.ResetTrigger("Roll");
        
        moveSpeed = initialSpeed;

        yield return new WaitForSeconds(slowdownDuration);

        moveSpeed = initialSpeed;
    }

    void UpdateDashTimer()
    {
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }
        else
        {
            dashTimer = 0f;
        }
    }

    void HandleMeleeAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !isMeleeAttacking && canInput)
        {
            StartCoroutine(MeleeAttack());
        }
    }

    IEnumerator MeleeAttack()
    {
        isMeleeAttacking = true;
        animator.SetTrigger("MeleeAttack");
        
        float initialMoveSpeed = moveSpeed;
        moveSpeed = 0f;

        yield return new WaitForSeconds(meleeAttackDuration);

        animator.ResetTrigger("MeleeAttack");
        isMeleeAttacking = false;

        moveSpeed = initialMoveSpeed;

        yield return new WaitForSeconds(0.5f);

        canInput = true;
    }

    public void DisableInput()
        {
            canInput = false;
        }

    public void EnableInput()
        {
            canInput = true;
        }
}