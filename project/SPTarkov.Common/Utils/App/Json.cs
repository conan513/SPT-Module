﻿using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Linq;

namespace SPTarkov.Common.Utils.App
{
    public static class Json
	{
		public static string Serialize<T>(T data)
		{
			return JsonConvert.SerializeObject(data);
		}

		public static T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static void Save<T>(string filepath, T data)
		{
			string json = Serialize<T>(data);
			File.WriteAllText(filepath, json);
		}

		/// <summary>
		/// Save an object as json with formatting
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filepath">Full path to file</param>
		/// <param name="data">Object to save to json file</param>
		/// <param name="format">NewtonSoft.Json Formatting</param>
		public static void SaveWithFormatting<T>(string filepath, T data, Formatting format)
		{
			File.WriteAllText(filepath, JsonConvert.SerializeObject(data, format));
		}

		/// <summary>
		/// Load a class from file and don't save it if it doesn't exist.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filepath">Full path to the file to load</param>
		/// <param name="AllowNullValues">Allow null class property values to be returned. Default is false</param>
		/// <returns>Returns a class object or null</returns>
		public static T LoadClassWithoutSaving<T>(string filepath, bool AllowNullValues = false) where T : class
        {
			if(File.Exists(filepath))
            {
			    string json = File.ReadAllText(filepath);

			    T classObject = JsonConvert.DeserializeObject<T>(json);

				if (!AllowNullValues)
				{
					if (classObject.GetType().GetProperties().Any(x => x.GetValue(classObject) == null))
					{
						return null;
					}
				}

				return classObject;
            }

			return null;
		}

		public static T Load<T>(string filepath) where T : new()
		{
			if (!File.Exists(filepath))
			{
				Save(filepath, new T());
				return Load<T>(filepath);
			}

			string json = File.ReadAllText(filepath);
			return Deserialize<T>(json);
		}
	}
}
