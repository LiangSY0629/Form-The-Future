using System;
using UnityEngine;

[Serializable]
public abstract class DialogueEffect
{

}

[Serializable]
public class SpeakerPose : DialogueEffect
{
    public string charaterName;
    public string animtorName;

}


[Serializable]
public class CameraMove : DialogueEffect
{
    public Vector3 finalPositon;

}

[Serializable]
public class AffinityEffect
{
    public string charaterName;
    public int addNumber;

}
