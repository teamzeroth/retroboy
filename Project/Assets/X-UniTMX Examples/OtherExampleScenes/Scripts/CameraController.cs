/*! 
 * X-UniTMX: A tiled map editor file importer for Unity3d
 * https://bitbucket.org/Chaoseiro/x-unitmx
 * 
 * Copyright 2013-2014 Guilherme "Chaoseiro" Maia
 *           2014 Mario Madureira Fontes
 */
using UnityEngine;
using System.Collections;

namespace X_UniTMX.Internal
{
	public class CameraController : MonoBehaviour
	{

		public float PixelsPerUnit = 32.0f;
		public Vector3 CamPos = Vector3.zero;
		float ortographicSize;
		public float Scale = 1.0f;
		float scrollWheel;

		// Use this for initialization
		void Start()
		{
			CamPos = Camera.main.transform.position;
			ortographicSize = Camera.main.orthographicSize;
			Scale = 1;//Screen.height / 320.0f;
		}

		// Update is called once per frame
		void Update()
		{
			ortographicSize = Screen.height / 2.0f / PixelsPerUnit / Scale;

			if (Input.GetKey(KeyCode.W))
			{
				CamPos.y += 1 / PixelsPerUnit * 10;
			}
			if (Input.GetKey(KeyCode.S))
			{
				CamPos.y -= 1 / PixelsPerUnit * 10;
			}
			if (Input.GetKey(KeyCode.A))
			{
				CamPos.x -= 1 / PixelsPerUnit * 10;
			}
			if (Input.GetKey(KeyCode.D))
			{
				CamPos.x += 1 / PixelsPerUnit * 10;
			}
			scrollWheel = Input.GetAxis("Mouse ScrollWheel");
			if (scrollWheel != 0)
			{
				Scale += scrollWheel;
				if (Scale < 0.1f)
					Scale = 0.1f;
			}
			Camera.main.transform.position = CamPos;
			Camera.main.orthographicSize = ortographicSize;
		}

		void OnGUI()
		{
			GUI.Label(new Rect(0.95f * Screen.width, 0.01f * Screen.height, 0.1f * Screen.width, 0.1f * Screen.height), "Scale:");
			Scale = GUI.VerticalSlider(new Rect(0.95f * Screen.width, 0.05f * Screen.height, 0.01f * Screen.width, 0.1f * Screen.height), Scale, 10.0f, 0.1f);
		}
	}
}
