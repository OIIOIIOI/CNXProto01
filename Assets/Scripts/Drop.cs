using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{

    public LayerMask hittableLayers;

    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 100f;
    public float airAccelerationRatio = 0.5f;
    public float airDecelerationRatio = 2f;
    public float gravity = 50f;
    public GameObject pickupParticles;

    protected bool facingRight = true;
    protected bool alive = false;

    protected Character2D character;
    protected Vector2 moveVector;
    protected SpriteRenderer spriteRenderer;
    protected ContactFilter2D contactFilter;
    protected Collider2D dropCollider;

    private void OnEnable()
    {
        alive = true;
    }

    void Awake()
    {
        character = GetComponent<Character2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dropCollider = GetComponent<Collider2D>();
        
        contactFilter.layerMask = hittableLayers;
        contactFilter.useLayerMask = true;
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

        if (alive && Physics2D.IsTouching(dropCollider, contactFilter))
        {
            Collider2D[] targets = new Collider2D[1];
            Physics2D.GetContacts(dropCollider, contactFilter, targets);
            if (targets.Length > 0)
            {
                Collider2D coll = targets[0];
                switch (coll.tag)
                {
                    case "Player":
                        Debug.Log("player coin");
                        break;

                    default:
                        break;
                }
            }

            alive = false;

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.simulated = false;

            dropCollider.enabled = false;
            spriteRenderer.enabled = false;

            if (pickupParticles != null)
            {
                Vector3 spawn = transform.position;
                spawn.z = -1f;
                GameObject b = GameObject.Instantiate(pickupParticles, spawn, Quaternion.identity);
            }

            DieForReal();
        }
    }

    public void SetMoveVector (float x, float y)
    {
        moveVector.Set(x, y);
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

    void DieForReal()
    {
        Destroy(gameObject);
    }

}
