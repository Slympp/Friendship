using System.Collections;
using UnityEngine;

namespace Objects.Pickables {
    public class PotionPickable : MonoBehaviour {
        private const int FriendshipValue = 35;
        private const float PickUpDuration = 3f;

        private bool _pickedUp;
        
        private void OnTriggerEnter2D(Collider2D c) {
            if (_pickedUp)
                return;
            
            if (GameManager.GameManager.Instance.CurrentFriendship == 100)
                return;
            
            if (c.CompareTag("Player")) {
                _pickedUp = true;
                StartCoroutine(OnPickup());
            }
        }

        private IEnumerator OnPickup() {
            GameManager.GameManager.Instance.UpdateFriendshipAmount(FriendshipValue);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(PickUpDuration);
            Destroy(gameObject);
        }
    }
}