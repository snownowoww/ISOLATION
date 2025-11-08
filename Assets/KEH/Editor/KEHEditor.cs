using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KEH))]
public class KEHEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 표시
        DrawDefaultInspector();

        KEH keh = (KEH)target;

        EditorGUILayout.Space(10);

        // 땅 생성 버튼
        if (GUILayout.Button("땅 생성", GUILayout.Height(30)))
        {
            if (!Application.isPlaying)
            {
                keh.GenerateGroundInEditor();
                Debug.Log("땅이 생성되었습니다.");
            }
            else
            {
                Debug.LogWarning("플레이 중에는 에디터 생성 버튼을 사용할 수 없습니다.");
            }
        }

        // 땅 제거 버튼
        if (GUILayout.Button("땅 제거", GUILayout.Height(25)))
        {
            if (!Application.isPlaying)
            {
                keh.ClearGround();
                Debug.Log("기존 땅이 삭제되었습니다.");
            }
            else
            {
                Debug.LogWarning("플레이 중에는 에디터 삭제 버튼을 사용할 수 없습니다.");
            }
        }
    }
}