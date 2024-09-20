using System.Collections;
using UnityEngine;
using TMPro;


public class Gun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 40f;
    public float bulletDrop = 0.1f;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public int maxAmmoCount = 6;
    public float ammoCount = 6f;
    public TextMeshProUGUI ammoCountText;

    public float maxCylinderIntensity = 4.26f;
    public float maxTorusIntensity = 4.26f;
    public float maxSphereIntensity = 4.32f;
    public float minIntensity = 4.20f;
    public float resetIntensity = 4.26f;

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
    public AudioSource gunshotAudio;
    public AudioSource reloadAudio;
    public AudioSource grappleAudio;

    private GameObject currentGrappleBall;
    private bool isSwinging = false;
    private Coroutine currentGrappleCoroutine;
    private bool isGrappling = false;
    private bool isReloading = false;

    void Start()
    {
        UpdateEmissionIntensity(minIntensity);
        UpdateAmmoText();
    }

    void Update()
    {
        if (!isReloading)
        {
            if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime) // Left mouse click
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
                StartCoroutine(HandleGunShootAnimation());
            }

            if (Input.GetMouseButtonDown(1)) // Right mouse click
            {
                Grapple();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ammoCount != maxAmmoCount)
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
        if (ammoCount > 0 && !isReloading)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            StartCoroutine(BulletDrop(rb));

            rb.useGravity = false;
            ammoCount--;

            UpdateAmmoText();

            if (gunshotAudio != null)
            {
                gunshotAudio.time = 0.3f;
                gunshotAudio.Play();
            }

            if (ammoCount == 0)
            {
                StartCoroutine(FadeEmissionToZero());
            }
            else
            {
                UpdateEmissionIntensity(resetIntensity);
            }

            StartCoroutine(HandleGunShootAnimation());
        }
    }

    void UpdateAmmoText()
    {
        if (ammoCountText != null)
        {
            ammoCountText.text = ammoCount.ToString();
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

            if (grappleAudio != null)
            {
                grappleAudio.time = 0.4f;
                grappleAudio.Play();
            }

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

        if (grappleAudio != null)
        {
            grappleAudio.Stop();
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

    IEnumerator HandleGunShootAnimation()
    {
        if (ammoCount > 0 && !isReloading)
        {
            Animator animator = gunObject.GetComponent<Animator>();
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("gunShoot") && stateInfo.normalizedTime < 1.0f)
            {
                yield break;
            }

            animator.Play("gunShoot");

            yield return new WaitForSeconds(0.3f);

            animator.Play("New State");
        }
    }

    IEnumerator GunReload()
    {
        isReloading = true;

        gunObject.GetComponent<Animator>().Play("gunReload");

        Debug.Log("Reloading...");

        float startIntensity = minIntensity;
        float endIntensity = resetIntensity;
        float reloadDuration = 1.5f;
        float elapsed = 0f;

        // Play gunshot sound
        if (reloadAudio != null)
        {
            reloadAudio.time = 0.1f;
            reloadAudio.Play();
        }

        while (elapsed < reloadDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / reloadDuration));

            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, t);

            UpdateEmissionIntensity(currentIntensity);

            yield return null;
        }

        UpdateEmissionIntensity(endIntensity);

        // Restore ammo
        ammoCount = maxAmmoCount;

        isReloading = false;
        Debug.Log("Reload Complete");
    }


    void UpdateEmissionIntensity(float intensity)
    {

        float currentCylinderIntensity = Mathf.Clamp(intensity, minIntensity, resetIntensity);
        float currentTorusIntensity = Mathf.Clamp(intensity, minIntensity, resetIntensity);
        float currentSphereIntensity = Mathf.Clamp(intensity, minIntensity, resetIntensity);

        Debug.Log("Cylinder Intensity: " + currentCylinderIntensity);
        Debug.Log("Torus Intensity: " + currentTorusIntensity);
        Debug.Log("Sphere Intensity: " + currentSphereIntensity);

        cylinderMaterial.SetColor("_EmissionColor", Color.yellow * (currentCylinderIntensity + 10));
        torusMaterial.SetColor("_EmissionColor", Color.blue * (currentTorusIntensity + 10.5f));
        sphereMaterial.SetColor("_EmissionColor", Color.red * (currentSphereIntensity + 10.5f));
    }
    IEnumerator FadeEmissionToZero()
    {
        float elapsed = 0f;
        float fadeDuration = 2f;
        float startIntensity = resetIntensity;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float currentIntensity = Mathf.Lerp(startIntensity, minIntensity, elapsed / fadeDuration);

            UpdateEmissionIntensity(currentIntensity);

            yield return null;
        }

        UpdateEmissionIntensity(minIntensity);
    }
}
