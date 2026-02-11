using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Renderer))]
public class Goal : MonoBehaviour{
	static public bool 	goalMet = false;

	void OnTriggerEnter(Collider other) {
		if (goalMet) {
			return;
		}

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

			// Let the GameManager handle round/game-complete UI and level progression
			if (GameManager.Instance != null) {
				GameManager.Instance.OnGoalHit();
			} else {
				Debug.LogError("Goal reached by Projectile, but no GameManager instance was found in the scene.");
			}
		}
	}
}
