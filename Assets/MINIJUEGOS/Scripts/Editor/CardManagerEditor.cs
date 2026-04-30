using System.Collections;
using System.Collections.Generic;   
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(CardManager))]

public class CardManagerEditor : Editor
{
    SerializedObject manager;
    SerializedProperty _pairAmount;
    SerializedProperty _width;
    SerializedProperty _height;
    SerializedProperty _spriteList;

    int spriteAmount;
    float w, h;

    void OnEnable()
    {
        manager = new SerializedObject(target); 
        _pairAmount = manager.FindProperty("pairAmount");
        _width = manager.FindProperty("width");
        _height = manager.FindProperty("height");
        _spriteList = manager.FindProperty("spriteList");
        spriteAmount = _spriteList.arraySize;
    }


    public override void OnInspectorGUI()
    {
        manager.Update();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(_pairAmount);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(_width);
        EditorGUILayout.PropertyField(_height);

        float tmp = _width.intValue * (float)_height.intValue / 2;
        _pairAmount.intValue = (int)System.Math.Ceiling(tmp);

        if (_pairAmount.intValue > spriteAmount)
        {
            EditorGUILayout.HelpBox("Demaciados pares para la cantidad de sprites", MessageType.Error);
        }

        if(_width.intValue < 0)
        {
            _width.intValue = 0;
        }
        if (_height.intValue < 0)
        {
            _height.intValue = 0;
        }

        EditorGUILayout.EndVertical();
        manager.ApplyModifiedProperties();
        DrawDefaultInspector();
    }
}
