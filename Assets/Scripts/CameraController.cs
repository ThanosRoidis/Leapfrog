using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {


    public Material mat;


    PlayerController player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
	  if(GameController.instance.isRunning) {
      Vector2 cameraPos = transform.position;
      Vector2 playerPos = player.transform.position;

      cameraPos = Vector2.Lerp(cameraPos, playerPos, 5 * Time.deltaTime);
      transform.position = new Vector3(cameraPos.x, cameraPos.y, transform.position.z);
    }
	}



    void OnPostRender()
    {
        if (player.clicked) // I check if the mouse is down on a specific game object
         {
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            GL.Color(Color.red);
            GL.Vertex((Vector3)player.clickLocation);
            GL.Vertex(Input.mousePosition);
            GL.End();
            GL.PopMatrix();
            //Debug.Log(player.clickLocation);
            //Debug.Log(Input.mousePosition);
        }
    }

}
