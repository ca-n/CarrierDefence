using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private float _lifetime;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _lifetime = 0;
    }

    private void FixedUpdate()
    {
        _lifetime += 0.5f;
        _rigidbody.AddForce(transform.forward * _lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ocean"))
        {
            Destroy(gameObject);
        }
    }
}
