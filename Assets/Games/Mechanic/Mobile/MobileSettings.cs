using UnityEngine;

/// <summary>
/// Réglages mobiles appliqués automatiquement au lancement, sans objet dans la scène.
/// </summary>
public static class MobileSettings
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        // Sur mobile, Unity tourne à 30 fps par défaut.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}
