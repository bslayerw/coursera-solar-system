using System;
using UnityEngine;

namespace Utils
{
    public class LockRotation : MonoBehaviour
    {
        private Quaternion _initRot;
        // Start is called before the first frame update
        private void Start()
        {
            _initRot = transform.rotation;
        }

        private void LateUpdate () 
        { 
            transform.rotation = _initRot;
        }
    }
}
