using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Speed Parameters")]
    [SerializeField]
    private float speed;

    [SerializeField]
    private float laneSpeed;

    [Header("Jump Parameters")]
    [SerializeField]
    private float jumpLength;

    [SerializeField]
    private float jumpHeigth;

    [Header("Slide Parameters")]
    [SerializeField]
    private float slideLength;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private BoxCollider _boxCollider;

    // Horizontal Lane variables
    private int currentLane = 1;
    private Vector3 targetPosition;

    // Swipe variables
    private bool isSwiping = false;
    private Vector2 startSwipePosition;

    // Jump variables
    private bool isJumping = false;
    private float jumpStart;

    // Slide variables
    private bool isSliding = false;
    private float slideStart;
    private Vector3 _boxColliderSize;
    private Vector3 _boxColliderCenter;

    // Camera variables
    private float _cameraAnimationDuration;
    private bool startToRun = false;

    private bool isDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        _boxColliderSize = _boxCollider.size;
        _boxColliderCenter = _boxCollider.center;
        _cameraAnimationDuration = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().GetAnimationDuration();
    }

    // Update is called once per frame
    void Update()
    {

        if (isDead)
        {
            return;
        }

        if (Time.time <= _cameraAnimationDuration)
        {
            return;
        }
        else if (!startToRun)
        {
            startToRun = true;
            _animator.Play("runStart");
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MovePlayerHorizontal();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MovePlayerHorizontal(false);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        if (Input.touchCount == 1)
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
                    else
                    {
                        if (magnitude.y > 0f)
                        {
                            Jump();
                        }
                        else
                        {
                            Slide();
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

        if (isJumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if (ratio >= 1f)
            {
                isJumping = false;
                _animator.SetBool("Jumping", false);
            }
            else
            {
                this.targetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeigth;
            }
        }
        else
        {
            this.targetPosition.y = Mathf.MoveTowards(this.targetPosition.y, 0, 5 * Time.deltaTime);
        }

        if (isSliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLength;
            if (ratio > 1f)
            {
                isSliding = false;
                _animator.SetBool("Sliding", false);
                _boxCollider.size = _boxColliderSize;
                _boxCollider.center = _boxColliderCenter;
            }
        }

        Vector3 targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (!isJumping)
        {
            jumpStart = transform.position.z;
            _animator.SetFloat("JumpSpeed", speed / jumpLength);
            _animator.SetBool("Jumping", true);
            isJumping = true;
        }
    }

    private void Slide()
    {
        if (!isJumping && !isSliding)
        {
            slideStart = transform.position.z;
            _animator.SetFloat("JumpSpeed", speed / slideLength);
            _animator.SetBool("Sliding", true);
            Vector3 slideBoxColliderSize = _boxColliderSize;
            slideBoxColliderSize.y = slideBoxColliderSize.y / 2;
            _boxCollider.center = _boxColliderCenter / 2;
            _boxCollider.size = slideBoxColliderSize;
            isSliding = true;
        }
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
        if (isDead)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        if (startToRun)
        {
            _rigidbody.velocity = Vector3.forward * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<Obstacle>().Impacted();

            isDead = true;
            _animator.SetTrigger("Hit");
            _animator.SetBool("Dead", true);
        }
    }

}