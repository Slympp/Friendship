using System.Collections;
using UnityEngine;

namespace Objects.Entities.Players {
    
    public class PlayerAudioController : MonoBehaviour {
        
        [SerializeField] private AudioClip OnJump;
        [SerializeField] private AudioClip OnJumpImpact;

        private bool isWalking = false;
        private float walkLoopDelay = 0.2f;
        
        [SerializeField] private AudioSource m_WalkAudioSource;
        [SerializeField] private AudioSource m_JumpAudioSource;
        
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
        
        public void PlayOnJumpSound() {
            m_JumpAudioSource.PlayOneShot(OnJump);
        }

        public void PlayOnJumpImpactSound() {
            m_JumpAudioSource.PlayOneShot(OnJumpImpact);
        }
    }
}