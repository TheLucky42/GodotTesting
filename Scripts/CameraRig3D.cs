using Godot;

public partial class CameraRig3D : Node3D
{
    [Export] public NodePath PlayerPath;
    [Export] public float FollowSpeed;
    [Export] public float RotationSpeed;
    [Export] public float MinTurnSpeed;

    private RigidBody3D _player;
    private Vector3 _lastDirection = new Vector3(0, 0, -1);

    public override void _Ready()
    {
        _player = GetNode<RigidBody3D>(PlayerPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_player == null)
            return;

        float dt = (float)delta;

        GlobalPosition = GlobalPosition.Lerp(_player.GlobalPosition, FollowSpeed * dt);

        Vector3 horizontalVel = _player.LinearVelocity;
        horizontalVel.Y = 0f;

        if (horizontalVel.Length() > MinTurnSpeed)
            _lastDirection = horizontalVel.Normalized();

        // Important: this keeps forward world -Z aligned with rig yaw 0
        float targetYaw = Mathf.Atan2(-_lastDirection.X, -_lastDirection.Z);

        Vector3 rot = Rotation;
        rot.Y = Mathf.LerpAngle(rot.Y, targetYaw, RotationSpeed * dt);
        Rotation = rot;
    }
}