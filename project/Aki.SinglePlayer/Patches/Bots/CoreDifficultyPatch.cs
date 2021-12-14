using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.SinglePlayer.Utils;
using System.Linq;
using System.Reflection;

namespace Aki.SinglePlayer.Patches.Bots
{
    public class CoreDifficultyPatch : ModulePatch
	{
        protected override MethodBase GetTargetMethod()
        {
			var methodName = "LoadCoreByString";
			var flags = BindingFlags.Public | BindingFlags.Static;

			return Constants.EftTypes.Single(x => x.GetMethod(methodName, flags) != null)
				.GetMethod(methodName, flags);
		}

		[PatchPrefix]
		private static bool PatchPrefix(ref string __result)
		{
            __result = RequestHandler.GetJson("/singleplayer/settings/bot/difficulty/core/core");
			return string.IsNullOrWhiteSpace(__result);
        }
    }
}
