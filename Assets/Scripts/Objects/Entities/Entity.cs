using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Abilities;
using Objects.Entities.Players;
using Objects.Projectiles;
using UnityEngine;

namespace Objects.Entities {
    public abstract class Entity : MonoBehaviour {

        
        [Header("Entity")] 
        [SerializeField] protected string Name;
        
        [SerializeField] protected int MaxHealth;
        protected int CurrentHealth;
        public bool IsDead => CurrentHealth == 0;
        
        [SerializeField] protected Transform WeaponRig;
        [SerializeField] protected Transform ProjectileRig;

        protected Transform _transform;
        protected Rigidbody2D _rigidbody2D;

        protected bool m_Rooted;
        protected bool m_Shielded;
        
        private float OnHitFlashDuration = 0.3f;
        private List<SpriteRenderer> Sprites;
        private IEnumerator m_FlashRoutine;

        protected Color m_CurrentColor = Color.white;
        private readonly Color _mFlashColor = Color.red;
        private readonly Color _rootedColor = new Color(0.3457565f, 0.846f, 0.3604696f, 1);

        public List<GameObject> Projectiles { get; private set; } = new List<GameObject>();

        protected EntityAudioController _entityAudioController;
        
        protected void Init() {
            CurrentHealth = MaxHealth;
            
            _transform = transform;
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _entityAudioController = transform.parent.GetComponent<EntityAudioController>();

            if (_entityAudioController == null)
                _entityAudioController = GetComponent<EntityAudioController>();
            
            Sprites = GetComponentsInChildren<SpriteRenderer>().ToList();
        }
        
        protected void Update() {
            if (!IsDead && !m_Rooted)
                UpdateMovement();
        }
        
        protected void TriggerAbility(BaseAbility ability, float fireRateModifier = 1f) {
            StartCoroutine(ability.TriggerCooldown(fireRateModifier));
            StartCoroutine(ability.Fire());
            _entityAudioController.PlayOneShotSound(ability.OnCastSound);
        }
        
        public virtual void Damage(int value, Entity origin) {

            if (!IsDead) {
                int updatedHealth = CurrentHealth - value;
                if (updatedHealth < 0)
                    updatedHealth = 0;
                
                FlashOnHit();
                Debug.Log($"{Name} take {value} damage from {(origin != null ? origin.Name : "Invalid")} [{updatedHealth}/{MaxHealth}]");

                CurrentHealth = updatedHealth;
            }
        }

        public virtual void Heal(int value) {

            if (!IsDead) {
                CurrentHealth += value;
                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;
            
                Debug.Log($"{Name} healed {value} [{CurrentHealth}/{MaxHealth}]");
            }
        }

        public virtual void Shield(bool b) {
            m_Shielded = b;
        }

        private void FlashOnHit() {
            if (m_FlashRoutine != null)
                StopCoroutine(m_FlashRoutine);
            
            m_FlashRoutine = FlashOnItOvertime();
            StartCoroutine(m_FlashRoutine);
        }

        private IEnumerator FlashOnItOvertime() {
            float elapsed = 0;
            
            if (m_Rooted) {
                m_CurrentColor = _rootedColor;
                UpdateSpritesColor(m_CurrentColor);
            }

            while (elapsed < OnHitFlashDuration * 2) {
                Color color = Color.Lerp(m_CurrentColor, _mFlashColor, Mathf.PingPong(elapsed, OnHitFlashDuration));
                UpdateSpritesColor(color);
                
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        protected void UpdateSpritesColor(Color c) {
            foreach (SpriteRenderer r in Sprites) {
                r.color = c;
            }
        }

        public void ClearProjectiles() {
            foreach (GameObject g in Projectiles) {
                if (g != null)
                    Destroy(g);
            }
        }
        
        protected abstract void UpdateMovement();
    }
}