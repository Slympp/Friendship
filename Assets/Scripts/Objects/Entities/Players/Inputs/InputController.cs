using UnityEngine;

namespace Entities.Players {
    public static class InputController {


        public static Vector2 Movement(int playerId) => new Vector2(Input.GetAxisRaw($"Player{playerId}_Horizontal"), 
                                                    Input.GetAxisRaw($"Player{playerId}_Vertical"));
        public static bool Shoot(int playerId) => Input.GetAxis($"Player{playerId}_Shoot") >= 0.9;
        public static bool Jump(int playerId) => Input.GetButton($"Player{playerId}_Jump");
        public static bool Crouch(int playerId) => Input.GetButton($"Player{playerId}_Crouch");
        public static bool Lock(int playerId) => Input.GetAxis($"Player{playerId}_Lock") >= 0.9;
        public static bool OffensiveAbility(int playerId) => Input.GetButtonDown($"Player{playerId}_OffensiveAbility");
        public static bool SupportAbility(int playerId) => Input.GetButtonDown($"Player{playerId}_SupportAbility");
    }
}
