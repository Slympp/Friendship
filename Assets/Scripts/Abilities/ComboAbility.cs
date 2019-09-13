using System;
using System.Collections;
using System.Collections.Generic;
using Objects.Entities.Players;
using Objects.Projectiles;
using GameManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Abilities  {
    
    public class ComboAbility : MonoBehaviour {

        [HideInInspector] public PlayerController Player;
        
        [SerializeField] private List<GameObject> BaseProjectiles;
        [SerializeField] private int ProjectilesCount;
        [SerializeField] private float OriginRadius;
        [SerializeField] private float OriginOffset;
        [SerializeField] private Vector2 SpawnDelay;
        [SerializeField] private Vector2 Speed;

        private List<ComboProjectile> m_Projectiles;
        [SerializeField] private AudioSource m_Audio;
        [SerializeField] private CinemachineCameraShaker CameraShaker;

        public void Use() {
            StartCoroutine(nameof(Fire));
        }
        
        private IEnumerator Fire() {
            if (Player == null) yield return null;

            m_Audio.Play();
            CameraShaker.ShakeCamera(1.2f);
            
            m_Projectiles = new List<ComboProjectile>();
            for (int i = 0; i < ProjectilesCount; i++) {
                int randomProjectileIndex = Random.Range(0, BaseProjectiles.Count);
                
                GameObject obj = Instantiate(BaseProjectiles[randomProjectileIndex]);
                if (obj == null) continue;

                ComboProjectile projectile = obj.GetComponent<ComboProjectile>();
                if (projectile == null) continue;
                
                projectile.Init(Player);
                projectile.Speed = Random.Range(Speed.x, Speed.y);
                
                obj.SetActive(false);
                m_Projectiles.Add(projectile);
            }
            
            foreach (BaseProjectile projectile in m_Projectiles) {
                projectile.transform.position = GetRandomOriginPosition();
                projectile.gameObject.SetActive(true);
                yield return new WaitForSeconds(Random.Range(SpawnDelay.x, SpawnDelay.y));
            }
        }

        private Vector3 GetRandomOriginPosition() {
            
            Vector3 position = Player.transform.position;
            position += new Vector3(Random.Range(-OriginRadius - OriginRadius / 2, OriginRadius - OriginRadius / 2), OriginOffset);
            return position;
        }

        private void OnDrawGizmosSelected() {

            if (Player == null) return;

            Vector3 start, end;
            start = end = Player.transform.position;
            start += new Vector3(-OriginRadius * 2, OriginOffset);
            end += new Vector3(0, OriginOffset);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(start, end);
        }
    }
}