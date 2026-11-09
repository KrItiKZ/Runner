using UnityEngine;

public class LaneButton2D : MonoBehaviour
{
    [SerializeField] private PlayerController2D player;

    public void MoveUp()
    {
        if (player != null) player.MoveLaneUp();
    }

    public void MoveDown()
    {
        if (player != null) player.MoveLaneDown();
    }
}
