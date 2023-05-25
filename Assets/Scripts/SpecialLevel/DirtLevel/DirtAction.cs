using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtAction : MonoBehaviour
{
    [Range(.1f, 1f)]
    public float CleanSpeed = .5f;
    public bool ActiveSelf;

    private float cleanExtent = 0.1f;
    private MyTimer CleanTimer = new MyTimer(.06f);
    List<Material> materials = new List<Material>();
    Color color;

    private void Awake()
    {
        Renderer[] meshRenderers = transform.GetComponentsInChildren<Renderer>();

        materials.Clear();
        foreach (var meshRender in meshRenderers)
        {
            materials.Add(meshRender.material);

        }

        color = materials[0].color;
    }


    private void Update()
    {
        CleanTimer.OnUpdate(Time.deltaTime);

    }



    private void OnParticleCollision(GameObject other)
    {
        if (CleanTimer.IsFinish)
        {
            CleanTimer.ReStart();
            CleanDirt();
        }
    }

    private void CleanDirt()
    {
        if (color.a > 0.30f)
        {
            color.a -= cleanExtent * CleanSpeed;
            //material.color = color;
            foreach (var material in materials)
            {
                material.color = color;
            }
        }
        else
        {
            transform.GetComponent<Collider>().enabled = false;

            Messenger.Broadcast(StringMgr.GetWinCondition);
            Messenger.Broadcast(StringMgr.DirtClean);

            if (ActiveSelf)
            {
                color.a = 0;
                foreach (var material in materials)
                {
                    material.color = color;
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }




}
