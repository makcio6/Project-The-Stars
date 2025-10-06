using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(10);
        PlayerStats stats = (PlayerStats)target;
        GUILayout.Label("Debug buttons");
        if (GUILayout.Button("Добавить контейнер здоровья"))
        {
            stats.AddHealthContainer();
        }

        if (GUILayout.Button("Убрать контейнер здоровья"))
        {
            stats.RemoveHealthContainer();
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("Получить урон"))
        {
            stats.TakeDamage(1);
        }

        if (GUILayout.Button("Лечение"))
        {
            stats.Heal(1);
        }

        if (GUILayout.Button("Полное лечение"))
        {
            stats.Heal(999);
        }

    }
}
