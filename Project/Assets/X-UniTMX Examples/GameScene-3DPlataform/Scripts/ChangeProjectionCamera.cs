using UnityEngine;
using System.Collections;

namespace X_UniTMX.Internal
{
	public class ChangeProjectionCamera : MonoBehaviour
	{


		void OnTriggerEnter(Collider other)
		{
			if (other.tag.Equals("Player"))
			{
				Camera.main.orthographic = !Camera.main.orthographic;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag.Equals("Player"))
			{
				Camera.main.orthographic = !Camera.main.orthographic;
			}
		}


	}
}
