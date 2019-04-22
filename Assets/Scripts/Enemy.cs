using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character2D))]

public class Enemy : MonoBehaviour
{

    public GameObject deathParticles;

    protected ParticleSystem hitParticles;
    protected bool alive = false;

    private void OnEnable()
    {
        alive = true;
    }

    public virtual void Start()
    {
        hitParticles = GetComponent<ParticleSystem>();
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
