using System.Collections;
using System.Collections.Generic;
using Abilities;
using Objects.Entities.Players;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

namespace Objects.Entities.Enemies {
    public abstract class EnemyController : Entity {

        [Header("Targeting")]
        [SerializeField] protected LayerMask TargetingMask;
        [SerializeField] protected float TargetingRange;
        [SerializeField] protected float TargetingAngle;
        [SerializeField] protected HorizontalAiming Horizontal;
        [SerializeField] protected VerticalAiming Vertical;

        private const float TargetingOffset = 0.5f;
        private const float RefreshDelay = 0.5f;
        private const float WaypointReachDistance = 0.8f;
        
        protected PlayerController m_TargetController;
        protected Transform        m_TargetTransform;

        [Header("IA")] 
        [SerializeField] private float MovementSpeed;
        [SerializeField] protected Transform StartWaypoint;
        [SerializeField] protected Transform EndWaypoint;
        private Transform m_CurrentWaypoint;
        
        [SerializeField] protected float AttackRange = 5f;
        [SerializeField] private BaseAbility Ability;
        private BaseAbility m_Ability;
        
        protected EnemyAnimatorController m_EnemyAnimatorController;
        
        private const float       LifeStealMultiplier = 0.5f;
        private       bool        m_Marked;
        private       IEnumerator m_LifeStealRoutine;
        
        private IEnumerator m_RootRoutine;
        
        private bool m_Shielded;
        [SerializeField] private Vector2 m_ShieldOffset = Vector2.zero;
        private const string m_ShieldedIndicatorPath = "FX/ShieldIndicator";
        private GameObject m_ShieldedIndicator;

        private const string m_MarkedIndicatorPath = "FX/HealingMarkIndicator";
        private readonly Vector2 m_MarkedOffset = new Vector2(0, 1.75f);
        private GameObject m_MarkedIndicator;
        
        [SerializeField] private Vector2 m_RootedOffset = new Vector2(0, 1.75f);
        private const string m_RootedIndicatorPath = "FX/RootedIndicator";
        private GameObject m_RootedIndicator;

        private const string m_OnDeathFXPath = "FX/PoofEnemyDeath";
        private readonly Vector2 m_OnDeathOffset = new Vector2(0, 1.3f);
        private float m_OnDeathFXLifetime = 0.25f;

        [Header("Scoring")] 
        [SerializeField] protected float TimeToKillCap = 3f;
        [SerializeField] protected Vector2 BaseScore = new Vector2(10, 5);
        private bool m_FirstHit = false;
        private float m_TimeSinceFirstHit;

        protected void Awake() {
            Init();

            m_EnemyAnimatorController = GetComponent<EnemyAnimatorController>();
            m_EnemyAnimatorController.Init(GetComponent<Animator>());
            
            m_Ability = Ability.Init(WeaponRig, ProjectileRig, this);

            m_MarkedIndicator = Instantiate(Resources.Load<GameObject>(m_MarkedIndicatorPath), _transform);
            m_MarkedIndicator.transform.localPosition = m_MarkedOffset;
            m_MarkedIndicator.SetActive(false);
            
            m_RootedIndicator = Instantiate(Resources.Load<GameObject>(m_RootedIndicatorPath), _transform);
            m_RootedIndicator.transform.localPosition = m_RootedOffset;
            m_RootedIndicator.SetActive(false);
            
            m_ShieldedIndicator = Instantiate(Resources.Load<GameObject>(m_ShieldedIndicatorPath), _transform);
            m_ShieldedIndicator.transform.localPosition = m_ShieldOffset;
            m_ShieldedIndicator.SetActive(false);

            m_CurrentWaypoint = StartWaypoint;
            InvokeRepeating(nameof(SetTarget), 0, RefreshDelay);
        }

        protected void Patrol() {
            if (Vector2.Distance(transform.position, m_CurrentWaypoint.position) < WaypointReachDistance) {
                m_CurrentWaypoint = m_CurrentWaypoint == StartWaypoint ? EndWaypoint : StartWaypoint;
            } else {
                MoveTowards(GetWaypoint(m_CurrentWaypoint.position.x));
            }
        }

        protected void Attack() {
            m_EnemyAnimatorController.SetMoving(false);
            if (!m_Ability.OnCooldown) {
                m_EnemyAnimatorController.TriggerShooting();
                TriggerAbility(m_Ability);
            }
        }
        
        protected void MoveTowards(Vector2 targetPosition) {
            m_EnemyAnimatorController.SetMoving(true);
            _transform.right = targetPosition - (Vector2)_transform.position;
            _rigidbody2D.AddForce(_transform.right * MovementSpeed);
        }

        public override void Damage(int value, Entity origin) {
            int oldHealth = CurrentHealth;

            if (!m_FirstHit) {
                m_FirstHit = true;
                StartCoroutine(nameof(ScoreTimer));
            }
            
            if (m_Shielded) return;
            
            base.Damage(value, origin);

            if (m_Marked && origin != null) {
                int healValue = Mathf.FloorToInt((oldHealth - CurrentHealth) * LifeStealMultiplier);
                origin.Heal(healValue);
            }
            
            if (IsDead) {
                Debug.Log($"Enemy {Name} is dead");

                OnDeath();
                
                GameObject onDeathFX = Instantiate(Resources.Load<GameObject>(m_OnDeathFXPath), (Vector2)_transform.position + m_OnDeathOffset, Quaternion.identity);
                Destroy(onDeathFX, m_OnDeathFXLifetime);
                
                AttributeScore();
                ClearProjectiles();
                
                Destroy(gameObject);
            }
        }

