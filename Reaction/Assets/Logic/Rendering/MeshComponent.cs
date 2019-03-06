using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshComponent : Reaction.Component<MeshComponent>
{
    private static readonly List<Vector3> vertices = new List<Vector3>();
    private static readonly List<int> triangles = new List<int>();

    private MeshFilter _meshFilter;

    public Mesh Mesh {
        get {
            if (_meshFilter == null) {
                _meshFilter = GetComponent<MeshFilter>();
            }
            return _meshFilter.sharedMesh;
        }
        set {
            if (_meshFilter == null)
            {
                _meshFilter = GetComponent<MeshFilter>();
            }
            _meshFilter.sharedMesh = value;
        }
    }

    private static bool RayIntersectsTriangle(Ray ray,
                                 Vector3 tri0, Vector3 tri1, Vector3 tri2,
                                 out float pickDistance, out float barycentricU, out float barycentricV) {
        pickDistance = 0;
        barycentricU = 0;
        barycentricV = 0;

        // Find vectors for two edges sharing vert0
        Vector3 edge1 = tri1 - tri0;
        Vector3 edge2 = tri2 - tri0;
        
        // Begin calculating determinant - also used to calculate barycentricU parameter
        Vector3 pvec = Vector3.Cross(ray.direction, edge2);

        // If determinant is near zero, ray lies in plane of triangle
        float det = Vector3.Dot(edge1, pvec);
        if (det < 0.0001f)
            return false;
        
        // Calculate distance from vert0 to ray origin
        Vector3 tvec = ray.origin - tri0;
        
        // Calculate barycentricU parameter and test bounds
        barycentricU = Vector3.Dot(tvec, pvec);
        if (barycentricU < 0.0f || barycentricU > det)
            return false;
        
        // Prepare to test barycentricV parameter
        Vector3 qvec = Vector3.Cross(tvec, edge1);
        
        // Calculate barycentricV parameter and test bounds
        barycentricV = Vector3.Dot(ray.direction, qvec);
        if (barycentricV < 0.0f || barycentricU + barycentricV > det)
            return false;
        
        // Calculate pickDistance, scale parameters, ray intersects triangle
        pickDistance =Vector3.Dot(edge2, qvec);
        float fInvDet = 1.0f / det;
        pickDistance *= fInvDet;
        barycentricU *= fInvDet;
        barycentricV *= fInvDet;
        
        return true;
    }

    public struct RayCastInfo {
        public float pickDistance;
        public float barycentricU;
        public float barycentricV;
        public int triangleIndex;
        public Vector3 normal;
    }

    public bool IntersectsRay(Ray ray, out RayCastInfo info)
    {
        info = new RayCastInfo();

        Mesh mesh = Mesh;
        if (mesh == null) return false;


        vertices.Clear();
        mesh.GetVertices(vertices);
        triangles.Clear();
        mesh.GetTriangles(triangles, 0);

        if (triangles.Count == 0) return false;

        float minDistance = 10000000.0f;

        bool hit = false;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (RayIntersectsTriangle(ray,
                                      vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]],
                                      out float distance, out float u, out float v))
            {

                if (distance < minDistance)
                {
                    minDistance = distance;
                    info.pickDistance = distance;
                    info.barycentricU = u;
                    info.barycentricV = v;
                    info.triangleIndex = i;
                }

                hit = true;
            }
        }

        return hit;
    }


}

