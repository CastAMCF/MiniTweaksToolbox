using Harmony12;
using System.Linq;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(CarPaintLogic))]
	[HarmonyPatch("RunFX")]
	public static class CarPaintLogic_Patcher_RunFX
	{
		[HarmonyPrefix]
		public static void CarPaintLogic_RunFX(ref Color ___currentRGB, ref ParticleSystem ___paintParticles, ref CarLoader ___carLoader)
		{
			if (___paintParticles != null && ___carLoader != null && !Main.carColorCheck.Equals(""))
			{
				___currentRGB = Main.carColor;

				float z = ___carLoader.GetCarParts().FirstOrDefault(p => p.name.Equals("body")).handle.transform.position.z;
				float y = ___carLoader.GetCarParts().FirstOrDefault(p => p.name.Equals("body")).handle.transform.position.y;
				float x = ___carLoader.GetCarParts().FirstOrDefault(p => p.name.Equals("body")).handle.transform.position.x;
				float height = ___carLoader.GetCarParts().FirstOrDefault(p => p.name.Equals("body")).handle.transform.localScale.y;

				___paintParticles.transform.position = new Vector3(x, y + (height / 2), z);
            }
            else
            {
				___paintParticles.transform.position = new Vector3(-5.246998f, 0.905f, -24.314f);
			}
		}
	}
}
