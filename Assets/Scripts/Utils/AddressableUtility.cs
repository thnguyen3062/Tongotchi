
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.AddressableAssets;
using UnityEditor;
using UnityEngine;

public static class AddressableUtility
{
    [MenuItem("Tools/Add Scene and References to Addressables")]
    public static void AddSceneAndReferencesToAddressables()
    {
        // Select the scene to make Addressable
        string scenePath = EditorUtility.OpenFilePanel("Select Scene", "Assets", "unity");

        if (string.IsNullOrEmpty(scenePath))
        {
            Debug.LogError("No scene selected.");
            return;
        }

        // Load the scene
        string relativePath = "Assets" + scenePath.Substring(Application.dataPath.Length);
        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(relativePath);

        if (sceneAsset == null)
        {
            Debug.LogError("Could not load the scene at: " + relativePath);
            return;
        }

        // Make the scene Addressable
        AddAssetToAddressables(sceneAsset, "Scenes");

        // Load the scene contents
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(relativePath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
        var rootObjects = scene.GetRootGameObjects();

        // Set to store references that need to be added to Addressables
        HashSet<Object> referencedAssets = new HashSet<Object>();

        // Collect references from all root game objects in the scene
        foreach (var rootObject in rootObjects)
        {
            var components = rootObject.GetComponentsInChildren<Component>(true);

            foreach (var component in components)
            {
                var so = new SerializedObject(component);
                var sp = so.GetIterator();
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference && sp.objectReferenceValue != null)
                    {
                        Object referencedAsset = sp.objectReferenceValue;
                        string assetPath = AssetDatabase.GetAssetPath(referencedAsset);

                        if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets") && !(referencedAsset is MonoScript))
                        {
                            referencedAssets.Add(referencedAsset);
                        }
                    }
                }
            }
        }

        // Add all referenced assets to Addressables
        foreach (var asset in referencedAssets)
        {
            AddAssetToAddressables(asset, "Scene References");
        }

        Debug.Log("All referenced assets have been added to Addressables.");

        // Close the scene after processing
        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
    }

    private static void AddAssetToAddressables(Object asset, string groupName)
    {
        string assetPath = AssetDatabase.GetAssetPath(asset);
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (settings == null)
        {
            Debug.LogError("AddressableAssetSettings not found.");
            return;
        }

        var group = settings.FindGroup(groupName) ?? settings.CreateGroup(groupName, false, false, false, null, typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.CreateOrMoveEntry(guid, group);

        if (entry != null)
        {
            entry.address = asset.name;
            Debug.Log($"Added {asset.name} to Addressable group {groupName}");
        }
        else
        {
            Debug.LogWarning($"Failed to add {asset.name} to Addressables");
        }
    }
}
#endif