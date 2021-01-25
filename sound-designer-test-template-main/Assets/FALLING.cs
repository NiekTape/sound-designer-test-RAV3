using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class FALLING : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string FALLING_EVENT;
    FMOD.Studio.EventInstance FALLING_INST;

    [FMODUnity.EventRef]
    public string FALLING_LAND_EVENT;
    FMOD.Studio.EventInstance FALLING_LAND_INST;

    private float updateRate = 15f; 
    private float updateStep;
    private float lastUpdateTime;

    private float JUMP_TIME;
    float A;
    float B;
    int S;
    float speed;
    float max_speed=0;
    bool FALL;
    int LAND;
    Vector3 NORM_POSITION;
    public float delta_position = 0.25f;

    private ExampleCharacterController CHARACTER_CONTROLLER;

   
    void Start()
    {
        CHARACTER_CONTROLLER = gameObject.GetComponent<ExampleCharacterController>();

        A = gameObject.transform.position.y;

    }

  



    private void FixedUpdate()
    {

        if (Time.realtimeSinceStartup - lastUpdateTime > updateStep) 
        {
        lastUpdateTime = Time.realtimeSinceStartup;
        }


        



        // void Update()
        //  {

        B = gameObject.transform.position.y;
        speed = (A - B);
        
        

      





        JUMP_TIME = CHARACTER_CONTROLLER._timeSinceLastAbleToJump;

        if (JUMP_TIME > 0 && FALL != true)
        {
            
            FALLING_INST = FMODUnity.RuntimeManager.CreateInstance(FALLING_EVENT);
            FALLING_INST.start();
            FALL = true;
            if (speed > max_speed)
            {
                max_speed = speed;
            }
            if (LAND == 0)
                LAND = 1;

            NORM_POSITION = gameObject.transform.position;
            NORM_POSITION.y = NORM_POSITION.y + delta_position;
            Physics.Raycast(NORM_POSITION, Vector3.down, out RaycastHit hit, 0.5f);
            if (hit.collider.tag == "concrete")
                S = 0;
            else
                S = 1;


        }

        if (FALL == true)
            FALLING_INST.setParameterByName("JUMP_TIME", speed);

        if (JUMP_TIME == 0)
        {
            FALLING_INST.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            FALL = false;
            if (LAND == 1) //приземление
            {
                FALLING_LAND_INST = FMODUnity.RuntimeManager.CreateInstance(FALLING_LAND_EVENT);
                FALLING_LAND_INST.setParameterByName("power_falling",max_speed*100);
                FALLING_LAND_INST.start();
               
                
                LAND = 0;
                max_speed = 0;
                FALLING_LAND_INST.release();
                FALLING_INST.release();



            }

        }


        A = B;




    }
}

