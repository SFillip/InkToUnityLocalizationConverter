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

    public TextAsset SelectedStory
    {
        set { selectedStory = new Story(value.text); SelectedTextAsset = value; Repaint(); }
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

        if (Selection is not null)
        {
            foreach (var v in Selection.GetRowEnumerator())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(v.KeyEntry.Key);

                foreach (var entry in v.TableEntries)
                {
                    GUILayout.Space(5);
                    GUILayout.Label(entry.LocalizedValue);
                }
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndVertical();
        GUILayout.Space(50);

        SelectedStory = (TextAsset)EditorGUILayout.ObjectField(SelectedTextAsset, typeof(TextAsset), false);
        if (selectedStory is not null)
        {
            if (GUILayout.Button("Convert"))
            {
                while (selectedStory.canContinue)
                {
                    string line = selectedStory.Continue();
                    List<string> tags = selectedStory.currentTags;

                    bool keyFound = false;
                    foreach (var v in Selection.GetRowEnumerator())
                    {
                        if (v.KeyEntry.Key == tags[InkConverterSettings.GetOrCreateInkConverterSettings().StandartKeyTagIndex])
                            keyFound = true;

                        if (!keyFound)
                        {
                            foreach (var t in selection.Tables)
                            {
                                ((StringTable)t.asset).AddEntry(tags[InkConverterSettings.GetOrCreateInkConverterSettings().StandartKeyTagIndex], line);
                            }
                        }
                    }


                }
            }
        }
    }
}
