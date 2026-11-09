using UnityEngine;

public class JumpButton : MonoBehaviour
{
    [SerializeField] private PlayerController2D player;

    public void PressJump()
    {
        if (player != null) player.Jump();
    }
}
