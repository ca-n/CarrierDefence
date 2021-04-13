using System;
using UnityEngine;

public class EnemyBomberBehaviour : EnemyBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private long delayBetweenDrops;

    protected new void Start()
    {
        base.Start();
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.forward * speed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Starts dropping bombs on approach to the carrier
        if (other.CompareTag("CarrierTrigger"))
        {
            InvokeRepeating(nameof(DropBomb), 0, delayBetweenDrops);
        }
        // TODO: When I reach world boundary, destroy me.
    }

    private void DropBomb()
    {
        GameObject bomb = Instantiate(bombPrefab);
        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        bombRigidbody.velocity = _rigidbody.velocity;
    }

    private void OnTriggerExit(Collider other)
    {
        // Stops dropping bombs after passing the carrier
        if (other.CompareTag("CarrierTrigger"))
        {
            CancelInvoke(nameof(DropBomb));
        }
    }

    private void FixedUpdate()
    {
        // TODO: Bomber flight characteristics
        // Plan: Fly in a straight line over the origin
        // the bombs will handle themselves
        
        // TODO: When shot down, fall out of the sky
        // TODO: When shot down, emit smoke/fire
    }
}