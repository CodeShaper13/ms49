using UnityEngine;

/// <summary>
/// CellBehaviors should implement to interact with Minecarts.
/// </summary>
public interface IMinecartInteractor {

    /// <summary>
    /// A reference to the Minecart this interactor is interacting
    /// with.  Not set until the Minecart comes to a halt.
    /// </summary>
    EntityMinecart minecart { get; set; }

    /// <summary>
    /// Called every frame a Minecart is over or next to this
    /// behavior.
    /// </summary>
    bool shouldCartInteract(EntityMinecart cart);

    /// <summary>
    /// Returns where is World units the cart should stop along the
    /// track.
    /// </summary>
    Vector3 getCartStopPoint();
}
