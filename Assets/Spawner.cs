using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

  [Header("GameObjects")]
  public GameObject[] platformPrefabs;
  public Transform spawnPoint;

  [Header("Spawner Options")]
  public float obstacleMaxLevelDiff = 1;

  [Header("Misc.")]
  public int currentLevelIndex;
  private GameLevel currentLevel;

  private float xLeft;
  private float xRight;

  private float maxObstacleLength = 0;
  private float lastPlatformHeight = 0;

  public static Spawner instance;

  void Awake() {
    instance = this;
  }

  void Start() {
    xLeft = 0.05f + Camera.main.ScreenToWorldPoint(Vector3.zero).x;
    xRight = (-0.05f) + Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

    maxObstacleLength = xRight - xLeft;
    lastPlatformHeight = spawnPoint.position.y;

    for (int i = 0; i < platformPrefabs.Length; i++) {
      platformPrefabs[i].GetComponent<Platform>().objectID = i;
    }

    ChangeLevel(currentLevelIndex);
  }

  // Update is called once per frame
  void Update() {


    //Debug.Log (Screen.);

    //Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + Time.deltaTime, Camera.main.transform.position.z);


    if (GameController.instance.isRunning) {

      // Spawn Platform
      if (spawnPoint.position.y - lastPlatformHeight > currentLevel.platformDistance) {
        int spawnId = -1;
        bool done = false;

        do {
          float r = Random.value;

          float lastSum = 0;
          for (int i = 0; i < platformPrefabs.Length; i++) {
            float sum = 0;

            for (int j = 0; j <= i; j++)
              sum += currentLevel.platformPercentages[j];

            if (r >= lastSum && r < sum) {
              spawnId = platformPrefabs[i].GetComponent<Platform>().objectID;
              break;
            }
            else
              lastSum = sum;
          }

          done = true;

        } while (done == false);

        SpawnPlatform(spawnId);
      }
    }
  }

  void SpawnPlatform(int id) {

    GameObject platform;
    Platform platScript;

    float xPos = Random.Range(xLeft, xRight);
    Vector3 spawnPos = new Vector3(xPos, spawnPoint.transform.position.y, 0);

    platform = (GameObject) Instantiate(platformPrefabs[id], spawnPos, platformPrefabs[id].transform.rotation);
    platScript = platform.GetComponent<Platform>();
    platScript.Initialize(/*maxObstacleLength*/);
    platScript.instanceID = id;

  
    //if (!platScript.fullWidth) {
    //  int side = Random.Range(0, 3);
    //  if (side == 0)
    //    platform.transform.position = new Vector3(xLeft + platform.GetComponent<Renderer>().bounds.size.x / 2, spawnPoint.position.y, 0);
    //  else if (side == 1)
    //    platform.transform.position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);
    //  else
    //    platform.transform.position = new Vector3(xRight - platform.GetComponent<Renderer>().bounds.size.x / 2, spawnPoint.position.y, 0);
    //}


    lastPlatformHeight = spawnPoint.position.y + Random.Range(-currentLevel.platformDistanceRange/2, currentLevel.platformDistanceRange/2);
  }


  public GameLevel GenerateLevel(int levelIndex) {
    ArrayList activePlatforms = new ArrayList();
    float totalPriority = 0;

    foreach (GameObject obs in platformPrefabs) {
      Platform obScript = obs.GetComponent<Platform>();
      if (obScript.start_lvl <= levelIndex && Mathf.Abs(obScript.main_lvl - levelIndex) < obstacleMaxLevelDiff) {
        activePlatforms.Add(obScript);
        totalPriority += obScript.priority;

        //				Debug.Log (obScript.objectID);
      }
    }

    int activeNum = activePlatforms.Count;

    GameLevel level = new GameLevel();
    level.platformDistance = 1;
    level.platformDistanceRange = 1;


    level.platformPercentages = new float[platformPrefabs.Length];

    for (int i = 0; i < level.platformPercentages.Length; i++)
      level.platformPercentages[i] = 0;

    if (activeNum == 1) {
      level.platformPercentages[((Platform) activePlatforms[0]).objectID] = 1;
      return level;
    }



    foreach (Platform obScript in activePlatforms) {
      float norm_priority = obScript.priority / totalPriority;
      float level_penalty = norm_priority * Mathf.Abs(obScript.main_lvl - levelIndex) / obstacleMaxLevelDiff;

      level.platformPercentages[obScript.objectID] += norm_priority;
      level.platformPercentages[obScript.objectID] -= level_penalty;

      float other_bonus = level_penalty / (activeNum - 1);
      foreach (Platform otherPlatScript in activePlatforms) {
        if (otherPlatScript.objectID != obScript.objectID)
          level.platformPercentages[otherPlatScript.objectID] += other_bonus;
      }
    }

    //Debug.Log ("LEVEL " + levelIndex);
    //foreach (BasePlatform ob in activeObstacles)
    //	Debug.Log ("ID: " + ob.objectID + "  PR: " + level.obstaclePerc [ob.objectID]);

    return level;
  }

  public void ChangeLevel(int index) {
    currentLevelIndex = index;
    currentLevel = GenerateLevel(index);
    //currentLevel = ((GameObject)Resources.Load ("Level" + index.ToString ())).GetComponent<GameLevel> ();

  }
}
