using System;
using Konrad.Characters.Animations;
using Unity.Mathematics;
using UnityEngine;

namespace Konrad.Characters
{
    
    [RequireComponent(typeof(Stats),typeof(Inventory))]
    public class Character : MonoBehaviour
    {
        [SerializeField] Transform[] groundChecks;
        LayerMask _groundLayer;
        [SerializeField] Faction faction;
        [SerializeField] float runAcceleration = 0.6f;
        [SerializeField] float runMaxSpeed = 5f;
        [SerializeField] float walkSpeed = 2f;
        [SerializeField] float jumpForce = 10f;
        public Faction Faction => faction;
        
        internal Animator Animator { get; private set; }
        internal Rigidbody2D Rigidbody { get; private set; }
        internal Collider2D Collider { get; private set; }
        internal Stats Stats { get; private set; }
        internal Inventory Inventory { get; private set; }
        
        readonly Vector3 _rightScale = new(1, 1, 1);
        readonly Vector3 _leftScale = new(-1, 1, 1);
        
        public bool FacingRight => transform.localScale == _rightScale;

        /// <summary>
        /// Flips the character to face the given direction on the x axis.
        /// </summary>
        /// <param name="right">true = Right, false = left</param>
        public void FaceX(bool right)
        {
            transform.localScale = right switch
            {
                true => _rightScale,
                false => _leftScale
            };
        }
        
        public int Health { get; internal set; } = 200;
        public bool IsAlive => Health > 0;
        public bool IsStunned => _timeStunned > 0;
        float _timeStunned;
        

        #region Requesting an action from the Character
        
        // Attacking:
        float _timeSinceLastAttack;
        bool _attackRequested;
        int _currentAttackMove = 1;
        /// <summary>
        /// The character will perform an attack if it is able damaging enemies in its range.
        /// </summary>
        public void Attack() => _attackRequested = true;

        // Movement:
        bool _isMoving;
        Vector2 _movementAxis;
        /// <summary>
        /// The character will run in the x direction of the given vector.
        /// There is also some control when the character is in the air.
        /// A high value on the y axis will make the character perform a jump.
        /// </summary>
        /// <param name="movementAxis"></param>
        public void Move(Vector2 movementAxis) => _movementAxis = movementAxis;

        // Jumping:
        bool _isGrounded;
        int _jumpRequest;
        /// <summary>
        /// If the character is grounded, a jump will be performed.
        /// If called while in the air, a jump will be queued for a short time, executed if the character lands.
        /// </summary>
        public void Jump() => _jumpRequest = 8;
        
        // Defending/Blocking:
        [NonSerialized] public bool DefenseEnabled = true;
        public bool InDefensePosture { get; internal set; }
        internal float TimeInDefensePosture { get; private set; }
        internal float TimeSinceLastDefense { get; private set; }
        bool _defenseRequested;
        /// <summary>
        /// The character will enter a defensive posture if it is not already in one for the next frame.
        /// If a constant defense is desired, call this method every frame.
        /// </summary>
        /// <param name="request">If we want to enter a defensive posture or exit it.</param>
        public void Defend(bool request) => _defenseRequested = request;
        
        // Commentary about stunned state:
        /// <summary>
        /// The character will be stunned for the given amount of time unable to perform any action.
        /// </summary>
        /// <param name="time"></param>
        internal void Stun(float time) => _timeStunned = time;
        
        #endregion
        
        
        #region Unity Events and Updates
        protected void Start()
        {
            Animator = GetComponentInChildren<Animator>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<Collider2D>();
            Stats = GetComponent<Stats>();
            Inventory = GetComponent<Inventory>();
            _groundLayer = LayerMask.GetMask("Ground");
        }

