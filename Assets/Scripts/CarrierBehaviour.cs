using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Events;
using Managers;
using UnityEngine;

public class CarrierBehaviour : MonoBehaviour
{
    public CarrierDamagedEvent onCarrierDamaged;
    public int Health { get; private set; }
    private const int MaxHealth = 10000;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip rocketExplosion;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject hugeExplosion;
    
    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        onCarrierDamaged.AddListener(UIManager.Instance.HandleCarrierDamaged);
        _audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("EnemyRocket"))
        {
            DamageCarrier(250);
            Instantiate(explosion, other.transform.position, Quaternion.Euler(Vector3.left * 90));
            
            _audioSource.PlayOneShot(rocketExplosion);
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))
        {
            DamageCarrier(FighterManager.Instance.BulletDamage);
            Destroy(other.gameObject);
        }
    }

    public void DamageCarrier(int damage)
    {
        Health = Mathf.Clamp(Health - damage, 0, MaxHealth);
        onCarrierDamaged.Invoke(Health / (float) MaxHealth);
        if (Health == 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Instantiate(hugeExplosion, transform.position, Quaternion.Euler(Vector3.left * 90));
        Invoke(nameof(CallGameOver), 0.1f);
    }

    private void CallGameOver()
    {
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
}
