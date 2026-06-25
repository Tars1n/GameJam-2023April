using UnityEngine;
public interface IGameInput
{
    public Vector3Int GetInputPosForHilight();

    public Vector3Int? GetInputBoolForMove();
}