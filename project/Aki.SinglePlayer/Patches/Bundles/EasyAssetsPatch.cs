using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Diz.Jobs;
using Diz.Resources;
using JetBrains.Annotations;
using Aki.Common.Utils.Patching;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils.Bundles;
using IEasyBundle = GInterface253;                  // Property: SameNameAsset 
using IBundleLock = GInterface254;                  // Property: IsLocked
using BundleLock = GClass2220;                      // Property: MaxConcurrentOperations
using DependencyGraph = GClass2221<GInterface253>;  // Method: GetDefaultNode()

namespace Aki.SinglePlayer.Patches.Bundles
{
    public class EasyAssetsPatch : GenericPatch<EasyAssetsPatch>
    {
        private static Type easyBundleType;
        private static string bundlesFieldName;

        public EasyAssetsPatch() : base(prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            easyBundleType = PatcherConstants.TargetAssembly.GetTypes().Single(type => type.IsClass && type.GetProperty("SameNameAsset") != null);
            bundlesFieldName = $"{easyBundleType.Name.ToLower()}_0";

            var targetType = PatcherConstants.TargetAssembly.GetTypes().Single(IsTargetType);
            return AccessTools.GetDeclaredMethods(targetType).Single(IsTargetMethod);
        }

        private static bool IsTargetType(Type type)
        {
            var fields = type.GetFields();

            if (fields.Length > 2)
                return false;

            if (!fields.Any(x => x.Name == "Manifest"))
                return false;

            return type.GetMethod("Create", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly) != null;
        }

        private static bool IsTargetMethod(MethodInfo mi)
        {
            var parameters = mi.GetParameters();
            return (parameters.Length != 5 || parameters[0].Name != "bundleLock" || parameters[1].Name != "defaultKey" || parameters[4].Name != "shouldExclude") ? false : true;
        }

        static bool PatchPrefix(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude, ref Task __result)
        {
            __result = Init(__instance, bundleLock, defaultKey, rootPath, platformName, shouldExclude);
            return false;
        }

        public static async Task Init(EasyAssets __instance, [CanBeNull] IBundleLock bundleLock, string defaultKey, string rootPath, string platformName, [CanBeNull] Func<string, bool> shouldExclude)
        {
            var traverse = Traverse.Create(__instance);
            var path = $"{rootPath.Replace("file:///", "").Replace("file://", "")}/{platformName}/";
            
            var manifestLoading = AssetBundle.LoadFromFileAsync(path + platformName);
            await manifestLoading.Await();

            var assetBundle = manifestLoading.assetBundle;
            var assetLoading = assetBundle.LoadAllAssetsAsync();
            await assetLoading.Await();

            traverse.Field<AssetBundleManifest>("Manifest").Value = (AssetBundleManifest)assetLoading.allAssets[0];
            var manifest = traverse.Field<AssetBundleManifest>("Manifest").Value;

            // add ModManifest
            var result = manifest.GetAllAssetBundles().ToList<string>();
            var resourcesModbundles = new List<string>();

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.bundles)
            {
                resourcesModbundles.Add(kvp.Key);
            }

            var bundleNames = result.Union(resourcesModbundles).ToList<string>().ToArray<string>();

            traverse.Field(bundlesFieldName).SetValue(Array.CreateInstance(easyBundleType, bundleNames.Length));

            if (bundleLock == null)
            {
                bundleLock = new BundleLock(int.MaxValue);
            }

            var bundles = traverse.Field(bundlesFieldName).GetValue<IEasyBundle[]>();

            for (var i = 0; i < bundleNames.Length; i++)
            {
                bundles[i] = (IEasyBundle)Activator.CreateInstance(easyBundleType, new object[] { bundleNames[i], path, manifest, bundleLock });
                await JobScheduler.Yield();
            }

            traverse.Field(bundlesFieldName).SetValue(bundles);
            traverse.Property<DependencyGraph>("System").Value = new DependencyGraph(bundles, defaultKey, shouldExclude);
        }
    }
}