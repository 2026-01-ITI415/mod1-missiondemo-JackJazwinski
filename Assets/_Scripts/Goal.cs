using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Renderer))]
public class Goal : MonoBehaviour{
	static public bool 	goalMet = false;

	void OnTriggerEnter(Collider other) {
		// Debug to confirm the trigger is firing and what hit it
		Debug.Log("Goal.OnTriggerEnter with: " + other.name);

		// When the trigger is hit by something, check to see if it's a Projectile
		// Use GetComponentInParent so it still works if the collider is on a child
		Projectile proj = other.GetComponentInParent<Projectile>();
		if (proj != null) {
			Debug.Log("Goal hit by Projectile, advancing level.");

			// If so, set goalMet = true
			Goal.goalMet = true;

			// Also set the alpha of the color to higher opacity
			Material mat = GetComponent<Renderer>().material;
			Color c = mat.color;
			c.a = 0.75f;
			mat.color = c;

			// Tell the LevelManager to load the next level after a delay
			if (LevelManager.Instance != null) {
				StartCoroutine(LoadNextLevelAfterDelay(3f));
			} else {
				Debug.LogError("Goal reached by Projectile, but no LevelManager instance was found in the scene.");
			}
		}
	}

	IEnumerator LoadNextLevelAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (LevelManager.Instance != null) {
			LevelManager.Instance.NextLevel();
		}
	}
}
