using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Character2D))]

public class Player : MonoBehaviour
{

    public enum players {
        Player1,
        Player2
    };

    public players playerNum = players.Player1;
    string playerPrefix;

    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 100f;
    public float airAccelerationRatio = 0.5f;
    public float airDecelerationRatio = 2f;
    public float gravity = 50f;
    public float jumpSpeed = 14.5f;
    protected bool canJump = true;

    public Weapon weapon;

    protected bool facingRight = true;

    protected Character2D character;
    protected Vector2 moveVector;
    protected SpriteRenderer spriteRenderer;
    
    protected Coroutine shootingCoroutine;
    protected float nextShotTime;

    protected Shaker cameraShaker;
    protected FuturoScript IRLSFXOMG;

    void Awake ()
    {
        character = GetComponent<Character2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShaker = Camera.main.GetComponent<Shaker>();

        IRLSFXOMG = GetComponent<FuturoScript>();
    }

    void Start ()
    {
        nextShotTime = Time.time;
        playerPrefix = (playerNum == players.Player1) ? "P1" : "P2";
    }

    void Update ()
    {
        // If on the ground
        if (CheckForGrounded())
        {
            GroundedHorizontalMovement();
            GroundedVerticalMovement();
            // Jump
            if (canJump && InputManager.Instance.IsHeld(playerPrefix + "Jump"))
            {
                canJump = false;
                moveVector.y = jumpSpeed;
            }
        }
        // If falling/jumping
        else
        {
            AirborneHorizontalMovement();
            AirborneVerticalMovement();
        }

        // If shooting
        if (InputManager.Instance.IsHeld(playerPrefix + "Fire"))
        {
            if (shootingCoroutine == null)
            {
                // Start shooting
                shootingCoroutine = StartCoroutine(Shoot());
                // Zoom
                //cameraShaker.Zoom(5f);
                // Activate 4D effects
                if (IRLSFXOMG != null)
                    IRLSFXOMG.Activate4D(FuturoScript.Message4D.Shoot);
            }
        }
        else if (shootingCoroutine != null)
        {
            // Stop shooting
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
            // Dezoom
            //cameraShaker.Zoom(6f);
            // Deactivate 4D effects
            if (IRLSFXOMG != null)
            {
                IRLSFXOMG.Activate4D(FuturoScript.Message4D.Aftermath);
                Invoke("BackToAmbient", 1f);
            }
        }

        // Reset jump
        if (!canJump && !InputManager.Instance.IsHeld(playerPrefix + "Jump"))
            canJump = true;
    }

    void BackToAmbient ()
    {
        if (IRLSFXOMG != null)
            IRLSFXOMG.Activate4D(FuturoScript.Message4D.Ambient);
    }

    void FixedUpdate ()
    {
        character.Move(moveVector * Time.deltaTime);
    }
    
    public bool CheckForGrounded ()
    {
        //bool wasGrounded = m_Animator.GetBool(m_HashGroundedPara);// Useful to play a sound when landing
        bool grounded = character.isGrounded;
        //m_Animator.SetBool(m_HashGroundedPara, grounded);
        return grounded;
    }

    public void GroundedHorizontalMovement ()
    {
        float input = InputManager.Instance.GetAxis(playerPrefix + "HorizontalPad");
        float desiredSpeed = input * maxSpeed;
        float ratio = (input != 0) ? acceleration : deceleration;
        moveVector.x = Mathf.MoveTowards(moveVector.x, desiredSpeed, ratio * Time.deltaTime);

        UpdateFacing(input);
    }

    public void GroundedVerticalMovement ()
    {
        moveVector.y -= gravity * Time.deltaTime;

        if (moveVector.y < -gravity * Time.deltaTime * 3f)
            moveVector.y = -gravity * Time.deltaTime * 3f;
    }

    public void AirborneHorizontalMovement ()
    {
        float input = InputManager.Instance.GetAxis(playerPrefix + "HorizontalPad");
        float desiredSpeed = input * maxSpeed;
        float ratio = (input != 0) ? acceleration * airAccelerationRatio : deceleration * airDecelerationRatio;
        moveVector.x = Mathf.MoveTowards(moveVector.x, desiredSpeed, ratio * Time.deltaTime);

        UpdateFacing(input);
    }

    public void AirborneVerticalMovement ()
    {
        if (Mathf.Approximately(moveVector.y, 0f))
            moveVector.y = 0f;
        moveVector.y -= gravity * Time.deltaTime;
    }

    void UpdateFacing (float input)
    {
        if (Mathf.Approximately(input, 0))
            return;

        bool wasFacingRight = facingRight;
        facingRight = input > 0;
        if (facingRight != wasFacingRight)
            spriteRenderer.flipX = !facingRight;
    }

    protected IEnumerator Shoot ()
    {
        while (InputManager.Instance.IsHeld(playerPrefix + "Fire"))
        {
            if (Time.time >= nextShotTime)
            {
                SpawnBullet();
                nextShotTime = Time.time + weapon.GetShotSpawnDelay();

                if (cameraShaker != null)
                    cameraShaker.Shake(0.4f * weapon.visualPower, 0.4f * weapon.visualPower);
            }
            yield return null;
        }
    }

    protected void SpawnBullet ()
    {
        float facing = facingRight ? 1f : -1f;

        for (int i = 0; i < weapon.burstShots; i++)
        {
            Vector3 spawn = transform.TransformPoint(weapon.GetShotSpawnPosition(facing));
            GameObject b = GameObject.Instantiate(weapon.shotPrefab, spawn, Quaternion.identity);
            b.GetComponent<SpriteRenderer>().flipX = !facingRight;
            b.GetComponent<SpriteRenderer>().color = weapon.shotColor;
            b.transform.localScale = new Vector3(weapon.shotSize, weapon.shotSize, 1f);
            b.GetComponent<Rigidbody2D>().velocity = weapon.GetShotVelocity(facing);
        }

        //float recoil = CheckForGrounded() ? 4f : 10f;// do it better by allowing to exceed maxSpeed (dampen recoil over time?)
        moveVector.x = Mathf.MoveTowards(moveVector.x, moveVector.x - weapon.GetRecoil(facing), 200f);
    }

}
