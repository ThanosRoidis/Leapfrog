using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerController : MonoBehaviour {


  public bool clicked;

  public Vector2 clickLocation;

  public float maxForce = 1000f;
  public float maxDragDistance = 100f;
  public float clickRadius = 1;

  // Update is called once per frame
  void Update() {
    if (Input.GetMouseButtonDown(0)) {
      Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
      clickPos.z = 0;
      float dist = (clickPos - transform.position).magnitude;

      if (dist < clickRadius) {
        clickLocation = Input.mousePosition;
        clicked = true;
      }
    }


    Vector2 currentMouseLocation = Input.mousePosition;

    if (Input.GetMouseButtonUp(0) && clicked) {
      Vector2 forceDirection = clickLocation - currentMouseLocation;
      float forceMagnitude = forceDirection.magnitude;
      if (forceMagnitude > maxDragDistance) {
        forceMagnitude = maxDragDistance;
      }
      forceMagnitude /= maxDragDistance;
      //Debug.Log(forceMagnitude);


      forceDirection.Normalize();
      this.GetComponent<Rigidbody2D>().AddForce(forceDirection * forceMagnitude * maxForce);
      clicked = false;
    }

    if (clicked) {

    }
  }


  void OnDrawGizmosSelected() {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, clickRadius);
  }
}
