using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 存/取
/// 2. 预制
/// </summary>
public class PoolMgr : MonoBehaviour
{
    private static PoolMgr instance;

    public static PoolMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject();
                instance = gameObject.AddComponent<PoolMgr>();
                DontDestroyOnLoad(gameObject);
                gameObject.name = "PoolMgr";
            }

            return instance;
        }
    }


    private Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();


    private string prefabName;
    GameObject result = null;
    public GameObject GetObject(GameObject prefab)
    {
        prefabName = prefab.name;

        if (pools.ContainsKey(prefabName))
        {
            if (pools[prefabName].Count > 0)
            {
                result = pools[prefabName][0];
                pools[prefabName].RemoveAt(0);

                if (result)
                {
                    result.SetActive(true);
                    return result;
                }
            }
        }

        //未在池中
        result = Instantiate(prefab);
        result.name = prefab.name;

        return result;
    }



    private string objName;
    /// <summary>
    /// 放回时会自动隐藏，无需额外隐藏
    /// </summary>
    /// <param name="obj"></param>
    public void BackObject(GameObject obj)
    {
        objName = obj.name;

        if (!pools.ContainsKey(objName))
        {
            pools.Add(objName, new List<GameObject>());
        }

        if (!pools[objName].Contains(obj))
        {
            pools[objName].Add(obj);
        }


        if (obj.activeSelf)
        {
            obj.SetActive(false);
        }

    }



    public void PrepareObjects(GameObject prefab, int num)
    {
        StartCoroutine(PrepareObjectsAsync(prefab, num));
    }

    GameObject tempObj;
    IEnumerator PrepareObjectsAsync(GameObject prefab, int num)
    {
        for (int i = 0; i < num; i++)
        {
            yield return new WaitForEndOfFrame();

            tempObj = GetObject(prefab);

            BackObject(tempObj);
        }

    }


}
