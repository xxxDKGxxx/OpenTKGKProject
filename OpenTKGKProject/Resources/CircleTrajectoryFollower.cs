namespace OpenTKGKProject.Resources;

public sealed class CircleTrajectoryFollower(Vector3 middle, float radius, float speed, float scale, float offset = 0.0f)
{
    private float _currentAngle;

    public void Update(IModel model, float dt)
    {
        _currentAngle += dt * speed;

        if (_currentAngle > MathHelper.TwoPi)
        {
            _currentAngle -= MathHelper.TwoPi;
        }

        var xPos = middle.X + radius * MathF.Sin(_currentAngle);
        var zPos = middle.Z + radius * MathF.Cos(_currentAngle);

        var newPosition = new Vector3(xPos, middle.Y, zPos);

        var rotation = Matrix4.CreateRotationY(_currentAngle + offset);
        var translation = Matrix4.CreateTranslation(newPosition);

        model.ModelMatrix = Matrix4.CreateScale(scale) * rotation * translation;
    }
}