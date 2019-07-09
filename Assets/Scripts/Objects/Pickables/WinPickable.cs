using UnityEngine;

namespace Objects.Pickables {
    public class WinPickable : MonoBehaviour {
        
        private void OnTriggerEnter2D(Collider2D c) {
            if (c.CompareTag("Player")) {
                GameManager.GameManager.Instance.EnableWin();
                Destroy(gameObject);
            }
        }
    }
}