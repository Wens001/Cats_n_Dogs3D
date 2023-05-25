using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(CUIGraphic), true)]
public class CUIGraphicEditor : Editor {

    protected static bool isCurveGpFold = false;

    protected Vector3[] reuse_Vector3s = new Vector3[4];

    public override void OnInspectorGUI()
    {
        CUIGraphic script = (CUIGraphic)this.target;

        EditorGUILayout.HelpBox("CurlyUI应该与大多数Unity UI一起工作。对于图像，使用CUIImage；对于文本，使用CUIText；对于其他（例如RawImage），使用CUIGraphic", MessageType.Info);
                      
        if(script.UIGraphic == null)
        {
            EditorGUILayout.HelpBox("CUI是Unity用户界面的扩展。必须使用统一图形组件（例如图像、文本、RawImage）设置Ui图形", MessageType.Error);
        }
        else
        {
            if(script.UIGraphic is Image && script.GetType() != typeof(CUIImage))
            {
                EditorGUILayout.HelpBox("尽管CUI组件是通用的。建议对图像使用CUIImage", MessageType.Warning);
            }
            else if (script.UIGraphic is Text && script.GetType() != typeof(CUIText))
            {
                EditorGUILayout.HelpBox("尽管CUI组件是通用的。建议对文本使用CUIText", MessageType.Warning);
            }

            EditorGUILayout.HelpBox("现在CUI已经准备好了，将顶部和底部bezier曲线的控制点更改为对UI进行曲线/变形。当用户界面在弯曲/变形时看起来很糟糕时，提高分辨率应该会有帮助.", MessageType.Info);

        }
        
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        
        // draw the editor that shows the position ratio of all control points from the two bezier curves
        isCurveGpFold = EditorGUILayout.Foldout(isCurveGpFold, "曲线位置比率");
        if (isCurveGpFold)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("顶部曲线");
            EditorGUI.indentLevel++;
            Vector3[] controlPoints = script.RefCurvesControlRatioPoints[1].array;

            EditorGUI.BeginChangeCheck();
            for (int p = 0; p < controlPoints.Length; p++)
            {
                reuse_Vector3s[p] = EditorGUILayout.Vector3Field(string.Format("P {0}", p+1), controlPoints[p]);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "更改比率点");
                EditorUtility.SetDirty(script);

                System.Array.Copy(reuse_Vector3s, script.RefCurvesControlRatioPoints[1].array, controlPoints.Length);
                script.UpdateCurveControlPointPositions();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("底部曲线");
            EditorGUI.indentLevel++;
            controlPoints = script.RefCurvesControlRatioPoints[0].array;

            EditorGUI.BeginChangeCheck();
            for (int p = 0; p < controlPoints.Length; p++)
            {
                reuse_Vector3s[p] = EditorGUILayout.Vector3Field(string.Format("P {0}", p+1), controlPoints[p]);
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(script, "更改比率点");
                EditorUtility.SetDirty(script);

                System.Array.Copy(reuse_Vector3s, controlPoints, controlPoints.Length);
                script.UpdateCurveControlPointPositions();
            }
            EditorGUI.indentLevel--;
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("将Bezier曲线拟合到rect变换"))
        {
            Undo.RecordObject(script, "适应矩形变换");
            Undo.RecordObject(script.RefCurves[0], "适应矩形变换");
            Undo.RecordObject(script.RefCurves[1], "适应矩形变换");
            EditorUtility.SetDirty(script);

            script.FixTextToRectTrans();

            script.Refresh();
        }

        EditorGUILayout.Space();

        // disable group to prevent allowing the reference be used when there is no reference CUI
        EditorGUI.BeginDisabledGroup(script.RefCUIGraphic == null);

        if (GUILayout.Button("曲线的参考CUI组件"))
        {
            Undo.RecordObject(script, "参考 CUI");
            Undo.RecordObject(script.RefCurves[0], "参考 CUI");
            Undo.RecordObject(script.RefCurves[1], "参考 CUI");
            EditorUtility.SetDirty(script);

            script.ReferenceCUIForBCurves();

            script.Refresh();
        }

        EditorGUILayout.HelpBox("通过参照另一个CUI自动设置曲线的控制点。您需要先设置Ref CUI图形（例如cuimage）.", MessageType.Info);

        EditorGUI.EndDisabledGroup();
        


    }

    protected virtual void OnSceneGUI()
    {
        
        // for CUITextEditor, allow using scene UI to change the control points of the bezier curves

        CUIGraphic script = (CUIGraphic)this.target;

        script.ReportSet();

        for (int c = 0; c < script.RefCurves.Length; c++)
        {

            CUIBezierCurve curve = script.RefCurves[c];

            if (curve.ControlPoints != null)
            {

                Vector3[] controlPoints = curve.ControlPoints;

                Transform handleTransform = curve.transform;
                Quaternion handleRotation = curve.transform.rotation;

                for (int p = 0; p < CUIBezierCurve.CubicBezierCurvePtNum; p++)
                {
                    EditorGUI.BeginChangeCheck();
                    Handles.Label(handleTransform.TransformPoint(controlPoints[p]) , string.Format("P {0}", p+1));
                    Vector3 newPt = Handles.DoPositionHandle(handleTransform.TransformPoint(controlPoints[p]), handleRotation);
                    if (EditorGUI.EndChangeCheck())
                    {
                        
                        Undo.RecordObject(curve, "移动点");
                        Undo.RecordObject(script, "移动点");
                        EditorUtility.SetDirty(curve);
                        controlPoints[p] = handleTransform.InverseTransformPoint(newPt);

                    }
                }

                Handles.color = Color.gray;
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[0]), handleTransform.TransformPoint(controlPoints[1]));
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[1]), handleTransform.TransformPoint(controlPoints[2]));
                Handles.DrawLine(handleTransform.TransformPoint(controlPoints[2]), handleTransform.TransformPoint(controlPoints[3]));

                int sampleSize = 10;

                Handles.color = Color.white;
                for (int s = 0; s < sampleSize; s++)
                {
                    Handles.DrawLine(handleTransform.TransformPoint(curve.GetPoint((float)s / sampleSize)), handleTransform.TransformPoint(curve.GetPoint((float)(s + 1) / sampleSize)));
                }

                curve.EDITOR_ControlPoints = controlPoints;

            }
        }


        if (script.RefCurves != null)
        {
            Handles.DrawLine(script.RefCurves[0].transform.TransformPoint(script.RefCurves[0].ControlPoints[0]), script.RefCurves[1].transform.TransformPoint(script.RefCurves[1].ControlPoints[0]));
            Handles.DrawLine(script.RefCurves[0].transform.TransformPoint(script.RefCurves[0].ControlPoints[3]), script.RefCurves[1].transform.TransformPoint(script.RefCurves[1].ControlPoints[3]));
        }
        

        script.Refresh();
    }

}

#endif