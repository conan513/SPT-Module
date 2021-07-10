﻿using System;
using System.Collections.Generic;
using Aki.Common.Utils;

namespace Aki.Loader
{
    public static class Loader
    {
        private static readonly List<string> _repositories;

        static Loader()
        {
            _repositories = new List<string>();
        }

        public static void AddRepository(string path)
        {
            if (VFS.Exists(path) && VFS.GetDirectories(path).Length > 0)
            {
                _repositories.Add(path);
            }
        }

        public static void LoadAllAssemblies()
        {
            foreach (var repository in _repositories)
            {
                var dirs = VFS.GetDirectories(repository);

                foreach (var dir in dirs)
                {
                    var file = $"{dir}/module.dll";

                    if (VFS.Exists(file))
                    {
                        Exception error = RunUtil.LoadAndRun(file);

                        if (error == null)
                        {
                            Log.Info($"Aki.Loader: Loaded '{file}'!");
                        }
                        else
                        {
                            Log.Error($"Aki.Loader: Failed to load '{file}'! Exception below.");
                            Log.Write(error.Message);
                            Log.Write(error.StackTrace);
                        }
                    }
                    else
                    {
                        Log.Error($"Aki.Loader: Failed to find module.dll in '{dir}'");
                    }
                }
            }
        }
    }
}
