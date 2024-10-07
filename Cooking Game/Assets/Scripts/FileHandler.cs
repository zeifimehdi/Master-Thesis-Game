using System.IO;
using UnityEngine;

public static class FileHandler
{
    public static void CreateFile(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".txt";

        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) { }
        }

        Debug.Log("---File Path---" + filePath);
    }

    public static void WriteToFile(string fileName, string data)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".txt";

        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine(data);
        }
    }
}