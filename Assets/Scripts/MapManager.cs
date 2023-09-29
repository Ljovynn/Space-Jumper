using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.Shapes;

public class MapManager : MonoBehaviour
{
    private float goalDistance = 1000;
    private float generationBufferXPos = 50;

    private float startingPlatformXPos = 0;
    private float startingPlatformYDistance = 5;
    private float imporantAreaBufferDistance = 10;
    private float spaceshipSpawnDistanceY = 5;

    private float groundMinLength = 10;
    private float groundMaxLength = 70;
    private float groundMinHeight = -27;
    private float groundMaxHeight = -8;

    private float coneMinHeight = 5;
    private float coneMaxHeight = 28;
    private float coneMinLength = 8;
    private float coneMaxLength = 27;
    private float coneMinDistance = 1;
    private float coneMaxDistance = 20;
    private float coneMinYPosBelowGround = 4;
    private float coneMaxYPosBelowGround = 12;
    private float coneMaxXRotation = 10;
    private float coneMaxZRotation = 40;

    private float asteroidLargeMinSize = 10;
    private float asteroidLargeMaxSize = 30;
    private float asteroidSmallMinSize = 2;
    private float asteroidSmallMaxSize = 8;
    private float asteroidMinDistance = 2;
    private float asteroidMaxDistance = 7;
    private float asteroidZVariance = 3;
    private int asteroidMaxCluster = 5;
    private float asteroidMinClusterDistance = 9;
    private float asteroidMaxClusterDistance = 40;
    private float asteroidSmallMinYPos = 7;
    private float asteroidLargeMinYPos = 10;
    private float asteroidMaxYPos = 40;

    private float prismMinLength = 5;
    private float prismMaxLength = 60;

    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject conePrefab;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject prismPrefab;
    [SerializeField] private GameObject spaceshipPrefab;
    [SerializeField] private GameObject startingPlatformPrefab;
    [SerializeField] private GameObject goalPrefab;

    private GameObject startingPlatform;
    private GameObject spaceship;
    private GameObject goal;

    private List<GroundHeightData> groundHeights;
    private int groundHeightsIndex;

    // Start is called before the first frame update
    void Start()
    {
        LoadMap();
    }

    void Instantiate()
    {

    }

    void LoadMap()
    {
        groundHeights = new List<GroundHeightData>();
        groundHeightsIndex = 0;
        LoadGround();
        LoadCones();
        LoadAsteroids();

        LoadStartingPlatformAndSpaceship();
        LoadGoal();
    }

    void LoadGround()
    {
        float previousGroundYPos = 0;
        float previousGroundLength = 20;
        float edgeOfLastGroundX = startingPlatformXPos -generationBufferXPos;
        while (edgeOfLastGroundX < goalDistance + generationBufferXPos)
        {
            GameObject ground = Instantiate(groundPrefab);

            //puts new groudn block right after the previous one
            float groundLength = Random.Range(groundMinLength, groundMaxLength);
            float groundYPos = Random.Range(groundMinHeight, groundMaxHeight);
            ground.transform.localScale = new Vector3(groundLength, ground.transform.localScale.y, ground.transform.localScale.z);
            ground.transform.localPosition = new Vector3(edgeOfLastGroundX + (groundLength / 2), groundYPos, 0);

            //create prism, to smoothen the edges between ground platforms
            GameObject prism = Instantiate(prismPrefab);

            float prismLength = Random.Range(prismMinLength, prismMaxLength);
            //make it so prism isnt longer than ground blocks
            if (prismLength > previousGroundLength * 2)
            {
                prismLength = previousGroundLength * 2;
            }
            if ( prismLength > groundLength * 2)
            {
                prismLength = groundLength * 2;
            }

            //puts prism exactly between ground blocks
            float prismHeight = System.Math.Max(groundYPos - previousGroundYPos, previousGroundYPos - groundYPos);
            float prismXPos = edgeOfLastGroundX;
            float prismYPos = System.Math.Min(groundYPos, previousGroundYPos);
            prismYPos += ground.transform.localScale.y / 2;
            prism.transform.localScale = new Vector3(prismLength, prismHeight, prism.transform.localScale.z);
            prism.transform.localPosition = new Vector3(prismXPos, prismYPos, 0);

            edgeOfLastGroundX += groundLength;
            previousGroundLength = groundLength;
            previousGroundYPos = ground.transform.localPosition.y;
            groundHeights.Add(new GroundHeightData(ground.transform.position.x - groundLength / 2, groundYPos + ground.transform.localScale.y / 2));
        }
    }

