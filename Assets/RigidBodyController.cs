﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class RigidBodyController : MonoBehaviour
{


    public float Speed = 0.05f;
    public float JumpHeight = 0.2f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
    private float fallMultiplierFloat = 3f;
    private float lowJumpMultiplierFloat = 2f;
    enum Mode
    {
        Walk,
        Run
    };
    private Mode curMode = Mode.Walk;
    private Animator _anim;
    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    //private bool _isGrounded = true;
    //private Transform _groundChecker;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _body = GetComponent<Rigidbody>();
        //_groundChecker = transform.GetChild(0);
        _body.detectCollisions = true;
    }

    void Update()
    {
        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);


        _inputs = Vector3.zero;
        _inputs.x = CrossPlatformInputManager.GetAxis("Horizontal");
        _inputs.z = CrossPlatformInputManager.GetAxis("Vertical");
        // This is for facing direction
        if (_inputs != Vector3.zero)
            transform.forward = _inputs;

        //faster falling
        if (_body.velocity.y < 0)
        {
            _body.velocity += Vector3.up* Physics.gravity.y * (fallMultiplierFloat - 1) * Time.deltaTime;
        }

        //control jump height by length of time jump button held
        if (_body.velocity.y > 0 && !Input.GetButton("Jump")) {
            _body.velocity += Vector3.up* Physics.gravity.y * (lowJumpMultiplierFloat - 1) * Time.deltaTime;
        }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            StartCoroutine("Jumping");
        }
        //if (Input.GetButtonDown("Dash"))
        //{
        //    Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime)));
        //    _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        //}
        if (!_inputs.x.Equals(0) || !_inputs.y.Equals(0)){
            if (curMode == Mode.Walk)
            {
                Speed = 0.05f;
                _anim.SetTrigger("walk");
            } else {
                Speed = 0.15f;
                _anim.SetTrigger("run");
            }
          } else {
              _anim.SetTrigger("idle");
          }

        if (CrossPlatformInputManager.GetButtonUp("RunWalk")){
            if (curMode == Mode.Run)
            {
                curMode = Mode.Walk;
            }
            else
            {
                curMode = Mode.Run;
            }
        }
    }


    void FixedUpdate()
{
    _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
}

    IEnumerator Jumping() {
        _anim.SetTrigger("jump");
        yield return new WaitForSeconds(0.5f);
        _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);

    }   
}
