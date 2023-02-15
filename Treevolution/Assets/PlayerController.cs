using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10f; //Speed of the player

    const string horizontalAxis = "Horizontal";
    const string verticalAxis = "Vertical";

    private Rigidbody rb;

    private float horizontalInput => Input.GetAxis(horizontalAxis);
    private float verticalInput => Input.GetAxis(verticalAxis);

    private void Start()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector3(horizontalInput, rb.velocity.y, verticalInput);
    }
}