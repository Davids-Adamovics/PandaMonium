using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public float bulletDrop = 0.1f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public GameObject grappleBallPrefab;
    public float grappleRange = 30f;
    public float grappleSpeed = 50f;
    public float playerMoveSpeed = 20f;
    public float swingForce = 5f;
    public LineRenderer grappleLine;
    public Transform player;
    public Rigidbody playerRb;

    private GameObject currentGrappleBall;
    private bool isSwinging = false;
    private Coroutine currentGrappleCoroutine;
    private bool isGrappling = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time > nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Grapple();
        }

        if (Input.GetMouseButtonDown(1) && isGrappling)
        {
            StopGrapple();
        }

        if (isSwinging)
        {
            Swing();
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
        StartCoroutine(BulletDrop(rb));

        rb.useGravity = false;
    }


    void Grapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grappleRange))
        {
            if (currentGrappleCoroutine != null)
            {
                StopCoroutine(currentGrappleCoroutine);
            }

            if (currentGrappleBall != null)
            {
                Destroy(currentGrappleBall);
            }

            currentGrappleBall = Instantiate(grappleBallPrefab, bulletSpawnPoint.position, Quaternion.identity);

            grappleLine.enabled = true;
            grappleLine.positionCount = 2;
            grappleLine.SetPosition(0, bulletSpawnPoint.position);
            grappleLine.SetPosition(1, currentGrappleBall.transform.position);

            currentGrappleCoroutine = StartCoroutine(GrappleMove(currentGrappleBall, hit.point));
        }
    }

    IEnumerator GrappleMove(GameObject grappleBall, Vector3 target)
    {
        while (Vector3.Distance(grappleBall.transform.position, target) > 0.1f)
        {
            grappleBall.transform.position = Vector3.MoveTowards(grappleBall.transform.position, target, grappleSpeed * Time.deltaTime);

            grappleLine.SetPosition(0, bulletSpawnPoint.position);
            grappleLine.SetPosition(1, grappleBall.transform.position);

            yield return null;
        }

        isSwinging = true;
        currentGrappleCoroutine = StartCoroutine(MovePlayerToGrapple(target));
    }

    IEnumerator MovePlayerToGrapple(Vector3 target)
    {
        while (Vector3.Distance(player.position, target) > 1f)
        {
            player.position = Vector3.MoveTowards(player.position, target, playerMoveSpeed * Time.deltaTime);

            grappleLine.SetPosition(0, bulletSpawnPoint.position);
            grappleLine.SetPosition(1, target);

            yield return null;
        }

        grappleLine.enabled = false;
        if (currentGrappleBall != null)
        {
            Destroy(currentGrappleBall);
        }

        isSwinging = false;
        isGrappling = false;
    }

    void Swing()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        playerRb.AddForce(Vector3.right * horizontalInput * swingForce, ForceMode.Acceleration);
    }

    IEnumerator BulletDrop(Rigidbody bulletRb)
    {
        while (bulletRb != null)
        {
            bulletRb.velocity += Vector3.down * bulletDrop * Time.deltaTime;
            yield return null;
        }
    }

    void StopGrapple()
    {
        if (currentGrappleCoroutine != null)
        {
            StopCoroutine(currentGrappleCoroutine);
            currentGrappleCoroutine = null;
        }

        grappleLine.enabled = false;
        if (currentGrappleBall != null)
        {
            Destroy(currentGrappleBall);
        }

        isSwinging = false;
        isGrappling = false;
    }
}
