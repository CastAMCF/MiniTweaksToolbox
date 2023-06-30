using Harmony12;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(PuszkaWlewOleju))]
	[HarmonyPatch("Update")]
	public static class PuszkaWlewOleju_Patcher_Update_Prefix
    {
		[HarmonyPrefix]
		public static void PuszkaWlewOleju_Update_Prefix(PuszkaWlewOleju __instance, CarLoader ___carLoader, ref float ___power, ref ParticleSystem.EmissionModule ___emitEmission, ref ParticleSystem.EmissionModule ___emitFullEmission)
		{
            if (___carLoader.GetOilLevel() == 1f && Main.Settings.noOilDrain)
            {
                ___power = 0f;
                __instance.GetComponent<AudioSource>().Pause();
                ___emitFullEmission.enabled = false;
                ___emitEmission.enabled = false;
            }
        }
	}
}
