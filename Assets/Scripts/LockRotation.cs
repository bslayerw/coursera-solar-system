using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private Quaternion _initRot;
    // Start is called before the first frame update
    void Start()
    {
        _initRot = transform.rotation;
    }

    void LateUpdate () {
        //If attached to box do not translate do not rotate
        if (gameObject.transform.parent != null)
        {
            transform.rotation = _initRot;
        }
    }
}
