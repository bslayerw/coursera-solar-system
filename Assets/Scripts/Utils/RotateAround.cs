using Settings;
using UnityEngine;

namespace Utils
{
	public class RotateAround : MonoBehaviour {

		[Tooltip("This is the object that the script's game object will rotate around")]
		public Transform target; // the object to rotate around
		[Tooltip("This is the speed at which the object rotates")]
		public int speed; // the speed of rotation

		private void Start()
		{
			if (target != null) return;
			target = gameObject.transform;
			Debug.Log ("RotateAround target not specified. Defaulting to this GameObject");
		}

		// Update is called once per frame
		private void Update () {
			// RotateAround takes three arguments, first is the Vector to rotate around
			// second is a vector that axis to rotate around
			// third is the degrees to rotate, in this case the speed per second
			var finalSpeed = speed * UISettings.Instance.speedMultiplier;
			var transform1 = target.transform;
			transform.RotateAround(transform1.position,transform1.up,finalSpeed * Time.deltaTime);
		}
	}
}
