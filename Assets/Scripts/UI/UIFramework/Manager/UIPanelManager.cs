using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using LitJson;
public class UIPanelManager
{
    private static UIPanelManager _instance;
    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }
    public static UIPanelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UIPanelManager();
            }


            return _instance;
        }
    }

    private Dictionary<string, string> panelPathDict;
    private Dictionary<string, BasePanel> panelDict;
    private Stack<BasePanel> panelStack;

    private UIPanelManager()
    {
        ParseUIPanelTypeJson();
        ParsePlantInfoJson();
    }

    /// <summary>
    /// UI入栈
    /// </summary>
    /// <param name="panelType"></param>
    public void PushPanel(string panelType)
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }

        //停止上一个界面
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();

            if (topPanel.name.Contains(panelType))
            {
                return;
            }

            topPanel.OnPause();
        }

        BasePanel panel = GetPanel(panelType);
        if (panel)
        {
            panelStack.Push(panel);
            panel.OnEnter();
        }
       
    }

    /// <summary>
    /// UI退栈
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
        {
            panelStack = new Stack<BasePanel>();
        }
        if (panelStack.Count <= 0)
        {
            return;
        }

        //退出栈顶面板
        BasePanel topPanel = panelStack.Pop();
        topPanel.OnExit();

        //恢复上一个面板
        if (panelStack.Count > 0)
        {
            BasePanel panel = panelStack.Peek();
            panel.OnResume();
        }
    }

    public void PausePanel()
    {
        if (panelStack == null || panelStack.Count == 0)
        {
            return;
        }

        BasePanel panel = panelStack.Peek();
        panel.OnPause();
    }

    public void ResumePanel()
    {

    }



    private BasePanel GetPanel(string panelType)
    {

        if (panelDict == null)
        {
            panelDict = new Dictionary<string, BasePanel>();
        }

        BasePanel panel = panelDict.GetValue(panelType);

        //如果没有实例化面板，寻找路径进行实例化，并且存储到已经实例化好的字典面板中
        if (panel == null)
        {
            string path = panelPathDict.GetValue(panelType);

            GameObject panelGo = GameObject.Instantiate(Resources.Load<GameObject>(path), CanvasTransform, false);

            panel = panelGo.GetComponent<BasePanel>();

            if (panelDict.ContainsKey(panelType))
            {
                panelDict[panelType] = panel;
            }
            else
            {
                panelDict.Add(panelType, panel);
            }
        }
        return panel;
    }

    //解析json文件
    private void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<string, string>();
        TextAsset textUIPanelType = Resources.Load<TextAsset>("UIPanelTypeJson");
        UIPanelInfoList panelInfoList = JsonMapper.ToObject<UIPanelInfoList>(textUIPanelType.text);

        foreach (UIPanelInfo panelInfo in panelInfoList.panelInfoList)
        {
            panelPathDict.Add(panelInfo.panelType, panelInfo.path);
            //Debug.Log(panelInfo.panelType + ":" + panelInfo.path);
        }
    }

    //解析农场植物信息json
    void ParsePlantInfoJson()
    {
        TextAsset asset = Resources.Load<TextAsset>("PlantInfoJson");
        PlantInfoList panelInfoList = JsonMapper.ToObject<PlantInfoList>(asset.text);
        FarmMgr.plantInfoDict = new Dictionary<int, PlantInfo>();
        foreach (PlantInfo child in panelInfoList.plantInfoList)
        {
            FarmMgr.plantInfoDict[child.id] = child;
        }
    }
}

/*
 *      UIPanelManager panelManager = UIPanelManager.Instance;  //初始化
 *      panelManager.PushPanel(UIPanelType.MainMenu);           //入栈 (Panel继承BasePanel,Panel的Prefab位置保存在Json里)
 *      panelManager.PopPanel();                                //出栈
 * 
 */
