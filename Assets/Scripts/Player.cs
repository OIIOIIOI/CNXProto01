using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Character2D))]

public class Player : MonoBehaviour
{

    public float maxSpeed = 10f;
    public float acceleration = 50f;
    public float deceleration = 100f;
    public float airAccelerationRatio = 0.5f;
    public float airDecelerationRatio = 2f;
    public float gravity = 50f;
    public float jumpSpeed = 10f;
    protected bool canJump = true;

    public GameObject bullet;
    public Transform bulletSpawnRight;
    public Transform bulletSpawnLeft;
    protected float shotsPerSecond = 20f;
    protected float bulletSpeed = 20f;

    protected bool facingRight = true;

    protected Character2D character;
    protected Vector2 moveVector;
    protected SpriteRenderer spriteRenderer;

    protected float shotSpawnGap;
    protected WaitForSeconds shotSpawnWait;   
    protected Coroutine shootingCoroutine;
    protected float nextShotTime;

    protected Shaker cameraShaker;
    protected FuturoScript IRLSFXOMG;
    
    public Text infoText;

    void Awake ()
    {
        character = GetComponent<Character2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraShaker = Camera.main.GetComponent<Shaker>();

        IRLSFXOMG = GetComponent<FuturoScript>();
    }

    void Start ()
    {
        shotSpawnGap = 1f / shotsPerSecond;
        nextShotTime = Time.time;
        shotSpawnWait = new WaitForSeconds(shotSpawnGap);
    }

    void Update ()
    {
        // If on the ground
        if (CheckForGrounded())
        {
            GroundedHorizontalMovement();
            GroundedVerticalMovement();
            // Jump
            if (canJump && InputManager.Instance.IsHeld(InputManager.Button.Jump))
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
        if (InputManager.Instance.IsHeld(InputManager.Button.Fire1))
        {
            if (shootingCoroutine == null)
            {
                // Start shooting
                shootingCoroutine = StartCoroutine(Shoot());
                // Zoom
                cameraShaker.Zoom(5f);
                // Activate 4D effects
                if (IRLSFXOMG != null)
                    IRLSFXOMG.Activate4D(FuturoScript.Message4D.Shoot);
            }
        }
        else if (shootingCoroutine != null)
        {
            // Start shooting
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
            // Dezoom
            cameraShaker.Zoom(6f);
            // Deactivate 4D effects
            if (IRLSFXOMG != null)
            {
                IRLSFXOMG.Activate4D(FuturoScript.Message4D.Aftermath);
                Invoke("BackToAmbient", 1f);
            }
        }

        // Reset jump
        if (!canJump && !InputManager.Instance.IsHeld(InputManager.Button.Jump))
            canJump = true;

        infoText.text = "Pressed: " + InputManager.Instance.IsPressed(InputManager.Button.Jump) + "\n";
        infoText.text += "Held: " + InputManager.Instance.IsHeld(InputManager.Button.Jump) + "\n";
        infoText.text += "Released: " + InputManager.Instance.IsReleased(InputManager.Button.Jump) + "\n";
        infoText.text += "H axis: " + InputManager.Instance.GetAxis(InputManager.Axis.Horizontal).ToString() + "\n";
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
        float input = InputManager.Instance.GetAxis(InputManager.Axis.Horizontal);
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
        float input = InputManager.Instance.GetAxis(InputManager.Axis.Horizontal);
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
        while (InputManager.Instance.IsHeld(InputManager.Button.Fire1))
        {
            if (Time.time >= nextShotTime)
            {
                SpawnBullet();
                nextShotTime = Time.time + shotSpawnGap;

                if (cameraShaker != null)
                    cameraShaker.Shake(0.15f, 0.2f);
            }
            yield return null;
        }
    }

    protected void SpawnBullet ()
    {
        float facing = facingRight ? 1f : -1f;

        Vector2 spawn = facingRight ? bulletSpawnRight.position : bulletSpawnLeft.position;
        spawn.x += Random.Range(-0.1f, 0.1f);
        spawn.y += Random.Range(-0.1f, 0.1f);

        GameObject b = GameObject.Instantiate(bullet, spawn, Quaternion.identity);
        b.GetComponent<SpriteRenderer>().flipX = !facingRight;
        b.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed * facing, Random.Range(-2f, 2f));

        //float recoil = CheckForGrounded() ? 4f : 10f;// do it better by allowing to exceed maxSpeed (dampen recoil over time?)
        float recoil = 4f;
        moveVector.x = Mathf.MoveTowards(moveVector.x, moveVector.x - recoil * facing, 200f);
    }

}
