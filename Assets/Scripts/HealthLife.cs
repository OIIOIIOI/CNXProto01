using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthLife : MonoBehaviour
{

    public int hitPoints = 1;
    public int lives = 1;
    public UnityEvent hitCallbacks;
    public UnityEvent lifeLostCallbacks;
    public UnityEvent deathCallbacks;

    protected int startingHitPoints;

    void Start ()
    {
        startingHitPoints = hitPoints;
    }

    public void TakeHit (int damage = 1)
    {
        hitPoints -= damage;
        //Debug.Log("TH: " + hitPoints + " HP left, " + lives + " lives left");

        bool isDead = false;

        if (hitPoints <= 0)
            isDead = LoseLife();

        if (!isDead)
            hitCallbacks.Invoke();
    }

    public bool LoseLife ()
    {
        lives -= 1;

        if (lives <= 0)
            Die();
        else
            hitPoints = startingHitPoints;

        lifeLostCallbacks.Invoke();
        return lives > 0;
    }

    void Die ()
    {
        deathCallbacks.Invoke();
    }

}
