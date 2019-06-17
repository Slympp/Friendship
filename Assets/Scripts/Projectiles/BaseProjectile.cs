using System;
using UnityEngine;

namespace Projectiles {
    public class BaseProjectile : MonoBehaviour {

        [SerializeField] private LayerMask CollisionMask;
        [SerializeField] private float MovementSpeed;

        void FixedUpdate() {
            transform.Translate(MovementSpeed * Time.deltaTime * transform.right, Space.World);
        }

        void OnTriggerEnter2D(Collider2D c) {
            if (CollisionMask == (CollisionMask | (1 << c.gameObject.layer))) {
                DestroySelf();
            }
        }

        private void OnBecameInvisible() {
            DestroySelf();
        }

        private void DestroySelf() {
            Destroy(gameObject);
        }
    }
}