        protected void FixedUpdate()
        {
            // Death
            if (IsAlive == false)
            {
                ForceDefense(false);
                Animator.SetBool(AnimatorConst.Hurt, false);
                Animator.SetBool(AnimatorConst.Death, true);
                this.enabled = false;
                return;
            }
            
            if (IsStunned)
            {
                ForceDefense(false);
                _isMoving = false;
                Animator.SetBool(AnimatorConst.Hurt, true);
                _timeStunned -= Time.deltaTime;
                return;
            }
            Animator.SetBool(AnimatorConst.Hurt, false);
            
            UpdateAttacking();
            UpdateDefense();
            
            //Movement
            Vector2 velocity = Rigidbody.velocity;
            UpdateMovement(ref velocity);
            Animator.SetInteger(AnimatorConst.AnimState, _isMoving ? 1 : 0);
            UpdateJumping(ref velocity);
            Animator.SetFloat(AnimatorConst.SpeedY, velocity.y);
            Rigidbody.velocity = velocity;

            //Others States
            bool wasGrounded = _isGrounded;
            _isGrounded = false;
            var position = transform.position;
            foreach (var check in groundChecks)
            {
                if (Physics2D.Linecast(position, check.position, _groundLayer))
                    _isGrounded = true;
            }
            
            if (_isGrounded && !wasGrounded) Animator.SetBool(AnimatorConst.Grounded, true);
        }

        void UpdateMovement(ref Vector2 velocity)
        {
            bool atMaxSpeed = runMaxSpeed - math.abs(velocity.x) < 0f;
            if (_timeSinceLastAttack > 0.4f && !InDefensePosture && !atMaxSpeed) //Avoid movement while attacking or defending
            {
                switch (_movementAxis.x)
                {
                    case > 0f:
                        _isMoving = true;
                        velocity += new Vector2(runAcceleration, 0f);
                        break;
                    case < 0f:
                        _isMoving = true;
                        velocity -= new Vector2(runAcceleration, 0f);
                        break;
                    default:
                        _isMoving = false;
                        break;
                }
            }

            switch (velocity.x)
            {
                case > 0.5f:
                    
                    if (_movementAxis.x > 0) transform.localScale = _rightScale;
                    break;
                case < -0.5f:
                    if (_movementAxis.x < 0) transform.localScale = _leftScale;
                    break;
            }
        }

        void UpdateJumping(ref Vector2 velocity)
        {
            if (_movementAxis.y > 0.9f) Jump();
            if (_jumpRequest > 0)
            {
                if (_isGrounded)
                {
                    float force = jumpForce;
                    if (InDefensePosture) force *= 0.25f;
                    velocity.y = force;
                    _jumpRequest = 0;
                    Animator.SetTrigger(AnimatorConst.Jump);
                    Animator.SetBool(AnimatorConst.Grounded, false);
                }
                else _jumpRequest--;
            }
            
        }

        void UpdateAttacking()
        {
            _timeSinceLastAttack += Time.deltaTime;
            // attackCooldown decreases as dexterity increases
            float attackCooldown = 1f - Stats.Dexterity * 0.01f;
            if (_attackRequested == false || _timeSinceLastAttack < attackCooldown) return;

            DefenseEnabled = false;
            _attackRequested = false;
            _timeSinceLastAttack = 0f;
            _currentAttackMove = _currentAttackMove == 1 ? 2 : 1;
            Animator.SetTrigger("Attack" + _currentAttackMove);
            Inventory.EquippedWeapon.DD.DealDamage();
        }

        void ForceDefense(bool defend)
        {
            DefenseEnabled = defend;
            _defenseRequested = defend;
            UpdateDefense();
        }

        void UpdateDefense()
        {
            bool defend = _defenseRequested && DefenseEnabled;
            if (InDefensePosture == false && TimeSinceLastDefense < 0.25f) // Avoid spamming defense
                defend = false;

            Animator.SetBool(AnimatorConst.DefenseStance, defend);
            InDefensePosture = defend;

            if (InDefensePosture)
            {
                TimeInDefensePosture += Time.deltaTime;
                TimeSinceLastDefense = 0f;
            }
            else
            {
                TimeInDefensePosture = 0f;
                TimeSinceLastDefense += Time.deltaTime;
            }

            _defenseRequested = false;
        }

        #endregion
    }
}