using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{

    [Header("Speed Parameters")]
    [Range(5, 20)]
    [SerializeField]
    [Tooltip("Player start speed")]
    private int minSpeed;

    [Range(20, 40)]
    [SerializeField]
    [Tooltip("Player max speed reachable")]
    private int maxSpeed;

    [Range(1f, 1.5f)]
    [SerializeField]
    [Tooltip("Player speed multiplier, how speed increase in every new level")]
    private float speedMultiplier;

    [SerializeField]
    [Tooltip("Lane speed to move to sides")]
    private float laneSpeed;

    [Header("Jump Parameters")]
    [SerializeField]
    [Tooltip("Distance of the jump")]
    private float jumpLength;

    [SerializeField]
    [Tooltip("Height of the jump")]
    private float jumpHeigth;

    [Header("Slide Parameters")]
    [SerializeField]
    [Tooltip("Distance of the Slide")]
    private float slideLength;

    [Header("SFX")]
    [SerializeField]
    [Tooltip("Jump SFX")]
    private AudioClip jumpClip;

    [SerializeField]
    [Tooltip("Slide SFX")]
    private AudioClip slideClip;

    [SerializeField]
    [Tooltip("Death SFX")]
    private AudioClip deathClip;

    [SerializeField]
    [Tooltip("4321 SFX")]
    private AudioClip readStartClip;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private BoxCollider _boxCollider;
    private GameController _gameController;
    private AudioSource _audioSource;

    // Speed variables
    private float _speed;

    // Horizontal Lane variables
    private int currentLane = 1;
    private Vector3 targetPosition;
    private bool isMovingRigth = false;
    private bool isMovingLeft = false;

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

    // Game State variables
    private bool _isDead = false;
    private bool _isPause = false;

    // Controller variable
    private static bool _playerHasController = true;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
        _audioSource = GetComponent<AudioSource>();
        _gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

        _boxColliderSize = _boxCollider.size;
        _boxColliderCenter = _boxCollider.center;
        _cameraAnimationDuration = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>().GetAnimationDuration();

        _speed = minSpeed;

        _startTimeToLockCamera = Time.time + _cameraAnimationDuration;
        _startTimeToRun = _startTimeToLockCamera + 3.25f;
    }

    void Update()
    {

        if (_isPause)
        {
            _animator.speed = 0f;
            return;
        } 
        else if (_animator.speed != 1f)
        {
            _animator.speed = 1f;
        }

        if (_isDead)
        {
            _gameController.GameOver();
            this.targetPosition.y = Mathf.MoveTowards(this.targetPosition.y, 0, 5 * Time.deltaTime);
            Move();
            return;
        }

        if (!_cameraLock && Time.time >= _startTimeToLockCamera)
        {
            _animator.Play("Start");
            _cameraLock = true;
            _gameController.ShowUiGame();
        }

        if (_cameraLock && Time.time >= _startTimeToLockCamera + 0.25f && !_audioSource.isPlaying && !_startToRun)
        {
            MakeSound(readStartClip);
        }

        if (!_startToRun && Time.time >= _startTimeToRun)
        {
            _animator.Play("runStart");
            _startToRun = true;
            _gameController.ShowPauseButton();
        }

        if (!_cameraLock || !_startToRun)
        {
            return;
        }

        if (_playerHasController)
        {
            DesktopInputs();
            MobileInputs();
        }

        GravityEffects();
        Move();
    }

    private void Move()
    {
        Vector3 targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            isMovingLeft = false;
            isMovingRigth = false;
        }
    }

    private void DesktopInputs()
    {
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
    }

    private void MobileInputs()
    {
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

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                isSwiping = true;
                startSwipePosition = Input.GetTouch(0).position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwiping = false;
            }
        }
    }

    private void GravityEffects()
    {
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
    }

    public bool Jump()
    {
        if (!isJumping)
        {
            jumpStart = transform.position.z;
            _animator.SetFloat("JumpSpeed", _speed / jumpLength);
            _animator.SetBool("Jumping", true);
            _animator.SetBool("Sliding", false);
            isJumping = true;
            isSliding = false;

            MakeSound(jumpClip);

            return true;
        }

        return false;
    }

    public bool Slide()
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

            MakeSound(slideClip);

            return true;
        }

        return false;
    }

    public void MovePlayerHorizontal(bool isToRight = true)
    {
        int newLane = currentLane + (isToRight ? 1 : -1);
        if (newLane < 0 || newLane > 2) return;

        currentLane = newLane;
        targetPosition = new Vector3(currentLane - 1, 0f, 0f);
        if (isToRight) isMovingRigth = true;
        if (!isToRight) isMovingLeft = true;
    }

    void FixedUpdate()
    {
        if (_isPause || _isDead || !_startToRun)
        {
            _rigidbody.velocity = Vector3.zero;
            return;
        }

        _rigidbody.velocity = Vector3.forward * _speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            other.GetComponent<Obstacle>().Impacted();

            _isDead = true;
            _animator.SetTrigger("Hit");
            _animator.SetBool("Dead", true);

            MakeSound(deathClip);
        }
        else if (other.CompareTag("Coin"))
        {
            other.GetComponent<Collectable>().Collected();
            LevelManager.GetInstance().AddCoin();
        }
    }

    private void MakeSound(AudioClip audioClip)
    {
        _audioSource.Stop();
        _audioSource.clip = audioClip;
        _audioSource.Play();
    }

    public void IncreaseSpeed()
    {
        int speedMultiplication = SpeedMultiplication();
        _speed *= 1f + ((speedMultiplier - 1f) / speedMultiplication);
        if (_speed >= maxSpeed) _speed = maxSpeed;
    }

    public int SpeedMultiplication()
    {
        float proportion = _speed / minSpeed * 100 - 100;
        return (proportion < 50) ? 1 : (proportion < 100) ? 2 : proportion < 150 ? 3 : proportion < 200 ? 4 : 5;
    }

    public void Pause()
    {
        _isPause = true;
    }

    public void UnPause()
    {
        _isPause = false;
    }

    public static void UpdatePlayerController(bool hasController)
    {
        _playerHasController = hasController;
    }

}