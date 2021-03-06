using Godot;
using System;

public class Rock : RigidBody2D
{
    [Signal]
    delegate void Boom();
    
    public Vector2 _screensize;
    private int _size;
    private float _radius;
    private float _scaleFactor = 0.2f;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
//        GetNode<Sprite>("Explosion").Hide();
    }

    public void Start(Vector2 pos, Vector2 velocity, int size)
    {
        Position = pos;
        _size = size;
        var mass = _size * 1.5;
        Sprite rock = GetNode<Sprite>("RockSprite");
        rock.Scale = new Vector2(1, 1) * _scaleFactor * size;
        _radius = rock.Texture.GetSize().x / 2 * _scaleFactor * size;
        CircleShape2D collisionShape = new CircleShape2D();
        collisionShape.Radius = _radius;
        GetNode<CollisionShape2D>("CollisionShape2D").Shape = collisionShape;
        LinearVelocity = velocity;
        Random r = new Random();
        AngularVelocity = r.Next(-15, 15) / 10f;
        GetNode<Sprite>("Explosion").Scale = new Vector2(0.75f, 0.75f) * _size;
    }

    public override void _IntegrateForces(Physics2DDirectBodyState physics_state)
    {
        base._IntegrateForces(physics_state);
        Transform2D xform = physics_state.GetTransform();
        Vector2 origin;
        Transform2D wibble;
        if (xform.Origin.x > _screensize.x + _radius)
        {
            origin = new Vector2(0 - _radius, xform.Origin.y);
            wibble = new Transform2D(xform.x, xform.y, origin);
            physics_state.SetTransform(wibble);
        }
        if (xform.Origin.x < 0 - _radius)
        {
            origin = new Vector2(_screensize.x + _radius, xform.Origin.y);
            wibble = new Transform2D(xform.x, xform.y, origin);
            physics_state.SetTransform(wibble);
        }

        if (xform.Origin.y > _screensize.y + _radius)
        {
            origin = new Vector2(xform.Origin.x, 0 - _radius);
            wibble = new Transform2D(xform.x, xform.y, origin);
            physics_state.SetTransform(wibble);
        }

        if (xform.Origin.y < 0 - _radius)
        {
            origin = new Vector2(xform.Origin.x, _screensize.y + _radius);
            wibble = new Transform2D(xform.x, xform.y, origin);
            physics_state.SetTransform(wibble);
        }
    }

    public void Explode()
    {
        Layers = 0;
        GetNode<Sprite>("RockSprite").Hide();
        GetNode<Sprite>("Explosion").Show();
        GetNode<AnimationPlayer>("Explosion/AnimationPlayer").Play("explosion");
        EmitSignal("Boom", _size, _radius, Position, LinearVelocity);
        LinearVelocity = new Vector2();
        AngularVelocity = 0;
    }
    
    private void _on_AnimationPlayer_animation_finished(String anim_name)
    {
        QueueFree();
    }
}



