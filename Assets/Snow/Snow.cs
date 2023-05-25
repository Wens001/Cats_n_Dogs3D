using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snow : MonoBehaviour
{

    private RenderTexture rt;
    private RenderTexture rtcopy;
    public Texture drawing;
    public Vector2 drawSize = new Vector2(50,50);
    public Material stamp_mat;
    public Texture defaultTexture;
    private void Awake()
    {
        rt = GetComponent<MeshRenderer>().sharedMaterial.GetTexture("MaskTex") as RenderTexture;
        rtcopy = new RenderTexture(rt.width ,rt.height,32,rt.graphicsFormat);
        rtcopy.Create();
    }


    public void DrawDefault()
    {
        if (defaultTexture == null)
            return;
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
        Rect rect = new Rect(0, 0, rt.width, rt.height);
        Graphics.DrawTexture(rect, defaultTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }

    

    public void Draw(float x,float y ,Texture drawTex,Vector2 drawSize)
    {
        x = x * rt.width - drawSize.x * .5f;
        y = (1 - y) * rt.height - drawSize.y * .5f;

        Graphics.Blit(rt,rtcopy);
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);

        Rect rect = new Rect(x,y, drawSize.x, drawSize.y);

            Graphics.DrawTexture(rect, drawTex);

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void Start()
    {
        DrawDefault();
        SnowBrushesInit();
    }

    private void OnDestroy()
    {
        if (rtcopy)
            rtcopy.Release();
    }

    #region Raycast Test

    private Camera m_mainCamera;
    public Camera mainCamera
    {
        get
        {
            if (m_mainCamera == null)
                m_mainCamera = Camera.main;
            return m_mainCamera;
        }
    }
    void Update()
    {
        if (snowBrushes.Length == 0)
        {
            if (Input.GetMouseButton(0) && mainCamera)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                    Draw(hit.textureCoord.x, hit.textureCoord.y, drawing, drawSize);
            }
        }
        else
        {
            BrushingWithBrush();
        }
    }

    #endregion



    #region 查询所有可与雪互动的物体

    private SnowBrush[] snowBrushes;
    Ray ray = new Ray();

    private  void SnowBrushesInit()
    {
        snowBrushes = FindObjectsOfType<SnowBrush>();
    }


    private void BrushingWithBrush()
    {
        foreach (var snowBrush in snowBrushes)
        {
            ray.origin = Camera.main.transform.position;
            ray.direction = snowBrush.transform.position - Camera.main.transform.position;

            var hits = Physics.RaycastAll(ray, 100, LayerMask.GetMask("Ground"));
            foreach (var hit in hits)
            {
                if (hit.transform == transform)
                {
                    Draw(hit.textureCoord.x, hit.textureCoord.y, drawing, snowBrush.BrushSize);
                    break;
                }
            }

        }
        

    }







    #endregion



}
