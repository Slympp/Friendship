using UnityEngine;

namespace Objects.Pickables {
    public class PotionPickable : MonoBehaviour {
        private const int FriendshipValue = 50;
        
        private void OnTriggerEnter2D(Collider2D c) {
            if (GameManager.GameManager.Instance.CurrentFriendship == 100)
                return;
            
            if (c.CompareTag("Player")) {
                GameManager.GameManager.Instance.UpdateFriendshipAmount(FriendshipValue);
                Destroy(gameObject);
            }
        }
    }
}