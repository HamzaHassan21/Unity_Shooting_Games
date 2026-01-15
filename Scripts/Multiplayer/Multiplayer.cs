using Photon.Realtime; // Needed for Player and photonView.Owner
using Photon.Pun;       // Needed for PhotonView
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun.UtilityScripts; // this adds AddScore() and GetScore()


public class Multiplayer : MonoBehaviour, IPunObservable
{
    public float movementSpeed = 10f;
    private Rigidbody rigidBody;

    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;

    float nextFire;
    public GameObject bulletFiringEffect;
    public AudioClip playerShootingAudio;

    PhotonView photonView;

    [HideInInspector]
    public int health = 100;

    public Slider healthBar;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        Debug.Log("IsMine: " + photonView.IsMine);
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Move();

        if (Input.GetKey(KeyCode.Space))
            photonView.RPC("Fire", RpcTarget.AllViaServer);
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0, v);

        if (inputDir != Vector3.zero)
        {
            // Rotate instantly to face the input direction
            Quaternion targetRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = targetRotation;

            // Move forward in the facing direction
            Vector3 movementDir = transform.forward * movementSpeed * Time.deltaTime;
            rigidBody.MovePosition(rigidBody.position + movementDir);
        }
    }


    [PunRPC]
    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
            bullet.GetComponent<MultiplayerBulletController>()?.InitialiseBullet(transform.rotation * Vector3.forward, photonView.Owner);

            AudioManager.Instance.Play3D(playerShootingAudio, transform.position);
            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            MultiplayerBulletController bullet = collision.gameObject.GetComponent<MultiplayerBulletController>();
            TakeDamage(bullet);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            healthBar.value = health;
        }
    }

    void TakeDamage(MultiplayerBulletController bullet)
    {
        health -= bullet.damage;
        healthBar.value = health;

        if (health <= 0)
        {
            bullet.owner.AddScore(1); 
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        health = 100;
        healthBar.value = health;
    }
}