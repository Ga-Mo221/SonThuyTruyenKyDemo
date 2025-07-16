using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_save.json");

    public static void SavePlayer(PlayerStats stats)
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Đã lưu dữ liệu vào: " + SavePath);
    }

    public static PlayerStats LoadPlayer()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("Không tìm thấy file save.");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        PlayerStats stats = JsonUtility.FromJson<PlayerStats>(json);
        return stats;
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}
