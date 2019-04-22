using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public GameObject[] list;

    protected float nextSpawnTime;
    protected List<GameObject> children;

    void Start()
    {
        nextSpawnTime = Time.time + Random.Range(2f, 4f);

        children = new List<GameObject>();

        StartCoroutine(Spawn());
    }

    void FixedUpdate ()
    {
        List<GameObject> deadChildren = new List<GameObject>();
        foreach (GameObject child in children)
        {
            if (child == null)
                deadChildren.Add(child);
        }
        foreach (GameObject child in deadChildren)
        {
            children.Remove(child);
        }
    }

    protected IEnumerator Spawn ()
    {
        while (true)
        {
            if (Time.time >= nextSpawnTime && children.Count < 1)
            {
                Vector3 spawn = transform.position;
                GameObject b = GameObject.Instantiate(list[Random.Range(0, list.Length)], spawn, Quaternion.identity);
                children.Add(b);

                nextSpawnTime = Time.time + Random.Range(2f, 4f);
            }
            yield return null;
        }
    }

}
