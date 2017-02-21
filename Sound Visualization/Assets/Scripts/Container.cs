﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public static class NormalSolver
{
    // Resources: http://schemingdeveloper.com/2014/10/17/better-method-recalculate-normals-unity/
    public static Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC, Vector3[] vertices)
    {
        Vector3 pointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        // ab x ac
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public static void RecalculateNormals(this Mesh mesh)
    {
        var triangles = mesh.GetTriangles(0);
        var vertices = mesh.vertices;
        var triNormals = new Vector3[triangles.Length / 3]; //Holds the normal of each triangle
        var normals = new Vector3[vertices.Length];

        float angle = 90f * Mathf.Deg2Rad;

        var dictionary = new Dictionary<VertexKey, VertexEntry>(vertices.Length);

        //Goes through all the triangles and gathers up data to be used later
        for (var i = 0; i < triangles.Length; i += 3)
        {
            int vertexIndexA = triangles[i];
            int vertexIndexB = triangles[i + 1];
            int vertexIndexC = triangles[i + 2];

            Vector3 p1 = vertices[vertexIndexB] - vertices[vertexIndexA];
            Vector3 p2 = vertices[vertexIndexC] - vertices[vertexIndexA];

            //Calculate the normal of the triangle
            Vector3 normal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC, vertices);
            int triIndex = i / 3;
            triNormals[triIndex] = normal;

            VertexEntry entry;
            VertexKey key;

            // For each of the three points of the triangle
            //  > Add this triangle as part of the triangles they're connected to.

            if (!dictionary.TryGetValue(key = new VertexKey(vertices[vertexIndexA]), out entry))
            {
                entry = new VertexEntry();
                dictionary.Add(key, entry);
            }
            entry.Add(vertexIndexA, triIndex);

            if (!dictionary.TryGetValue(key = new VertexKey(vertices[vertexIndexB]), out entry))
            {
                entry = new VertexEntry();
                dictionary.Add(key, entry);
            }
            entry.Add(vertexIndexB, triIndex);

            if (!dictionary.TryGetValue(key = new VertexKey(vertices[vertexIndexC]), out entry))
            {
                entry = new VertexEntry();
                dictionary.Add(key, entry);
            }
            entry.Add(vertexIndexC, triIndex);
        }

        // For each point in space
        foreach (var value in dictionary.Values)
        {
            // For each triangle T1 that point belongs to
            for (var i = 0; i < value.Count; ++i)
            {
                var sum = new Vector3();
                // For each other triangle T2 (including self) that point belongs to
                for (var j = 0; j < value.Count; ++j)
                {
                    // The corresponding vertex is actually the same vertex
                    if (value.VertexIndex[i] == value.VertexIndex[j])
                    {
                        // Add to temporary Vector3
                        sum += triNormals[value.TriangleIndex[j]];
                    }
                    else
                    {
                        float dot = Vector3.Dot(
                            triNormals[value.TriangleIndex[i]],
                            triNormals[value.TriangleIndex[j]]);
                        dot = Mathf.Clamp(dot, -0.99999f, 0.99999f);
                        float acos = Mathf.Acos(dot);
                        // The angle between the two triangles is less than the smoothing angle
                        if (acos <= angle)
                        {
                            // Add to temporary Vector3
                            sum += triNormals[value.TriangleIndex[j]];
                        }
                    }
                }
                // Normalize temporary Vector3 to find the average
                normals[value.VertexIndex[i]] = sum.normalized;
            }
        }

        mesh.normals = normals;
    }

    private struct VertexKey
    {
        private readonly long _x;
        private readonly long _y;
        private readonly long _z;

        //Change this if you require a different precision.
        private const int Tolerance = 100000;

        public VertexKey(Vector3 position)
        {
            _x = (long)(Mathf.Round(position.x * Tolerance));
            _y = (long)(Mathf.Round(position.y * Tolerance));
            _z = (long)(Mathf.Round(position.z * Tolerance));
        }
    }

    private sealed class VertexEntry
    {
        public int[] TriangleIndex = new int[4];
        public int[] VertexIndex = new int[4];

        private int _reserved = 4;
        private int _count;

        public int Count { get { return _count; } }

        public void Add(int vertIndex, int triIndex)
        {
            //Auto-resize the arrays when needed
            if (_reserved == _count)
            {
                _reserved *= 2;
                Array.Resize(ref TriangleIndex, _reserved);
                Array.Resize(ref VertexIndex, _reserved);
            }
            TriangleIndex[_count] = triIndex;
            VertexIndex[_count] = vertIndex;
            ++_count;
        }
    }
}

public class Container : MonoBehaviour
{
    public float safeZone;
    public float resolution;
    public float threshold;
    public ComputeShader computeShader;
    public bool calculateNormalsWithUnityBuiltIn;

    private CubeGrid grid;

    private Mesh mesh;

    public void Start()
    {
        this.grid = new CubeGrid(this, this.computeShader);
    }

    public void Update()
    {
        this.grid.evaluateAll(this.GetComponentsInChildren<MetaBall>());

        mesh = this.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = this.grid.vertices.ToArray();
        mesh.triangles = this.grid.getTriangles();

        if (this.calculateNormalsWithUnityBuiltIn)
        {
            mesh.RecalculateNormals();
        }
        else
        {
            NormalSolver.RecalculateNormals(mesh);
        }
    }


}