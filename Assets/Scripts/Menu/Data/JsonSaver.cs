using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using UnityEngine;

namespace LevelManagement.Data
{
	public class JsonSaver
	{
		private static readonly string fileName = "saveData1.sav";

		public static string GetSaveFileName()
		{
			return Application.persistentDataPath + "/" + fileName;
		}

		public void Save(SaveData data)
		{
			data.hashValue = String.Empty;

			string json = JsonUtility.ToJson(data);

			data.hashValue = GetSHA256(json);
			json = JsonUtility.ToJson(data);
			// Debug.Log("hashString = " + hashString);

			string saveFileName = GetSaveFileName();
			FileStream filestream = new FileStream(saveFileName, FileMode.Create);
			using (StreamWriter writer = new StreamWriter(filestream))
			{
				writer.Write(json);
			}
		}

		public bool Load(SaveData data)
		{
			string loadFilename = GetSaveFileName();
			if (File.Exists(loadFilename))
			{
				using (StreamReader reader = new StreamReader(loadFilename))
				{
					string json = reader.ReadToEnd();

					// check hash before reading
					if (CheckData(json))
					{
						JsonUtility.FromJsonOverwrite(json, data);
						// Debug.Log("hashes are equal");
					}
					else
					{
						// Debug.Log("you've been hacked");
						Debug.LogWarning("JsonSaver Load: invalid hash. Aborting file read...");
					}

				}
				return true;
			}
			return false;
		}

		public void Delete()
		{
			File.Delete(GetSaveFileName());
		}

		bool CheckData(string json)
		{
			SaveData tempSaveData = new SaveData();
			JsonUtility.FromJsonOverwrite(json, tempSaveData);

			string oldHash = tempSaveData.hashValue;
			tempSaveData.hashValue = String.Empty;

			string tempJson = JsonUtility.ToJson(tempSaveData);
			string newHash = GetSHA256(tempJson);

			return (oldHash == newHash);
		}

		string GetSHA256(string text)
		{
			byte[] textToBytes = Encoding.UTF8.GetBytes(text);
			SHA256Managed mySHA256 = new SHA256Managed();

			byte[] hashValue = mySHA256.ComputeHash(textToBytes);

			// return hex string
			return GetHexStringFromHash(hashValue);
		}

		public string GetHexStringFromHash(byte[] hash)
		{
			string hexString = String.Empty;
			foreach (byte b in hash)
			{
				hexString += b.ToString("x2");
			}
			return hexString;
		}
	}
}
