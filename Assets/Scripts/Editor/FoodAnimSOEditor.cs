using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoodAnimSO))]
public class FoodAnimSOEditor : Editor
{
    SerializedProperty foodAnimProperty;

    private void OnEnable()
    {
        foodAnimProperty = serializedObject.FindProperty("m_FoodAnim");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Use the default property field to draw the array with default controls
        EditorGUILayout.PropertyField(foodAnimProperty, true);

        // Custom display of elements with images
        for (int i = 0; i < foodAnimProperty.arraySize; i++)
        {
            SerializedProperty elementProperty = foodAnimProperty.GetArrayElementAtIndex(i);
            SerializedProperty idProperty = elementProperty.FindPropertyRelative("id");
            SerializedProperty spritesProperty = elementProperty.FindPropertyRelative("sprites");

            EditorGUILayout.PropertyField(idProperty);

            // Only show the first sprite for preview
            if (spritesProperty.arraySize > 0)
            {
                SerializedProperty firstSpriteProperty = spritesProperty.GetArrayElementAtIndex(0);
                Sprite firstSprite = firstSpriteProperty.objectReferenceValue as Sprite;

                if (firstSprite != null)
                {
                    Rect rect = GUILayoutUtility.GetRect(64, 64, GUILayout.ExpandWidth(false));
                    DrawSprite(rect, firstSprite);
                }
                else
                {
                    GUILayout.Label("No Sprite Assigned", GUILayout.Height(64), GUILayout.Width(64));
                }
            }
            else
            {
                GUILayout.Label("No Sprites in Array", GUILayout.Height(64), GUILayout.Width(64));
            }

            // Draw the rest of the elements in the sprites array
            EditorGUILayout.PropertyField(spritesProperty, new GUIContent("Sprites"), true);

            EditorGUILayout.Space();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawSprite(Rect rect, Sprite sprite)
    {
        if (sprite == null) return;

        Texture2D texture = sprite.texture;
        Rect spriteRect = sprite.rect;
        Rect textureCoords = new Rect(
            spriteRect.x / texture.width,
            spriteRect.y / texture.height,
            spriteRect.width / texture.width,
            spriteRect.height / texture.height
        );

        // Create a new rect that maintains the aspect ratio
        float aspectRatio = spriteRect.width / spriteRect.height;
        if (rect.width / rect.height > aspectRatio)
        {
            // The rect is wider than the sprite aspect ratio
            float width = rect.height * aspectRatio;
            rect = new Rect(rect.x + (rect.width - width) / 2, rect.y, width, rect.height);
        }
        else
        {
            // The rect is taller than the sprite aspect ratio
            float height = rect.width / aspectRatio;
            rect = new Rect(rect.x, rect.y + (rect.height - height) / 2, rect.width, height);
        }

        GUI.DrawTextureWithTexCoords(rect, texture, textureCoords);
    }
}
