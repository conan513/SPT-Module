﻿using Aki.Bundles.Models;
using Aki.Bundles.Utils;
using Aki.Reflection.Patching;
using Diz.DependencyManager;
using UnityEngine.Build.Pipeline;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Aki.Bundles.Patches
{
    public class EasyBundlePatch : ModulePatch
    {
        static EasyBundlePatch()
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState<ELoadState>.Bind);
        }

        protected override MethodBase GetTargetMethod()
        {
            return EasyBundleHelper.Type.GetConstructors()[0];
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance, string key, string rootPath, CompatibilityAssetBundleManifest manifest, IBundleLock bundleLock)
        {
            var path = rootPath + key;
            var dependencyKeys = manifest.GetDirectDependencies(key) ?? new string[0];

            if (BundleSettings.Bundles.TryGetValue(key, out BundleInfo bundle))
            {
                if (dependencyKeys.Length > 0)
                {
                    dependencyKeys = dependencyKeys.Union(bundle.DependencyKeys).ToArray();
                }
                
                path = bundle.Path;
            }

            _ = new EasyBundleHelper(__instance)
            {
                Key = key,
                Path = path,
                KeyWithoutExtension = Path.GetFileNameWithoutExtension(key),
                DependencyKeys = dependencyKeys,
                LoadState = new BindableState<ELoadState>(ELoadState.Unloaded, null),
                BundleLock = bundleLock
            };
        }
    }
}
