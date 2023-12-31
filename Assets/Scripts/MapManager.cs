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
    private float goalYDistance = 4;
    private float generationBufferXPos = 50;

    private float poisonHeight = 60;
    private Vector2 approachingPoisonStart = new Vector2(-60, 10);
    private float[] poisionSpeed = { 2, 4, 8 };

    private float startingPlatformXPos = 0;
    private float startingPlatformYDistance = 9;
    private float imporantAreaBufferDistance = 12;
    private float spaceshipSpawnDistanceY = 2;

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
    private float asteroidLargeMaxSize = 28;
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
    private float asteroidMaxYPos = 54;

    private float prismMinLength = 5;
    private float prismMaxLength = 60;

    [SerializeField] private GameObject mapPrefab;

    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject conePrefab;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject prismPrefab;
    [SerializeField] private Spaceship spaceshipPrefab;
    [SerializeField] private GameObject startingPlatformPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private ApproachingPoison approachingPoisonPrefab;
    [SerializeField] private CeilingPoison ceilingPoisonPrefab;

    private GameObject startingPlatform;
    private GameObject goal;
    private ApproachingPoison approachingPoison;
    private CeilingPoison ceilingPoison;

    Transform map;
    Transform grounds;
    Transform flatGrounds;
    Transform prisms;
    Transform cones;
    Transform asteroids;

    private List<GroundHeightData> groundHeights;
    private int groundHeightsIndex;

    

    public static MapManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            groundHeights = new List<GroundHeightData>();
            return;
        }
        Destroy(gameObject);
    }

    public void StartGame()
    {
        GameManager.Instance.spaceship = Instantiate(spaceshipPrefab);

        map = Instantiate(mapPrefab).transform;
        grounds = map.GetChild(0);
        flatGrounds = grounds.GetChild(0);
        prisms = grounds.GetChild(1);
        cones = map.GetChild(1);
        asteroids = map.GetChild(2);

        startingPlatform = Instantiate(startingPlatformPrefab, map.transform);
        goal = Instantiate(goalPrefab, map.transform);
        approachingPoison = Instantiate(approachingPoisonPrefab, map.transform);
        ceilingPoison = Instantiate(ceilingPoisonPrefab, map.transform);
        ceilingPoison.yPos = poisonHeight;

        LoadMap();
    }

    public void Reset()
    {
        DestroyMap();
        LoadMap();
    }

    void LoadMap()
    {
        Debug.Log("Loading map");
        groundHeights.Clear();
        groundHeightsIndex = 0;
        LoadGround();
        LoadCones();
        LoadAsteroids();
        LoadPoison();

        LoadStartingPlatformAndSpaceship();
        LoadGoal();
    }

    void DestroyMap()
    {
        //destroys all children in relevant children of Map gameobject
        for (int i = flatGrounds.childCount - 1; i >= 0; i--)
        {
            Destroy(flatGrounds.GetChild(i).gameObject);
        }
        for (int i = prisms.childCount - 1; i >= 0; i--)
        {
            Destroy(prisms.GetChild(i).gameObject);
        }
        for (int i = cones.childCount - 1; i >= 0; i--)
        {
            Destroy(cones.GetChild(i).gameObject);
        }
        for (int i = asteroids.childCount - 1; i >= 0; i--)
        {
            Destroy(asteroids.GetChild(i).gameObject);
        }
    }

    void LoadGround()
    {
        float previousGroundYPos = 0;
        float previousGroundLength = 20;
        float edgeOfLastGroundX = startingPlatformXPos -generationBufferXPos;
        while (edgeOfLastGroundX < goalDistance + generationBufferXPos)
        {
            GameObject ground = Instantiate(groundPrefab, flatGrounds);

            //puts new ground block right after the previous one
            float groundLength = Random.Range(groundMinLength, groundMaxLength);
            float groundYPos = Random.Range(groundMinHeight, groundMaxHeight);
            ground.transform.localScale = new Vector3(groundLength, ground.transform.localScale.y, ground.transform.localScale.z);
            ground.transform.localPosition = new Vector3(edgeOfLastGroundX + (groundLength / 2), groundYPos, 0);

            //create prism, to smoothen the edges between ground platforms
            GameObject prism = Instantiate(prismPrefab, prisms);

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

                GameObject cone = Instantiate(conePrefab, cones);
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

                    GameObject asteroid = Instantiate(asteroidPrefab, asteroids);
                    asteroid.transform.localScale = new Vector3(asteroidSize, asteroidSize, asteroidSize);
                    asteroid.transform.localPosition = new Vector3(asteroidXPos, asteroidYPos, asteroidZPos);
                }

                previousAsteroidXPos += asteroidDistance;
            }
        }
    }

    void LoadPoison()
    {
        approachingPoison.SetValues(new Vector3(approachingPoisonStart.x, approachingPoisonStart.y, 0), poisionSpeed[GameManager.Instance.Level - 1]);
    }

    void LoadStartingPlatformAndSpaceship()
    {
        groundHeightsIndex = 0;
        float startingPlatformYPos = GetGroundHeightInXPos(startingPlatformXPos) + startingPlatformYDistance;

        startingPlatform.transform.position = new Vector3(startingPlatformXPos, startingPlatformYPos, 0);

        GameManager.Instance.spaceship.transform.position = new Vector3(startingPlatformXPos, startingPlatformYPos + spaceshipSpawnDistanceY, 0);
        //GameManager.Instance.spaceship.ResetConditions();
    }

    void LoadGoal()
    {
        float goalYPos = GetGroundHeightInXPos(startingPlatformXPos + goalDistance) + goalYDistance;

        goal.transform.position = new Vector3(startingPlatformXPos + goalDistance, goalYPos, 0);
    }

    //gets the Y pos of the ground at a certain X pos
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
}
