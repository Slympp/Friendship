using UnityEngine;

namespace Entities.Players {
    public static class InputController {

        public static int PlayerId = 1;

        public static Vector2 Movement => new Vector2(Input.GetAxisRaw($"Player{PlayerId}_Horizontal"), 
                                                    Input.GetAxisRaw($"Player{PlayerId}_Vertical"));
        public static bool Shoot => Input.GetAxis($"Player{PlayerId}_Shoot") >= 0.9;
        public static bool Jump => Input.GetButton($"Player{PlayerId}_Jump");
        public static bool Crouch => Input.GetButton($"Player{PlayerId}_Crouch");
        public static bool Lock => Input.GetAxis($"Player{PlayerId}_Lock") >= 0.9;
        public static bool OffensiveAbility => Input.GetButtonDown($"Player{PlayerId}_OffensiveAbility");
        public static bool SupportAbility => Input.GetButtonDown($"Player{PlayerId}_SupportAbility");
    }
}
