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

    [Header("Private")]
    private VideoCapture m_webcam;
    private Mat m_imgFlux = new Mat();
    private Mat m_imgHSV = new Mat();
    private Mat m_imgMazeGrey = new Mat();
    public enum Tache { Grab, SelectTerrain, DrawTerrain }
    public Tache actualTask = Tache.Grab;
    public bool select;
    private VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
    #endregion

    /////////////////////////////////////////////////////////

    #region Unity's functions
    void Start()
    {
        select = false;
        m_webcam = new VideoCapture(m_webcamID);
        m_webcam.ImageGrabbed += ImageGrabbed;
    }

    void Update()
    {
        if (select)
        {
            actualTask = Tache.SelectTerrain;
            select = false;
        }


        switch (actualTask)
        {
            case Tache.Grab:
                m_webcam.Grab();
                break;
            case Tache.SelectTerrain:
                SelectTerrain();
                break;
            case Tache.DrawTerrain:
                break;
        }
    }

    private void OnDestroy()
    {
        m_webcam.Dispose();
        CvInvoke.DestroyAllWindows();
    }
    #endregion

    /////////////////////////////////////////////////////////

    #region Accessors
    // Getters


    /////////////////

    //Setters

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
    }

    void ShowImg(Mat imgToShow = null)
    {
        if (null == imgToShow)
        {
            imgToShow = m_imgFlux;
        }

        CvInvoke.Flip(imgToShow, imgToShow, FlipType.Horizontal);
        CvInvoke.Resize(imgToShow, imgToShow, new Size(imgToShow.Size.Width / 2, imgToShow.Size.Height / 2));
        CvInvoke.Imshow("Webcam", imgToShow);
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

    private void SelectTerrain()
    {
        VectorOfPoint biggestContour = new VectorOfPoint();
        int biggestContourIndex = -1;
        double biggestContourArea = 0;

        //List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();

        for (int i = 0; i < contours.Size; i++)
        {
            Debug.Log("contours[i] : " + contours[i]);
            VectorOfPoint points = contours[i];
            //for (int j = 0; j < points.Size; j++)
            //{
                if (CvInvoke.ContourArea(contours[i]) > biggestContourArea)
                {
                    biggestContourIndex = i;
                    biggestContour = contours[i];
                    biggestContourArea = CvInvoke.ContourArea(contours[i]);
                }
        //        Debug.Log("points[j] : (" + points[j].X + ", " + points[j].Y + ")");
        ////        verticesList.Add(new Vector3(points[j].X, points[j].Y, 0));
        //        uvsList.Add(new Vector2(points[j].X, points[j].Y));
        ////        //vertices[j] = new Vector3(points[j].X, points[j].Y, 0);
        ////        //uvs[j] = new Vector2(points[j].X, points[j].Y);
        ////        Debug.Log("points[j] : " + points[j]);
            //}
        }

        for (int i = 0; i < biggestContour.Size; i++)
        {
            uvsList.Add(new Vector2(biggestContour[i].X, biggestContour[i].Y));
        }

        ////GameObject go = new GameObject("Terrain");
        ////go.AddComponent<MeshFilter>();
        ////Mesh mesh = go.GetComponent<MeshFilter>().mesh;

        ////Vector3[] vertices = verticesList.ToArray();
        ////Vector2[] uvs = uvsList.ToArray();
        Vector2[] vertices2D = uvsList.ToArray();

        ////mesh.Clear();
        ////mesh.vertices = vertices;
        ////mesh.uv = uvs;
        ////mesh.triangles = new Triangulator(uvs).Triangulate();





        //////DECOMMENTER ICI
        //Vector2[] vertices2D = new Vector2[] {
        //    new Vector2(0,0),
        //    new Vector2(0,50),
        //    new Vector2(10,50),
        //    new Vector2(10,10),
        //    new Vector2(50,10),
        //    new Vector2(50,0),
        //    //new Vector2(150,150),
        //    //new Vector2(150,100),
        //    //new Vector2(100,100),
        //    //new Vector2(100,50),
        //    //new Vector2(150,50),
        //    //new Vector2(150,0),
        //};

        ////foreach (var point in uvsList)
        ////{
        ////    Debug.Log("point : " + point);
        ////}



        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        GameObject go = new GameObject("Terrain");
        go.transform.Rotate(new Vector3(0, 90, 180));
        go.transform.position = new Vector3(-400, 300, -550);
        go.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = go.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;


        actualTask = Tache.DrawTerrain;
    }
    #endregion
}
#pragma warning restore CS0618 // Le type ou le membre est obsolète