using System.Collections;
using UnityEngine;

namespace Objects.Entities.Players {
    public class PlayerFXController : MonoBehaviour
    {

        [Header("Audio")] 
        [SerializeField] private AudioClip OnJump;
        [SerializeField] private AudioClip OnJumpImpact;
        
        [Header("Running Dust")]
        [SerializeField] private Vector2 DustSpawnDelay;
        [SerializeField] private Vector2 DustLifetime;
        [SerializeField] private float DustSpawnOffset;
        public bool DustSpawnOnCooldown { get; private set; }
        private                  string     m_RunningDustPrefabPath = "FX/Dust";
        private                  GameObject m_RunningDustPrefab;
        
        private string m_JumpDustPrefabPath = "FX/JumpDust";
        private GameObject m_JumpDustPrefab;

        [Header("Healing Aura")]
        [SerializeField] private  float      HealingAuraDuration;
        [SerializeField] private Vector3     HealingAuraOffset;
        private                  string      m_HealingAuraPrefabPath = "FX/HealingAura";
        private                  GameObject  m_HealingAura;
        
        [Header("Buff Aura")]
        [SerializeField] private Vector3    BuffAuraOffset;
        private                  string     m_BuffAuraPrefabPath = "FX/HealingAura";
        private                  GameObject m_BuffAura;
        
        private const string m_OnDeathFXPath = "FX/PlayerDeathFX";
        private readonly Vector2 m_OnDeathOffset = new Vector2(0, 0.55f);
        private GameObject m_DeathFX;

        private AudioSource m_Audio;
        
        void Awake() {
            m_Audio = GetComponent<AudioSource>();
            
            m_RunningDustPrefab = Resources.Load<GameObject>(m_RunningDustPrefabPath);
            m_JumpDustPrefab = Resources.Load<GameObject>(m_JumpDustPrefabPath);
            
            m_HealingAura = Instantiate(Resources.Load<GameObject>(m_HealingAuraPrefabPath), transform);
            m_HealingAura.SetActive(false);
            m_HealingAura.transform.localPosition = HealingAuraOffset;
            
            m_BuffAura = Instantiate(Resources.Load<GameObject>(m_BuffAuraPrefabPath), transform);
            m_BuffAura.SetActive(false);
            m_BuffAura.transform.localPosition = BuffAuraOffset;
        }
        
        public void SpawnRunningDust(Vector3 position) {
            SwitchDustSpawnCooldown();
            SpawnDust(m_RunningDustPrefab, position);
            Invoke(nameof(SwitchDustSpawnCooldown), Random.Range(DustSpawnDelay.x, DustSpawnDelay.y));
        }

        public void SpawnJumpDust(Vector3 position) {
            SpawnDust(m_JumpDustPrefab, position);
        }

        private void SpawnDust(GameObject prefab, Vector3 position) {
            position.y += DustSpawnOffset;
            GameObject dust = Instantiate(prefab, position, Quaternion.identity);
            dust.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            Destroy(dust, Random.Range(DustLifetime.x, DustLifetime.y));
        }

        private void SwitchDustSpawnCooldown() {
            DustSpawnOnCooldown = !DustSpawnOnCooldown;
        }

        public void ToggleHealingAura() {
            StopCoroutine(nameof(DisplayHealingAura));
            StartCoroutine(nameof(DisplayHealingAura));
        }
        
        private IEnumerator DisplayHealingAura() {
            float elapsed = 0;
            
            m_HealingAura.SetActive(true);
            while (elapsed < HealingAuraDuration) {
                elapsed += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            m_HealingAura.SetActive(false);
        }

        public void ToggleBuffAura(bool b) {
            m_BuffAura.SetActive(b);
        }

        public void PlayOnJumpSound() {
            m_Audio.PlayOneShot(OnJump);
        }

        public void PlayOnJumpImpactSound() {
            m_Audio.PlayOneShot(OnJumpImpact);
        }

        public void ToggleDeath(bool b, GameObject playerObject) {

            if (b) {
                playerObject.SetActive(false);
                m_DeathFX = Instantiate(Resources.Load<GameObject>(m_OnDeathFXPath), (Vector2)transform.position + m_OnDeathOffset, Quaternion.identity, transform);
            } else {
                if (m_DeathFX != null)
                    Destroy(m_DeathFX);
                playerObject.SetActive(true);
            }
        }
    }
}