using UnityEngine;

namespace Entities.Players {
    public static class InputController {

        public static Vector2 Movement => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        public static bool Jump => Input.GetKey(KeyCode.K);
        public static bool Crouch => Input.GetKey(KeyCode.LeftControl);
        public static bool Lock => Input.GetKey(KeyCode.L);
        public static bool Shoot => Input.GetKey(KeyCode.J);
        public static bool MainAbility => Input.GetKeyDown(KeyCode.L);
        public static bool SupportAbility => Input.GetKeyDown(KeyCode.H);
        //        public static Vector2 Shoot    => new Vector2(Input.GetAxisRaw("HorizontalShoot"), Input.GetAxisRaw("VerticalShoot"));
    }
}
