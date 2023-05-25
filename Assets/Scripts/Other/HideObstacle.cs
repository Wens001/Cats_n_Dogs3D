using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObstacle : MonoBehaviour
{

    private void Awake()
    {
        MeshRenderer[] meshes = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var mesh in meshes)
        {
            mesh.enabled = false;
        }
    }





}
