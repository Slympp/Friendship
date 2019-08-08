using Objects.Entities.Players;
using UnityEngine;

namespace Abilities {
    public class AuraHandler : MonoBehaviour {
        private bool m_Initialized;
        private AuraAbility m_Ability;
        private Transform m_Rig;

        private GameObject m_CachedTarget;
        
        public void Init(AuraAbility ability, Transform rig) {
            m_Ability = ability;
            m_Rig = rig;
            m_Initialized = true;
        }
        
        void Update() {
            if (!m_Ability.Active)
                return;
            
            if (m_Initialized && !m_Ability.OnCooldown) {

                RaycastHit2D hit = Physics2D.Raycast(transform.position, m_Rig.right, m_Ability.GetRange());
                if (hit.collider == null || !hit.collider.CompareTag("Player")) {
                    if (gameObject == m_CachedTarget) return;

                    m_CachedTarget = gameObject;
                    m_Ability.SetCurrentTarget(gameObject.GetComponent<PlayerController>());

                } else {
                    if (hit.collider.gameObject == m_CachedTarget) return;
                    
                    m_CachedTarget = hit.collider.gameObject;
                    PlayerController hitPlayer = hit.collider.gameObject.GetComponentInChildren<PlayerController>();
                    if (hitPlayer != null)
                        m_Ability.SetCurrentTarget(hitPlayer);
                }
            } else {
                m_Ability.SetCurrentTarget(null);  
            }
        }

        public void ResetCachedTarget() {
            m_CachedTarget = null;
        }
    }
}