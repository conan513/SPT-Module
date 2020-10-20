﻿using UnityEngine;
using SPTarkov.Common.Utils.Patching;
using SPTarkov.SinglePlayer.Patches.Bots;
using SPTarkov.SinglePlayer.Patches.Matchmaker;
using SPTarkov.SinglePlayer.Patches.Progression;
using SPTarkov.SinglePlayer.Patches.Quests;
using SPTarkov.SinglePlayer.Patches.RaidFix;
using SPTarkov.SinglePlayer.Patches.ScavMode;

namespace SPTarkov.SinglePlayer
{
    public class Instance : MonoBehaviour
    {
        private void Start()
		{
            Debug.LogError("SPTarkov.SinglePlayer: Loaded");

			PatcherUtil.Patch<OfflineLootPatch>();
			PatcherUtil.Patch<OfflineSaveProfilePatch>();
            //PatcherUtil.Patch<OfflineSpawnPointPatch>();
            PatcherUtil.Patch<WeaponDurabilityPatch>();
            PatcherUtil.Patch<SingleModeJamPatch>();
            PatcherUtil.Patch<ExperienceGainPatch>();

            PatcherUtil.Patch<Patches.Healing.MainMenuControllerPatch>();
			PatcherUtil.Patch<Patches.Healing.PlayerPatch>();

			PatcherUtil.Patch<MatchmakerOfflineRaidPatch>();
			PatcherUtil.Patch<MatchMakerSelectionLocationScreenPatch>();
			PatcherUtil.Patch<InsuranceScreenPatch>();

            PatcherUtil.Patch<BossSpawnChancePatch>();
			PatcherUtil.Patch<BotTemplateLimitPatch>();
            PatcherUtil.Patch<GetNewBotTemplatesPatch>();
            PatcherUtil.Patch<RemoveUsedBotProfilePatch>();
            //PatcherUtil.Patch<SpawnPmcPatch>();
			PatcherUtil.Patch<CoreDifficultyPatch>();
			PatcherUtil.Patch<BotDifficultyPatch>();
            
            PatcherUtil.Patch<OnDeadPatch>();
            PatcherUtil.Patch<OnShellEjectEventPatch>();
            PatcherUtil.Patch<BotStationaryWeaponPatch>();

            PatcherUtil.Patch<BeaconPatch>();
			PatcherUtil.Patch<DogtagPatch>();

            PatcherUtil.Patch<LoadOfflineRaidScreenPatch>();
            PatcherUtil.Patch<ScavPrefabLoadPatch>();
            PatcherUtil.Patch<ScavProfileLoadPatch>();
            PatcherUtil.Patch<ScavExfilPatch>();

            PatcherUtil.Patch<EndByTimerPatch>();
        }
    }
}
