using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character2D))]

public class EnemyA : MonoBehaviour
{

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void Die ()
    {
        Destroy(gameObject);
    }

}
