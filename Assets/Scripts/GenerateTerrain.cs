#pragma warning disable CS0618 // Le type ou le membre est obsolète
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> GenerateTerrain
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;
using Emgu.CV;
using System;
using System.Drawing;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenerateTerrain : MonoBehaviour
{
    #region Variables
    [Header("Public")]
    [SerializeField]
    private int m_webcamID = 0;
    [SerializeField, Range(0, 180)]
    private int m_minH = 50;
    [SerializeField, Range(0, 255)]
    private int m_minS = 100;
    [SerializeField, Range(0, 255)]
    private int m_minV = 155;
    [Space, SerializeField, Range(0, 180)]
    private int m_maxH = 90;
    [SerializeField, Range(0, 255)]
    private int m_maxS = 255;
    [SerializeField, Range(0, 255)]
    private int m_maxV = 255;
    [SerializeField]
    private bool m_drawContour = true;
    [SerializeField, Range(1, 50)]
    private int m_density = 30;

    [Header("Private")]
    private VideoCapture m_webcam;
    private Mat m_imgFlux = new Mat();
    private Mat m_imgHSV = new Mat();
    private Mat m_imgMazeGrey = new Mat();
    public enum Task { Grab, SelectTerrain, DrawTerrain }
    public Task actualTask = Task.Grab;
    private bool selectMaze;
    private VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
    Image webcamUIImage;
    Image labyUIImage;
    Image flipUIImage;
    #endregion

    /////////////////////////////////////////////////////////

    #region Unity's functions
    void Start()
    {
        selectMaze = false;
        m_webcam = new VideoCapture(m_webcamID);
        m_webcam.ImageGrabbed += ImageGrabbed;
        webcamUIImage = GameObject.Find("Webcam").GetComponent<Image>();
        labyUIImage = GameObject.Find("Labyrinthe").GetComponent<Image>();
        flipUIImage = GameObject.Find("FlipImage").GetComponent<Image>();
    }

    void Update()
    {
        if (selectMaze)
        {
            actualTask = Task.SelectTerrain;
            selectMaze = false;
        }


        switch (actualTask)
        {
            case Task.Grab:
                m_webcam.Grab();
                break;
            case Task.SelectTerrain:
                //SelectTerrain();
                DrawPoints();
                break;
            case Task.DrawTerrain:
                break;
        }
    }

    //private void OnDestroy()
    //{
    //    m_webcam.Dispose();
    //    CvInvoke.DestroyAllWindows();
    //}
    #endregion

    /////////////////////////////////////////////////////////

    #region Accessors
    public void SetSelectMaze(bool value)
    {
        selectMaze = value;
    }
    #endregion

    /////////////////////////////////////////////////////////

    #region Functions
    void ImageGrabbed(object sender, EventArgs e)
    {
        if (null == sender)
        {
            return;
        }

        VideoCapture cam = sender as VideoCapture;
        cam.Retrieve(m_imgFlux);

        TrackMaze();

        ShowImg(m_imgMazeGrey);

        var imgClone = m_imgFlux.Clone();
        CvInvoke.Flip(imgClone, imgClone, FlipType.Horizontal);
        CvInvoke.Resize(imgClone, imgClone, new Size(imgClone.Size.Width / 2, imgClone.Size.Height / 2));

        Sprite sprite = Sprite.Create(ConvertMatToTexture2D(imgClone, 500, 500), new Rect(0, 0, 500, 500), Vector2.zero);
        
        webcamUIImage.sprite = sprite;
        flipUIImage.sprite = sprite;
        //CvInvoke.Imshow("Webcam", aze);
    }

    void ShowImg(Mat imgToShow = null)
    {
        if (null == imgToShow)
        {
            imgToShow = m_imgFlux;
        }


        CvInvoke.Flip(imgToShow, imgToShow, FlipType.Horizontal);
        CvInvoke.Resize(imgToShow, imgToShow, new Size(imgToShow.Size.Width / 2, imgToShow.Size.Height / 2));
        CvInvoke.Imshow("Labyrinthe", imgToShow);

        Sprite sprite = Sprite.Create(ConvertMatToTexture2D(imgToShow, 500, 500), new Rect(0, 0, 500, 500), Vector2.zero);
        labyUIImage.sprite = sprite;
    }

    void TrackMaze()
    {
        CvInvoke.CvtColor(m_imgFlux, m_imgHSV, ColorConversion.Bgr2Hsv);

        Image<Hsv, Byte> trackMaze = m_imgHSV.ToImage<Hsv, Byte>();
        Image<Gray, Byte> greyMaze = trackMaze.InRange(new Hsv(m_minH, m_minS, m_minV), new Hsv(m_maxH, m_maxS, m_maxV));

        m_imgMazeGrey = greyMaze.Mat;
        if (m_drawContour)
        {
            DrawContour(m_imgMazeGrey);
        }
    }

    void DrawContour(Mat img)
    {
        VectorOfPoint biggestContour = new VectorOfPoint();
        int biggestContourIndex = -1;
        double biggestContourArea = 0;
        Mat hierarchy = new Mat();

        CvInvoke.FindContours(img, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

        for (int i = 0; i < contours.Size; i++)
        {
            if (CvInvoke.ContourArea(contours[i]) > biggestContourArea)
            {
                biggestContourIndex = i;
                biggestContour = contours[i];
                biggestContourArea = CvInvoke.ContourArea(contours[i]);
            }
        }

        CvInvoke.CvtColor(m_imgMazeGrey, m_imgMazeGrey, ColorConversion.Gray2Bgr);


        if (-1 != biggestContourIndex)
        {
            MCvScalar color = new MCvScalar(255, 0, 0);
            CvInvoke.DrawContours(img, contours, biggestContourIndex, color, 5);
        }
    }

    //private void SelectTerrain()
    //{
    //    VectorOfPoint biggestContour = new VectorOfPoint();
    //    int biggestContourIndex = -1;
    //    double biggestContourArea = 0;

    //    //List<Vector3> verticesList = new List<Vector3>();
    //    List<Vector2> uvsList = new List<Vector2>();

    //    for (int i = 0; i < contours.Size; i++)
    //    {
    //        Debug.Log("contours[i] : " + contours[i]);
    //        VectorOfPoint points = contours[i];
    //        //for (int j = 0; j < points.Size; j++)
    //        //{
    //            if (CvInvoke.ContourArea(contours[i]) > biggestContourArea)
    //            {
    //                biggestContourIndex = i;
    //                biggestContour = contours[i];
    //                biggestContourArea = CvInvoke.ContourArea(contours[i]);
    //            }
    //    //        Debug.Log("points[j] : (" + points[j].X + ", " + points[j].Y + ")");
    //    ////        verticesList.Add(new Vector3(points[j].X, points[j].Y, 0));
    //    //        uvsList.Add(new Vector2(points[j].X, points[j].Y));
    //    ////        //vertices[j] = new Vector3(points[j].X, points[j].Y, 0);
    //    ////        //uvs[j] = new Vector2(points[j].X, points[j].Y);
    //    ////        Debug.Log("points[j] : " + points[j]);
    //        //}
    //    }

    //    for (int i = 0; i < biggestContour.Size; i++)
    //    {
    //        uvsList.Add(new Vector2(biggestContour[i].X, biggestContour[i].Y));
    //    }

    //    ////GameObject go = new GameObject("Terrain");
    //    ////go.AddComponent<MeshFilter>();
    //    ////Mesh mesh = go.GetComponent<MeshFilter>().mesh;

    //    ////Vector3[] vertices = verticesList.ToArray();
    //    ////Vector2[] uvs = uvsList.ToArray();
    //    Vector2[] vertices2D = uvsList.ToArray();

    //    ////mesh.Clear();
    //    ////mesh.vertices = vertices;
    //    ////mesh.uv = uvs;
    //    ////mesh.triangles = new Triangulator(uvs).Triangulate();





    //    //////DECOMMENTER ICI
    //    //Vector2[] vertices2D = new Vector2[] {
    //    //    new Vector2(0,0),
    //    //    new Vector2(0,50),
    //    //    new Vector2(10,50),
    //    //    new Vector2(10,10),
    //    //    new Vector2(50,10),
    //    //    new Vector2(50,0),
    //    //    //new Vector2(150,150),
    //    //    //new Vector2(150,100),
    //    //    //new Vector2(100,100),
    //    //    //new Vector2(100,50),
    //    //    //new Vector2(150,50),
    //    //    //new Vector2(150,0),
    //    //};

    //    ////foreach (var point in uvsList)
    //    ////{
    //    ////    Debug.Log("point : " + point);
    //    ////}



    //    // Use the triangulator to get indices for creating triangles
    //    Triangulator tr = new Triangulator(vertices2D);
    //    int[] indices = tr.Triangulate();

    //    // Create the Vector3 vertices
    //    Vector3[] vertices = new Vector3[vertices2D.Length];
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
    //    }

    //    // Create the mesh
    //    Mesh msh = new Mesh();
    //    msh.vertices = vertices;
    //    msh.triangles = indices;
    //    msh.RecalculateNormals();
    //    msh.RecalculateBounds();

    //    // Set up game object with mesh;
    //    GameObject go = new GameObject("Terrain");
    //    go.transform.Rotate(new Vector3(0, 90, 180));
    //    go.transform.position = new Vector3(-400, 300, -550);
    //    go.AddComponent(typeof(MeshRenderer));
    //    MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
    //    filter.mesh = msh;


    //    actualTask = Task.DrawTerrain;
    //}

    private void DrawPoints()
    {
        VectorOfPoint biggestContour = new VectorOfPoint();
        int biggestContourIndex = -1;
        double biggestContourArea = 0;
        List<Vector2> uvsList = new List<Vector2>();

        for (int i = 0; i < contours.Size; i++)
        {
            VectorOfPoint points = contours[i];
            if (CvInvoke.ContourArea(contours[i]) > biggestContourArea)
            {
                biggestContourIndex = i;
                biggestContour = contours[i];
                biggestContourArea = CvInvoke.ContourArea(contours[i]);
            }
        }

        GameObject terrain = new GameObject("Terrain");
        terrain.AddComponent<MeshRenderer>();
        terrain.AddComponent<MeshFilter>();
        terrain.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Red");
        terrain.transform.Rotate(new Vector3(0, 180, 0));
        terrain.transform.Translate(new Vector3(-1200, 0, -30));
        for (int i = 0; i < biggestContour.Size; i++)
        {
            if (0 != i % m_density)
            {
                continue;
            }
            GameObject mur = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mur.transform.localScale = new Vector3(10, 10, 30);
            mur.transform.position = new Vector3(biggestContour[i].X, biggestContour[i].Y, 0);
            mur.transform.parent = terrain.transform;
            mur.AddComponent<Rigidbody>();
            Rigidbody murRB = mur.GetComponent<Rigidbody>();

            //////////////////////////////// Commenter pour faire exploser le labyrinthe
            //murRB.isKinematic = true;
            murRB.useGravity = false;
            //murRB.constraints = RigidbodyConstraints.FreezeAll;
            murRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            ///////////////////////////////

            //mur.GetComponent<Renderer>().material = Resources.Load<Material>("Materials/Red");

            uvsList.Add(new Vector2(biggestContour[i].X, biggestContour[i].Y));
        }

        terrain.AddComponent<MeshCombiner>();
        terrain.GetComponent<MeshCombiner>().CombineMeshes();
        terrain.AddComponent<MeshCollider>();

        // 225 / 451
        foreach (Transform child in GameObject.Find("Terrain").transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log(terrain.transform.childCount);

        actualTask = Task.DrawTerrain;
    }

    //private Texture2D ImgToTexture(Mat image)
    //{
    //    var texture = ConvertMatToTexture2D(image.Clone(), image.Size.Width, image.Size.Height);

    //    GetComponent<MeshRenderer>().material.mainTexture = texture;
    //    var rotation = transform.rotation;
    //    rotation.y += .01f;
    //    transform.rotation = rotation;
    //}

    private Texture2D ConvertMatToTexture2D(Mat matImage, int width, int height)
    {
        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }

        CvInvoke.Resize(matImage, matImage, new Size(width, height));

        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }

        if (matImage.IsEmpty)
        {
            return new Texture2D(width, height);
        }

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(matImage.ToImage<Rgba, Byte>().Bytes);
        texture.Apply();

        return texture;
    }

    public void Reset()
    {
        SceneManager.LoadScene(0);
    
    }
    #endregion
}
#pragma warning restore CS0618 // Le type ou le membre est obsolète