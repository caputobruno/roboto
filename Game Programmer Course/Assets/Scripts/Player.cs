using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float velocity;
    [SerializeField] float jumpVelocity;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] LayerMask _waterLayerMask;
    [SerializeField] float footOffset;
    [SerializeField] float _knockbackVelocity = 400;
    [SerializeField] Collider2D _duckingCollider;
    [SerializeField] Collider2D _standingCollider;
    [SerializeField] float _wallDetectionDistance;
    [SerializeField] int _wallDetectionPoints;
    [SerializeField] float _buffer;


    Rigidbody2D _rig;
    SpriteRenderer _sprite;
    Animator _anim;
    AudioSource _audioSource;
    PlayerInput _playerInput;

    [System.NonSerialized] public float vertical;

    string currentAnim;
    Vector2 _origin;
    float _bottomY;
    float horizontalInput;
    float verticalInput;
    float jumpEndTime = 0;
    

    PlayerData _playerData = new PlayerData();
    RaycastHit2D[] _results = new RaycastHit2D[20];
    public bool IsDucking;
    private Vector2 _targetSpeed;

    public bool IsInWater;
    public bool IsTouchingRightWall;
    public bool IsTouchingLeftWall;
    private PlayerInteractionController _playerInteractController;

    public event Action CoinsChanged;
    public event Action HealthChanged;

    public int Coins {  get => _playerData.Coins; private set => _playerData.Coins = value; }
    public int Health => _playerData.Health;

    public Vector2 Direction { get; private set; } = Vector2.right;
    private void OnValidate()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _bottomY = _sprite.bounds.extents.y;
    }

    void OnEnable() => FindObjectOfType<CinemachineTargetGroup>()?.AddMember(transform, 1, 5);

    void OnDisable() => FindObjectOfType<CinemachineTargetGroup>()?.RemoveMember(transform);

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _bottomY = _sprite.bounds.extents.y;


        _rig = GetComponent<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = GetComponent<PlayerInput>();
        _playerInteractController = GetComponent<PlayerInteractionController>();

        currentAnim = "idle";

        FindAnyObjectByType<PlayerCanvas>().Bind(this);

    }

    private void Update()
    {
        if (GameManager.IsLoading)
            return;

        UpdateAnim();

        if (GameManager.CinematicPlaying)
        {
            _rig.velocity = Vector2.zero;
            return;
        }

        InputCheck();
        UpdateWallTouching();
        UpdateDirection();
        Jump();
        Duck();
        Move();

        _playerData.Position = _rig.position;
        _playerData.Velocity = _rig.velocity;
    }

    private void UpdateWallTouching()
    {
        IsTouchingRightWall = CheckForWall(Vector3.right);
        IsTouchingLeftWall = CheckForWall(Vector3.left);

    }

    void InputCheck()
    {
        var input = _playerInput.actions["Move"].ReadValue<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;

    }

    void Move()
    {
        _targetSpeed = new Vector2(velocity * horizontalInput, vertical);

        if (IsTouchingLeftWall && horizontalInput < 0)
            _targetSpeed.x = 0;
        if (IsTouchingRightWall && horizontalInput > 0)
            _targetSpeed.x = 0;

        _rig.velocity = _targetSpeed;
    } 

    void Jump()
    {
        vertical = _rig.velocity.y;

        if (_playerInput.actions["Jump"].WasPerformedThisFrame() && IsGrounded())
        {
            jumpEndTime = Time.time + 0.1f;
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        if (_rig.velocity.y > 0 && _playerInput.actions["Jump"].WasReleasedThisFrame())
        {
            vertical = 0;
            jumpEndTime = Time.time;
        }

        if(Time.time < jumpEndTime)
            vertical = jumpVelocity;
    }

    bool IsGrounded()
    {
        _bottomY = _sprite.bounds.extents.y;
        _origin = new Vector2(transform.position.x - footOffset, transform.position.y - _bottomY);
        Vector2 origin2 = new(transform.position.x + footOffset, transform.position.y - _bottomY);

        int hits = Physics2D.Raycast(_origin, Vector2.down, new ContactFilter2D() { layerMask = _groundLayer }, _results, 0.1f);
        bool grounded = false;

        for(int i = 0; i < hits; i++)
        {
            var hit = _results[i];

            if (!hit.collider)
                continue;

            grounded = true;
        }

        if (grounded)
            return grounded;

        hits = Physics2D.Raycast(origin2, Vector2.down, new ContactFilter2D() { layerMask = _groundLayer }, _results, 0.1f);

        for (int i = 0; i < hits; i++)
        {
            var hit = _results[i];

            if (!hit.collider)
                continue;

            grounded = true;
        }

        var water = Physics2D.OverlapPoint(_origin, _waterLayerMask);
        if (water != null)
        {
            IsInWater = true;
            grounded = true;
        }

        return grounded;
    }

    void Duck()
    {
        IsDucking = _anim.GetBool("isDucking");
        _standingCollider.enabled = !IsDucking;
        _duckingCollider.enabled = IsDucking;

        if (verticalInput >= 0 || Mathf.Abs(verticalInput) < Mathf.Abs(horizontalInput) || !IsGrounded()) return;

        SetAnim("duck");
        horizontalInput = 0;
        
    }

    void UpdateAnim() => SetAnim(IsGrounded() ? (Mathf.Abs(_rig.velocity.x) > 0.1f ? "run" : "idle") : "jump");

    void SetAnim(string animBool)
    {
        if(currentAnim == animBool) return;

        _anim.SetBool(currentAnim, false);
        currentAnim = animBool;
        _anim.SetBool(animBool, true);
    }

    void UpdateDirection()
    {
        if (_rig.velocity.x == 0)
            return;

        if (_rig.velocity.x < -1 && transform.localScale.x > 0)
        {
            Flip();
            Direction = Vector2.left;
        }

        if (_rig.velocity.x > 1 && transform.localScale.x < 0)
        {
            Flip();
            Direction = Vector2.right;
        }
    }

    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        _playerInteractController.FlipText();
    }

    public void AddPoint()
    {
        Coins++;
        CoinsChanged?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        _origin = new Vector2(transform.position.x - footOffset, transform.position.y - _bottomY);
        Gizmos.DrawLine(_origin, _origin + Vector2.down * 0.1f);

        _origin = new Vector2(transform.position.x + footOffset, transform.position.y - _bottomY);
        Gizmos.DrawLine(_origin, _origin + Vector2.down * 0.1f);


        DrawGizmosForSide(Vector3.left);
        DrawGizmosForSide(Vector3.right);
    }

    private void DrawGizmosForSide(Vector3 direction)
    {
        var activeCollider = IsDucking ? _duckingCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentedSize = colliderHeight / (_wallDetectionPoints - 1);

        for (int i = 0; i < _wallDetectionPoints; i++)
        {
            var origin = transform.position - new Vector3(0, (activeCollider.bounds.size.y / 2) - activeCollider.offset.y, 0);
            origin += new Vector3(0, _buffer + segmentedSize * i, 0);
            origin += direction * _wallDetectionDistance;
            Gizmos.DrawWireSphere(origin, 0.05f);
        }
    }

    bool CheckForWall(Vector3 direction)
    {
        var activeCollider = IsDucking ? _duckingCollider : _standingCollider;
        float colliderHeight = activeCollider.bounds.size.y - 2 * _buffer;
        float segmentedSize = colliderHeight / (_wallDetectionPoints - 1);

        for (int i = 0; i < _wallDetectionPoints; i++)
        {
            var origin = transform.position - new Vector3(0, (activeCollider.bounds.size.y / 2) - activeCollider.offset.y, 0);
            origin += new Vector3(0, _buffer + segmentedSize * i, 0);
            origin += direction * _wallDetectionDistance;

            int hits = Physics2D.Raycast(origin, direction, new ContactFilter2D() { layerMask = _groundLayer, useLayerMask = true }, _results, 0.1f);

            for (int j = 0; j < hits; j++)
            {
                var hit = _results[j];

                if (hit.collider)
                    return true;
            }
        }
        return false;
    }

    public void Bind(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void RestorePositionAndVelocity()
    {
        _rig.position = _playerData.Position;
        _rig.velocity = _playerData.Velocity;
    }

    public void TakeDamage(Vector2 hitNormal)
    {
        _playerData.Health--;

        if(_playerData.Health <= 0)
        {
            SceneManager.LoadScene("Menu");
            _playerData.Health = _playerData.maxHealth;
            return;
        }

        HealthChanged?.Invoke();

        _rig.AddForce(-hitNormal * _knockbackVelocity);
    }

    internal void Bounce(Vector2 normal, float bounciness)
    {
        _rig.AddForce(-normal * bounciness);
    }
}
