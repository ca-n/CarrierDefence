using System;
using UnityEngine;

public class EnemyBomberBehaviour : EnemyBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float delayBetweenDrops;
    private bool _bombing;

    protected new void Start()
    {
        base.Start();
        _rigidbody.useGravity = false;
        _rigidbody.velocity = transform.forward * speed;
        _rigidbody.drag = 0;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        print(other.name + " entered");
        // Starts dropping bombs on approach to the carrier
        if (other.CompareTag("BomberAttackTrigger"))
        {
            _bombing = true;
            InvokeRepeating(nameof(DropBomb), 0, delayBetweenDrops);
        }
    }

    private void DropBomb()
    {
        if (!_bombing)
        {
            CancelInvoke(nameof(DropBomb));
            return;
        }
        GameObject bomb = Instantiate(bombPrefab);
        bomb.transform.position = transform.position;
        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        bombRigidbody.velocity = _rigidbody.velocity;
        bombRigidbody.AddForce(Vector3.down * 5);
    }

    private void OnTriggerExit(Collider other)
    {
        print(other.name + " exited");

        if (other.CompareTag("HardBoundary"))
        {
            // When I reach the outer boundary, destroy me.
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        // Ensures the plane is always pointing where it's going
        transform.rotation.SetLookRotation(_rigidbody.velocity);
        
        if (_bombing)
        {
            Vector3 pos = transform.position;
            Vector3 origin = new Vector3(0f, pos.y, 0f);
            float distance = Vector3.Distance(pos, origin);
            if (distance < 10f)
            {
                _bombing = false;
            }
        }
    }
}