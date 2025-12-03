using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ObstacleSpawnManager _owner;

    public void SetOwner(ObstacleSpawnManager owner)
    {
        _owner = owner;
    }

    private void OnDestroy()
    {
        if (_owner != null)
        {
            _owner.NotifyJellyfishDestroyed(this);
        }
    }
}