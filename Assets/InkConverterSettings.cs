using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InkConverterSettings : ScriptableObject
{
    public const string InkConverterSettingsPath="Assets/InkConverter/ConverterSettings.asset";

    public int StandartKeyTagIndex;

    public string pathToStringTables;

    internal static InkConverterSettings GetOrCreateInkConverterSettings()
    {
        var settings=AssetDatabase.LoadAssetAtPath<InkConverterSettings>(InkConverterSettingsPath);

        if(settings == null){
            settings=ScriptableObject.CreateInstance<InkConverterSettings>();

            settings.StandartKeyTagIndex=0;
            settings.pathToStringTables="Localization/StringTables";

            AssetDatabase.CreateAsset(settings,InkConverterSettingsPath);
            AssetDatabase.SaveAssets();
        }

        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateInkConverterSettings());
    }
}

static class InkConverterSettingsProvider{

    [SettingsProvider]
    public static SettingsProvider CreateInkConverterSettingsProvider(){
        var provider=new SettingsProvider(" ",SettingsScope.Project){
            label="InkConverterSettings",
            guiHandler=(searchContext)=>
            {
                var settings=InkConverterSettings.GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("StandartKeyTagIndex"), new GUIContent("StandartKeyTagIndex"));
                EditorGUILayout.PropertyField(settings.FindProperty("pathToStringTables"), new GUIContent("Path to StringTables"));
                settings.ApplyModifiedPropertiesWithoutUndo();  
            },

            keywords=new HashSet<string>(new[] {"StandartKeyTagIndex", "PathToStringTables",})
        };

        return provider;
    }
}
