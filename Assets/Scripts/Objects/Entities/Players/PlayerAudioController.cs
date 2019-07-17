using UnityEngine;

namespace Objects.Entities.Players {
    
    public class PlayerAudioController : MonoBehaviour {
        
        [SerializeField] private AudioClip OnJump;
        [SerializeField] private AudioClip OnJumpImpact;

        private bool isWalking = false;
        
        [SerializeField] private AudioSource m_WalkAudioSource;
        [SerializeField] private AudioSource m_JumpAudioSource;
        
        void Awake() {
            m_WalkAudioSource = GetComponent<AudioSource>();
        }

        public void ToggleWalk(bool enabled) {
            if (enabled && !m_WalkAudioSource.isPlaying && !isWalking) {
                Debug.Log("Play");
                isWalking = true;
                m_WalkAudioSource.Play();
            } else if (!enabled && m_WalkAudioSource.isPlaying && isWalking) {
                Debug.Log("Stop");
                isWalking = false;
                m_WalkAudioSource.Stop();
            }
        }
        
        public void PlayOnJumpSound() {
            Debug.Log("Jump");
            m_JumpAudioSource.PlayOneShot(OnJump);
        }

        public void PlayOnJumpImpactSound() {
            m_JumpAudioSource.PlayOneShot(OnJumpImpact);
        }
    }
}