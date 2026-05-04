using UnityEngine;

[System.Serializable]
public class Interaction
{
    [Header("Conditions")]
    public string requiredObject;

    [Header("Effects")]
    public InteractionType type;
    public string stringArg;
    public float length;

    [Header("Next Interaction")]
    public int nextInteractionID;
    public bool playNextInteractionInstantly;
}

public enum InteractionType
{
    getPlayerObject,
    loosePlayerObject,
    goAway,
    sendInteractiveObjectAway,
    placeInteractiveObject,
    displayHUDElement,
    playSound,
    setCheckpoint,
    killPlayer,
    teleportPlayer
}
