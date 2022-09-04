using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
    [HarmonyPatch(typeof(CharacterMotor))]
    [HarmonyPatch("ApplyGravityAndJumping")]
    public static class CharacterMotor_Patcher_ApplyGravityAndJumping_Postfix
    {
        [HarmonyPostfix]
        public static void CharacterMotor_ApplyGravityAndJumping_Postfix(CharacterMotor __instance, ref Vector3 velocity, ref Vector3 __result)
        {
            if (ModHelper.jump)
            {
                __instance.grounded = false;
                __instance.jumping.jumping = true;
                __instance.jumping.lastStartTime = Time.time;
                __instance.jumping.lastButtonDownTime = -100f;
                __instance.jumping.holdingJumpButton = true;

                float jumpHeight = 1.45f;
                float jumpSpeed = 0.35f;

                __instance.jumping.jumpDir = Vector3.Slerp(new Vector3(0f, jumpHeight, 0f), __instance.groundNormal, jumpSpeed);

                velocity.y = 0f;
                velocity += __instance.jumping.jumpDir * Mathf.Sqrt(2f * __instance.jumping.baseHeight * __instance.movement.gravity);
                if (__instance.movingPlatform.enabled && (__instance.movingPlatform.movementTransfer == CharacterMotor.MovementTransferOnJump.InitTransfer || __instance.movingPlatform.movementTransfer == CharacterMotor.MovementTransferOnJump.PermaTransfer))
                {
                    __instance.movement.frameVelocity = __instance.movingPlatform.platformVelocity;
                    velocity += __instance.movingPlatform.platformVelocity;
                }
                __instance.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);

                ModHelper.jump = false;

                __result = velocity;
            }
            else
            {
                ModHelper.jump = false;
            }
        }
    }
}
