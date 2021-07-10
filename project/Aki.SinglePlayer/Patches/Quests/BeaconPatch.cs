using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using Aki.Common.Utils.Patching;

namespace Aki.SinglePlayer.Patches.Quests
{
    public class BeaconPatch : GenericPatch<BeaconPatch>
    {
        public BeaconPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Single(IsTargetMethod);
        }

        private bool IsTargetMethod(MethodInfo method)
        {
            if (!method.IsVirtual)
            {
                return false;
            }

            var parameters = method.GetParameters();

            return (parameters.Length == 2
                && parameters[0].Name == "item"
                && parameters[1].Name == "zone"
                && parameters[0].ParameterType == typeof(Item)
                && parameters[1].ParameterType == typeof(string));
        }

        private static bool PatchPrefix(Player __instance, Item item, string zone)
        {
            __instance.Profile.ItemDroppedAtPlace(item.TemplateId, zone);
            return false;
        }
    }
}
