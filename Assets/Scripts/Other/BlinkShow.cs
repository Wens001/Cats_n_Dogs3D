using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BlinkShow : MonoBehaviour
{
    public int ChangeChannel = 1;

    private List<Material> materials = new List<Material>();
    private Color color;

    private void Awake()
    {
        var renderers = transform.GetComponentsInChildren<Renderer>();

        foreach (var render in renderers)
        {
            var mat = render.materials[ChangeChannel];
            if (mat)
            {
                materials.Add(mat);
            }
        }
        if (materials.Count > 0)
        {
            color = materials[0].color;
        }

        StartObjBlink();
    }

    private void OnEnable()
    {
        Messenger.AddListener(StringMgr.GameWin, StopBlink);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(StringMgr.GameWin, StopBlink);
    }


    private void StartObjBlink()
    {
        float num = 0.8f;
        DOTween.To(() => num, x => num = x, 0f, 1.2f)
            .OnUpdate(()=> {
                foreach (var mat in materials)
                {
                    color.a = num;
                    mat.color = color;
                }
            })
            .SetLoops(-1, LoopType.Yoyo);
    }



    private void StopBlink()
    {
        DOTween.KillAll();

    }


}
