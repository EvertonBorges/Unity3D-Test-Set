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

    private bool isSwiping = false;
    private Vector2 startSwipePosition;

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
            MovePlayerHorizontal();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayerHorizontal(false);
        }

        if(Input.touchCount == 1)
        {
            if (isSwiping)
            {
                Vector2 diff = Input.GetTouch(0).position - startSwipePosition;
                Vector2 magnitude = new Vector2(diff.x / Screen.width, diff.y / Screen.height);
                if (magnitude.magnitude > 0.01f)
                {
                    if (Mathf.Abs(magnitude.x) > Mathf.Abs(magnitude.y))
                    {
                        if (magnitude.x > 0f)
                        {
                            MovePlayerHorizontal();
                        }
                        else
                        {
                            MovePlayerHorizontal(false);
                        }
                    }

                    isSwiping = false;
                }
            }

            if(Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isSwiping = true;
                startSwipePosition = Input.GetTouch(0).position;
            }

            if(Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwiping = false;
            }
        }

        Vector3 targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }

    void MovePlayerHorizontal(bool isToRight = true)
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