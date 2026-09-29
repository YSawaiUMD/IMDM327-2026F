// IMDM327 Material
// Use CSV or JSON to load data into the simulation. Both formats are supported, but they use different data types. 
// The CSV format uses a struct, while the JSON format uses a class. This script demonstrates how to load both formats and access their data.
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
public class SolarSystem : MonoBehaviour
{
    // These components can be attached independently.
    DataCSV solarCSV;
    DataJSON solarJSON;
    const float G = 6.674e-11f; // Gravitational constant
    PlanetProperty[] planetProperties;
    private int numberOfSphere = 10;
    public float fasterTime = 10000000f;
    class PlanetProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject planet;
        public float mass;
        public float radius;
        public Vector3 velocity;
        public Vector3 acceleration;
        public Vector3 actualPosition;
    }

    // CSV data and JSON data use their own data types.
    public BodyProperty[] solarBodiesCSV;
    // public SolarBody[] solarBodiesJSON;

    // Both loader scripts finish reading their files in Awake().
    void Start()
    {
        // CSV: use this block when a DataCSV component is attached.
        solarCSV = GetComponent<DataCSV>();

        if (solarCSV != null)
        {
            solarBodiesCSV = solarCSV.bp;
            Debug.Log("Loaded " + solarBodiesCSV.Length + " bodies from solar.csv.");
            Debug.Log("First body: mass = " + solarBodiesCSV[0].mass + ", distance = " + solarBodiesCSV[0].distance + ", initial_velocity = " + solarBodiesCSV[0].initial_velocity);
        }

        // JSON: use this block when a DataJSON component is attached.
        // solarJSON = GetComponent<DataJSON>();
        // if (solarJSON != null)
        // {
        //     solarBodiesJSON = solarJSON.solarData.bodies;
        //     Debug.Log("Loaded " + solarBodiesJSON.Length + " bodies from solar.json.");
        //     Debug.Log("First body: " + solarBodiesJSON[0].name + ", mass: " + solarBodiesJSON[0].mass);
        // }

        // GameObject array to hold the planets in the simulation.
        planetProperties = new PlanetProperty[numberOfSphere];
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            planetProperties[i] = new PlanetProperty();
            planetProperties[i].planet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            if (i == 0)
            {
                planetProperties[i].planet.GetComponent<Renderer>().material.color = Color.orange;
                planetProperties[i].planet.transform.localScale = Vector3.one * 10f;
            }
            else
            {
                planetProperties[i].planet.transform.localScale = Vector3.one * 4f;
            }
            TrailRenderer trailRenderer = planetProperties[i].planet.AddComponent<TrailRenderer>();
            // Configure the TrailRenderer's properties
            trailRenderer.time = 100.0f;  // Duration of the trail
            trailRenderer.startWidth = 0.5f;  // Width of the trail at the start
            trailRenderer.endWidth = 0.1f;    // Width of the trail at the end
            // a material to the trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
            // Set the colour gradient along the trail.
            Gradient gradient = new Gradient();
            Color targetColor = Color.HSVToRGB((float)i / numberOfSphere, 1f, 1f);
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(Color.white, 0f), // (color, normalized position)
                    new GradientColorKey(targetColor, 0.8f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f, 0f), // (alpha, normalized position) 
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;
            planetProperties[i].planet.GetComponent<Renderer>().material.color = targetColor;
        }

        // Apply the loaded data to the simulation. This is where you would set up your bodies in the scene based on the loaded data.
        for (int i = 0; i < solarBodiesCSV.Length; i++)
        {
            planetProperties[i].mass = solarBodiesCSV[i].mass;
            planetProperties[i].radius = solarBodiesCSV[i].radius;

            // What is missing here? You need to set the initial position and velocity of each planet based on the loaded data.
            // ***WRITE YOUR CODE HERE***
            float theta = 2 * Mathf.PI * UnityEngine.Random.Range(0f, 1f);
            planetProperties[i].actualPosition = new Vector3(solarBodiesCSV[i].distance * Mathf.Cos(theta), solarBodiesCSV[i].distance * Mathf.Sin(theta), 0);
            planetProperties[i].velocity = new Vector3(-Mathf.Sin(theta) * solarBodiesCSV[i].initial_velocity, Mathf.Cos(theta) * solarBodiesCSV[i].initial_velocity, 0);

        }
    }
    void FixedUpdate()
    {
        // Loop for N-body gravity
        // How should we design the loop?

        // 00. Initialize the acceleration for each body to zero at the start of each frame
        for (int i = 0; i < numberOfSphere; i++)
        {
            planetProperties[i].acceleration = Vector3.zero;
        }
        // 01. Loop through each body to calculate the gravitational forces acting on it
        for (int i = 0; i < numberOfSphere; i++)
        {
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                Vector3 distanceVector = planetProperties[j].actualPosition - planetProperties[i].actualPosition;
                Vector3 accelerationI = CalculateGravity(distanceVector, planetProperties[j].mass);
                Vector3 accelerationJ = CalculateGravity(-distanceVector, planetProperties[i].mass);
                planetProperties[i].acceleration += accelerationI;
                planetProperties[j].acceleration += accelerationJ;
            }
        }
        // 02. Loop through each body to update its velocity and position based on the calculated acceleration
        for (int i = 0; i < numberOfSphere; i++)
        {
            planetProperties[i].velocity += planetProperties[i].acceleration * Time.deltaTime * fasterTime;
            planetProperties[i].actualPosition += planetProperties[i].velocity * Time.deltaTime * fasterTime;
            float scaledDistance = Mathf.Sqrt(planetProperties[i].actualPosition.magnitude / 1e8f);
            Vector3 direction = planetProperties[i].actualPosition.normalized;
            planetProperties[i].planet.transform.position = direction * scaledDistance;
        }
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float otherMass)
    {
        Vector3 gravity = Vector3.zero;
        gravity = G * otherMass / distanceVector.sqrMagnitude * distanceVector.normalized;
        return gravity;
    }
}
