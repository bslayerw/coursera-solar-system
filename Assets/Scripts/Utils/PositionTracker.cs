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
    public class PositionTracker : MonoBehaviour
    {
        // Required
        public Transform objectToTrack = null;

        private Vector3 _positionOffset = Vector3.zero;
        private void Start()
        {
            // when we start we want to take our current position and subtract it from object to track to get our
            // offset. 
            _positionOffset = objectToTrack.transform.position - transform.position;
            Debug.Log("_positionOffset =" + _positionOffset + " objectToTrack.position =" + objectToTrack.position);
        }
        private void LateUpdate () 
        {
            if (objectToTrack is null)
            {
                return;
            }
            transform.position = objectToTrack.position - _positionOffset;
        }
    }
}