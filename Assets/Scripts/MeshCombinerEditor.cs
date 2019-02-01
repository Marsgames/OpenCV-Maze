#pragma warning disable CS0618 // Le type ou le membre est obsolète
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> MeshCombinerEditor
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshCombiner))]
public class MeshCombinerEditor : Editor
{
    private void OnSceneGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        if (Handles.Button(mc.transform.position + Vector3.up * 5, Quaternion.LookRotation(Vector3.up), 1, 1, Handles.CylinderCap))
        {
            mc.CombineMeshes();
        }
    }
}
#pragma warning restore CS0618 // Le type ou le membre est obsolète