﻿/* LoadOfflineRaidScreenPatch.cs
 * License: NCSA Open Source License
 * 
 * Copyright: Merijn Hendriks
 * AUTHORS:
 * Merijn Hendriks
 * reider123
 * Ginja
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using EFT;
using EFT.UI.Matchmaker;
using EFT.UI.Screens;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Utils.Reflection;
using MenuController = GClass1190;
using WeatherSettings = GStruct92;
using BotsSettings = GStruct232;
using WavesSettings = GStruct93;

namespace Aki.SinglePlayer.Patches.ScavMode
{
    using OfflineRaidAction = Action<bool, WeatherSettings, BotsSettings, WavesSettings>;

    public class LoadOfflineRaidScreenPatch : GenericPatch<LoadOfflineRaidScreenPatch>
    {
        private static readonly string kBotsSettingsFieldName = "gstruct232_0";
        private static readonly string kWeatherSettingsFieldName = "gstruct92_0";
        private static readonly string kWavesSettingsFieldName = "gstruct93_0";

        private const string kMainControllerFieldName = "gclass1190_0";
        private const string kMenuControllerInnerType = "Class818";
        private const string kTargetMethodName = "method_2";
        private const string kLoadReadyScreenMethodName = "method_35";
        private const string kReadyMethodName = "method_50";

        public LoadOfflineRaidScreenPatch() : base(transpiler: nameof(PatchTranspiler)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MenuController).GetNestedTypes(BindingFlags.NonPublic)
                .Single(x => x.Name == kMenuControllerInnerType)
                .GetMethod(kTargetMethodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        static IEnumerable<CodeInstruction> PatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var index = 26;
            var callCode = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(LoadOfflineRaidScreenPatch), "LoadOfflineRaidScreenForScav"));

            codes[index].opcode = OpCodes.Nop;
            codes[index + 1] = callCode;
            codes.RemoveAt(index + 2);

            return codes.AsEnumerable();
        }

        private static MenuController GetMenuController()
        {
            return PrivateValueAccessor.GetPrivateFieldValue(typeof(MainApplication), 
                kMainControllerFieldName, ClientAppUtils.GetMainApp()) as MenuController;
        }


        // Refer to MatchmakerOfflineRaid's subclass's OnShowNextScreen action definitions if these structs numbers change.
        public static void LoadOfflineRaidNextScreen(bool local, WeatherSettings weatherSettings, BotsSettings botsSettings, WavesSettings wavesSettings)
        {
            var menuController = GetMenuController();

            if (menuController.SelectedLocation.Id == "laboratory")
            {
                wavesSettings.IsBosses = true;
            }

            SetMenuControllerFieldValue(menuController, "bool_0", local);
            SetMenuControllerFieldValue(menuController, kBotsSettingsFieldName, botsSettings);
            SetMenuControllerFieldValue(menuController, kWavesSettingsFieldName, wavesSettings);
            SetMenuControllerFieldValue(menuController, kWeatherSettingsFieldName, weatherSettings);

            typeof(MenuController).GetMethod(kLoadReadyScreenMethodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(menuController, null);
        }

        public static void LoadOfflineRaidScreenForScav()
        {
            var menuController = (object)GetMenuController();
            var gclass = new MatchmakerOfflineRaid.GClass2022();

            gclass.OnShowNextScreen += LoadOfflineRaidNextScreen;
            gclass.OnShowReadyScreen += (OfflineRaidAction)Delegate.CreateDelegate(typeof(OfflineRaidAction), menuController, kReadyMethodName);
            gclass.ShowScreen(EScreenState.Queued);
        }

        private static void SetMenuControllerFieldValue(MenuController instance, string fieldName, object value)
        {
            PrivateValueAccessor.SetPrivateFieldValue(typeof(MenuController), fieldName, instance, value);
        }
    }
}