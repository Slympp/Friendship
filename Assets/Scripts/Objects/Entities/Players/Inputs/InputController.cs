using UnityEngine;

namespace Entities.Players {
    public static class InputController {


        public static Vector2 Movement(string inputSource) => new Vector2(Input.GetAxisRaw($"{inputSource}_Horizontal"), 
                                                    Input.GetAxisRaw($"{inputSource}_Vertical"));
        public static bool Shoot(string inputSource) => Input.GetAxis($"{inputSource}_Shoot") >= 0.9;
        public static bool Jump(string inputSource) => Input.GetButton($"{inputSource}_Jump");
        public static bool Crouch(string inputSource) => Input.GetButton($"{inputSource}_Crouch");
        public static bool Lock(string inputSource) => Input.GetAxis($"{inputSource}_Lock") >= 0.9;
        public static bool OffensiveAbility(string inputSource) => Input.GetButtonDown($"{inputSource}_OffensiveAbility");
        public static bool SupportAbility(string inputSource) => Input.GetButtonDown($"{inputSource}_SupportAbility");
    }
}
