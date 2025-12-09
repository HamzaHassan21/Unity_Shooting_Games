using Photon.Pun;
using Photon.Realtime;           //  ensures Player is Photon’s Player
using Photon.Pun.UtilityScripts; //  ensures AddScore() works
using UnityEngine;

public class MultiplayerBulletController : MonoBehaviourPunCallbacks
{
    Rigidbody rigidBody;
    public float bulletSpeed = 15f;
    public int damage = 10;
    public AudioClip BulletHitAudio;
    public GameObject bulletImpactEffect;

    [HideInInspector]
    public Photon.Realtime.Player owner; 

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void InitialiseBullet(Vector3 originalDirection, Photon.Realtime.Player givenPlayer)
    {
        transform.forward = originalDirection;
        rigidBody.linearVelocity = transform.forward * bulletSpeed;
        owner = givenPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        Destroy(gameObject);
    }
}