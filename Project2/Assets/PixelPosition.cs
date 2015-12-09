using UnityEngine;
using System.Collections;

public class PixelPosition : MonoBehaviour {

	int pixelToUnit = 64;

	float pixelX;
	float pixelY;

	void LateUpdate () {

		pixelX = Mathf.Floor(transform.position.x * pixelToUnit) / pixelToUnit;
		pixelY = Mathf.Floor(transform.position.y * pixelToUnit) / pixelToUnit;

		transform.position = new Vector3 (pixelX, pixelY, transform.position.z);
	}
}
