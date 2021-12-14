﻿using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Reflection.Patching;
using Aki.Reflection.Utils;
using Aki.Core.Models;
using FilesChecker;

namespace Aki.Core.Patches
{
    public class ConsistencySinglePatch : Patch
    {
        public ConsistencySinglePatch() : base(T: typeof(ConsistencySinglePatch), prefix: nameof(PatchPrefix))
        {
        }

        protected override MethodBase GetTargetMethod()
        {
            return Constants.FilesCheckerTypes.Single(x => x.Name == "ConsistencyController")
                .GetMethods().Single(x => x.Name == "EnsureConsistencySingle" && x.ReturnType == typeof(Task<ICheckResult>));
        }

        private static bool PatchPrefix(ref object __result)
        {
            __result = Task.FromResult<ICheckResult>(new FakeFileCheckerResult());
            return false;
        }
    }
}