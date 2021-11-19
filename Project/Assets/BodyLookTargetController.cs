using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyLookTargetController : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private float _distanceFromBody;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = -_camera.transform.forward * _distanceFromBody;
    }
}
