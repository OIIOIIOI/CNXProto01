using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : Enemy
{

    public bool jumps = false;

    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 100f;
    public float airAccelerationRatio = 0.5f;
    public float airDecelerationRatio = 2f;
    public float gravity = 50f;
    public float jumpSpeed = 10f;

    protected bool facingRight = true;
    protected float nextJumpTime;

    protected Character2D character;
    protected Vector2 moveVector;
    protected SpriteRenderer spriteRenderer;

    void Awake()
    {
        character = GetComponent<Character2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Start ()
    {
        base.Start();

        nextJumpTime = Time.time + Random.Range(1f, 2f);
        StartCoroutine(Jump());
    }

    void Update()
    {
        // If on the ground
        if (CheckForGrounded())
        {
            GroundedHorizontalMovement();
            GroundedVerticalMovement();
        }
        // If falling/jumping
        else
        {
            AirborneHorizontalMovement();
            AirborneVerticalMovement();
        }
    }

    void FixedUpdate()
    {
        character.Move(moveVector * Time.deltaTime);
    }

    protected IEnumerator Jump ()
    {
        while (alive)
        {
            if (Time.time >= nextJumpTime)
            {
                moveVector.y = jumpSpeed;
                nextJumpTime = Time.time + Random.Range(1f, 2f);
            }
            yield return null;
        }
    }

    public bool CheckForGrounded()
    {
        bool grounded = character.isGrounded;
        return grounded;
    }

    public void GroundedHorizontalMovement()
    {
        float input = 0;
        float desiredSpeed = input * maxSpeed;
        float ratio = (input != 0) ? acceleration : deceleration;
        moveVector.x = Mathf.MoveTowards(moveVector.x, desiredSpeed, ratio * Time.deltaTime);

        UpdateFacing(input);
    }

    public void GroundedVerticalMovement()
    {
        moveVector.y -= gravity * Time.deltaTime;

        if (moveVector.y < -gravity * Time.deltaTime * 3f)
            moveVector.y = -gravity * Time.deltaTime * 3f;
    }

    public void AirborneHorizontalMovement()
    {
        float input = 0;
        float desiredSpeed = input * maxSpeed;
        float ratio = (input != 0) ? acceleration * airAccelerationRatio : deceleration * airDecelerationRatio;
        moveVector.x = Mathf.MoveTowards(moveVector.x, desiredSpeed, ratio * Time.deltaTime);

        UpdateFacing(input);
    }

    public void AirborneVerticalMovement()
    {
        if (Mathf.Approximately(moveVector.y, 0f))
            moveVector.y = 0f;
        moveVector.y -= gravity * Time.deltaTime;
    }

    void UpdateFacing(float input)
    {
        if (Mathf.Approximately(input, 0))
            return;

        bool wasFacingRight = facingRight;
        facingRight = input > 0;
        if (facingRight != wasFacingRight)
            spriteRenderer.flipX = !facingRight;
    }

}
