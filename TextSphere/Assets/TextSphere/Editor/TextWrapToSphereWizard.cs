using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

public class TextWrapToSphereWizard : ScriptableWizard
{
    public TMP_Text text;
    [Min(0.1f)]
    public float sphereSize = 10f;

    [MenuItem("GameObject/Create Sphere Text")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<TextWrapToSphereWizard>("Create Sphere Text", "Create");
    }

    private void OnWizardCreate()
    {
        if (text == null)
            return;

        var textInfo = text.textInfo;
        var charInfo = textInfo.characterInfo;
        var wordInfo = textInfo.wordInfo;

        var bounds = text.textBounds;
        Vector2 range = bounds.size;
        range.x = Mathf.Max(range.x, range.y);
        range.y = Mathf.Max(range.x, range.y);

        var root = new GameObject("WordSphere");

        for (int wordIndex = 0; wordIndex < wordInfo.Length; wordIndex++)
        {
            if (wordInfo[wordIndex].characterCount <= 0)
                continue;

            List<Vector3> wordVertices = new List<Vector3>(textInfo.meshInfo[0].vertexCount);
            List<Vector3> wordNormals = new List<Vector3>(textInfo.meshInfo[0].vertexCount);
            List<Vector4> wordTangents = new List<Vector4>(textInfo.meshInfo[0].vertexCount);
            List<Vector3> wordUVs = new List<Vector3>(textInfo.meshInfo[0].vertexCount);
            List<Color32> wordColors = new List<Color32>(textInfo.meshInfo[0].vertexCount);
            List<int> wordTriangles = new List<int>(textInfo.meshInfo[0].vertexCount);

            int charOffset = wordInfo[wordIndex].firstCharacterIndex;

            for (int charIndex = 0; charIndex < wordInfo[wordIndex].characterCount; charIndex++)
            {
                // Get the index of the mesh used by this character.
                int materialIndex = charInfo[charIndex + charOffset].materialReferenceIndex;

                var vertIndex = charInfo[charIndex + charOffset].vertexIndex;
                var vertices = textInfo.meshInfo[materialIndex].vertices;
                var normals = textInfo.meshInfo[materialIndex].normals;
                var tangents = textInfo.meshInfo[materialIndex].tangents;
                var uvs0 = textInfo.meshInfo[materialIndex].uvs0;
                var colors = textInfo.meshInfo[materialIndex].colors32;

                var vert0 = vertices[vertIndex + 0];
                vert0 = PolarToCartesian(UVToPolar(vert0, bounds.min, range), sphereSize);

                var vert1 = vertices[vertIndex + 1];
                vert1 = PolarToCartesian(UVToPolar(vert1, bounds.min, range), sphereSize);

                var vert2 = vertices[vertIndex + 2];
                vert2 = PolarToCartesian(UVToPolar(vert2, bounds.min, range), sphereSize);

                var vert3 = vertices[vertIndex + 3];
                vert3 = PolarToCartesian(UVToPolar(vert3, bounds.min, range), sphereSize);

                wordVertices.Add(vert0);
                wordVertices.Add(vert1);
                wordVertices.Add(vert2);
                wordVertices.Add(vert3);

                wordNormals.Add(normals[vertIndex + 0]);
                wordNormals.Add(normals[vertIndex + 1]);
                wordNormals.Add(normals[vertIndex + 2]);
                wordNormals.Add(normals[vertIndex + 3]);

                wordTangents.Add(tangents[vertIndex + 0]);
                wordTangents.Add(tangents[vertIndex + 1]);
                wordTangents.Add(tangents[vertIndex + 2]);
                wordTangents.Add(tangents[vertIndex + 3]);

                wordUVs.Add(uvs0[vertIndex + 0]);
                wordUVs.Add(uvs0[vertIndex + 1]);
                wordUVs.Add(uvs0[vertIndex + 2]);
                wordUVs.Add(uvs0[vertIndex + 3]);

                wordColors.Add(colors[vertIndex + 0]);
                wordColors.Add(colors[vertIndex + 1]);
                wordColors.Add(colors[vertIndex + 2]);
                wordColors.Add(colors[vertIndex + 3]);

                int triIndex = charIndex * 4;

                wordTriangles.Add(triIndex + 0);
                wordTriangles.Add(triIndex + 1);
                wordTriangles.Add(triIndex + 2);

                wordTriangles.Add(triIndex + 0);
                wordTriangles.Add(triIndex + 2);
                wordTriangles.Add(triIndex + 3);

                //add backface triangles for double sided collision
                wordTriangles.Add(triIndex + 2);
                wordTriangles.Add(triIndex + 1);
                wordTriangles.Add(triIndex + 0);

                wordTriangles.Add(triIndex + 3);
                wordTriangles.Add(triIndex + 2);
                wordTriangles.Add(triIndex + 0);
            }

            var word = wordInfo[wordIndex].GetWord();
            var obj = new GameObject($"{word}", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            obj.transform.parent = root.transform;
            var meshFilter = obj.GetComponent<MeshFilter>();
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            var mesh = new Mesh();
            mesh.name = word;
            mesh.SetVertices(wordVertices);
            mesh.SetNormals(wordNormals);
            mesh.SetTangents(wordTangents);
            mesh.SetUVs(0, wordUVs);
            mesh.SetColors(wordColors);
            mesh.SetTriangles(wordTriangles, 0);

            meshFilter.sharedMesh = mesh;
            meshRenderer.sharedMaterial = Instantiate(text.materialForRendering);

            var collider = obj.GetComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
    }

    private void OnWizardUpdate()
    {
        if (text == null)
            errorString = "Please select a TextMeshPro object";
        else
            errorString = "";
    }

    public Vector2 UVToPolar(Vector2 point, Vector2 origin, Vector2 size)
    {
        Vector2 polar;

        polar.x = (((point.x - origin.x) / size.x) * 2f * Mathf.PI) * Mathf.Rad2Deg;
        polar.y = (((point.y /*- origin.y*/) / size.y) * 2f * Mathf.PI) * Mathf.Rad2Deg;

        return polar;
    }

    public Vector3 PolarToCartesian(Vector2 polar, float radius = 1f)
    {
        Vector3 origin = Vector3.forward * radius;
        var rotation = Quaternion.Euler(-polar.y, -polar.x, 0);
        var cartesian = rotation * origin;
        return cartesian;
    }
}
