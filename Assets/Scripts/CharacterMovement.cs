using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
    
{   Animator animator;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public float move = 5f;
    public float jump = 10f;
    private float jtime = 0f;
    [SerializeField] private LayerMask ground;
    
    public enum CharacterState
    {
        Idle,
        Moving,
        Jumping,
        Crouching,
        Attacking
    }

    private CharacterState currentState = CharacterState.Idle;
    
    private void SetState(CharacterState newState)
    {
        StopAllCoroutines();
        currentState = newState;

        switch (currentState)
        {
            case CharacterState.Idle:
                StartCoroutine(IdleState());
                break;
            case CharacterState.Moving:
                StartCoroutine(MovingState());
                break;
            case CharacterState.Jumping:
                StartCoroutine(JumpingState());
                break;
            case CharacterState.Crouching:
                StartCoroutine(CrouchingState());
                break;
            case CharacterState.Attacking:
                StartCoroutine(AttackingState());
                break;
            


        }
    }
    private IEnumerator IdleState()
    {
        while (currentState == CharacterState.Idle)
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("isCrouch", false);
            animator.SetBool("isJump", false);
          
            yield return null;
        }
    }

    private IEnumerator MovingState()
    {
        while (currentState == CharacterState.Moving)
        {
            animator.SetBool("isMoving", true);
            animator.SetBool("isCrouch", false);
            animator.SetBool("isAttack", false);
            float horizontalInput = Input.GetAxis("Horizontal");
            transform.Translate(Vector2.right * horizontalInput * move * Time.deltaTime);
            if (horizontalInput != 0)
            {
                
                spriteRenderer.flipX = horizontalInput < 0;
            }

            yield return null;
        }
    }

    private IEnumerator JumpingState()
    {
        while (currentState == CharacterState.Jumping)
        {
           animator.SetBool("isJump", true);
            animator.SetBool("isAttack", false);
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, jump);
            
            

            yield return null;
        }
    }

    private IEnumerator CrouchingState() 
    {
        while (currentState == CharacterState.Crouching)
        {

            animator.SetBool("isCrouch", true);
            animator.SetBool("isMoving", false);
            animator.SetBool("isJump", false);
            animator.SetBool("isAttack", false);
            yield return null;
        }
    }
    private IEnumerator AttackingState() 
    {
        while (currentState == CharacterState.Attacking)
        {
            animator.SetBool("isAttack", true);
            animator.SetBool("isCrouch", false);
            animator.SetBool("isMoving", false);
            animator.SetBool("isJump", false);
            yield return null;
        }
    }

    private bool IsGrounded()
    {
        float extht = .1f;
        RaycastHit2D raycastHit = Physics2D.Raycast(boxCollider.bounds.center, Vector2.down, boxCollider.bounds.extents.y+ extht, ground );
        return raycastHit.collider != null;
    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

   
    void Update()
    {
       
        
        if ((Input.GetAxis("Vertical")) > 0f && IsGrounded() && Time.time > jtime)
        {
            jtime = Time.time + 1f;
            animator.SetBool("isJump", false);
            SetState(CharacterState.Jumping);
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0f)
        {
            SetState(CharacterState.Moving);
        }
        else if ((Input.GetAxis("Vertical")) < 0f)
        { 
            SetState(CharacterState.Crouching);
        }
        else if ((Input.GetKeyDown(KeyCode.Space)))
        {
            SetState(CharacterState.Attacking);
        }
        else
        {
            SetState(CharacterState.Idle);
        }
    }
}

