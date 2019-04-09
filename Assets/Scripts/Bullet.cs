using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public LayerMask hittableLayers;
    public SpriteRenderer spriteRenderer;
    public Collider2D bulletCollider;
    public ParticleSystem particles;

    protected bool alive = false;
    protected bool destroyWhenOutOfView = true;
    protected float timeBeforeAutodestruct = 1.0f;
    protected float timer;
    protected ContactFilter2D contactFilter;

    [HideInInspector]
    public int damage = 1;

    private void OnEnable()
    {
        timer = 0.0f;
        alive = true;
    }

    void Awake ()
    {
        contactFilter.layerMask = hittableLayers;
        contactFilter.useLayerMask = true;
    }

    void FixedUpdate ()
    {
        if (destroyWhenOutOfView)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
            bool onScreen = screenPoint.z > 0 &&
                screenPoint.x > -0.01f &&
                screenPoint.x < 1 + 0.01f &&
                screenPoint.y > -0.01f &&
                screenPoint.y < 1 + 0.01f;
            if (!onScreen)
                Destroy(this.gameObject);
        }

        if (timeBeforeAutodestruct > 0f)
        {
            timer += Time.deltaTime;
            if (timer > timeBeforeAutodestruct)
                Destroy(this.gameObject);
        }

        if (alive && Physics2D.IsTouching(bulletCollider, contactFilter))
        {
            Collider2D[] targets = new Collider2D[1];
            Physics2D.GetContacts(bulletCollider, contactFilter, targets);
            if (targets.Length > 0)
            {
                Collider2D coll = targets[0];
                switch (coll.tag)
                {
                    case "Player":
                        //Debug.Log("hit player");
                        break;

                    case "Enemy":
                        //Debug.Log("hit enemy");
                        HealthLife hl = coll.gameObject.GetComponent<HealthLife>();
                        if (hl != null)
                            hl.TakeHit(damage);
                        break;

                    default:
                        break;
                }
            }

            alive = false;

            bulletCollider.enabled = false;
            spriteRenderer.enabled = false;

            if (particles != null)
                particles.Play();

            Invoke("DieForReal", 2f);
        }
    }

    void DieForReal ()
    {
        Destroy(this.gameObject);
    }

}
