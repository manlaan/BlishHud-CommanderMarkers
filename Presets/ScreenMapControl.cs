using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using static Blish_HUD.GameService;

namespace Manlaan.CommanderMarkers.Presets;

public class ScreenMap : Control
{
    public static class Data
    {
        private const double BlishScale = 1 / .897;

        private static int _lastTick;

        private static Vector2 _mapCenter;
        public static Vector2 MapCenter => UpdateAndReturn(ref _mapCenter);

        private static Matrix _mapRotation;
        public static Matrix MapRotation => UpdateAndReturn(ref _mapRotation);

        private static float _scale;
        public static float Scale => UpdateAndReturn(ref _scale);

        private static Rectangle _screenBounds;
        public static Rectangle ScreenBounds => UpdateAndReturn(ref _screenBounds);

        private static Vector2 _boundsCenter;
        public static Vector2 BoundsCenter => UpdateAndReturn(ref _boundsCenter);

        private static T UpdateAndReturn<T>(ref T value)
        {
            if (Gw2Mumble.Tick != _lastTick)
            {
                _lastTick = Gw2Mumble.Tick;

                _mapCenter = Gw2Mumble.UI.MapCenter.ToXnaVector2();
                _mapRotation = Matrix.CreateRotationZ(
                    Gw2Mumble.UI.IsCompassRotationEnabled && !Gw2Mumble.UI.IsMapOpen
                        ? (float)Gw2Mumble.UI.CompassRotation
                        : 0);

                _screenBounds = MumbleUtils.GetMapBounds();
                _scale = (float)(BlishScale / Gw2Mumble.UI.MapScale);
                _boundsCenter = _screenBounds.Location.ToVector2() + _screenBounds.Size.ToVector2() / 2f;
            }

            return value;
        }
    }

    private readonly List<IMapEntity> _entities = new();
    private readonly MapData _mapData;
    private readonly ScreenMapBounds _mapBounds;

    private BitmapFont _bitmapFont = GameService.Content.DefaultFont32;
    public ScreenMap(MapData mapData)
    {
        _mapData = mapData;
        _mapBounds = new ScreenMapBounds(_mapData);
    }

    protected override CaptureType CapturesInput() => CaptureType.None;
    public void AddEntity(IMapEntity entity)
        => _entities.Add(entity);

    public void RemoveEntity(IMapEntity entity)
        => _entities.Remove(entity);

    public void ClearEntities() => _entities.Clear();
    public override void DoUpdate(GameTime gameTime)
    {
        base.DoUpdate(gameTime);
        Location = Data.ScreenBounds.Location;
        Size = Data.ScreenBounds.Size;
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (!GameIntegration.Gw2Instance.IsInGame || _mapData.Current == null)
            return;
        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;

        bounds.Location = Location;
        _mapBounds.Rectangle = bounds;
        var promptDrawn = !GameService.Gw2Mumble.UI.IsMapOpen;
        foreach (var entity in _entities)
        {
            entity.DrawToMap(spriteBatch, _mapBounds);
            if(!promptDrawn && entity.DistanceFrom(playerPosition) < 15f){
                promptDrawn = true;
                DrawPrompt(spriteBatch, entity);
            }
        }
    }

    protected void DrawPrompt(SpriteBatch spriteBatch, IMapEntity marker)
    {
        Rectangle _promptRectangle = new Rectangle(GameService.Graphics.SpriteScreen.Width / 2 - 150, GameService.Graphics.SpriteScreen.Height - 100, 300, 40);
        spriteBatch.DrawStringOnCtrl(this, $"Press Interact to place markers\n{marker.GetMarkerText()}", _bitmapFont, _promptRectangle, Color.Orange, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Center, verticalAlignment: VerticalAlignment.Top);

    }
}