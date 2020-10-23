using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WaterWaves : MonoBehaviour
{

    public float perlinScale;
    public float waveSpeed;
    public float waveHeight;
    public float offset;
    public static WaterWaves instance;


    private void Awake()
    {
        instance = GameObject.FindGameObjectWithTag("WaterTile").GetComponent<WaterWaves>();
    }

    void Update()
    {
        CalcNoise();
    }

    void CalcNoise()
    {
        MeshFilter mF = GetComponent<MeshFilter>();
        //MeshCollider mC = GetComponent<MeshCollider>();

        //mC.sharedMesh = mF.mesh;

        Vector3[] vertices = mF.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            var xz = transform.TransformPoint(vertices[i]);
            float pX = (xz.x * perlinScale) + (Time.time * waveSpeed) + offset;
            float pZ = (xz.z * perlinScale) + (Time.time * waveSpeed) + offset;
            vertices[i].y = instance.GetWaveHeight(pX, pZ);
            //transform.TransformPoint(vertices[i]);                        
        }

        mF.mesh.vertices = vertices;
        mF.mesh.RecalculateNormals();
        mF.mesh.RecalculateBounds();
    }

    public float GetWaveHeight(float _pX, float _pZ)
    {
        return (Mathf.PerlinNoise(_pX, _pZ) * waveHeight);
    }
}