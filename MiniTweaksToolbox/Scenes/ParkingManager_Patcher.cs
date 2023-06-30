using Harmony12;

namespace MiniTweaksToolbox
{
    [HarmonyPatch(typeof(ParkingManager))]
    [HarmonyPatch("Start")]
    public static class ParkingManager_Patcher_Start_Postfix
    {
        [HarmonyPostfix]
        public static void ParkingManager_Start_Postfix()
        {
            ModHelper.playerPosition.y = ModHelper.playerPosition.y + GlobalData.GoToParkingLevel * 5;
        }
    }
}