    void LoadCones()
    {
        float previousConeXPos = startingPlatformXPos -generationBufferXPos;
        groundHeightsIndex = 0;
        while (previousConeXPos < goalDistance + generationBufferXPos)
        {
            float coneDistance = Random.Range(coneMinDistance, coneMaxDistance);
            float coneXPos = previousConeXPos + coneDistance;

            if (IsWithinImportantAreaBufferRange(coneXPos))
            {

            }
            else
            {
                //puts it slightly below the ground
                float coneYPos = GetGroundHeightInXPos(coneXPos) - Random.Range(coneMinYPosBelowGround, coneMaxYPosBelowGround);
                float coneLength = Random.Range(coneMinLength, coneMaxLength);
                float coneHeight = Random.Range(coneMinHeight, coneMaxHeight);

                float coneXRotation = Random.Range(-coneMaxXRotation, coneMaxXRotation);
                float coneZRotation = Random.Range(-coneMaxZRotation, coneMaxZRotation);

                GameObject cone = Instantiate(conePrefab);
                cone.transform.localScale = new Vector3(coneLength, coneHeight, cone.transform.localScale.z);
                cone.transform.localPosition = new Vector3(coneXPos, coneYPos, 0);
                cone.transform.Rotate(new Vector3(coneXRotation, 0, coneZRotation));
            }

            previousConeXPos += coneDistance;
        }
    }

    void LoadAsteroids()
    {
        float previousAsteroidXPos = startingPlatformXPos -generationBufferXPos;
        groundHeightsIndex = 0;
        while (previousAsteroidXPos < goalDistance + generationBufferXPos)
        {
            int clusterAmount = Random.Range(1, asteroidMaxCluster + 1);
            previousAsteroidXPos = previousAsteroidXPos + Random.Range(asteroidMinClusterDistance, asteroidMaxClusterDistance);

            float minSize = asteroidSmallMinSize;
            float maxSize = asteroidSmallMaxSize;
            float minYPos = asteroidSmallMinYPos;

            if (clusterAmount == 1)
            {
                minSize = asteroidLargeMinSize;
                maxSize = asteroidLargeMaxSize;
                minYPos = asteroidLargeMinYPos;
            }
            //for each asteroid in the cluster
            for (int i = 0; i < clusterAmount; i++)
            {
                float asteroidDistance = Random.Range(asteroidMinDistance, asteroidMaxDistance);
                float asteroidXPos = previousAsteroidXPos + asteroidDistance;

                if (IsWithinImportantAreaBufferRange(asteroidXPos))
                {

                }
                else
                {
                    float asteroidSize = Random.Range(minSize, maxSize);

                    //makes sure it doesnt clip into the ground
                    float asteroidMinYPos = System.Math.Max(minYPos, asteroidSize / 2);
                    float asteroidDistanceAboveGround = Random.Range(asteroidMinYPos, asteroidMaxYPos);
                    float asteroidYPos = GetGroundHeightInXPos(asteroidXPos) + asteroidDistanceAboveGround;

                    float asteroidZPos = Random.Range(-asteroidZVariance, asteroidZVariance);

                    GameObject asteroid = Instantiate(asteroidPrefab);
                    asteroid.transform.localScale = new Vector3(asteroidSize, asteroidSize, asteroidSize);
                    asteroid.transform.localPosition = new Vector3(asteroidXPos, asteroidYPos, asteroidZPos);
                }

                previousAsteroidXPos += asteroidDistance;
            }
        }
    }

    void LoadStartingPlatformAndSpaceship()
    {
        groundHeightsIndex = 0;
        float startingPlatformYPos = GetGroundHeightInXPos(startingPlatformXPos) + startingPlatformYDistance;

        startingPlatform = Instantiate(startingPlatformPrefab);
        startingPlatform.transform.position = new Vector3(startingPlatformXPos, startingPlatformYPos, 0);

        spaceship = Instantiate(spaceshipPrefab);
        spaceship.transform.position = new Vector3(startingPlatformXPos, startingPlatformYPos + spaceshipSpawnDistanceY, 0);
    }

    void LoadGoal()
    {
        float goalYPos = GetGroundHeightInXPos(startingPlatformXPos + goalDistance);
        goal = Instantiate(goalPrefab);
        goal.transform.position = new Vector3(startingPlatformXPos + goalDistance, goalYPos, 0);
    }

    float GetGroundHeightInXPos(float xPos)
    {
        for (int i = groundHeightsIndex; i < groundHeights.Count; i++)
        {
            groundHeightsIndex = i;
            if (xPos < groundHeights[i].PosX)
            {
                return groundHeights[i - 1].Height;
            }
        }
        return groundHeights[groundHeights.Count - 1].Height;
    }

    //checks if something is too close to starting platform or the goal
    bool IsWithinImportantAreaBufferRange(float xPos)
    {
        if (xPos > startingPlatformXPos - imporantAreaBufferDistance && xPos < startingPlatformXPos + imporantAreaBufferDistance)
        {
            return true;
        }

        if (xPos > startingPlatformXPos + goalDistance - imporantAreaBufferDistance && xPos < startingPlatformXPos + goalDistance + imporantAreaBufferDistance)
        {
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
