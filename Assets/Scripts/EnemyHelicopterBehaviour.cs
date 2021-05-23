using System.Collections;
using UnityEngine;
using UnityEngine.Timeline;

public class EnemyHelicopterBehaviour : EnemyBehaviour
{
    [SerializeField] private Transform mainRotor;
    [SerializeField] private Transform tailRotor;
    [SerializeField] private GameObject rocketPrefab;
    private float _rocketX = 25f;
    private bool _attacking;
    protected new void Start()
    {
        base.Start();
        _rigidbody.useGravity = false;
        _rigidbody.drag = 0;
        _rigidbody.velocity = transform.forward * speed;
        transform.Rotate(Vector3.right * 15);
        _attacking = false;

    }

    private void FixedUpdate()
    {
        Vector3 rotorRotation = Vector3.up * 36;
        mainRotor.Rotate(rotorRotation);
        tailRotor.Rotate(rotorRotation);
        if (crashing)
        {
            _rigidbody.AddTorque(Vector3.up * 20);
        }
    }

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if (other.CompareTag("HeliAttackTrigger") && !_attacking && !crashing)
        {
            _attacking = true;
            _rigidbody.velocity = Vector3.zero;
            transform.Rotate(Vector3.left * 15);
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        while (!crashing)
        {
            FireRocket();
            yield return new WaitForSeconds(2f);
        }
    }

    private void FireRocket()
    {
        Vector3 position = transform.position + new Vector3(_rocketX, 0, 0);
        _rocketX *= -1;
        GameObject rocket = Instantiate(rocketPrefab, position, Quaternion.identity);
        rocket.transform.LookAt(Vector3.zero);
    }
}