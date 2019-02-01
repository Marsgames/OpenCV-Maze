#pragma warning disable CS0618 // Le type ou le membre est obsolète
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> GenerateTerrainInspector
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(GenerateTerrain))]
public class GenerateTerrainInspector : Editor
{
    #region Variables
    private bool showRotate;
    #endregion

    /////////////////////////////////////////////////////////

    #region Unity's functions
    public override void OnInspectorGUI()
    {
        showRotate = GameObject.Find("Terrain");

        DrawDefaultInspector();

        GenerateTerrain myTarget = (GenerateTerrain)target;
        if (GUILayout.Button("Select Maze"))
        {
            myTarget.SetSelectMaze(true);
            showRotate = true;
        }

        //if (showRotate)
        //{
        //    if (GUILayout.Button("Rotate Terrain"))
        //    {
        //        GameObject terrain = GameObject.Find("Terrain");

        //        var rotation = terrain.transform.rotation.eulerAngles;

        //        rotation.z += rotation.z >= 270 ? 90 : -270;
        //    }
        //}


        if (GUILayout.Button("Reset"))
        {
            myTarget.Reset();
        }



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

    #endregion
}
#pragma warning restore CS0618 // Le type ou le membre est obsolète