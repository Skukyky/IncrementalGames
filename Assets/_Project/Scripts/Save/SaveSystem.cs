using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Lecture / écriture du fichier de sauvegarde. Ne connaît pas le jeu : il reçoit et rend un ItemSaveData.
/// L'écriture passe par un fichier temporaire pour qu'un crash ou une coupure en pleine écriture
/// ne corrompe jamais la sauvegarde existante.
/// </summary>
public static class SaveSystem
{
    public const string FILENAME_SAVEDATA = "savedata.json";

    private static string SavePath => Path.Combine(Application.persistentDataPath, FILENAME_SAVEDATA);

    public static void Save(ItemSaveData data)
    {
        if (data == null)
            return;

        string path = SavePath;
        string tempPath = path + ".tmp";
        try
        {
            File.WriteAllText(tempPath, JsonUtility.ToJson(data));
            if (File.Exists(path))
            {
                File.Replace(tempPath, path, null);
            }
            else
            {
                File.Move(tempPath, path);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SaveSystem: échec de la sauvegarde : " + e.Message);
        }
    }

    public static ItemSaveData Load()
    {
        string path = SavePath;
        if (!File.Exists(path))
            return null;

        try
        {
            ItemSaveData data = JsonUtility.FromJson<ItemSaveData>(File.ReadAllText(path));
            data?.MigrateLegacyFields();
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("SaveSystem: sauvegarde illisible : " + e.Message);
            return null;
        }
    }
}
