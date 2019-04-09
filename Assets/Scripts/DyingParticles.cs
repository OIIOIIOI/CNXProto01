using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DyingParticles : MonoBehaviour
{

    void Start()
    {
        float delay = 0.5f;

        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps != null)
            delay = ps.main.duration;

        Invoke("DieForReal", delay);
    }
    
    void DieForReal ()
    {
        Destroy(gameObject);
    }

}
