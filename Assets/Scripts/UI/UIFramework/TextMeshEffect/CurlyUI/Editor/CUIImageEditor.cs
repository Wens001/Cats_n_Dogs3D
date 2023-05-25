using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(CUIImage))]
public class CUIImageEditor : CUIGraphicEditor {

    //protected cornerPos

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CUIImage script = (CUIImage)this.target;

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        EditorGUI.BeginDisabledGroup(!(script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Tiled));
        Vector2 newCornerRatio = EditorGUILayout.Vector2Field("Corner Ratio", script.cornerPosRatio);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(script, "更改角比率");
            EditorUtility.SetDirty(script);
            script.cornerPosRatio = newCornerRatio;
        }

        if (GUILayout.Button("使用本机角点比率"))
        {
            Undo.RecordObject(script, "Change Corner Ratio");
            EditorUtility.SetDirty(script);
            script.cornerPosRatio = script.OriCornerPosRatio;
        }

        if (script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Filled)
        {
            EditorGUILayout.HelpBox("使用CUIImage，还可以调整填充或切片图像的角的大小。编辑器场景中的灰色球体也可以移动以更改角点的大小.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox("使用CUIImage，还可以调整填充或切片图像的角的大小。需要将图像设置为填充或切片才能使用此功能.", MessageType.Info);
        }

        EditorGUI.EndDisabledGroup();

    }

    [System.Obsolete]
    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        CUIImage script = (CUIImage)this.target;

        if (script.UIImage.type == Image.Type.Sliced || script.UIImage.type == Image.Type.Tiled) {
            Vector3 cornerPos = Vector3.zero;//

            if (script.IsCurved) {
                cornerPos = script.GetBCurveSandwichSpacePoint(script.cornerPosRatio.x, script.cornerPosRatio.y);
            }
            else
            {
                cornerPos.x = script.cornerPosRatio.x * script.RectTrans.rect.width - script.RectTrans.pivot.x * script.RectTrans.rect.width;
                cornerPos.y = script.cornerPosRatio.y * script.RectTrans.rect.height - script.RectTrans.pivot.y * script.RectTrans.rect.height;
            }

            Handles.color = Color.gray;
            EditorGUI.BeginChangeCheck();
            Vector3 newCornerPos = Handles.FreeMoveHandle(script.transform.TransformPoint(cornerPos), script.transform.rotation, HandleUtility.GetHandleSize(script.transform.TransformPoint(cornerPos)) / 7, Vector3.one, Handles.SphereCap);
            Handles.Label(newCornerPos, string.Format("角点移动"));

            newCornerPos = script.transform.InverseTransformPoint(newCornerPos);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "移动角点");
                EditorUtility.SetDirty(script);

                script.cornerPosRatio = new Vector2(newCornerPos.x, newCornerPos.y);
                script.cornerPosRatio.x = (script.cornerPosRatio.x + script.RectTrans.pivot.x * script.RectTrans.rect.width) / script.RectTrans.rect.width;
                script.cornerPosRatio.y = (script.cornerPosRatio.y + script.RectTrans.pivot.y * script.RectTrans.rect.height) / script.RectTrans.rect.height;
            }
        }     

    }

}

#endif