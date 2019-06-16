using UnityEngine;

namespace Entities.Players {
    
    public class PlayerController : MonoBehaviour {
        
        private CharacterController2D _characterController;
    
        void Awake() {
            _characterController = GetComponent<CharacterController2D>();
        }

        void FixedUpdate() {

            Vector2 movement = InputController.Movement;
            _characterController.Move(movement.x, movement.y < 0, movement.y > 0);
        }
    }
}
