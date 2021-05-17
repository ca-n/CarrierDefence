using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Types;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int speed;
    protected Rigidbody _rigidbody;
    protected bool crashing;
    
    // Start is called before the first frame update
    protected void Start()
    {
        //EnemyManager.Instance.onEnemySpawned.Invoke();
        transform.LookAt(new Vector3(0f, transform.position.y, 0f));
        _rigidbody = GetComponent<Rigidbody>();
        crashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    protected void CrashPlane()
    {
        crashing = true;
        _rigidbody.useGravity = true;
        //TODO: Create smoke and flame effect
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            Invoke(nameof(Explode), 0.25f);
        }

        if (other.CompareTag("PlayerBullet"))
        {
            health -= FighterManager.Instance.BulletDamage;
            if (!crashing && health <= 0)
            {
                this.CrashPlane();
                WaveManager.Instance.onEnemyDowned.Invoke(gameObject.tag);
            }
        }
    }

    void Explode()
    {
        //TODO: Explosion
        Destroy(gameObject);
    }
}
