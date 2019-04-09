using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    
    public DropConfig[] list;

    public void Drop ()
    {
        foreach (DropConfig drop in list)
        {
            for (int i = 0; i < drop.count; i++)
            {
                Vector3 spawn = transform.position;
                GameObject b = GameObject.Instantiate(drop.prefab, spawn, Quaternion.identity);
                Drop d = b.GetComponent<Drop>();
                if (d != null)
                {
                    d.SetMoveVector(Random.Range(-4f, 4f), 4f);
                }
            }
        }
    }

}

[System.Serializable]
public class DropConfig
{
    public GameObject prefab;
    public int count = 1;
}
