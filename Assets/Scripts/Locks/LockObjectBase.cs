using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockObjectBase : MonoBehaviour
{
    public CatOrDog suitHead;

    [HideInInspector] public TouchMove curHead;
    /// <summary>
    /// 记录咬住头的另一个头
    /// </summary>
    [HideInInspector] public TouchMove otherHead;



}
