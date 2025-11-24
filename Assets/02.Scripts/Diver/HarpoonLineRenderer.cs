using UnityEngine;

public class HarpoonLineRenderer : MonoBehaviour
{
    [SerializeField] private HarpoonShooter _shooter;
    [SerializeField] private LineRenderer _line;

    private HarpoonProjectile _projectile;

    private void Update()
    {
        if (_shooter == null || _line == null) return;
        
        _projectile = _shooter.CurrentProjectile;

        if (_projectile == null)
        {
            _line.enabled = false;
            return;
        }

        _line.enabled = true;
        _line.SetPosition(0, _shooter.HarpoonReturnPoint);
        _line.SetPosition(1, _projectile.Position);
    }
}
