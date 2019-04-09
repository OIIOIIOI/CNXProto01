using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthLife : MonoBehaviour
{

    public int hitPoints = 1;
    public int lives = 1;
    public UnityEvent deathCallbacks;

    protected int startingHitPoints;

    void Start ()
    {
        startingHitPoints = hitPoints;
    }

    public void TakeHit (int damage = 1)
    {
        hitPoints -= damage;
        if (hitPoints <= 0)
            LoseLife();
        Debug.Log("TH: " + hitPoints + " HP left, " + lives + " lives left");
    }

    public void LoseLife ()
    {
        lives -= 1;
        if (lives <= 0)
            Die();
        else
            hitPoints = startingHitPoints;
    }

    void Die ()
    {
        Debug.Log("DEATH");
        deathCallbacks.Invoke();
    }

}
