using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    [Header("Speed Parameters")]

    [Range(5, 20)]
    [SerializeField]
    private int minSpeed;

    [Range(20, 40)]
    [SerializeField]
    private int maxSpeed;

    [Range(1f, 1.5f)]
    [SerializeField]
    private float speedMultiplier;

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

    [Header("Camera Parameters")]
    [Range(0f, 3f)]
    [SerializeField]
    private float timeToStartAfterCameraLock;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private BoxCollider _boxCollider;
    private GameController _gameController;

    // Speed variables
    private float _speed;

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
    private float _startTimeToLockCamera;
    private float _startTimeToRun;
    private bool _cameraLock = false;
    private bool _startToRun = false;

    // Death variables
    private bool isDead = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        _boxColliderSize = _boxCollider.size;
        _boxColliderCenter = _boxCollider.center;
        _cameraAnimationDuration = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().GetAnimationDuration();

        _speed = minSpeed;

        _startTimeToLockCamera = Time.time + _cameraAnimationDuration;
        _startTimeToRun = _startTimeToLockCamera + 3f;
    }

    void Update()
    {

        if (isDead)
        {
            this.targetPosition.y = Mathf.MoveTowards(this.targetPosition.y, 0, 5 * Time.deltaTime);
            Move();
            return;
        }

        if (!_cameraLock && Time.time >= _startTimeToLockCamera)
        {
            _animator.Play("Start");
            _cameraLock = true;
        }

        if (!_startToRun && Time.time >= _startTimeToRun)
        {
            _animator.Play("runStart");
            _startToRun = true;
        }

        if (!_cameraLock || !_startToRun)
        {
            return;
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
            float ratio = (transform.position.z - jumpStart) / (!isSliding ? jumpLength : jumpLength / 3);
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
        }

        Move();
    }

    private void Move()
    {
        Vector3 targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }

    private void Jump()
    {
        if (!isJumping)
        {
            jumpStart = transform.position.z;
            _animator.SetFloat("JumpSpeed", _speed / jumpLength);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Sliding", false);
            isJumping = true;
            isSliding = false;
        }
    }

    private void Slide()
    {
        if (!isSliding)
        {
            slideStart = transform.position.z;
            _animator.SetFloat("JumpSpeed", _speed / slideLength);
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

        if (_startToRun)
        {
            _rigidbody.velocity = Vector3.forward * _speed;
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
        else if (other.CompareTag("Coin"))
        {
            other.GetComponent<Collectable>().Collected();
            LevelManager.GetInstance().AddCoin();
        }
    }

    public void IncreaseSpeed()
    {
        _speed *= speedMultiplier;
        if (_speed >= maxSpeed) _speed = maxSpeed;
        print("Speed: " + _speed);
    }

    public int SpeedMultiplication()
    {
        float proportion = _speed / minSpeed * 100 - 100;
        return (proportion < 50) ? 1 : (proportion < 100) ? 2 : proportion < 150 ? 3 : proportion < 200 ? 4 : 5;
    }

}