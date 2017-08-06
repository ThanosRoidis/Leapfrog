using UnityEngine;
using System.Collections;

public class Movable : MonoBehaviour
{


    public bool startOnAwake = true;

    public bool circularPath = false;

    public Transform[] path = new Transform[2];

    public float[] moveDurations = new float[2];

    public float[] pauseDurations = new float[2];

    private int nextPosition;

    private Rigidbody2D _rigidBody;



    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (startOnAwake) {
            StartMoving();
        }
    }


    public void StartMoving()
    {
        StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        int nextPosition = 1;
        while (true) {
            Vector3 startPos = transform.position;
            Vector3 targetPos = path[nextPosition].position;
            float moveDuration = moveDurations[nextPosition];
            float pauseDuration = pauseDurations[nextPosition];

            _rigidBody.velocity = (targetPos - startPos) / moveDuration;
            yield return new WaitForSeconds(moveDuration);

            _rigidBody.velocity = Vector2.zero;
            yield return new WaitForSeconds(pauseDuration);

            if (circularPath) {
                nextPosition = (nextPosition + 1) % path.Length;
            } else {
                nextPosition++;
                if (nextPosition == path.Length) {
                    ReversePath();
                    nextPosition = 1;
                }
            }

        }
    }

    private void ReversePath()
    {
        float[] durationsCopy = new float[path.Length];
        Transform[] pathCopy = new Transform[path.Length];

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

    void OnDrawGizmosSelected()
    {
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
                Gizmos.DrawLine(path[i].position, path[i + 1].position);
                Vector3 labelPos = (path[i].position + path[i + 1].position) / 2;

                /*
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                    UnityEditor.Handles.Label(labelPos, moveDurations[i] + "");
                */
            }
        }

    }
}
