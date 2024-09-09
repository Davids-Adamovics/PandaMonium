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

    public int maxAmmoCount = 13;
    public float ammoCount = 12f;

    // Gun emission intensities
    public float maxCylinderIntensity = 4.26f;
    public float maxTorusIntensity = 4.26f;
    public float maxSphereIntensity = 4.32f;
    public float minIntensity = 4f;

    // Material references
    public Material cylinderMaterial;
    public Material torusMaterial;
    public Material sphereMaterial;

    public GameObject grappleBallPrefab;
    public float grappleRange = 30f;
    public float grappleSpeed = 50f;
    public float playerMoveSpeed = 20f;
    public float swingForce = 5f;
    public LineRenderer grappleLine;
    public Transform player;
    public Rigidbody playerRb;
    public GameObject gunObject;

    private GameObject currentGrappleBall;
    private bool isSwinging = false;
    private Coroutine currentGrappleCoroutine;
    private bool isGrappling = false;
    private bool isReloading = false;


    void Start()
    {
        UpdateEmissionIntensity();
    }
    void Update()
    {
        if (!isReloading)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
                StartCoroutine(GunShoot());
            }

            if (Input.GetButtonDown("Fire2"))
            {
                Grapple();
            }
        }


        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(GunReload());
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
        if (ammoCount > 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            StartCoroutine(BulletDrop(rb));

            rb.useGravity = false;
            ammoCount--;
            UpdateEmissionIntensity(); // Update intensity after shooting
        }
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

    IEnumerator GunShoot()
    {
        gunObject.GetComponent<Animator>().Play("gunShoot");
        yield return new WaitForSeconds(1.0f);
        gunObject.GetComponent<Animator>().Play("New State");
    }

    IEnumerator GunReload()
    {
        isReloading = true;
        gunObject.GetComponent<Animator>().CrossFade("gunReload", 0.1f);

        // Wait for the reload animation to finish
        yield return new WaitForSeconds(gunObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        ammoCount = maxAmmoCount; // Reset ammo
        UpdateEmissionIntensity(); // Update intensity after reload
        isReloading = false;
    }


    void UpdateEmissionIntensity()
    {
        float intensityDecreasePerShot = 0.05f;
        float resetIntensity = 4.26f;

        // Calculate the intensity based on ammo count
        float intensityFactor = ammoCount / maxAmmoCount;

        // Calculate current intensity, ensure it doesn’t drop below minIntensity
        float currentCylinderIntensity = Mathf.Clamp(resetIntensity - (resetIntensity - minIntensity) * (1 - intensityFactor), minIntensity, resetIntensity);
        float currentTorusIntensity = Mathf.Clamp(resetIntensity - (resetIntensity - minIntensity) * (1 - intensityFactor), minIntensity, resetIntensity);
        float currentSphereIntensity = Mathf.Clamp(resetIntensity - (resetIntensity - minIntensity) * (1 - intensityFactor), minIntensity, resetIntensity);

        // Log intensity for debugging
        Debug.Log("Cylinder Intensity: " + currentCylinderIntensity);
        Debug.Log("Torus Intensity: " + currentTorusIntensity);
        Debug.Log("Sphere Intensity: " + currentSphereIntensity);

        // Update emission intensity
        cylinderMaterial.SetColor("_EmissionColor", Color.yellow * (currentCylinderIntensity+10.5f));
        torusMaterial.SetColor("_EmissionColor", Color.blue * (currentTorusIntensity+11));
        sphereMaterial.SetColor("_EmissionColor", Color.red * (currentSphereIntensity+11));
    }


}
