using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
/// <summary>
/// Utility classes and helper functions.
/// </summary>
public static class Utility
{

    public static T[] ShuffleArray<T>(T[] array,int seed)
    {

        System.Random prng = new System.Random(seed);

        for (int i = 0; i < array.Length - 1; i++)
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }

    public static bool rectOverlaps(RectTransform rectTrans1, RectTransform rectTrans2)
    {
        //Rect rect1 = new Rect(rectTrans1.localPosition.x, rectTrans1.localPosition.y, rectTrans1.rect.width, rectTrans1.rect.height);
        //Rect rect2 = new Rect(rectTrans2.localPosition.x, rectTrans2.localPosition.y, rectTrans2.rect.width, rectTrans2.rect.height);

        return GetWorldSapceRect(rectTrans1).Overlaps(GetWorldSapceRect(rectTrans2));
    }

    public static Rect GetWorldSapceRect(RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }

    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    public static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    public static void buildRoomsOnMap(TileObject[,] map, Vector2Int[] walls)
    {
        int i = 0;

        while (i < walls.Length)
        {
            map[walls[i].x, walls[i].y].type = IGenerator.roomChar;
            i++;
        }
    }

    //Display a lines on the UI given a collection of points. Requires a game object LineGameObj with a UILineRenderer component
    public static void displaySegmentedLineUI(GameObject LineGameObj, RectTransform destination, Vector2Int[] Points, Vector2 originPos, float offset)
    {
        LineGameObj.transform.SetParent(destination.transform);
        LineGameObj.transform.position = destination.transform.position;
        LineGameObj.transform.localScale = Vector3.one;
        UILineRenderer LineRenderer = LineGameObj.GetComponent<UILineRenderer>();
        Vector2[] PointsF = Array.ConvertAll(Points, item => (Vector2)item);

        for (int i = Points.Length-1; i >=0 ; i--)
        {
            float dx = Points[i].x - Points[Points.Length - 1].x;
            float dy = Points[i].y - Points[Points.Length - 1].y;

            Vector2 pUI = new Vector2(originPos.x + offset * dx, originPos.y + offset * dy);
            PointsF[i] = pUI;
        }
        LineRenderer.Points = PointsF;
    }

    public static Vector3 GetGUIElementOffset(RectTransform rect)
    {
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rect.GetWorldCorners(objectCorners);

        var xnew = 0f;
        var ynew = 0f;
        var znew = 0f;

        for (int i = 0; i < objectCorners.Length; i++)
        {
            if (objectCorners[i].x < screenBounds.xMin)
            {
                xnew = screenBounds.xMin - objectCorners[i].x;
            }
            if (objectCorners[i].x > screenBounds.xMax)
            {
                xnew = screenBounds.xMax - objectCorners[i].x;
            }
            if (objectCorners[i].y < screenBounds.yMin)
            {
                ynew = screenBounds.yMin - objectCorners[i].y;
            }
            if (objectCorners[i].y > screenBounds.yMax)
            {
                ynew = screenBounds.yMax - objectCorners[i].y;
            }
        }

        return new Vector3(xnew, ynew, znew);

    }

    //Creates a plane without any texture
    public static GameObject CreatePlane(float width, float height)
    {
        GameObject gO = new GameObject();
        MeshRenderer meshRenderer = gO.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        MeshFilter meshFilter = gO.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        meshFilter.mesh = mesh;

        return gO;
    }

