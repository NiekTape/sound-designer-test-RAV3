using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class occlusion_sound : MonoBehaviour
{

    [FMODUnity.EventRef]
    public string ROOM_EVENT;
    FMOD.Studio.EventInstance ROOM_INST;
 

    public GameObject PlayerObject; 
    public LayerMask OcclusionLayer; 
    public float SoundWidth; 
    public float ListenerWidth; 
    public Vector3 SoundHeight;
    public Vector3 PlayerHeight; 

    private float lineCastHitCount = 0f; 
    
    private float updateRate = 10f;
    private float updateStep;
    private float lastUpdateTime;

    void Start()
    {
        updateStep = 1f / updateRate;
        LastUpdateTimeReset();

        ROOM_INST = FMODUnity.RuntimeManager.CreateInstance(ROOM_EVENT);
        ROOM_INST.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        ROOM_INST.start();
    }
    private void FixedUpdate()
    {
        
        if(Time.realtimeSinceStartup - lastUpdateTime > updateStep) 
        {
            CheckOcclusion(transform.position + SoundHeight, PlayerObject.transform.position + PlayerHeight); 
            lineCastHitCount = 0f;
            LastUpdateTimeReset();
        }
        
        
    }

    void LastUpdateTimeReset()
    {
        lastUpdateTime = Time.realtimeSinceStartup;
    }

    void CheckOcclusion(Vector3 sound, Vector3 player) 
    {
        Vector3 SoundLeft = CalculatePoint(sound, player, SoundWidth); 
        Vector3 SoundRight = CalculatePoint(sound, player, -SoundWidth);

        Vector3 PlayerLeft = CalculatePoint(player, sound , ListenerWidth);
        Vector3 PlayerRight = CalculatePoint(player, sound, -ListenerWidth);


        CastLine(SoundLeft, PlayerLeft); 
        CastLine(SoundLeft, player);
        CastLine(SoundLeft, PlayerRight);

        CastLine(sound, PlayerLeft);
        CastLine(sound, player);
        CastLine(sound, PlayerRight);

        CastLine(SoundRight, PlayerLeft);
        CastLine(SoundRight, player);
        CastLine(SoundRight, PlayerRight);

        SetParameter(); 

    }
    
    private Vector3 CalculatePoint(Vector3 a, Vector3 b, float m) 
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(a.x, 0f, a.z), new Vector3(b.x, 0f, b.z));
        float mn = (m / n);
        
            x = a.x + (mn * (a.z - b.z));
            z = a.z - (mn * (a.x - b.x));
       
        return new Vector3(x, a.y, z);
    }

    private void CastLine(Vector3 Start, Vector3 End)
    {
        RaycastHit hit;
        Physics.Linecast(Start, End, out hit, OcclusionLayer);

        if (hit.collider)
        {
           lineCastHitCount++;
        //    Debug.DrawLine(Start, End, Color.red); 
        }
      //  else
       //    Debug.DrawLine(Start, End, Color.green);
    }

    private void SetParameter()

    {
        
        ROOM_INST.setParameterByName("occlusion", lineCastHitCount/9);
       
    
    }


}


