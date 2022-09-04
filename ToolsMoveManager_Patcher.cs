using Harmony12;
using OrbCreationExtensions;
using UnityEngine;

namespace MiniTweaksToolbox
{
	[HarmonyPatch(typeof(ToolsMoveManager))]
	[HarmonyPatch("MoveTo")]
	public static class ToolsMoveManager_Patcher_MoveTo_Postfix
    {
		[HarmonyPostfix]
		public static void ToolsMoveManager_MoveTo_Postfix(ref IOSpecialType tool, ref CarPlace place, ref Transform ___EngineCrane)
		{
            CarLoader carLoaderForPlace = CarLoaderPlaces.Get().GetCarLoaderForPlace(place);
            Vector3 position = default(Vector3);
            Transform transform;

            switch (tool)
			{
                case IOSpecialType.EngineCrane:

                    transform = ___EngineCrane;
                    position.x = carLoaderForPlace.GetRoot().transform.Find("model(Clone)").transform.position.x;
                    position.z = carLoaderForPlace.GetRoot().transform.Find("model(Clone)").transform.position.z;
                    
                    if (carLoaderForPlace.GetEngine().transform.Find("korek_spustowy_1(0)").position.z > carLoaderForPlace.GetRoot().transform.Find("model(Clone)").transform.position.z)
                    {
                        position.z += carLoaderForPlace.GetRoot().transform.Find("model(Clone)").gameObject.GetWorldBounds().size.z / 2f;
                        position.z += ___EngineCrane.gameObject.GetWorldBounds().size.z / 2f;
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 205.89f, transform.eulerAngles.z);
                    }
                    else
                    {
                        position.z -= carLoaderForPlace.GetRoot().transform.Find("model(Clone)").gameObject.GetWorldBounds().size.z / 2f;
                        position.z -= ___EngineCrane.gameObject.GetWorldBounds().size.z / 2f;
                        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 25.89f, transform.eulerAngles.z);
                    }
                    
                    transform.position = position;
                    break;
                default:
                    return;
            }
        }
	}
}
