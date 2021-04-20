using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected int speed;
    protected Rigidbody _rigidbody;
    
    // Start is called before the first frame update
    protected void Start()
    {
        //EnemyManager.Instance.onEnemySpawned.Invoke();
        transform.LookAt(new Vector3(0f, transform.position.y, 0f));
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // Insures the plane is always pointing where it's going
        transform.rotation.SetLookRotation(_rigidbody.velocity);
    }

    void CrashPlane()
    {
        // Some planes don't use gravity. When they are downed, they should fall from the sky.
        _rigidbody.useGravity = true;
        //TODO: Create smoke and flame effect
    }
}
