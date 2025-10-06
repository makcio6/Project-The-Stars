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
        if (GUILayout.Button("�������� ��������� ��������"))
        {
            stats.AddHealthContainer();
        }

        if (GUILayout.Button("������ ��������� ��������"))
        {
            stats.RemoveHealthContainer();
        }

        GUILayout.Space(10);
        
        if (GUILayout.Button("�������� ����"))
        {
            stats.TakeDamage(1);
        }

        if (GUILayout.Button("�������"))
        {
            stats.Heal(1);
        }

        if (GUILayout.Button("������ �������"))
        {
            stats.Heal(999);
        }

    }
}
