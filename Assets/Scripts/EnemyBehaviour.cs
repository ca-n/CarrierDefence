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
    [SerializeField] private GameObject fire;
    [SerializeField] private GameObject explosion;
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
        Instantiate(fire, transform);
        Instantiate(fire, transform);
        Instantiate(fire, transform);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            Explode();
        }

        if (other.CompareTag("PlayerBullet"))
        {
            Debug.Log("hit hit");
            health -= FighterManager.Instance.BulletDamage;
            if (!crashing && health <= 0)
            {
                this.CrashPlane();
                WaveManager.Instance.onEnemyDowned.Invoke(gameObject.tag);
            }
        }
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.Euler(Vector3.left * 90));
        Destroy(gameObject);
    }
}
