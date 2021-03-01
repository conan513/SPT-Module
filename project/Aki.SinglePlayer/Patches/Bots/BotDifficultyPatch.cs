/* BotDifficultyPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * Martynas Gestautas
 */


using System.Reflection;
using System.Linq;
using UnityEngine;
using EFT;
using Aki.Common.Utils.HTTP;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class BotDifficultyPatch : GenericPatch<BotDifficultyPatch>
    {
        public BotDifficultyPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            var methodName = "LoadDifficultyStringInternal";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return PatcherConstants.TargetAssembly
                .GetTypes().Single(x => x.GetMethod(methodName, flags) != null)
                .GetMethod(methodName, flags);
        }

        private static bool PatchPrefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
        {
            __result = Request(role, botDifficulty);

            return string.IsNullOrWhiteSpace(__result);
        }

        private static string Request(WildSpawnType role, BotDifficulty botDifficulty)
        {
            var json = new Request(null, Config.BackendUrl).GetJson("/singleplayer/settings/bot/difficulty/" + role.ToString() + "/" + botDifficulty.ToString());

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogError("Aki.SinglePlayer: Received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data is NULL, using fallback");
                return null;
            }

            Debug.LogError("Aki.SinglePlayer: Successfully received bot " + role.ToString() + " " + botDifficulty.ToString() + " difficulty data");
            return json;
        }
    }
}
