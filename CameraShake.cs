// Assets/player/CameraShake.cs
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            Vector3 shakeOffset = Random.insideUnitSphere * strength;
            transform.localPosition = initialPosition + shakeOffset;
            yield return null;
        }
        transform.localPosition = initialPosition;
    }
}
