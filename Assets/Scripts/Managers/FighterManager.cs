using System;
using System.Collections;
using Types;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace Managers
{
    public class FighterManager : MonoSingleton<FighterManager>
    {
        [SerializeField] private int health;
        [SerializeField] private GameObject[] guns;
        [SerializeField] private GameObject[] thrusters;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private FighterSurfacesControl controlSurfaces;
        [SerializeField] private float pitchMultiplier;
        [SerializeField] private float yawMultiplier;
        [SerializeField] private float rollMultiplier;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float minSpeed;
        private int _fireRate;
        private float _speed;
        private bool _firing;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private AudioSource _audioSource;
        [SerializeField] private AudioClip fireSound;
        [SerializeField] private AudioClip explosionSound;
        [SerializeField] private GameObject explosion;

        private void Start()
        {
            _speed = 7.5f;
            _firing = false;
            UpgradeGuns(ScoreManager.Instance.GunLevel);
            UpgradeBulletDamage(ScoreManager.Instance.BulletLevel);
            UpgradeFireRate(ScoreManager.Instance.FireRateLevel);
            _audioSource = GetComponent<AudioSource>();
            foreach (var thruster in thrusters)
            {
                var lr = thruster.AddComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startWidth = 2;
                lr.endWidth = 0;
                lr.startColor = Color.cyan;
                lr.endColor = Color.clear;
                lr.useWorldSpace = false;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, Vector3.forward * 2);
                lr.enabled = true;
            }
        }

        public int BulletDamage { get; private set; }

        private IEnumerator Fire()
        {
            _firing = true;
            do
            {
                foreach (GameObject gun in guns)
                {
                    if (gun.activeSelf)
                    {
                        FireGun(gun);
                    }
                }
                yield return new WaitForSeconds(1.0f / (2.0f + _fireRate));
            } while (Input.GetMouseButton(0));
            _firing = false;
        }

        private void FireGun(GameObject gun)
        {
            GameObject bullet = Instantiate(bulletPrefab, gun.transform.position + gun.transform.up, gun.transform.rotation);
            TrailRenderer tr = bullet.GetComponent<TrailRenderer>();
            Color c = GetTracerColor();
            tr.startColor = c;
            tr.material.SetColor(EmissionColor, c);
            tr.widthMultiplier = 2;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddRelativeForce(Random.Range(0f, 1000f), 100000, Random.Range(0f, 1000f));
            _audioSource.PlayOneShot(fireSound);
            Destroy(bullet, 5f);
        }

        private Color GetTracerColor()
        {
            switch (ScoreManager.Instance.BulletLevel)
            {
                case 1:
                    // Yellow
                    return Color.yellow;
                case 2:
                    // Red
                    return Color.red;
                case 3:
                    // Blue
                    return Color.blue;
                case 4:
                    // Purple
                    return new Color(0.5f, 0, 1);
                default:
                    // Random
                    return GetRandomColor();
            }
        }

        private Color GetRandomColor()
        {
            switch (Random.Range(0, 6))
            {
                case 0:
                    return Color.red;
                case 1:
                    return Color.yellow;
                case 2:
                    return Color.green;
                case 3:
                    return Color.cyan;
                case 4: 
                    return Color.blue;
                case 5:
                    return Color.magenta;
                default:
                    return Color.black;
            }
        }

        private void FixedUpdate()
        {
            _speed = Mathf.Clamp(_speed - (transform.forward.y * 0.025f), minSpeed, maxSpeed);
            transform.Translate(Vector3.forward * _speed);
            // X = pitch, Y = yaw, Z = roll
            transform.Rotate(controlSurfaces.MRx * pitchMultiplier, -controlSurfaces.MRy * yawMultiplier, controlSurfaces.MRz * rollMultiplier);
            UpdateChaseCam();

            if (Input.GetMouseButtonDown(0) && !_firing)
            {
                StartCoroutine(Fire());
            }
        }

        private void UpdateChaseCam()
        {
            Transform cam = Camera.main.transform;
            Vector3 next = transform.position - transform.forward * 10 + Vector3.up * 10;
            Vector3 current = cam.position;
            float bias = 0.9f;
            cam.position = current * bias + next * (1.0f - bias);
            cam.LookAt(transform.position + transform.forward * 50f);
        }

        public void UpgradeGuns(int level)
        {
            // LEVEL 1: 0 + 1
            // LEVEL 2: 2 + 3
            // LEVEL 3: 4 + 5
            level = Mathf.Clamp(level, 1, 3);
            for (int i = 0; i < 2 * level; ++i)
            {
                guns[i].SetActive(true);
            }
        }

        public void UpgradeBulletDamage(int level)
        {
            level = Mathf.Max(1, level);
            BulletDamage = 25 * level;
        }

        public void UpgradeFireRate(int level)
        {
            level = Mathf.Max(1, level);
            _fireRate = level;
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "SoftBoundary":
                    UIManager.Instance.HideSoftBoundaryText();
                    break;
                case "Ocean":
                    Invoke(nameof(Explode), 0.25f);
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.gameObject.tag)
            {
                case "SoftBoundary":
                    UIManager.Instance.ShowSoftBoundaryText();
                    break;
                case "HardBoundary":
                    Explode();
                    break;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            switch (other.gameObject.tag)
            {
                case "Carrier":
                case "EnemyAwacs":
                case "EnemyHawkeye":
                case "EnemySeahawk":
                    Explode();
                    break;
            }
        }

        private void Explode()
        {
            Instantiate(explosion, transform.position, Quaternion.Euler(Vector3.left * 90));
            _audioSource.PlayOneShot(explosionSound);
            Invoke(nameof(CallGameOver), 0.1f);
        }

        private void CallGameOver()
        {
            GameManager.Instance.GameOver();
            Destroy(gameObject);
        }
    }
}