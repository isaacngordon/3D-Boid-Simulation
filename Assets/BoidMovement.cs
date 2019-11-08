using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    public float perception;
    public float forwardForce;
    public float minSpeed;
    public float maxSpeed;
    public Rigidbody rb;

    private Vector3 v, a;

    // Start is called before the first frame update
    void Start()
    {
        if (minSpeed < 0 || maxSpeed < minSpeed) throw new System.Exception("Clamp speeds are illegal values." +
            "");
        //TODO: set initial velocity
        rb.AddRelativeForce(new Vector3(forwardForce, forwardForce, forwardForce), ForceMode.VelocityChange);
        v = Vector3.zero;
        a = transform.forward * forwardForce;
        FaceForward();

    }

    // Update is called once per frame
    void Update()
    {
        v += a;
        float mag = v.magnitude;
        Vector3 dir = v / mag;
        mag = Mathf.Clamp(mag, minSpeed, maxSpeed);
        v = dir * mag;
        rb.velocity = v;
        a *= 0;
        FaceForward();

    }

    public void Move(ArrayList neighbors, Bounds bounds)
    {
        
        //flocking vectors
        Vector3 seperation = new Vector3(0, 0, 0);
        Vector3 alignment = new Vector3(0, 0, 0);
        Vector3 cohesion = new Vector3(0, 0, 0);

        //flocking math
        foreach (GameObject n in neighbors)
        {
            //add seperation math
            Vector3 diff = transform.position - n.transform.position;
            seperation += diff / Mathf.Pow(Vector3.Distance(transform.position, n.transform.position), 2);

            //alignment math
            alignment += n.GetComponent<Rigidbody>().velocity; 

            //add cohesion math
            cohesion += n.GetComponent<Transform>().position;
        }

        //normalize velocity vector components
        if (neighbors.Count > 0)
        {
            alignment /= neighbors.Count;
            cohesion /= neighbors.Count;
            seperation /= neighbors.Count;
        }

        cohesion -= transform.position;

        //combine for final velocity
        Vector3 sum = new Vector3(0, 0, 0);
        sum += Steering(alignment, v);

        sum += Steering(cohesion, v);

        sum += Steering(seperation, v);

        a = sum;

        //adjust boid velocity and position
        //rb.velocity += sum;
        //rb.AddRelativeForce(sum, ForceMode.Acceleration);
        KeepInside(bounds);
        Debug.Log(sum); 
    }

    private Vector3 Steering(Vector3 desired, Vector3 actual)
    {
        return desired - actual;
    }

    public void KeepInside(Bounds bounds)
    {
        Vector3 boidPos = transform.position;

        if (boidPos.x > (bounds.center + bounds.extents).x)
        {
            boidPos.x = (bounds.center - bounds.extents).x;
        }
        else if (boidPos.x < (bounds.center - bounds.extents).x)
        {
            boidPos.x = (bounds.center + bounds.extents).x;
        }

        if (boidPos.z > (bounds.center + bounds.extents).z)
        {
            boidPos.z = (bounds.center - bounds.extents).z;
        }
        else if (boidPos.z < (bounds.center - bounds.extents).z)
        {
            boidPos.z = (bounds.center + bounds.extents).z;
        }

        if (boidPos.y > 500)
        {
            boidPos.y = 0;
        }
        else if (boidPos.y < 0)
        {
            boidPos.y = 500;
        }

        transform.position = new Vector3(boidPos.x, boidPos.y, boidPos.z);
    }

    void FaceForward()
    {
        transform.up = rb.velocity;

    }
    

    private void OnDrawGizmosSelected()
    {
        Bounds b = gameObject.GetComponent<Renderer>().bounds;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(b.center, new Vector3(perception, perception, perception));
    }


    //---------------------------------------------------------------------
    float maxMag;
    float minMag;
    float mag;
    Vector3 pos;
    Vector3 velocity;
    Vector3 accelaration;
    Vector3 dir;
    Transform t;
    Rigidbody rigid;


    private void Upp()
    {

        Vector3 seperation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 cohesion = Vector3.zero;



        //update velocity
        velocity += accelaration * Time.deltaTime;
        float speed = velocity.magnitude;
        dir = velocity / speed;
        mag = Mathf.Clamp(speed, minMag, maxMag);
        velocity = dir * mag;

        //update position and facing direction
        t.position += velocity * Time.deltaTime;
        t.forward = dir;
        pos = t.position;

    }
}
