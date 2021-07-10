using System;
using System.Linq;
using System.Reflection;
using EFT;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils;

namespace Aki.SinglePlayer.Patches.Progression
{
    public class EndByTimerPatch : GenericPatch<EndByTimerPatch>
    {
        private static PropertyInfo _profileId;
        private static MethodInfo _stopRaid;

        static EndByTimerPatch()
        {
        }

        public EndByTimerPatch() : base(prefix: nameof(PrefixPatch))
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            _profileId = PatcherConstants.LocalGameType.BaseType
                .GetProperty("ProfileId", flags);

            _stopRaid = PatcherConstants.LocalGameType.BaseType
                .GetMethods(flags).SingleOrDefault(IsStopRaidMethod);
        }

        protected override MethodBase GetTargetMethod()
        {
            return PatcherConstants.LocalGameType.BaseType
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Single(x => x.Name.EndsWith("StopGame"));
        }

        private static bool IsStopRaidMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length == 4
                    && parameters[0].Name == "profileId"
                    && parameters[1].Name == "exitStatus"
                    && parameters[2].Name == "exitName"
                    && parameters[3].Name == "delay"
                    && parameters[0].ParameterType == typeof(string)
                    && parameters[1].ParameterType == typeof(ExitStatus)
                    && parameters[2].ParameterType == typeof(string)
                    && parameters[3].ParameterType == typeof(float));
        }

        private static bool PrefixPatch(object __instance)
        {
            var profileId = _profileId.GetValue(__instance) as string;
            var json = RequestHandler.GetJson("/singleplayer/settings/raid/endstate");
            var enabled = (!string.IsNullOrWhiteSpace(json)) ? Convert.ToBoolean(json) : false;

            if (!enabled)
            {
                return true;
            }

            _stopRaid.Invoke(__instance, new object[] { profileId, ExitStatus.MissingInAction, null, 0f });
            return false;
        }
    }
}
