/* CoreDifficultyPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * Martynas Gestautas
 */


using System.Reflection;
using UnityEngine;
using Aki.Common.Utils.HTTP;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;
using System.Linq;

namespace Aki.SinglePlayer.Patches.Bots
{
	public class CoreDifficultyPatch : GenericPatch<CoreDifficultyPatch>
	{
		public CoreDifficultyPatch() : base(prefix: nameof(PatchPrefix))
		{
        }

        protected override MethodBase GetTargetMethod()
        {
			var methodName = "LoadCoreByString";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return PatcherConstants.TargetAssembly
                .GetTypes().Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
		}

		public static bool PatchPrefix(ref string __result)
		{
            __result = Request();

			return string.IsNullOrWhiteSpace(__result);
        }

        private static string Request()
        {
            var json = new Request(null, Config.BackendUrl).GetJson("/singleplayer/settings/bot/difficulty/core/core");

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Aki.SinglePlayer: Received core bot difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received core bot difficulty data");
            return json;
        }
    }
}
