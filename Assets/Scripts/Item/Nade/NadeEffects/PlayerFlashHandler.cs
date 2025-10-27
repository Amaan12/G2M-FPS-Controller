using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerFlashHandler : MonoBehaviour
{
    public Volume postProcessVolume; // Assign your player's PostProcess Volume here (should have Bloom override)
    Bloom bloom;
    Coroutine flashRoutine;
    public float maxIntensity = 1000f;
    float originalIntensity;

    void Start()
    {
        if (postProcessVolume == null)
        {
            Debug.LogError("Post Process Volume is not assigned!");
            return;
        }

        // Get the Bloom override from the Volume profile
        if (!postProcessVolume.profile.TryGet(out bloom))
        {
            Debug.LogError("Bloom not found in Post Process Volume profile.");
        }
        else
        {
            Debug.Log("Bloom override acquired!");
        }

        originalIntensity = bloom.intensity.value;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ApplyFlash(5f);
        }
    }

    public void ApplyFlash(float duration)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine(FlashEffect(duration));
    }

    private IEnumerator FlashEffect(float duration)
    {
        float elapsed = 0f;
        Debug.Log("Flash effect started");

        // Lerp bloom intensity up quickly
        while (elapsed < 0.25f)
        {
            bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, maxIntensity, elapsed / 0.5f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = maxIntensity;

        // Lerp bloom intensity back down over the remaining duration
        elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            // Ease out curve (slow down near the end)
            t = 1f - Mathf.Pow(1f - t, 2);

            bloom.intensity.value = Mathf.Lerp(maxIntensity, originalIntensity, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        bloom.intensity.value = 0f;
        Debug.Log("Flash effect ended");
    }
}
