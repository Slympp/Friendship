using System.Collections;
using UnityEngine;

namespace Objects.Entities.Players {
    
    public class EntityAudioController : MonoBehaviour {
        
        private bool isWalking = false;
        private float walkLoopDelay = 0.2f;
        
        [SerializeField] private AudioSource m_WalkAudioSource;
        [SerializeField] private AudioSource m_EffectAudioSource;

        [SerializeField] private AudioClip OnCooldownSound;
        [SerializeField] private AudioClip OnReviveSound;
        [SerializeField] private AudioClip OnTakeDamageSound;
        [SerializeField] private AudioClip OnHealedSound;
        [SerializeField] private AudioClip OnDeathSound;
        
        void Awake() {
            m_WalkAudioSource = GetComponent<AudioSource>();
        }

        public void ToggleWalk(bool enabled) {
            if (enabled && !m_WalkAudioSource.isPlaying && !isWalking) {
                isWalking = true;
                StartCoroutine(nameof(WalkDelay));
            } else if (!enabled && m_WalkAudioSource.isPlaying && isWalking) {
                isWalking = false;
                StopCoroutine(nameof(WalkDelay));
                m_WalkAudioSource.Stop();
            }
        }

        private IEnumerator WalkDelay() {
            yield return new WaitForSeconds(walkLoopDelay);
            m_WalkAudioSource.PlayScheduled(0);
        }
        
        public void PlayOneShotSound(AudioClip clip) {
            m_EffectAudioSource.PlayOneShot(clip);
        }

        public void OnCooldown() {
            m_EffectAudioSource.PlayOneShot(OnCooldownSound);
        }
        
        public void OnRevive() {
            m_EffectAudioSource.PlayOneShot(OnReviveSound);
        }

        public void OnTakeDamage() {
            m_EffectAudioSource.PlayOneShot(OnTakeDamageSound);
        }

        public void OnHealed() {
            m_EffectAudioSource.PlayOneShot(OnHealedSound);
        }

        public void OnDeath() {
            ToggleWalk(false);
            m_EffectAudioSource.PlayOneShot(OnDeathSound);
        }
    }
}