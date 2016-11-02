using System.Collections.Generic;

using UnityEngine;

using Mapbox.Map;

public class SlippyTile : ScriptableObject
{
    private GameObject obj;
    private CanonicalTileId id;
    private float edge = 1;
    private int pendingTasks = 2;

    // Hardcoded from Unity Plane primitive.
    private const float planeWidth = 10;

    // Make this configurable, lower the less.
    private const float extrusionExaggeration = 250;

    public void SetTileId(CanonicalTileId id)
    {
        this.id = id;

        obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
        obj.hideFlags = HideFlags.DontSave;
        obj.name = id.ToString();
        obj.transform.localRotation = Quaternion.Euler(90, 0, 180);

        if (Application.isPlaying)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void SetEdge(float edge)
    {
        this.edge = edge;

        // Correct the Plane size by dividing by 10.
        obj.transform.localScale = new Vector3(edge, 1, edge) / 10;
    }

    public void SetAnchor(CanonicalTileId anchor, Vector2 offset)
    {
        obj.transform.localPosition = new Vector3(
            (id.X - anchor.X) * edge - offset.x,
            (id.Y - anchor.Y) * -edge + offset.y,
            0
        );
    }

    public void SetParent(Transform parent)
    {
        obj.transform.SetParent(parent, false);
    }

    public void SetRaster(byte[] data)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var raster = new Texture2D(256, 256);
        raster.LoadImage(data);

        var rend = obj.GetComponent<MeshRenderer>();
        rend.material.shader = Shader.Find("Unlit/Texture");
        rend.material.mainTexture = raster;
        rend.material.mainTexture.wrapMode = TextureWrapMode.Clamp;

        EnableIfComplete();
    }

    public void SetElevation(byte[] data)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var terrain = new Texture2D(256, 256);
        terrain.LoadImage(data);

        var mesh = obj.GetComponent<MeshFilter>().mesh;

        Vector3[] oldVertices = mesh.vertices;
        Vector3[] newVertices = new Vector3[oldVertices.Length];

        for (int i = 0; i < oldVertices.Length; ++i)
        {
            var ver = oldVertices[i];

            var x = Mathf.Lerp(255, 0, (ver.x + planeWidth / 2) / planeWidth);
            var y = Mathf.Lerp(255, 0, (ver.z + planeWidth / 2) / planeWidth);

            newVertices[i] = new Vector3(
                ver.x,
                GetHeightFromColor(terrain.GetPixel((int)x, (int)y)) / extrusionExaggeration,
                ver.z);
            }

        mesh.vertices = newVertices;
        mesh.RecalculateNormals();

        Destroy(terrain);

        EnableIfComplete();
    }

    // Needed?
    public void Clear()
    {
        var rend = obj.GetComponent<MeshRenderer>();

        if (Application.isEditor)
        {
            DestroyImmediate(rend.material.mainTexture);
            DestroyImmediate(rend);
            DestroyImmediate(obj);
        }
        else
        {
            Destroy(rend.material.mainTexture);
            Destroy(rend);
            Destroy(obj);
        }
    }

    private float GetHeightFromColor(Color c)
    {
        // Additional *256 to switch from 0-1 to 0-256.
        return (float)(-10000 + ((c.r * 256 * 256 * 256 + c.g * 256 * 256 + c.b * 256) * 0.1));
    }

    private void EnableIfComplete()
    {
        var rend = obj.GetComponent<MeshRenderer>();
        rend.enabled = --pendingTasks == 0;
    }
}
