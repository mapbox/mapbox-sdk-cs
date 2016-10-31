using System.Collections.Generic;

using UnityEngine;

using Mapbox;
using Mapbox.Map;
using Mapbox.Unity;

[ExecuteInEditMode]
public class Slippy : MonoBehaviour, IObserver<RasterTile>
{
    // Token created only for these examples, will get rotated. Do not use in production.
    public const string DebugToken = "pk.eyJ1IjoidG1wc2FudG9zIiwiYSI6ImNpdW56YmxrYTAwMTUydGw4bjdvbDB0djkifQ.sSt9IrAODfFnkzMsPHRU1A";

    public string Token = DebugToken;

    public double South = 0;
    public double West = 0;
    public double North = 0;
    public double East = 0;

    public int Zoom = 0;

    public float Edge = 1;

    private FileSource fs;
    private Map<RasterTile> raster;
    private Dictionary<string, SlippyTile> tiles = new Dictionary<string, SlippyTile>();

    private Vector3 lastPosition;
    private Vector2 dragOffset = new Vector2(0, 0);

    private CanonicalTileId nwAnchor;
    private CanonicalTileId seAnchor;

    void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var result = new RaycastHit();
        GetComponent<BoxCollider>().Raycast(ray, out result, Mathf.Infinity);

        lastPosition = transform.InverseTransformPoint(result.point);
    }

    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        var result = new RaycastHit();
        GetComponent<BoxCollider>().Raycast(ray, out result, Mathf.Infinity);

        var offset = lastPosition - transform.InverseTransformPoint(result.point);
        lastPosition = transform.InverseTransformPoint(result.point);

        dragOffset.x = dragOffset.x + offset.x;
        dragOffset.y = dragOffset.y + offset.y;


        if (Mathf.Abs(dragOffset.x) > Edge)
        {
            var d = dragOffset.x > 0 ? 1 : -1;

            West = new CanonicalTileId(Zoom, nwAnchor.X + d, nwAnchor.Y).ToGeoCoordinate().Longitude;
            East = new CanonicalTileId(Zoom, seAnchor.X + d, seAnchor.Y).ToGeoCoordinate().Longitude;

            dragOffset.x = dragOffset.x % Edge;
        }

        if (Mathf.Abs(dragOffset.y) > Edge)
        {
            var d = dragOffset.y < 0 ? 1 : -1;

            North = new CanonicalTileId(Zoom, nwAnchor.X, nwAnchor.Y + d).ToGeoCoordinate().Latitude;
            South = new CanonicalTileId(Zoom, seAnchor.X, seAnchor.Y + d).ToGeoCoordinate().Latitude;

            dragOffset.y = dragOffset.y % Edge;
        }
    }

    void Update()
    {
        UpdateFileSource();
        UpdateRasterMap();
        UpdateTiles();
    }

    void UpdateFileSource()
    {
        fs = fs ?? new FileSource(this);
        fs.AccessToken = Token;
    }

    void UpdateRasterMap()
    {
        if (raster == null)
        {
            raster = new Map<RasterTile>(fs);
            raster.Subscribe(this);
        }

        raster.SetGeoCoordinateBoundsZoom(new GeoCoordinateBounds(
            new GeoCoordinate(South, West),
            new GeoCoordinate(North, East)),
            Zoom);

        nwAnchor = TileCover.CoordinateToTileId(
            new GeoCoordinate(raster.GeoCoordinateBounds.North, raster.GeoCoordinateBounds.West),
            Zoom).Canonical;

        seAnchor = TileCover.CoordinateToTileId(
            new GeoCoordinate(raster.GeoCoordinateBounds.South, raster.GeoCoordinateBounds.East),
            Zoom).Canonical;
    }

    void UpdateTiles()
    {
        Vector2 tileOffset = new Vector2(
            ((seAnchor.X - nwAnchor.X) * Edge) / 2 + dragOffset.x,
            ((seAnchor.Y - nwAnchor.Y) * Edge) / 2 - dragOffset.y);

        foreach (KeyValuePair<string, SlippyTile> entry in tiles)
        {
            entry.Value.SetEdge(Edge);
            entry.Value.SetAnchor(nwAnchor, tileOffset);
        }

        var boxCollider = GetComponent<BoxCollider>();

        boxCollider.size = new Vector3(
            (seAnchor.X - nwAnchor.X + 3) * Edge,
            (seAnchor.Y - nwAnchor.Y + 3) * Edge,
            0.1f);
    }

    public void OnNext(RasterTile tile)
    {
        var id = tile.Id.ToString();
        var contains = tiles.ContainsKey(id);

        switch (tile.CurrentState)
        {
            case Tile.State.Loading:
                if (!contains)
                {
                    var slippy = ScriptableObject.CreateInstance<SlippyTile>();

                    slippy.SetTileId(tile.Id);
                    slippy.SetParent(transform);

                    tiles.Add(id, slippy);
                }
                break;
            case Tile.State.Loaded:
                if (contains && tile.Error == null)
                {
                    tiles[id].Render(tile);
                }
                break;
            case Tile.State.Canceled:
                if (contains)
                {
                    tiles[id].Clear();
                    tiles.Remove(id);
                }
                break;
        }
    }
}
