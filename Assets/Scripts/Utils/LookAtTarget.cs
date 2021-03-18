#region MIT License

// # Released under MIT License
// 
// Copyright (c) 2021 Byron Wright.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the
// following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#endregion

using UnityEngine;

namespace Utils
{
    public class LookAtTarget : MonoBehaviour
    {
        public delegate void ObjectSelectDelegate(GameObject go);
        public ObjectSelectDelegate ObjectClickDelegate;
        [Tooltip("This is the object that the script's game object will look at by default")]
        public GameObject defaultTarget; // the default target that the camera should look at

        [Tooltip(
            "This is the object that the script's game object is currently look at based on the player clicking on a gameObject"
            )]
        public GameObject currentTarget; // the target that the camera should look at

        // Start happens once at the beginning of playing. This is a great place to setup the behavior for this gameObject
        private void Start()
        {
            if (defaultTarget == null)
            {
                defaultTarget = gameObject;
                Debug.Log("defaultTarget target not specified. Defaulting to parent GameObject");
            }

            if (currentTarget == null)
            {
                currentTarget = gameObject;
                Debug.Log("currentTarget target not specified. Defaulting to parent GameObject");
            }
        }

        // Update is called once per frame
        // For clarity, Update happens constantly as your game is running
        private void Update()
        {
            // if primary mouse button is pressed
            if (Input.GetMouseButtonDown(0))
            {
                // determine the ray from the camera to the mousePosition
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // cast a ray to see if it hits any gameObjects
                RaycastHit[] hits;
                hits = Physics.RaycastAll(ray);

                // if there are hits
                if (hits.Length > 0)
                {
                    // get the first object hit
                    var hit = hits[0];
                    currentTarget = hit.collider.gameObject;
                    if (ObjectClickDelegate != null)
                    {
                        ObjectClickDelegate(currentTarget);
                    }
                    
                    // delegates can call this behaviors current target to get the new look at.
                    
                    Debug.Log("currentTarget changed to " + currentTarget.name);
                }
            }
            else if (Input.GetMouseButtonDown(1)) // if the second mouse button is pressed
            {
                currentTarget = defaultTarget;
                Debug.Log("defaultTarget changed to " + currentTarget.name);
            }

            // if a currentTarget is set, then look at it
            if (currentTarget != null)
            {
                // transform here refers to the attached gameobject this script is on.
                // the LookAt function makes a transform point it's Z axis towards another point in space
                // In this case it is pointing towards the target.transform
                transform.LookAt(currentTarget.transform);
            }
            else // reset the look at back to the default
            {
                currentTarget = defaultTarget;
                Debug.Log("defaultTarget changed to " + currentTarget.name);
            }
        }
    }
}