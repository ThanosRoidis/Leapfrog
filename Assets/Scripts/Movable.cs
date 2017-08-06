using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]

public class Movable : MonoBehaviour {


  public bool startOnAwake = true;

  public bool circularPath = false;

  public Transform[] points = new Transform[2];

  public float[] moveDurations = new float[2];

  public float[] pauseDurations = new float[2];

  private int nextPosition;

  private Rigidbody2D _rigidBody;
  private Vector3[] path; //= new Transform[2];



  // Use this for initialization
  void Start() {
    _rigidBody = GetComponent<Rigidbody2D>();

    path = new Vector3[points.Length];
    for (int i = 0; i < path.Length; i++)
      path[i] = points[i].position;

    if (startOnAwake) {
      StartMoving();
    }
  }


  public void StartMoving() {
    StartCoroutine(MoveCoroutine());
  }

  private IEnumerator MoveCoroutine() {
    int nextPosition = 1;
    while (true) {
      Vector3 startPos = transform.position;
      Vector3 targetPos = path[nextPosition];
      float moveDuration = moveDurations[nextPosition];
      float pauseDuration = pauseDurations[nextPosition];

      _rigidBody.velocity = (targetPos - startPos) / moveDuration;
      yield return new WaitForSeconds(moveDuration);

      _rigidBody.velocity = Vector2.zero;
      yield return new WaitForSeconds(pauseDuration);

      if (circularPath) {
        nextPosition = (nextPosition + 1) % path.Length;
      }
      else {
        nextPosition++;
        if (nextPosition == path.Length) {
          ReversePath();
          nextPosition = 1;
        }
      }

    }
  }

  private void ReversePath() {
    float[] durationsCopy = new float[path.Length];
    Vector3[] pathCopy = new Vector3[path.Length];

    for (int i = 0; i < path.Length; i++) {
      durationsCopy[i] = moveDurations[i];
      pathCopy[i] = path[i];
    }

    for (int i = 0; i < moveDurations.Length; i++) {
      moveDurations[i] = durationsCopy[durationsCopy.Length - 1 - i];
      path[i] = pathCopy[path.Length - 1 - i];
    }

    float lastElement = moveDurations[moveDurations.Length - 1];
    for (int i = moveDurations.Length - 1; i >= 1; i--) {
      moveDurations[i] = moveDurations[i - 1];
    }
    moveDurations[0] = lastElement;

  }

  void OnDrawGizmosSelected() {
    if (path != null) {

      /*
      if (Application.platform != RuntimePlatform.WebGLPlayer) {
          Gizmos.color = Color.white;
          for (int i = 0; i < path.Length; i++) {
              if (path[i] == null) {
                  continue;
              }
              UnityEditor.Handles.CircleCap(0, path[i].position, Quaternion.identity, 7);
              char letter = (char)((int)'A' + i);
              UnityEditor.Handles.Label(path[i].position, letter + ", " + pauseDurations[i]);
          }
      }*/

      for (int i = 0; i < path.Length - 1; i++) {
        if (path[i] == null) {
          continue;
        }
        Gizmos.DrawLine(path[i], path[i + 1]);
        Vector3 labelPos = (path[i] + path[i + 1]) / 2;

        /*
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            UnityEditor.Handles.Label(labelPos, moveDurations[i] + "");
        */
      }
    }

  }
}
