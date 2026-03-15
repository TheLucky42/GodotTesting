using Godot;

public partial class PlayerController : RigidBody3D
{
    [Export] public NodePath CameraRigPath;

    [Export] public float TorqueForce;
    [Export] public float MaxLinearSpeed;
    [Export] public float MaxAngularSpeed;

    private Node3D _cameraRig;

    public override void _Ready()
    {
        _cameraRig = GetNode<Node3D>(CameraRigPath);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_cameraRig == null)
            return;

        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 moveDir = GetCameraRelativeDirection(input);

        if (moveDir.LengthSquared() > 0.001f)
        {
            // This cross-product order matches your "forward moves forward" result
            Vector3 torqueAxis = moveDir.Cross(Vector3.Up).Normalized();
            ApplyTorque(torqueAxis * TorqueForce);
        }

        ClampSpeed();
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 forward = -_cameraRig.GlobalTransform.Basis.Z;
        Vector3 right = -_cameraRig.GlobalTransform.Basis.X;

        forward.Y = 0f;
        right.Y = 0f;

        forward = forward.Normalized();
        right = right.Normalized();

        Vector3 moveDir = right * input.X + forward * input.Y;

        if (moveDir.LengthSquared() > 1f)
            moveDir = moveDir.Normalized();

        return moveDir;
    }

    private void ClampSpeed()
    {
        Vector3 vel = LinearVelocity;
        Vector3 horizontal = new Vector3(vel.X, 0f, vel.Z);

        if (horizontal.Length() > MaxLinearSpeed)
        {
            horizontal = horizontal.Normalized() * MaxLinearSpeed;
            LinearVelocity = new Vector3(horizontal.X, vel.Y, horizontal.Z);
        }

        if (AngularVelocity.Length() > MaxAngularSpeed)
        {
            AngularVelocity = AngularVelocity.Normalized() * MaxAngularSpeed;
        }
    }
}