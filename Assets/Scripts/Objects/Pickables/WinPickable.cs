using UnityEngine;

namespace Objects.Pickables {
    public class WinPickable : MonoBehaviour {

        [SerializeField] private LayerMask CollisionMask;
        
        private void OnTriggerEnter2D(Collider2D c) {
            if (CollideMask(c) && c.CompareTag("Player")) {
                GameManager.GameManager.Instance.EnableWin();
                Destroy(gameObject);
            }
        }
        
        protected bool CollideMask(Collider2D c) {
            return CollisionMask == (CollisionMask | (1 << c.gameObject.layer));
        }
    }
}