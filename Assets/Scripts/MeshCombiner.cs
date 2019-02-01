#pragma warning disable CS0618 // Le type ou le membre est obsolète
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> MeshCombiner
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    #region Variables
    //public


    // SerializedField


    // private
    
    #endregion

    /////////////////////////////////////////////////////////

    #region Unity's functions
    void Start()
    {
        //CombineMeshes();
    }

    void Update()
    {
        
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
    public void CombineMeshes()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        Debug.Log(name + " is combining " + filters.Length + " meshes");

        Mesh finalMesh = new Mesh();

        CombineInstance[] combiners = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++)
        {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = filters[i].sharedMesh;
            combiners[i].transform = filters[i].transform.localToWorldMatrix;
        }

        finalMesh.CombineMeshes(combiners);

        GetComponent<MeshFilter>().sharedMesh = finalMesh;
    }
    #endregion
}
#pragma warning restore CS0618 // Le type ou le membre est obsolète