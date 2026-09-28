// 3-body Starter Code
// Fall 2026. IMDM 327
// Instructor. Myungin Lee
using UnityEngine;

public class ThreeBody : MonoBehaviour
{
    private const float G = 500f; // Gravitational constant for this simulation, not the real-world value.
    BodyProperty[] bp;
    private int numberOfSphere = 100;
    private float sphereRadius = 3f;
    public float maxSpeed = 150f;
    class BodyProperty // why struct?
    {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
        public GameObject body;
        public float mass;
        public Vector3 velocity;
        public Vector3 acceleration;
    }


    void Start()
    {
        // Allocate an array to store each body's properties.
        bp = new BodyProperty[numberOfSphere];
        // Loop generating the gameobject and assign initial conditions (type, position, (mass/velocity/acceleration)
        for (int i = 0; i < numberOfSphere; i++)
        {
            // Our gameobjects are created here:
            bp[i] = new BodyProperty();
            bp[i].body = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
            // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html
            bp[i].body.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
            // initial conditions
            float r = 325f;
            float speed = 10f;
            bp[i].mass = 5.5f;
            // position is (x,y,z). In this case, I want to plot them on the circle with r
            float theta = 2 * Mathf.PI * UnityEngine.Random.Range(0f, 1f);
            bp[i].body.transform.position = new Vector3(UnityEngine.Random.Range(0f, r) * Mathf.Cos(theta), UnityEngine.Random.Range(0f, r) * Mathf.Sin(theta), 0);
            bp[i].velocity = new Vector3(-Mathf.Sin(theta) * speed, Mathf.Cos(theta) * speed, 0);
            // ******** Fill in this part ********
            // z = 180 places the bodies in front of a camera near the origin looking along +Z. Try other positions too.


            // + This is just pretty trails
            TrailRenderer trailRenderer = bp[i].body.AddComponent<TrailRenderer>();
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

        }
    }

    void FixedUpdate()
    {
        // Loop for N-body gravity
        // How should we design the loop?
        for (int i = 0; i < numberOfSphere; i++)
        {
            bp[i].acceleration = Vector3.zero;
        }
        for (int i = 0; i < numberOfSphere; i++)
        {
            for (int j = i + 1; j < numberOfSphere; j++)
            {
                Vector3 distanceVector = bp[j].body.transform.position - bp[i].body.transform.position;
                Vector3 gravity = CalculateGravity(distanceVector, bp[i].mass, bp[j].mass);
                bp[i].acceleration += gravity;
                bp[j].acceleration -= gravity;
            }
        }
        for (int i = 0; i < numberOfSphere; i++)
        {
            bp[i].velocity += bp[i].acceleration * Time.deltaTime;
            bp[i].velocity = Vector3.ClampMagnitude(bp[i].velocity, maxSpeed);

            bp[i].body.transform.position += bp[i].velocity * Time.deltaTime;
        }
    }

    // Gravity Fuction to finish
    private Vector3 CalculateGravity(Vector3 distanceVector, float m1, float m2)
    {
        Vector3 gravity = Vector3.zero; // note this is also Vector3
                                        // **** Fill in the function below. 
                                        // gravity = ****;
        gravity = G * m1 * m2 * distanceVector.normalized / (distanceVector.sqrMagnitude);
        return gravity;
    }
}

