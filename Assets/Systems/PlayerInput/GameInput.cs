using UnityEngine;

public abstract class GameInput : MonoBehaviour
{


    public abstract Vector3Int? GetInputPosForHilight();

    public abstract Vector3Int? GetInputBoolForMove();

}