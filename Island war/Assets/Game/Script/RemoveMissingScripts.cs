using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RemoveMissingScripts : EditorWindow
{
    private Vector2 scrollPos;
    private List<GameObject> objectsWithMissing = new List<GameObject>();
    private Dictionary<GameObject, string> objectPaths = new Dictionary<GameObject, string>();
    private bool includeInactive = true;
    private bool scanPrefabs = true;
    private int missingCount = 0;
    private DefaultAsset targetFolder = null;

    [MenuItem("Tools/Remove Missing Scripts")]
    static void Init()
    {
        RemoveMissingScripts window = (RemoveMissingScripts)EditorWindow.GetWindow(typeof(RemoveMissingScripts));
        window.titleContent = new GUIContent("Remove Missing Scripts");
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Remove Missing Scripts Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.HelpBox("Tool này sẽ tìm và xóa tất cả các Missing Script trong Scene hoặc Prefab", MessageType.Info);
        EditorGUILayout.Space(10);

        // Options
        includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);
        scanPrefabs = EditorGUILayout.Toggle("Scan Prefabs in Project", scanPrefabs);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Scan Specific Folder:", EditorStyles.boldLabel);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", targetFolder, typeof(DefaultAsset), false);
        
        EditorGUILayout.Space(10);

        // Buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Scene", GUILayout.Height(30)))
        {
            ScanScene();
        }
        if (GUILayout.Button("Scan Selected", GUILayout.Height(30)))
        {
            ScanSelected();
        }
        EditorGUILayout.EndHorizontal();

        if (scanPrefabs && GUILayout.Button("Scan All Prefabs", GUILayout.Height(30)))
        {
            ScanAllPrefabs();
        }

        EditorGUI.BeginDisabledGroup(targetFolder == null);
        if (GUILayout.Button("Scan Folder Only", GUILayout.Height(30)))
        {
            ScanFolder();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(10);

        // Results
        if (missingCount > 0)
        {
            EditorGUILayout.HelpBox($"Tìm thấy {missingCount} Missing Script(s) trên {objectsWithMissing.Count} GameObject(s)", MessageType.Warning);
            
            if (GUILayout.Button("Remove All Missing Scripts", GUILayout.Height(40)))
            {
                RemoveAllMissingScripts();
            }
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Objects with Missing Scripts:", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            foreach (GameObject go in objectsWithMissing)
            {
                if (go != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                    if (GUILayout.Button("Select", GUILayout.Width(60)))
                    {
                        Selection.activeGameObject = go;
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        RemoveMissingScriptsFromObject(go);
                        objectsWithMissing.Remove(go);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }
        else if (objectsWithMissing.Count == 0 && missingCount == 0)
        {
            EditorGUILayout.HelpBox("Chưa thực hiện scan. Nhấn nút Scan để bắt đầu.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("Không tìm thấy Missing Script nào!", MessageType.Info);
        }
    }

    void ScanScene()
    {
        objectsWithMissing.Clear();
        objectPaths.Clear();
        missingCount = 0;

        GameObject[] allObjects = includeInactive ? 
            Resources.FindObjectsOfTypeAll<GameObject>() : 
            FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            // Skip prefab assets
            if (PrefabUtility.IsPartOfPrefabAsset(go))
                continue;

            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            if (count > 0)
            {
                objectsWithMissing.Add(go);
                missingCount += count;
            }
        }

        Debug.Log($"Scan hoàn tất: Tìm thấy {missingCount} missing script(s) trên {objectsWithMissing.Count} GameObject(s)");
    }

    void ScanSelected()
    {
        objectsWithMissing.Clear();
        objectPaths.Clear();
        missingCount = 0;

        if (Selection.gameObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Vui lòng chọn ít nhất một GameObject!", "OK");
            return;
        }

        foreach (GameObject go in Selection.gameObjects)
        {
            ScanGameObjectRecursive(go);
        }

        Debug.Log($"Scan hoàn tất: Tìm thấy {missingCount} missing script(s) trên {objectsWithMissing.Count} GameObject(s)");
    }

    void ScanGameObjectRecursive(GameObject go)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        if (count > 0)
        {
            objectsWithMissing.Add(go);
            missingCount += count;
        }

        foreach (Transform child in go.transform)
        {
            ScanGameObjectRecursive(child.gameObject);
        }
    }

    void ScanAllPrefabs()
    {
        objectsWithMissing.Clear();
        objectPaths.Clear();
        missingCount = 0;

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int scannedCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                scannedCount++;
                EditorUtility.DisplayProgressBar("Scanning Prefabs", $"Scanning: {prefab.name}", (float)scannedCount / prefabGUIDs.Length);

                ScanPrefabAndChildren(prefab, path);
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Scan {scannedCount} prefab(s) hoàn tất: Tìm thấy {missingCount} missing script(s) trên {objectsWithMissing.Count} GameObject(s)");
    }

    void ScanFolder()
    {
        objectsWithMissing.Clear();
        objectPaths.Clear();
        missingCount = 0;

        if (targetFolder == null)
        {
            EditorUtility.DisplayDialog("No Folder Selected", "Vui lòng kéo folder vào trước!", "OK");
            return;
        }

        string folderPath = AssetDatabase.GetAssetPath(targetFolder);
        
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("Invalid Folder", "Vui lòng chọn một folder hợp lệ!", "OK");
            return;
        }

        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        int scannedCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                scannedCount++;
                EditorUtility.DisplayProgressBar("Scanning Folder", $"Scanning: {prefab.name}", (float)scannedCount / prefabGUIDs.Length);

                ScanPrefabAndChildren(prefab, path);
            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Scan folder '{folderPath}': Tìm thấy {missingCount} missing script(s) trên {objectsWithMissing.Count} GameObject(s) trong {scannedCount} prefab(s)");
    }

    void ScanPrefabAndChildren(GameObject prefab, string prefabPath)
    {
        // Scan root prefab
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(prefab);
        if (count > 0)
        {
            if (!objectsWithMissing.Contains(prefab))
            {
                objectsWithMissing.Add(prefab);
                objectPaths[prefab] = prefabPath;
            }
            missingCount += count;
        }

        // Scan all children recursively
        Transform[] allChildren = prefab.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject == prefab) continue;

            int childCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject);
            if (childCount > 0)
            {
                if (!objectsWithMissing.Contains(child.gameObject))
                {
                    objectsWithMissing.Add(child.gameObject);
                    objectPaths[child.gameObject] = prefabPath;
                }
                missingCount += childCount;
            }
        }
    }

    void ScanPrefabChildren(GameObject prefab)
    {
        foreach (Transform child in prefab.transform)
        {
            int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject);
            if (count > 0)
            {
                objectsWithMissing.Add(child.gameObject);
                missingCount += count;
            }
            ScanPrefabChildren(child.gameObject);
        }
    }

    void RemoveAllMissingScripts()
    {
        if (EditorUtility.DisplayDialog("Xác nhận", 
            $"Bạn có chắc muốn xóa {missingCount} missing script(s)?", 
            "Xóa", "Hủy"))
        {
            int removedCount = 0;
            HashSet<string> processedPrefabs = new HashSet<string>();

            foreach (GameObject go in objectsWithMissing)
            {
                if (go != null)
                {
                    // Check if this is a prefab asset
                    if (objectPaths.ContainsKey(go))
                    {
                        string prefabPath = objectPaths[go];
                        
                        // Only process each prefab once
                        if (!processedPrefabs.Contains(prefabPath))
                        {
                            processedPrefabs.Add(prefabPath);
                            GameObject prefabRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                            
                            if (prefabRoot != null)
                            {
                                removedCount += RemoveMissingScriptsFromPrefabAsset(prefabRoot, prefabPath);
                            }
                        }
                    }
                    else
                    {
                        removedCount += RemoveMissingScriptsFromObject(go);
                    }
                }
            }

            Debug.Log($"Đã xóa {removedCount} missing script(s)");
            objectsWithMissing.Clear();
            objectPaths.Clear();
            missingCount = 0;
            
            EditorUtility.DisplayDialog("Hoàn tất", $"Đã xóa {removedCount} missing script(s)!", "OK");
        }
    }

    int RemoveMissingScriptsFromPrefabAsset(GameObject prefabRoot, string prefabPath)
    {
        int totalRemoved = 0;

        // Remove from root
        int rootCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(prefabRoot);
        if (rootCount > 0)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefabRoot);
            totalRemoved += rootCount;
        }

        // Remove from all children
        Transform[] allChildren = prefabRoot.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.gameObject == prefabRoot) continue;

            int childCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(child.gameObject);
            if (childCount > 0)
            {
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(child.gameObject);
                totalRemoved += childCount;
            }
        }

        if (totalRemoved > 0)
        {
            EditorUtility.SetDirty(prefabRoot);
            AssetDatabase.SaveAssets();
            Debug.Log($"Removed {totalRemoved} missing script(s) from prefab: {prefabPath}");
        }

        return totalRemoved;
    }

    int RemoveMissingScriptsFromObject(GameObject go)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        
        if (count > 0)
        {
            Undo.RegisterCompleteObjectUndo(go, "Remove Missing Scripts");
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            
            // Save prefab if it's a prefab
            if (PrefabUtility.IsPartOfPrefabInstance(go))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(go);
            }
            else if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
            }
        }
        
        return count;
    }
}