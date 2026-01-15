using UnityEngine;

public class BulletController : MonoBehaviour
{
    Rigidbody rigidBody;
    public float bulletSpeed = 15f;

    public int damage = 10;

    public AudioClip BulletHitAudio;

    public GameObject bulletImpactEffect;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        transform.forward = originalDirection;
        rigidBody.linearVelocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        Destroy(gameObject);
        
    }
}