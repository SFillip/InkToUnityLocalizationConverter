using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ink.Runtime;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization.Tables;

public class ConverterWindow : EditorWindow
{

    private StringTableCollection selection;
    public StringTableCollection Selection
    {
        get { return selection; }
        set
        {
            selection = value;
            Repaint();
        }
    }

    private Story selectedStory;
    private TextAsset SelectedTextAsset;

    private Vector2 scrollPos;

    public TextAsset SelectedStory
    {
        set
        {
            try
            {
                selectedStory = new Story(value.text); SelectedTextAsset = value; Repaint();
            }
            catch (Exception) { }
        }
    }

    [MenuItem("Window/ConverterWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConverterWindow));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        Selection = (StringTableCollection)EditorGUILayout.ObjectField(Selection, typeof(StringTableCollection), false);

        GUIStyle headerStyle = new GUIStyle();
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true
        , GUILayout.Width(200 + (200 * LocalizationEditorSettings.GetLocales().Count + 1))
        , GUILayout.MaxHeight(100), GUILayout.Height(100));

        GUILayout.BeginHorizontal();
        GUILayout.Label("KEY", headerStyle, GUILayout.Width(200));

        foreach (var local in LocalizationEditorSettings.GetLocales())
        {
            GUILayout.Label(local.LocaleName, headerStyle, GUILayout.Width(200));
        }

        GUILayout.EndHorizontal();

        if (Selection is not null)
        {

            foreach (var v in Selection.GetRowEnumerator())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(v.KeyEntry.Key);

                foreach (var entry in v.TableEntries)
                {
                    GUILayout.Label(entry.LocalizedValue);
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
        GUILayout.Space(50);

        SelectedStory = (TextAsset)EditorGUILayout.ObjectField(SelectedTextAsset, typeof(TextAsset), false);
        if (selectedStory is not null && Selection is not null)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.HelpBox($"Current index of the tag that schould be converted to the key is is: {InkConverterSettings.GetOrCreateInkConverterSettings().StandartKeyTagIndex}"
                , MessageType.Info);

            if (GUILayout.Button("Open Settings"))
                SettingsService.OpenProjectSettings("Project/InkConverterSettingsProvider");

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Convert"))
            {
                ConverterConfirmationDialog confirmationDialog = (ConverterConfirmationDialog)ScriptableObject.CreateInstance("ConverterConfirmationDialog");
                confirmationDialog.Init(selectedStory, selection);
                confirmationDialog.ShowWindow();
            }
        }
    }
}