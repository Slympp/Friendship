using Objects.Entities.Players;
using UnityEngine;

namespace Abilities {
    public class AuraHandler : MonoBehaviour {
        private bool _initialized;
        private AuraAbility m_Ability;
        
        public void Init(AuraAbility ability) {
            m_Ability = ability;
            _initialized = true;
        }
        
        void Update() {

            if (_initialized && !m_Ability.OnCooldown) {
                // Find target
                // If (targetFound) {
                    // 
                // else {
            }
        }
        
    }
}