    public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
    {
        if (camera == null)
        {
            camera = Camera.main;
        }
        var viewportPosition = camera.WorldToViewportPoint(worldPosition);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
    {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                            screenPosition.y / Screen.height,
                                            0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
    {
        var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
        var canvasRect = canvas.GetComponent<RectTransform>();
        var scale = canvasRect.sizeDelta;
        return Vector3.Scale(centerBasedViewPortPosition, scale);
    }


    public static bool in_bounds_General(Vector2Int id, int width, int height)
    {
        return 0 <= id.x && id.x < width && 0 <= id.y && id.y < height;
    }

    public static bool passable_General(Vector2Int id, TileObject[,] map)
    {
        return map[id.x, id.y].type != IGenerator.wallChar;
    }

    //this version get all neighbours walls excluded: is a more general version
    public static Vector2Int[] getNeighbours_General(TileObject[,] map, Vector2Int id, ITypeGrid TypeGrid, int width, int height)
    {
        Vector2Int[] results = new Vector2Int[] { };

        foreach (Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (in_bounds_General(next, width, height) && passable_General(next, map))
            {
                Array.Resize(ref results, results.Length + 1);
                results[results.GetUpperBound(0)] = next;
            }
        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }

    //this version get all neighbours including walls: is a more general version
    public static Vector2Int[] getAllNeighbours_General(Vector2Int id, ITypeGrid TypeGrid, int width, int height)
    {
        Vector2Int[] results = new Vector2Int[] { };

        foreach (Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (in_bounds_General(next, width, height))
            {
                Array.Resize(ref results, results.Length + 1);
                results[results.GetUpperBound(0)] = next;
            }
        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }

    //this version get all neighbours including walls and cells outside the grid: is a more general version
    public static Vector2Int[] getAllNeighboursWOBoundCheck_General(Vector2Int id, ITypeGrid TypeGrid, int width, int height)
    {
        Vector2Int[] results = new Vector2Int[] { };

        foreach (Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);

            Array.Resize(ref results, results.Length + 1);
            results[results.GetUpperBound(0)] = next;

        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }

    //this version get all moore neighbours including walls: is a more general version
    public static Vector2Int[] getAllMooreNeighbours_General(Vector2Int id, ITypeGrid TypeGrid, int width, int height)
    {
        Vector2Int[] results = new Vector2Int[] { };

        foreach (Vector2Int dir in TypeGrid.getDirs())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (in_bounds_General(next, width, height))
            {
                //results[results.Length] = next;
                Array.Resize(ref results, results.Length + 1);
                results[results.GetUpperBound(0)] = next;
            }
        }

        foreach (Vector2Int dir in TypeGrid.getDiags())
        {
            Vector2Int next = new Vector2Int(id.x + dir.x, id.y + dir.y);
            if (in_bounds_General(next, width, height))
            {
                //results[results.Length] = next;
                Array.Resize(ref results, results.Length + 1);
                results[results.GetUpperBound(0)] = next;
            }
        }

        if ((id.x + id.y) % 2 == 0)
        {
            Array.Reverse(results);
        }

        return results;
    }


    public static Texture2D AlphaBlend(this Texture2D aBottom, Texture2D aTop)
    {
        if (aBottom.width != aTop.width || aBottom.height != aTop.height)
            throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");

        var bData = aBottom.GetPixels();    //returns a  Color[]: array of pixels in the texture that have been selected. 
        var tData = aTop.GetPixels();       //Color array of top
        int count = bData.Length;           //num of pixels
        var rData = new Color[count];       //resulting array of pixels

        for (int i = 0; i < count; i++)
        {
            Color B = bData[i];
            Color T = tData[i];
            float srcF = T.a;
            float destF = 1f - T.a;
            float alpha = srcF + destF * B.a;
            Color R = (T * srcF + B * B.a * destF) / alpha;
            R.a = alpha;
            rData[i] = R;
        }

        var res = new Texture2D(aTop.width, aTop.height);
        res.SetPixels(rData);
        res.Apply();
        return res;
    }

    public static Texture2D CompositeTopOnBottomAtPos(this Texture2D aBottom, Texture2D aTop, Vector2Int pos)
    {


        for (int i = 0; i < aTop.width; i++)
        {
            for (int j = 0; j < aTop.height; j++)
            {
                aBottom.SetPixel((i + pos.x), (j + pos.y),aTop.GetPixel(i,j));
            }
        }

        aBottom.Apply();
        return aBottom;
    }

    public static Texture2D BlendOnWhite(this Texture2D aBottom, Color c)
    {


        var bData = aBottom.GetPixels();    //returns a  Color[]: array of pixels in the texture that have been selected. 
        int count = bData.Length;           //num of pixels
        var rData = new Color[count];       //resulting array of pixels

        for (int i = 0; i < count; i++)
        {
            Color B = bData[i];
            /*
            if (B == Color.white) { 
                rData[i] = (c+B)/2;
            }else
                rData[i] = B;*/
            Color T = c;
            float srcF = T.a;
            float destF = 1f - T.a;
            float alpha = B.a;
            Color R = (T  * B* B.a);
            R.a = alpha;
            rData[i] = R;
        }

        var res = new Texture2D(aBottom.width, aBottom.height);
        res.SetPixels(rData);
        res.Apply();
        return res;
    }

    public class TextureScale
    {
        public class ThreadData
        {
            public int start;
            public int end;
            public ThreadData(int s, int e)
            {
                start = s;
                end = e;
            }
        }

        private static Color[] texColors;
        private static Color[] newColors;
        private static int w;
        private static float ratioX;
        private static float ratioY;
        private static int w2;
        private static int finishCount;
        private static Mutex mutex;

        public static void Point(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, false);
        }

        public static void Bilinear(Texture2D tex, int newWidth, int newHeight)
        {
            ThreadedScale(tex, newWidth, newHeight, true);
        }

        private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
        {
            texColors = tex.GetPixels();
            newColors = new Color[newWidth * newHeight];
            if (useBilinear)
            {
                ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
                ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
            }
            else
            {
                ratioX = ((float)tex.width) / newWidth;
                ratioY = ((float)tex.height) / newHeight;
            }
            w = tex.width;
            w2 = newWidth;
            var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
            var slice = newHeight / cores;

            finishCount = 0;
            if (mutex == null)
            {
                mutex = new Mutex(false);
            }
            if (cores > 1)
            {
                int i = 0;
                ThreadData threadData;
                for (i = 0; i < cores - 1; i++)
                {
                    threadData = new ThreadData(slice * i, slice * (i + 1));
                    ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
                    Thread thread = new Thread(ts);
                    thread.Start(threadData);
                }
                threadData = new ThreadData(slice * i, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
                while (finishCount < cores)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                ThreadData threadData = new ThreadData(0, newHeight);
                if (useBilinear)
                {
                    BilinearScale(threadData);
                }
                else
                {
                    PointScale(threadData);
                }
            }

            tex.Resize(newWidth, newHeight);
            tex.SetPixels(newColors);
            tex.Apply();

            texColors = null;
            newColors = null;
        }

        public static void BilinearScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                int yFloor = (int)Mathf.Floor(y * ratioY);
                var y1 = yFloor * w;
                var y2 = (yFloor + 1) * w;
                var yw = y * w2;

                for (var x = 0; x < w2; x++)
                {
                    int xFloor = (int)Mathf.Floor(x * ratioX);
                    var xLerp = x * ratioX - xFloor;
                    newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
                                                           ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
                                                           y * ratioY - yFloor);
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        public static void PointScale(System.Object obj)
        {
            ThreadData threadData = (ThreadData)obj;
            for (var y = threadData.start; y < threadData.end; y++)
            {
                var thisY = (int)(ratioY * y) * w;
                var yw = y * w2;
                for (var x = 0; x < w2; x++)
                {
                    newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
                }
            }

            mutex.WaitOne();
            finishCount++;
            mutex.ReleaseMutex();
        }

        private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
        {
            return new Color(c1.r + (c2.r - c1.r) * value,
                              c1.g + (c2.g - c1.g) * value,
                              c1.b + (c2.b - c1.b) * value,
                              c1.a + (c2.a - c1.a) * value);
        }

        

    }

    public static void renderAliasOnUI(RectTransform container, ITypeGrid typeGrid, StructuredAlias alias, GameObject AliasPrefab, bool attachMapMetrics)
    {
        GameObject AliasGO = GameObject.Instantiate(AliasPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        container.parent.GetComponent<MapListManager>().addMapToDictionary(alias.AliasMap, alias.start, alias.end, alias.similarityDistance, AliasGO.GetInstanceID());

        AliasGO.transform.SetParent(container, false);
        Transform t = AliasGO.transform.Find("BorderMask/Content");
        RectTransform contentRect = t.GetComponent<RectTransform>();
        RectTransform prefabRect = typeGrid.TilePrefab.GetComponent<RectTransform>();
        initAliasGameObject(AliasGO);
        GeneratorUIManager.Instance.DisplayMap(alias.AliasMap, t, ParameterManager.Instance.GridType);

        GeneratorUIManager.Instance.ScaleToFitContainer(contentRect, new Rect(Vector2.zero, container.GetComponent<GridLayoutGroup>().cellSize));

        if (attachMapMetrics)
            AliasGO.GetComponentInChildren<HoverDisplayText>().textToDisplay = MapEvaluator.aggregateAliasDataMap(MapEvaluator.computeMetrics(alias.AliasMap, typeGrid, alias.start, alias.end), alias.similarityDistance);
        else
            AliasGO.GetComponentInChildren<HoverDisplayText>().gameObject.SetActive(false);


    }

    private static void initAliasGameObject(GameObject AliasGO)
    {
        HoverDisplayText scriptHoverDisplay = AliasGO.GetComponentInChildren<HoverDisplayText>();
        DragHandler dHand = AliasGO.GetComponentInChildren<DragHandler>();
        scriptHoverDisplay.DialogBoxInfo = GameObject.FindGameObjectWithTag("DialogBox");

        dHand.OriginalParent = dHand.ToMoveGameObj.transform.parent.parent.gameObject;
        dHand.canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        dHand.defaultColor = AliasGO.transform.parent.parent.GetComponent<Image>().color;
    }
}


/*
[System.Serializable]

public class Pair<T, U>
{
    [SerializeField]
    public T x;
    [SerializeField]
    public U y;

    public Pair()
    {
    }

    public Pair(T x, U y)
    {
        this.x = x;
        this.y = y;
    }

    

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Pair<T, U>)obj);
    }

    public bool Equals(Pair<T, U> obj)
    {
        return obj != null && obj == this;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }


    public static bool operator ==(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.x == p2.x && (dynamic)p1.y == p2.y);
    }

    public static bool operator !=(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1== p2);
    }

    public static bool operator <(Pair<T, U> p1, Pair<T, U> p2)
    {
        return ((dynamic)p1.x < p2.x && (dynamic)p1.y < p2.y);
    }

    public static bool operator >(Pair<T, U> p1, Pair<T, U> p2)
    {
        return !(p1<p2) && p1!=p2;
    }
};*/
