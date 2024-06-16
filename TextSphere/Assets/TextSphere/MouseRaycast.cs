using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MouseRaycast : MonoBehaviour
{
    public Color highlightColor = Color.red;
    public Color normalColor = Color.white;

    Camera cam;

    List<Material> lastHitMaterials = new List<Material>(50);

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        //reset color in last hit materials
        foreach(var mat in lastHitMaterials)
        {
            mat.SetColor("_FaceColor", normalColor);
        }
        lastHitMaterials.Clear();

        var ray = cam.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 10000f);
        if(hits != null && hits.Length > 0)      
        {
            foreach(var hit in hits)
            {
                if(hit.collider.TryGetComponent(out MeshRenderer r))
                {
                    r.sharedMaterial.SetColor("_FaceColor", highlightColor);
                    lastHitMaterials.Add(r.sharedMaterial);
                }
            }
        }
    }
}
