using UnityEngine;

using Mapbox.Map;

public class SlippyTile : ScriptableObject
{
    private GameObject obj;
    private CanonicalTileId id;
    private float edge = 1;

    public void SetTileId(CanonicalTileId id)
    {
        this.id = id;

        obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
        obj.hideFlags = HideFlags.DontSave;
        obj.name = id.ToString();

        if (Application.isPlaying)
        {
            obj.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void SetEdge(float edge)
    {
        this.edge = edge;
        obj.transform.localScale = new Vector3(edge, edge, 1);
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

    public void Render(RasterTile tile)
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var raster = new Texture2D(256, 256);
        raster.LoadImage(tile.Data);

        var rend = obj.GetComponent<MeshRenderer>();

        rend.enabled = true;
        rend.material.shader = Shader.Find("Unlit/Texture");
        rend.material.mainTexture = raster;
        rend.material.mainTexture.wrapMode = TextureWrapMode.Clamp;
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
}
