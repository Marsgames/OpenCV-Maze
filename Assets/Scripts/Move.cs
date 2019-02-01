#pragma warning disable CS0618 // Le type ou le membre est obsolète
/////////////////////////////////////////
//   RAPHAËL DAUMAS --> Move
//   https://raphdaumas.wixsite.com/portfolio
//   https://github.com/Marsgames
/////////////////////////////////////////
using UnityEngine;

public class Move : MonoBehaviour
{
    #region Variables
    public float speed = 100;
    Vector3 position;
    #endregion

    /////////////////////////////////////////////////////////

    #region Unity's functions
    void Start()
    {
    }

    void Update()
    {
        //var camPos = Camera.main.transform.position;
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * speed * Time.deltaTime;

        Vector3 pos = transform.position - new Vector3(-0, 120, 200);
        Camera.main.transform.position = pos;
        //Camera.main.transform.position = 
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