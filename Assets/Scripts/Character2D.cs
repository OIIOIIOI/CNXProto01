using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class Character2D : MonoBehaviour
{

    Rigidbody2D rbody;
    BoxCollider2D coll;
    Vector2 previousPosition;
    Vector2 currentPosition;
    Vector2 nextMovement;
    ContactFilter2D contactFilter;
    RaycastHit2D[] hitBuffer = new RaycastHit2D[5];
    RaycastHit2D[] foundHits = new RaycastHit2D[3];
    Collider2D[] groundColliders = new Collider2D[3];
    Vector2[] raycastPositions = new Vector2[3];

    public LayerMask groundedLayerMask;
    public bool isGrounded { get; protected set; }
    public Vector2 velocity { get; protected set; }

    void Awake ()
    {
        rbody = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        currentPosition = rbody.position;
        previousPosition = rbody.position;

        contactFilter.layerMask = groundedLayerMask;
        contactFilter.useLayerMask = true;
        contactFilter.useTriggers = false;

        Physics2D.queriesStartInColliders = false;
    }

    void FixedUpdate ()
    {
        previousPosition = rbody.position;
        currentPosition = previousPosition + nextMovement;
        velocity = (currentPosition - previousPosition) / Time.deltaTime;

        rbody.MovePosition(currentPosition);
        nextMovement = Vector2.zero;

        CheckCollision();
    }

    public void Move(Vector2 movement)
    {
        nextMovement += movement;
    }

    public void CheckCollision ()
    {
        Vector2 raycastDirection;
        Vector2 raycastStart;
        Vector2 raycastStartCentre;
        float raycastDistance;

        raycastStart = rbody.position + coll.offset;
        raycastDistance = coll.size.x * 0.5f + 0.2f;
        
        raycastDirection = Vector2.down;
        raycastStartCentre = raycastStart + Vector2.down * (coll.size.y * 0.5f - coll.size.x * 0.5f);

        raycastPositions[0] = raycastStartCentre + Vector2.left * coll.size.x * 0.5f;
        raycastPositions[1] = raycastStartCentre;
        raycastPositions[2] = raycastStartCentre + Vector2.right * coll.size.x * 0.5f;

        for (int i = 0; i < raycastPositions.Length; i++)
        {
            int count = Physics2D.Raycast(raycastPositions[i], raycastDirection, contactFilter, hitBuffer, raycastDistance);

            foundHits[i] = count > 0 ? hitBuffer[0] : new RaycastHit2D();
            groundColliders[i] = foundHits[i].collider;
        }

        Vector2 groundNormal = Vector2.zero;
        int hitCount = 0;

        for (int i = 0; i < foundHits.Length; i++)
        {
            if (foundHits[i].collider != null)
            {
                groundNormal += foundHits[i].normal;
                hitCount++;
            }
        }

        if (hitCount > 0)
            groundNormal.Normalize();

        Vector2 relativeVelocity = velocity;
        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i] == null)
                continue;
        }

        if (Mathf.Approximately(groundNormal.x, 0f) && Mathf.Approximately(groundNormal.y, 0f))
        {
            isGrounded = false;
        }
        else
        {
            isGrounded = relativeVelocity.y <= 0f;

            if (coll != null)
            {
                if (groundColliders[1] != null)
                {
                    float capsuleBottomHeight = rbody.position.y + coll.offset.y - coll.size.y * 0.5f;
                    float middleHitHeight = foundHits[1].point.y;
                    isGrounded &= middleHitHeight < capsuleBottomHeight + 0.1f;
                }
            }
        }

        for (int i = 0; i < hitBuffer.Length; i++)
            hitBuffer[i] = new RaycastHit2D();
    }

}
