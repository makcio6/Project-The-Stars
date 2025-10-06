using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;
    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("TimeManager");
                _instance = obj.AddComponent<TimeManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    //[Range(0f, 5f)]
    private float GloabalTimeScale = 1f;

    public float GlobalDeltaTime => Time.unscaledDeltaTime * GloabalTimeScale;
    public float GlobalFixedDeltaTime => Time.fixedDeltaTime * GloabalTimeScale;

    private float PlayerIndividualTimeScale = 1f; //temp
    private UInt16 LastRequesterId;



    /*
     distortionScale - individual parameter for GameObj requester in how much time is must be slowed. So if GameObj need to set time 2 times slower he must send value 2. 
     */
    public int CreateTimeDistortion(float distortionScale) //returning id for requester or -1 if TimeDistortion forbiden now
    {
        GloabalTimeScale = GloabalTimeScale / distortionScale;
        PlayerIndividualTimeScale = GloabalTimeScale * distortionScale;
        LastRequesterId++;
        return LastRequesterId;
        //return - 1;
    }

    public float GetIndividualDeltaTime(int RequesterId)
    {
        if (RequesterId < 0) return Time.unscaledDeltaTime * GloabalTimeScale;

        if (LastRequesterId == RequesterId)
            return PlayerIndividualTimeScale * Time.unscaledDeltaTime;

        return Time.unscaledDeltaTime * GloabalTimeScale;
    }

    public void DestroyTimeDistortion(int requesterId) //stoping Distortion for requester
    {

        GloabalTimeScale = 1f;
        PlayerIndividualTimeScale = 1f;
    }

}