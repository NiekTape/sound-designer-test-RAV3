using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class FTSP_SCRIPT : MonoBehaviour
{
    
    [FMODUnity.EventRef]
    public string FTSP_EVENT;
    FMOD.Studio.EventInstance FTSP_INST;

    [FMODUnity.EventRef]
    public string FTSP_STOP_EVENT;
    FMOD.Studio.EventInstance FTSP_STOP_INST;

    [FMODUnity.EventRef]
    public string JUMP_EVENT;
    FMOD.Studio.EventInstance JUMP_INST;

    [FMODUnity.EventRef]
    public string JUMP__STOP_EVENT;
    FMOD.Studio.EventInstance JUMP_STOP_INST;

    private float MOVE;
    private bool JUMP;
    private float JUMP_TIME;
    private ExampleCharacterController EXCHACONT;
    int I; // в движении
    int J; // в прыжке
    int S; // surface
    int START_JUMP_IN_MOVING; //прыжок начался в движении
    Vector3 NORM_POSITION;
    public float delta_position = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        EXCHACONT = gameObject.GetComponent<ExampleCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        NORM_POSITION = gameObject.transform.position;
        NORM_POSITION.y = NORM_POSITION.y + delta_position;
        Physics.Raycast(NORM_POSITION, Vector3.down, out RaycastHit hit, 0.5f);
        if (hit.collider.tag == "concrete")
        {
            S = 0;
        }
        else
        {
            S = 1;
        }




            MOVE = EXCHACONT._moveInputVector.x;
        JUMP = EXCHACONT._jumpConsumed;
        JUMP_TIME = EXCHACONT._timeSinceLastAbleToJump;
        if (MOVE != 0 && I == 0 && JUMP == false)   //движение
        {
            
            
            FTSP_INST = FMODUnity.RuntimeManager.CreateInstance(FTSP_EVENT);
            FTSP_INST.setParameterByName("SURFACE", S);
            FTSP_INST.start();
            
            I = 1;
           

        }
    
        if (MOVE == 0 && I != 0 && JUMP == false) //остановка после движения
        {

            FTSP_INST.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); //выключить шаги   
            FTSP_INST.release();
            I = 0;
            
            

            FTSP_STOP_INST = FMODUnity.RuntimeManager.CreateInstance(FTSP_STOP_EVENT);
            FTSP_STOP_INST.setParameterByName("jump_in_move", 1);
            FTSP_STOP_INST.setParameterByName("SURFACE", S);
            FTSP_STOP_INST.start();
            FTSP_STOP_INST.release();
        }

        if (JUMP == true && JUMP_TIME !=0)   //если прыжок
        {

            FTSP_INST.stop(FMOD.Studio.STOP_MODE.IMMEDIATE); //выключить шаги
            FTSP_INST.release();
            I = 0;
            
            if (J == 0) //начало прыжка
            {
                if (MOVE != 0) //прыжок в движении
                {
                    START_JUMP_IN_MOVING = 1;

                }
                if (MOVE == 0)  //или прыжок с места
                {
                    START_JUMP_IN_MOVING = 0;
                }

                JUMP_INST = FMODUnity.RuntimeManager.CreateInstance(JUMP_EVENT);
                JUMP_INST.setParameterByName("SURFACE", S);
                JUMP_INST.start();
                J = 1;
            }
        }

        if (JUMP != true && J == 1)     //приземление
            {
            JUMP_INST.release();
                JUMP_STOP_INST = FMODUnity.RuntimeManager.CreateInstance(JUMP__STOP_EVENT);
                JUMP_STOP_INST.setParameterByName("jump_in_move", START_JUMP_IN_MOVING);
            JUMP_STOP_INST.setParameterByName("SURFACE", S);
            JUMP_STOP_INST.start();
                J = 2;

            }
         

        
        if (JUMP != true && J == 2)     //прыжок закончен
        {
            
            J = 0;

            JUMP_STOP_INST.release();
        }


    }
    
}
