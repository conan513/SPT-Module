﻿/*
 * Loads and runs an assembly file.
 * Assembly file is expected to have a static function called
 * Main() with either no args or a string[] array.
 * For example: public static void Main(string[] args) is valid.
 * Also: private static void Main() is also valid.
 *
 * Loads all required dependencies as well.
 *
 * Author Epicguru.
 */

using System;
using System.IO;
using System.Reflection;

namespace Aki.Loader
{
    public static class RunUtil
    {
        private static string _depDir;
        private static bool _hasHooked = false;

        public static void LoadAndRun(string dllPath, params string[] args)
        {
            if (!_hasHooked)
            {
                AppDomain.CurrentDomain.AssemblyResolve += DomainAssemblyResolve;
                _hasHooked = true;
            }

            LoadAssAndEntryPoint(dllPath, out var entry, out bool hasStringArray);
            entry.Invoke(null, hasStringArray ? new object[] { args } : new object[0]);
        }

        internal static void LoadAssAndEntryPoint(string dllPath, out MethodInfo entryPoint, out bool hasStringArray)
        {
            Assembly asm = LoadAssembly(dllPath);
            entryPoint = null;

            var entry = FindMainFunction(asm, out hasStringArray);

            if (entry != null)
            {
                LoadDeps(asm, new FileInfo(dllPath).DirectoryName);
                entryPoint = entry;
            }

            throw new Exception($"Failed to find entry point in {asm.FullName}");
        }

        internal static Assembly LoadAssembly(string dllPath)
        {
            byte[] bytes;
            Assembly asm;

            try
            {
                bytes = File.ReadAllBytes(dllPath);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to read assembly bytes from '{dllPath}'", e);
            }

            try
            {
                asm = Assembly.Load(bytes);
            }
            catch (Exception e)
            {
                throw new Exception("Failed created assembly from file bytes. Duplicate assembly?", e);
            }

            return asm;
        }

        internal static Exception LoadDeps(Assembly a, string sourceFolder)
        {
            var domain = AppDomain.CurrentDomain;

            bool IsLoaded(AssemblyName name)
            {
                foreach (var item in domain.GetAssemblies())
                {
                    // TODO make this comparison better.
                    if (item.ToString() == name.ToString())
                    {
                        return true;
                    }
                }
                
                return false;
            }

            var refs = a.GetReferencedAssemblies();

            foreach (var item in refs)
            {
                if (!IsLoaded(item))
                {
                    Assembly created;

                    try
                    {
                        _depDir = sourceFolder;
                        created = domain.Load(item);
                    }
                    catch (Exception e)
                    {
                        return e;
                    }

                    // Note: source folder never changes, so all deps are expected be be in the same folder as main dll.
                    // For example, if A depends on B and B depends on C then
                    // when loading A, B.dll and C.dll should be in the same folder as A.dll
                    Exception newError = LoadDeps(created, sourceFolder);

                    if (newError != null)
                    {
                        return newError;
                    }
                }
            }

            return null;
        }

        private static Assembly DomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            string root = _depDir;
            string dllName = args.Name.Split(',')[0].Trim() + ".dll";
            string path = Path.Combine(root, dllName);

            Console.WriteLine($"Loading dependency '{dllName}' from '{root}'... ");
            return LoadAssembly(path);
        }

        internal static MethodInfo FindMainFunction(Assembly a, out bool hasStringArray)
        {
            foreach (var type in a.GetTypes())
            {
                if (!type.IsClass)
                {
                    continue;
                }

                if (type.IsGenericType)
                {
                    continue;
                }

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (method.IsGenericMethod)
                    {
                        continue;
                    }

                    // Must be called Main just like regular program.
                    if (method.Name == "Main")
                    {
                        // Allowed parameters: none, or an array of strings (such as string[] args)
                        var args = method.GetParameters();

                        if (args.Length == 0)
                        {
                            hasStringArray = false;
                            return method;
                        }

                        if (args.Length == 1 && args[0].ParameterType == typeof(string[]))
                        {
                            hasStringArray = true;
                            return method;
                        }
                    }
                }
            }

            hasStringArray = false;
            return null;
        }
    }
}
