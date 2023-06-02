using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalogClock : MonoBehaviour
{

    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    const float hoursToDegrees = 360 / 12;
    const float msToDegrees = 360 / 60;

    // Update is called once per frame
    void Update()
    {
        
        hourHand.localRotation = Quaternion.Euler((float)DateTime.Now.TimeOfDay.TotalHours * hoursToDegrees, 0f, -90f);
        minuteHand.localRotation = Quaternion.Euler((float)DateTime.Now.TimeOfDay.TotalMinutes * msToDegrees, 0f, -90f);
        secondHand.localRotation = Quaternion.Euler((float)DateTime.Now.TimeOfDay.TotalSeconds * msToDegrees, 0f, -90f);
    }
}
