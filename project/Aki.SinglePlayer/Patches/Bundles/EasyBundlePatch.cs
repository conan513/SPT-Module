using Aki.Common;
using Aki.Reflection.Patching;
using Aki.SinglePlayer.Models;
using Aki.SinglePlayer.Utils.Bundles;
using Diz.DependencyManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BindableState = GClass2265<Diz.DependencyManager.ELoadState>;
using IBundleLock = GInterface264;
using IEasyBundle = GInterface263;

namespace Aki.SinglePlayer.Patches.Bundles
{
    public class EasyBundlePatch : Patch
	{
        static EasyBundlePatch()
        {
            _ = nameof(IEasyBundle.SameNameAsset);
            _ = nameof(IBundleLock.IsLocked);
            _ = nameof(BindableState.Bind);
        }

        public EasyBundlePatch() : base(T: typeof(EasyBundlePatch), prefix: nameof(PatchPrefix), postfix: nameof(PatchPostfix))
        {
        }

        protected override MethodBase GetTargetMethod()
		{
            return EasyBundleHelper.Type.GetConstructors()[0];
        }

        private static bool PatchPrefix()
        {
            return false;
        }

        private static void PatchPostfix(object __instance, string key, string rootPath, AssetBundleManifest manifest, IBundleLock bundleLock)
		{
            var path = rootPath + key;
            var dependencyKeys = manifest.GetDirectDependencies(key);

            if (BundleSettings.Bundles.TryGetValue(key, out BundleInfo bundle))
            {
                path = bundle.Path;
            }

            foreach (KeyValuePair<string, BundleInfo> kvp in BundleSettings.Bundles)
            {
                if (!key.Equals(kvp.Key))
                {
                    continue;
                }

                var result = dependencyKeys == null ? new List<string>() : dependencyKeys.ToList();
                dependencyKeys = result.Union(kvp.Value.DependencyKeys).ToArray();
                break;
            }

            try
            {
                _ = new EasyBundleHelper(__instance)
                {
                    Key = key,
                    Path = path,
                    KeyWithoutExtension = Path.GetFileNameWithoutExtension(key),
                    DependencyKeys = dependencyKeys,
                    LoadState = new BindableState(ELoadState.Unloaded, null),
                    BundleLock = bundleLock
                };
            }
            catch (Exception ex)
            {
                Log.Error($"EasyBundlePatch encountered an error. Custom bundles will not be loaded!\n{ex.Message}\n{ex.StackTrace}");
                Debug.LogException(ex);
            }
        }
	}
}
