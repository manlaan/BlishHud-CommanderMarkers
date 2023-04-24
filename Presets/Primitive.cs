using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using static Blish_HUD.GameService;
using Manlaan.CommanderMarkers.Utils;

namespace Manlaan.CommanderMarkers.Presets;

public static class PrimitiveExtensions
{
    private static bool _requested = false;
    private static BasicEffect? _effect;
    private static BasicEffect? Effect
    {
        get
        {
            if (_effect == null && !_requested)
            {
                _requested = true;
                Graphics.QueueMainThreadRender(graphicsDevice =>
                {
                    if (_requested && _effect == null)
                    {
                        _requested = false;
                        _effect = new BasicEffect(graphicsDevice);
                    }
                });
            }

            return _effect;
        }
    }

    public static void DrawPrimitive(this SpriteBatch spriteBatch, Primitive primitive, Color color)
    {
        if (Effect is not BasicEffect effect)
            return;

        var vertices = primitive.Points.Select(p => new VertexPositionColor(p, color)).ToArray();

        effect.Projection = Gw2Mumble.PlayerCamera.WorldViewProjection;
        effect.CurrentTechnique.Passes[0].Apply();
        spriteBatch.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1);
    }
}

public class Primitive
{
    public List<Vector3> Points { get; }

    public class ScreenPrimitive
    {
        public List<List<Vector2>> Points { get; } = new List<List<Vector2>>();
        public float Depth { get; }
        private float MaxDepth => Gw2Mumble.PlayerCamera.FarPlaneRenderDistance;

        public ScreenPrimitive(IEnumerable<Vector3> points)
        {
            float sum = 0;
            int count = 0;
            List<Vector2>? list = null;
            foreach (var point in points)
            {
                if (list == null)
                {
                    list = new List<Vector2>();
                    Points.Add(list);
                }

                if (point.Z < 0)
                {
                    list = null;
                    continue;
                }

                list.Add(Flatten(point));
                sum += point.Z;
                count++;
            }

            Depth = sum / count / MaxDepth;
        }

        public void Draw(SpriteBatch spriteBatch, Color color, float thickness)
        {
            foreach (var list in Points)
                spriteBatch.DrawPolygon(Vector2.Zero, list, color, thickness, Depth, true);
        }

        public static Vector2 Flatten(Vector3 v)
            => new Vector2(
                (v.X / v.Z + 1) / 2 * Graphics.SpriteScreen.Width,
                (1 - v.Y / v.Z) / 2 * Graphics.SpriteScreen.Height);
    }

    public Primitive(IEnumerable<Vector3> points)
    {
        Points = points.ToList();
    }

    public Primitive(params Vector3[] points) : this(points.AsEnumerable())
    {
    }

    public IEnumerable<Vector3> Transform(Matrix matrix)
    {
        foreach (var p in Points)
            yield return Vector3.Transform(p, matrix);
    }

    public Primitive Transformed(Matrix matrix)
        => new Primitive(Transform(matrix));

    public IEnumerable<Vector2> Flat()
        => Points.Select(p => p.ToVector2());

    public ScreenPrimitive ToScreen()
        => new ScreenPrimitive(Points.Select(ToScreen));

    public static Primitive operator +(Primitive a, Primitive b)
        => new Primitive(a.Points.Concat(b.Points));

    public static Vector3 ToScreen(Vector3 point)
        => Vector3.Transform(point, Gw2Mumble.PlayerCamera.WorldViewProjection);

    public static Primitive HorizontalCircle(float radius, int sides)
    {
        var points2d = SpriteBatchExtensions.CreateCircle(radius, sides);
        var points = new Vector3[sides];
        for (var i = 0; i < sides; i++)
        {
            var src = points2d[i];
            points[i] = new Vector3(src.X, src.Y, 0);
        }

        return new Primitive(points);
    }

    public static Primitive VerticalArc(float rx, float ry, float start, float extents, int sides)
    {
        var points2d = SpriteBatchExtensions.CreateArc(rx, ry, start, extents, sides);
        var points = new Vector3[sides];
        for (var i = 0; i < sides; i++)
        {
            var src = points2d[i];
            points[i] = new Vector3(src.X, 0, src.Y);
        }

        return new Primitive(points);
    }
}