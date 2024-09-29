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
    public float knockbackForce = 10f;


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

    public Transform player;
    public Rigidbody playerRb;
    public GameObject gunObject;
    public AudioSource gunshotAudio;
    public AudioSource reloadAudio;
    public AudioSource grappleAudio;
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
            if (Input.GetMouseButtonDown(0) && Time.time > nextFireTime)
            { //SHOOT
                Shoot();
                nextFireTime = Time.time + fireRate;
                StartCoroutine(HandleGunShootAnimation());
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && ammoCount != maxAmmoCount)
        {
            StartCoroutine(GunReload());
        }
    } //UPDATE end

    void Shoot()
    {
        if (ammoCount > 0 && !isReloading)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            Vector3 crosshairScreenPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(crosshairScreenPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 shootDirection = (hit.point - bulletSpawnPoint.position).normalized;
                shootDirection.y += 0.1f;
                shootDirection.Normalize();

                rb.velocity = shootDirection * bulletSpeed;
            }
            else
            {
                rb.velocity = bulletSpawnPoint.forward * bulletSpeed;
            }

            rb.useGravity = false;
            ammoCount--;

            UpdateAmmoText();

            if (playerRb != null)
            {
                Vector3 knockbackDirection = bulletSpawnPoint.forward;
                knockbackDirection.y = 0;
                knockbackDirection.Normalize();

                StartCoroutine(ApplySmoothKnockback(-knockbackDirection, knockbackForce, 0.5f));
            }

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


    IEnumerator ApplySmoothKnockback(Vector3 direction, float totalForce, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float forceThisFrame = (totalForce / duration) * Time.deltaTime;
            playerRb.AddForce(direction * forceThisFrame, ForceMode.VelocityChange);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }


    void UpdateAmmoText()
    {
        if (ammoCountText != null)
        {
            ammoCountText.text = ammoCount.ToString();
        }
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
        ammoCount = maxAmmoCount - 1;
        UpdateAmmoText();

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
