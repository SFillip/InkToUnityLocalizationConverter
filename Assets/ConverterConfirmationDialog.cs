using System.Collections.Generic;
using UnityEditor;
using Ink.Runtime;
using UnityEngine.Localization.Tables;
using UnityEditor.Localization;
using UnityEngine;
using System.Linq;

public class ConverterConfirmationDialog : EditorWindow
{
    private Story selectedStory;
    private StringTableCollection selection;

    private Dictionary<string, string[]> toConvert;

    private Vector2 scrollPos;
    
    public void Init(Story selectedStory, StringTableCollection selection)
    {
        this.selectedStory = selectedStory;
        this.selection = selection;
    }

    public void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConverterConfirmationDialog));
    }

    void OnGUI()
    {
        DisplayTableHeader();
        DisplayTableBody();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Confirm"))
            FinishConvertion();

        if (GUILayout.Button("Cancel"))
            Close();
        EditorGUILayout.EndHorizontal();
    }

    void DisplayTableHeader()
    {
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
    }

    void DisplayTableBody()
    {
        toConvert = Convert();

        GUILayout.BeginVertical();
        for (int i = 0; i < toConvert.Keys.Count; i++)
        {
            GUILayout.BeginHorizontal();
            var entry = toConvert.ElementAt(i);

            GUILayout.Label(entry.Key);

            for (int j = 0; j < LocalizationEditorSettings.GetLocales().Count; j++)
            {
                try
                {
                    GUILayout.Label(entry.Value[j],GUILayout.Width(200));
                }
                catch
                {
                    GUILayout.Label("empty", GUILayout.Width(200));
                }
            }

            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    Dictionary<string, string[]> Convert()
    {
        Dictionary<string, string[]> toReturn = new Dictionary<string, string[]>();


        while (selectedStory.canContinue)
        {
            string line = selectedStory.Continue();
            List<string> tags = selectedStory.currentTags;

            string key = tags[InkConverterSettings.GetOrCreateInkConverterSettings().StandartKeyTagIndex];

            string[] values ={
                line
            };
            toReturn.Add(key, values);
        }

        selectedStory.ResetState();
        return toReturn;
    }

    void FinishConvertion()
    {

        for (int i = 0; i < toConvert.Keys.Count; i++)
        {
            var entry = toConvert.ElementAt(i);

            bool keyFound = false;
            foreach (var v in selection.GetRowEnumerator())
            {
                if (v.KeyEntry.Key == entry.Key)
                    keyFound = true;

                if (!keyFound)
                {
                        foreach(var t in selection.Tables)
                        {
                            ((StringTable)t.asset).AddEntry(entry.Key, entry.Value[0]);
                        }
                }
            }
        }
        Close();
    }
}
