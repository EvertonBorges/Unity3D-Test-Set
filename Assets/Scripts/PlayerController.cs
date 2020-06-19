using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private float laneSpeed;

    private Rigidbody _rigidbody;
    private int currentLane = 1;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayer();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayer(false);
        }

        Vector3 targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }

    void MovePlayer(bool isToRight = true)
    {
        int newLane = currentLane + (isToRight ? 1 : -1);
        if (newLane < 0 || newLane > 2) return;

        currentLane = newLane;
        targetPosition = new Vector3(currentLane - 1, 0f, 0f);
    }

    void FixedUpdate()
    {
        _rigidbody.velocity = Vector3.forward * speed;
    }

}