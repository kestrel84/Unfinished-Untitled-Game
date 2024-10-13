using System;
using UnityEngine;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private Vector2 _frameVelocity;
        private bool _cachedQueryStartInColliders;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;
        public event Action<bool, int> WallSlideChanged;

        #endregion

        private float _time;

        public Transform _spawnpoint;
        public HazardController _hazardController;
        private bool dead;


        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();

            transform.position = _spawnpoint.position;

            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.Space),
                DashDown = Input.GetButtonDown("Fire3") || Input.GetKeyDown(KeyCode.LeftShift),
                DashHeld = Input.GetButton("Fire3") || Input.GetKey(KeyCode.LeftShift),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (_frameInput.DashDown)
            {
                _timeDashWasPressed = _time;
            }
        }

        private void FixedUpdate()
        {
            if (_hazardController._playerCollided == true){
                dead = true;
                onDeath();
                _hazardController._playerCollided = false;
            }

            if (!dead){
                CheckCollisions();

                HandleDash();
                HandleJump();
                HandleWallJump();
                HandleDirection();
                HandleGravity();

            
                ApplyMovement();
            }
        }

        #region Collisions
        
        private float _frameLeftGrounded = float.MinValue;
        private bool _grounded;
        private bool _touchingWall;
        

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
            
            // Hit a wall
            if (Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.left, _stats.GrounderDistance, ~_stats.PlayerLayer)) {
                _wallJumpDirection = 1;
            } else if (Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.right, _stats.GrounderDistance, ~_stats.PlayerLayer)) {
                _wallJumpDirection = -1;
            } else {
                _wallJumpDirection = 0;
            }

            bool wallHit = (_wallJumpDirection != 0);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (!_grounded && groundHit)
            {                
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
            }
            // Left the Ground
            else if (_grounded && !groundHit)
            {
                _grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            //Landed on Wall
            if (!_touchingWall && wallHit){
                _bufferedJumpUsable = true;
                _touchingWall = true;
                WallSlideChanged?.Invoke(true, _wallJumpDirection);
            } else if (_touchingWall && !wallHit) { // Left wall
                _touchingWall = false;
                _frameLeftGrounded = _time;
                WallSlideChanged?.Invoke(false, _wallJumpDirection);
            }

            // Reset dash and wall jump if grounded 
            if (_grounded) {_dashToConsume = true; _wallJumpToConsume = true;}

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }

        #endregion

        #region Jumping

        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        //walljump stuff
        private bool _wallJumpToConsume;
        private int _wallJumpDirection;
        private bool CanWallJump => _touchingWall && !_grounded;


        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
        
            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !HasBufferedJump) return;

            if (_grounded || CanUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }
        
        private void HandleWallJump(){
            if (!_jumpToConsume && !HasBufferedJump) return;

            if (CanWallJump && _wallJumpToConsume){
                ExecuteWallJump();
                _wallJumpToConsume = false;
            }

            _jumpToConsume = false;
            
        }


        private void ExecuteWallJump(){
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _frameVelocity.x = _stats.JumpAwayFromWallSpeed * _wallJumpDirection;    
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Dash

        private bool _dashToConsume = false;
        private bool _dashing = false;
        private bool _dashEnd = false;
        private float _timeDashWasPressed;
        private Vector2 _dashDirection;
        private Vector2 _dashVect;

        private void HandleDash()
        {
            if(_frameInput.DashHeld && !_dashing && _dashToConsume)
            {
                _dashDirection = _frameInput.Move.normalized;
                _dashVect = _stats.DashSpeed * _dashDirection;
                _dashing = true;
                _dashToConsume = false;
            }

            if (_dashing && _time > _timeDashWasPressed + _stats.DashTime)
            {
                _dashing = false;
                _dashEnd = true;
                _timeDashWasPressed = 0;
            }
        }

        #endregion
           
        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (_grounded && _frameVelocity.y <= 0f)
            {
                _frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void ApplyMovement(){
            if (_dashing){
                _rb.velocity = _dashVect;
            } else if (_dashEnd){
                _frameVelocity = _dashVect * _stats.DashEndSpeedModifier;
                _rb.velocity = _frameVelocity;
                _dashEnd = false;
            } else {
                _rb.velocity = _frameVelocity;
            }
        }

        #region Death

        private void onDeath(){
            _frameVelocity.x = 0;
            _frameVelocity.y = 0;
            transform.position = _spawnpoint.position;
            dead = false;
        }

        #endregion


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif
    }

    public struct FrameInput
    {
        public bool JumpDown;
        public bool JumpHeld;
        public bool DashDown;
        public bool DashHeld;
        public Vector2 Move;
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;

        public event Action Jumped;

        public event Action<bool, int> WallSlideChanged;
        public Vector2 FrameInput { get; }
    }
}