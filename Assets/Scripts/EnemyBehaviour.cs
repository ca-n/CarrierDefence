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
        EnemyManager.Instance.onEnemySpawned.Invoke();
        transform.LookAt(new Vector3(0f, transform.position.y, 0f));
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
