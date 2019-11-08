using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public int flockSize;
    public GameObject boidPrefab;
    public GameObject terrain;

    Bounds terrainBounds;
    GameObject[] flock;

    // instatiate all boids on the terrain
    void Start()
    {
        terrainBounds = terrain.GetComponent<Terrain>().terrainData.bounds;

        if (flockSize < 1) throw new System.Exception("Flock size must be greater than 0.");

        //instatiate flock for n boids
        flock = new GameObject[flockSize];

        //fill flock with n boids
        for(int i = 0; i < flockSize; i++)
        {
            Vector3 rLocation = RandomPositionOnTerrain();
            Quaternion rAngle = RandomQuaternionRotation();
            flock[i] = Instantiate(boidPrefab, rLocation, rAngle);
        } 
    }

    // Update is called once per frame
    void Update()
    {
        SortFlock();
        foreach(GameObject boid in flock)
        {
            ArrayList neighbors = GetNeighbors(boid);
            boid.GetComponent<BoidMovement>().Move(neighbors, terrainBounds);

            //TODO: Refactor for raycasting instead
            //Keep boid inside of the terrain
            
        }
    }


    ArrayList GetNeighbors(GameObject boid)
    {
        ArrayList neighbors = new ArrayList();

        //TODO: refactor to get neighbors from flock in a more effecent manner if possible, as well as fix perception with ray casting

        //get neighborhood bounds
        float d = boid.GetComponent<BoidMovement>().perception;
        Vector3 size = new Vector3(d, d, d);
        Bounds neighborhood = new Bounds(boid.GetComponent<Transform>().position, size);

        //for each boid in flock, see if that boid exists in the neighborhood
        foreach(GameObject dude in flock)
        {
            Vector3 dudePosition = dude.GetComponent<Transform>().position;
            if (dude != boid && neighborhood.Contains(dudePosition))
            {
                neighbors.Add(dude);
            }
        }

        return neighbors;
    }

    Vector3 RandomPositionOnTerrain()
    {
        //generate random positions on x,y,z axis and build random 
        float randPos_x = Random.Range(terrainBounds.center.x - terrainBounds.extents.x, terrainBounds.center.x + terrainBounds.extents.x);
        float randPos_y = Random.Range(0, 500);
        float randPos_z = Random.Range(terrainBounds.center.z - terrainBounds.extents.z, terrainBounds.center.z + terrainBounds.extents.z);

        return new Vector3(randPos_x, randPos_y, randPos_z);
    }

    Quaternion RandomQuaternionRotation()
    {
        //generate random angles from x,y,z axis
        float random_angle_x = Random.Range(0f, 360f);
        float random_angle_y = Random.Range(0f, 360f);
        float random_angle_z = Random.Range(0f, 360f);

        return Quaternion.Euler(random_angle_x, random_angle_y, random_angle_z);
    }

    /**
     * Sorts slock by distance; essentially partioning it
     * 
     */
    void SortFlock()
    {
        //TODO: arrange the flock to optimize neighbor search
        /*
         * Maybe cast rays across field of vision for neighbors and obstacles,
         * use tags to tell if boid or not boid, and use flocking algorithm on boids and
         * just avoidance of obstacles
         */
    }
}
