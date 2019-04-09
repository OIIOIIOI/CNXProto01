using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character2D))]

public class EnemyA : MonoBehaviour
{

    public GameObject deathParticles;

    protected ParticleSystem hitParticles;
    protected bool alive = false;

    private void OnEnable()
    {
        alive = true;
    }

    void Start()
    {
        hitParticles = GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        
    }

    public void TakeHit ()
    {
        if (!alive)
            return;

        if (hitParticles != null)
            hitParticles.Play();
    }

    public void Die ()
    {
        if (!alive)
            return;

        alive = false;

        Vector3 spawn = transform.position;
        GameObject.Instantiate(deathParticles, spawn, Quaternion.identity);

        DieForReal();
    }

    public void DieForReal ()
    {
        Destroy(gameObject);
    }

}