        private IEnumerator ScoreTimer() {
            m_TimeSinceFirstHit = 0;
            while (m_TimeSinceFirstHit < TimeToKillCap) {
                yield return new WaitForEndOfFrame();
                m_TimeSinceFirstHit += Time.deltaTime;
            }
            yield return null;
        }

        private void AttributeScore() {
            float points = m_TimeSinceFirstHit < TimeToKillCap ? 
                m_TimeSinceFirstHit.Normalize(BaseScore.x, BaseScore.y, 0, TimeToKillCap) : 
                BaseScore.y;
            
            GameManager.GameManager.Instance.UpdateScoreAmount(points);
        }
        
        protected virtual void OnDeath() { }
        
        public void Shield(bool b) {
            m_Shielded = b;
            m_ShieldedIndicator.SetActive(b);
        }
        
       public void Mark(float duration) {
            if (m_LifeStealRoutine != null)
                StopCoroutine(m_LifeStealRoutine);
            
            m_LifeStealRoutine = MarkOvertime(duration);
            StartCoroutine(m_LifeStealRoutine);
        }
        
        public IEnumerator MarkOvertime(float duration) {
            float elapsed = 0;

            m_Marked = true;
            m_MarkedIndicator.SetActive(true);
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_MarkedIndicator.SetActive(true);
            m_Marked = false;
        }
        
        public void Root(float duration) {
            if (m_RootRoutine != null)
                StopCoroutine(m_RootRoutine);
            
            m_RootRoutine = RootOvertime(duration);
            StartCoroutine(m_RootRoutine);
        }

        private IEnumerator RootOvertime(float duration) {
            float elapsed = 0;

            m_Rooted = true;
            m_RootedIndicator.SetActive(true);
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            m_RootedIndicator.SetActive(false);
            m_CurrentColor = Color.white;
            UpdateSpritesColor(m_CurrentColor);
            m_Rooted = false;
        }

        private void SetTarget() {
            
            m_TargetController = null;
            m_TargetTransform = null;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, TargetingRange, TargetingMask);
            List<PlayerController> targets = new List<PlayerController>();
            foreach (Collider2D c in colliders) {
                if (c.gameObject == gameObject || c.CompareTag("Enemy")) continue;

                // Check angle
                Vector2 targetDir = (c.transform.position - _transform.position).normalized;
                Vector2 aimingDirection = GetDirection();

                if (Vector2.Angle(targetDir, aimingDirection) > TargetingAngle) continue;
                
                // Check LOS
                RaycastHit2D hit = Physics2D.Raycast(_transform.position, targetDir, AttackRange);
                Debug.DrawLine(WeaponRig.position, hit.point, Color.blue);
                
                if (hit.collider != null && !hit.collider.CompareTag("Player")) continue;
                
                PlayerController playerController = c.gameObject.GetComponentInChildren<PlayerController>();
                if (playerController != null)
                    targets.Add(playerController);
            }

            float maxDistance = TargetingRange;
            foreach (PlayerController p in targets) {
                float distance = Vector2.Distance(p.transform.position, _transform.position);
                if (distance < maxDistance) {
                    maxDistance = distance;
                    m_TargetController = p;
                    m_TargetTransform = m_TargetController.transform;
                }
            }

            if (m_TargetController != null) {
                Vector3 position = m_TargetTransform.position;
                position.y += TargetingOffset;
                float angle = Vector2.SignedAngle(_transform.right, position - _transform.position);

                if (_transform.eulerAngles.y.Equals(180))
                    angle *= -1;
                
                WeaponRig.localEulerAngles = new Vector3(0f, 0, angle);
            }
        }

        protected Vector2 GetWaypoint(float x) {
            return new Vector2(x, transform.position.y);
        }

        private void OnDrawGizmos() {

            if (WeaponRig != null) {
                Gizmos.color = Color.cyan;
                Vector3 direction = GetDirection();
                Vector3 position = WeaponRig.position;
                var topLine = Quaternion.Euler(0, 0, TargetingAngle) * direction * TargetingRange + position;
                Debug.DrawLine(position, topLine, Color.cyan);
                var bottomLine = Quaternion.Euler(0, 0, -TargetingAngle) * direction * TargetingRange + position;
                Debug.DrawLine(position, bottomLine, Color.cyan);
            }
            
            if (StartWaypoint != null && EndWaypoint != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(GetWaypoint(StartWaypoint.position.x), GetWaypoint(EndWaypoint.position.x));

                if (m_CurrentWaypoint == null) return;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(m_CurrentWaypoint.position, 0.2f);
            }
        }
        
        private Vector2 GetDirection() {
            return transform.TransformDirection((float)Horizontal, (float)Vertical, 0);
        }

        public enum HorizontalAiming {
            False = 0,
            True = 1
        }
        
        public enum VerticalAiming {
            Down = -1,
            None = 0,
            Up = 1
        }
    }
}