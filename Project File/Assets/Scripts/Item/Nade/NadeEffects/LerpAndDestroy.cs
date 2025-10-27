using System.Collections;
using UnityEngine;

public class LerpAndDestroy : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LerpScaleAndDestroy());
    }

    private IEnumerator LerpScaleAndDestroy()
    {
        yield return new WaitForSeconds(10f);

        Vector3 initialScale = transform.localScale;
        float duration = 1f; // Duration of the lerp
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(gameObject); // Destroy the object after lerping
    }
}
