using UnityEngine;
using System.Collections;

public class Rotatable : MonoBehaviour
{


    public bool startOnAwake = true;

    public bool reverse = false;

    [Tooltip("X is for angular velocity (degrees per second).\n Y is for rotation duration.\n Z is for pause duration after the end of the rotation.")]
    public Vector3[] rotationVDP;

    private int counter;

    private Rigidbody2D _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        counter = 0;

        if (startOnAwake) {
            StartRotating();
        }
    }

    public void StartRotating()
    {
        StartCoroutine(RotationCoroutine());
    }

    private IEnumerator RotationCoroutine()
    {
        bool reverseIncrease = true;
        while (true) {

            float speed = rotationVDP[counter].x;
            float duration = rotationVDP[counter].y;
            float pause = rotationVDP[counter].z;

            //rotate
            float initialRotation = transform.rotation.eulerAngles.z;
            _rigidBody.angularVelocity = speed;
            if (duration <= 0) {
                yield break;
            }
            yield return new WaitForSeconds(duration);
            float finalRotation = initialRotation + speed * duration;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, finalRotation));

            //pause
            _rigidBody.angularVelocity = 0;
            if (pause <= 0) {
                yield break;
            }
            yield return new WaitForSeconds(pause);

            if (reverse) {
                if (reverseIncrease) {
                    counter++;
                    if (counter == rotationVDP.Length) {
                        counter = rotationVDP.Length - 2;
                        reverseIncrease = false;

                        //reverse pauses
                        float[] pausesCopy = new float[rotationVDP.Length];
                        for (int i = 0; i < pausesCopy.Length; i++) {
                            pausesCopy[i] = rotationVDP[i].z;
                        }

                        for (int i = 0; i < rotationVDP.Length; i++) {
                            rotationVDP[i].z = pausesCopy[pausesCopy.Length - 1 - i];
                        }

                    }
                } else {
                    counter--;
                    if (counter == -1) {
                        counter = 1;
                        reverseIncrease = true;

                        //reverse pauses
                        float[] pausesCopy = new float[rotationVDP.Length];
                        for (int i = 0; i < pausesCopy.Length; i++) {
                            pausesCopy[i] = rotationVDP[i].z;
                        }
                        for (int i = 0; i < rotationVDP.Length; i++) {
                            rotationVDP[i].z = pausesCopy[pausesCopy.Length - 1 - i];
                        }

                    }
                }
            } else {
                counter = (counter + 1) % rotationVDP.Length;
            }
        }
    }
}
