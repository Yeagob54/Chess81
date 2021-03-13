using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Utilities
{
    public class FileManager : MonoBehaviour
    {
        [DllImport("__Internal")]
        private static extern void SyncFiles();

        [DllImport("__Internal")]
        private static extern void WindowAlert(string message);

        public static string LoadFromJson(string fileName)
        {
            string jsonData = "";

            using (var fileStream = File.OpenRead(fileName))//abrir archio texto
            {
                using (var streamReader = new StreamReader(fileStream))//Crea un variable de lectura del archivo de texto
                {
                    jsonData = streamReader.ReadToEnd();
                }

                fileStream.Close();
            }


            return jsonData;
        }

        public static void SaveToJson(string json, string fileName)
        {
            File.Delete(fileName);
            using (var fileStream = File.OpenWrite(fileName))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(json);
                }
                fileStream.Close();
            }
        }

        public static void SaveToJson(string json, FileStream file)
        {
            using (StreamWriter streamWriter = new StreamWriter(file))
            {
                streamWriter.Write(json);
            }
        }

        public static List<string> LoadStringList(string fileName)
        {
            List<string> rocks = new List<string>();
            print(fileName);
            if (!File.Exists(fileName))
            {
                File.Create(fileName);
                return rocks;
            }

            int index = 0;
            using (var fileStream = File.OpenRead(fileName))//abrir archio texto
            {
                using (var streamReader = new StreamReader(fileStream))//Crea un variable de lectura del archivo de texto
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null && line != "")
                    {
                        rocks.Add(line);

                        index++;
                    }
                }
            }
            return rocks;
        }

        public static void SaveString(string text, string fileName)
        {
            //To be sure always create a new file
            File.Delete(fileName);

            using (var fileStream = File.OpenWrite(fileName))//abrir archico texto
            {
                using (var streamWriter = new StreamWriter(fileStream))//Crea un variable de lectura del archivo de texto
                {
                        streamWriter.WriteLine(text);
                }
                fileStream.Close();
            }
        }

        public static void SaveStringList(List<string> elements, string fileName)
        {
            //To be sure always create a new file
            File.Delete(fileName);

            using (var fileStream = File.OpenWrite(fileName))//abrir archico texto
            {
                using (var streamWriter = new StreamWriter(fileStream))//Crea un variable de lectura del archivo de texto
                {
                    foreach (string data in elements)
                    {
                        streamWriter.WriteLine(data);
                    }
                }
                fileStream.Close();
            }
        }

        public static void Save(UserProfile userDetails)
        {
            string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);
            FileStream fileStream;

            try
            {
                if (File.Exists(dataPath))
                {
                    File.WriteAllText(dataPath, string.Empty);
                    fileStream = File.Open(dataPath, FileMode.Open);
                }
                else
                {
                    fileStream = File.Create(dataPath);
                }

                SaveToJson(userDetails.ToJson(), fileStream);

                fileStream.Close();

                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    SyncFiles();
                }
            }
            catch (Exception e)
            {
                PlatformSafeMessage("Failed to Save: " + e.Message);
            }
        }

        public static UserProfile Load()
        {
            UserProfile userDetails = new UserProfile(false); ;
            string dataPath = string.Format("{0}/GameDetails.dat", Application.persistentDataPath);

            try
            {
                if (File.Exists(dataPath))
                {
                    userDetails.FromJson(LoadFromJson(dataPath));
                }
            }
            catch (Exception e)
            {
                PlatformSafeMessage("Failed to Load: " + e.Message);
            }

            return userDetails;
        }

        private static void PlatformSafeMessage(string message)
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                WindowAlert(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

    }
      

}

