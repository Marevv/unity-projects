using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 200f;
    [SerializeField] private InputActionReference movementInputActionReference;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        if (!_rigidbody)
            _rigidbody = GetComponent<Rigidbody2D>();
        movementInputActionReference.action.Enable();
    }

    private void FixedUpdate()
    {
        _rigidbody.MovePosition(_rigidbody.position +
                                new Vector2(0.0f, movementInputActionReference.action.ReadValue<float>()) *
                                (speed * Time.fixedDeltaTime));
    }
}