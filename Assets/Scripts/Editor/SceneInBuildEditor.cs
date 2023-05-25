using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;

//对构建列表中的场景进行简单操作(主要用于，当使用ABundle构建包体时，在打包和编辑器运行的切换)
//将"Assets/Scenes"文件夹下的所有场景替换添加到构建列表中
//删除构建列表中除下标0外的所有场景。 
public class SceneInBuildEditor : Editor
{
    [MenuItem("MyTools/SceneEditor/AddScene")]
    private static void AddScenesToBuildSetting()
    {
        //string[] resFiles = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Scenes" });

        var resFiles = Directory.GetFiles("Assets/Scenes", "*.unity", SearchOption.AllDirectories);

        var NewScenes = new EditorBuildSettingsScene[resFiles.Length];

        for (int i = 0; i < resFiles.Length; i++)
        {
            //resFiles[i] = AssetDatabase.GUIDToAssetPath(resFiles[i]);

            NewScenes[i] = new EditorBuildSettingsScene(resFiles[i], true);

        }
        EditorBuildSettings.scenes = NewScenes;

        Debug.Log("成功添加场景到构建列表");
    }


    [MenuItem("MyTools/SceneEditor/DeleteScenesFor ABundle")]
    private static void DeleteScenes()
    {
        var newScenes = new EditorBuildSettingsScene[1];
        newScenes[0] = EditorBuildSettings.scenes[0];

        EditorBuildSettings.scenes = newScenes;

        Debug.Log("成功删除，除0下标外的所有场景");
    }




    [MenuItem("MyTools/SceneEditor/LogSceneNameInBuilding")]
    private static void LogAllScenesInBuildingName()
    {
        var scenes = EditorBuildSettings.scenes;
        string scenesName = "ScenesName: \n";
        foreach (var scene in scenes)
        {
            scenesName = scenesName + Path.GetFileNameWithoutExtension(scene.path) + "\n";
        }

        Debug.Log(scenesName);
    }

}
