using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    
    private Vector2 _input;

    private Animator _animator;
    
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        //StartCoroutine(Move(targetPos));
    }

    private void Move()
    {
                
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        
        if (_input.x != 0) _input.y = 0;


        if (_input == Vector2.zero)
        {
            _animator.SetBool(IsMoving, false);
            return;
        }

        _animator.SetFloat(MoveX, _input.x);
        _animator.SetFloat(MoveY, _input.y);
        _animator.SetBool(IsMoving, true);
        
        var currentPos = transform.position;
        var targetPos = currentPos;
        targetPos.x += _input.x;
        targetPos.y += _input.y;

        transform.position = Vector2.MoveTowards(currentPos, targetPos, moveSpeed * Time.deltaTime);

    }

    // IEnumerator Move(Vector3 targetPos)
    // {
    //     _isMoving = true;
    //     while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
    //     {
    //         transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    //         yield return null;
    //     }
    //
    //     transform.position = targetPos;
    //     _isMoving = false;
    // }
}
