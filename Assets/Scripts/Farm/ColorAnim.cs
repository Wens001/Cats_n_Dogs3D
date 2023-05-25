using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorAnim : MonoBehaviour
{
    public Image img;
    public Color[] colors;
    private int curId;
    private bool isChange;
    public float animTime=1;
    public Ease ease;
    public bool canAnim = true;
    // Start is called before the first frame update
    void Start()
    {
        if (colors == null || colors.Length < 1) {
            colors = new Color[1];
            colors[0] = img.color;
        }
        curId = 0;
        isChange = false;
        ChangeColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canAnim)
            return;
        if (isChange) {
            ChangeColor();
        }
    }

    void ChangeColor() {
        img.DOKill();
        isChange = false;
        Color target = colors[curId];
        img.DOColor(target, animTime).SetEase(ease).onComplete = () => {
            isChange = true;
            curId = curId + 1 < colors.Length ? ++curId : 0;
        };
    }
}
