using UnityEngine.UI;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player Movement//
    public float movementSpeed = 10f;
    private Rigidbody rigidBody;

    //Shooting//
    public float fireRate = 0.75f;  
    public GameObject bulletPrefab;  
    public Transform bulletPosition;
    float nextFire;
    public GameObject bulletFiringEffect;

    public AudioClip playerShootingAudio; 

    // Health
    [HideInInspector] public int health = 100; 
    public Slider healthBar;                // Healthbar slider

    void Start()
    {
        // Caching Rigidbody reference for movement
        rigidBody = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        // Handle movement 
        Move();

        // Fire when spacebar is pressed
        if (Input.GetKey(KeyCode.Space))
            Fire();
    }

    void Move()
    {
        // Skipping if no input given
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        // Get input direction
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Rotate player to face input direction
        var rotation = Quaternion.LookRotation(new Vector3(h, 0, v));
        transform.rotation = rotation;

        // Movement - move forward in facing direction
        Vector3 movementDir = transform.forward * Time.deltaTime * movementSpeed;
        rigidBody.MovePosition(rigidBody.position + movementDir);
    }

    void Fire()
    {
        if (Time.time > nextFire)
        {
            // setting fire rate
            nextFire = Time.time + fireRate;

            // Spawn bullet prefab at firing position
            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);

            // Initialize bullet trajectory
            bullet.GetComponent<BulletController>()?.InitializeBullet(transform.rotation * Vector3.forward);

            // Play firing VFX and audio
            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
            AudioManager.Instance.Play3D(playerShootingAudio, transform.position);

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detecting bullet collisions
        if (collision.gameObject.CompareTag("Bullet"))
        {
            BulletController bullet = collision.gameObject.GetComponent<BulletController>();
            TakeDamage(bullet.damage);
        }
    }

    void TakeDamage(int damage)
    {
        // Reducing health and updating UI (Healthbar)
        health -= damage;
        healthBar.value = health;

        // Handles death 0hp
        if (health <= 0)
            PlayerDied();
    }

    void PlayerDied()
    {
        // Disable player object when dead
        gameObject.SetActive(false);
    }

}