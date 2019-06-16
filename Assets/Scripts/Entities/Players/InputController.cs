using UnityEngine;

namespace Entities.Players {
    public static class InputController {
        
        public static Vector2 Movement => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        public static Vector2 Shoot    => new Vector2(Input.GetAxisRaw("HorizontalShoot"), Input.GetAxisRaw("VerticalShoot"));
    }
}
