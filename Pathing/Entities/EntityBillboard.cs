using Microsoft.Xna.Framework;

namespace Manlaan.CommanderMarkers.Pathing.Entities;

public class EntityBillboard : Billboard
{

    public Entity AttachedEntity { get; }

    /// <inheritdoc />
    public override Vector3 Position {
        get => AttachedEntity.Position + AttachedEntity.RenderOffset;
    }

    public EntityBillboard(Entity attachedEntity) {
        this.AttachedEntity = attachedEntity;
    }

}
