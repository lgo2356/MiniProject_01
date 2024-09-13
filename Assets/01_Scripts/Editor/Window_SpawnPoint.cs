using UnityEditor;
using UnityEngine;

internal class Window_SpawnPoint : EditorWindow
{
    private static Window_SpawnPoint window;
    private static Vector2 windowSize = new(319, 500);

    private SpawnPoint spawnPoint;
    private SerializedObject serializedObject;
    private Vector2 scrollPosition;

    [MenuItem("Window/SpawnData/Enemy %&F1", priority = 1)]
    private static void OpenWindow()
    {
        if (window == null)
        {
            window = EditorWindow.CreateInstance<Window_SpawnPoint>();
        }

        window.titleContent = new GUIContent("Create Enemy Spawn Point");
        window.minSize = window.maxSize = windowSize;

        window.ShowUtility();
    }

    private void OnEnable()
    {
        spawnPoint = CreateInstance<SpawnPoint>();
        serializedObject = new SerializedObject(spawnPoint);

        Selection.activeObject = null;
    }

    private void OnGUI()
    {
        // Map Size
        {
            SerializedProperty property = serializedObject.FindProperty("MapSize");
            EditorGUILayout.PropertyField(property);
        }

        // Enemy Prefab
        {
            SerializedProperty property = serializedObject.FindProperty("EnemyPrefab");
            EditorGUILayout.PropertyField(property);
        }

        // Spawn Count
        {
            SerializedProperty property = serializedObject.FindProperty("SpawnCount");
            EditorGUILayout.PropertyField(property);
        }

        // Spawn Points
        {
            SerializedProperty property = serializedObject.FindProperty("SpawnPoints");

            if (GUILayout.Button("Create Spawn Points"))
            {
                serializedObject.ApplyModifiedProperties();
                property.ClearArray();

                for (int i = 0; i < spawnPoint.SpawnCount; i++)
                {
                    property.InsertArrayElementAtIndex(i);
                    SerializedProperty childProperty = property.GetArrayElementAtIndex(i);

                    Vector2 point = new()
                    {
                        x = UnityEngine.Random.Range(spawnPoint.MapSize.x, spawnPoint.MapSize.y),
                        y = UnityEngine.Random.Range(spawnPoint.MapSize.x, spawnPoint.MapSize.y),
                    };

                    childProperty.vector2Value = point;
                }
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUILayout.PropertyField(property);
            }
            EditorGUILayout.EndScrollView();
        }

        // Save Button
        if (GUILayout.Button("Save ScriptableObject File"))
        {
            string path = $"{Application.dataPath}/05_ScriptableObjects/";
            path = EditorUtility.SaveFilePanel("Save ScriptableObject File", path, "", "asset");

            if (path.Length > 0)
            {
                DirectoryHelpers.ToRelativePath(ref path);

                serializedObject.ApplyModifiedProperties();
                SpawnPoint spawnPoint = (SpawnPoint)serializedObject.targetObject;

                bool isCheck = true;
                isCheck &= (spawnPoint.EnemyPrefab != null);
                isCheck &= (spawnPoint.SpawnCount > 0);
                isCheck &= (spawnPoint.SpawnPoints != null);

                Debug.Assert(isCheck, "잘못된 값을 입력했습니다.");

                if (isCheck)
                {
                    Enemy enemy = spawnPoint.EnemyPrefab.GetComponent<Enemy>();

                    isCheck &= (enemy != null);
                    isCheck &= (spawnPoint.SpawnPoints.Length > 0);

                    AssetDatabase.CreateAsset(spawnPoint, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    Selection.activeObject = spawnPoint;

                    string fileName = FileHelpers.GetFileName(path);
                    EditorUtility.DisplayDialog("Create SO File", $"{fileName} 생성 완료", "확인");
                }
            }
        }
    }
}
