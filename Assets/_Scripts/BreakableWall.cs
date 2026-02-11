using UnityEngine;
using System.Collections;

public class BreakableWall : MonoBehaviour
{
    public float fadeDuration = 10f;
    private bool isBreaking = false;

    private Renderer wallRenderer;
    private Collider wallCollider;
    private Rigidbody wallRigidbody;

    private void Awake()
    {
        wallRenderer = GetComponent<Renderer>();
        wallCollider = GetComponent<Collider>();
        wallRigidbody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile") && !isBreaking)
        {
            StartCoroutine(FadeAndDestroy());
        }
    }

    IEnumerator FadeAndDestroy()
    {
        isBreaking = true;

        float timer = 0f;
        Color originalColor = wallRenderer.material.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            wallRenderer.material.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                alpha
            );

            // Disable collider halfway (passthrough) but keep wall in place
            if (timer >= fadeDuration / 2f && wallCollider.enabled)
            {
                wallCollider.enabled = false;
                // Prevent falling: make kinematic so it stays on the ground while fading
                if (wallRigidbody != null)
                {
                    wallRigidbody.isKinematic = true;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
