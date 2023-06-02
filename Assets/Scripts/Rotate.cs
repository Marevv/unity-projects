using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rotate : MonoBehaviour
{
    [SerializeField] float angle = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, angle * Time.deltaTime);

        var gameControllers = new List<UnityEngine.XR.InputDevice>();

        foreach(var device in gameControllers)
        {
            //if(device.TryGetFeatureValue(UnityEngine.XR.CommonUsages)
        }
    }
}
