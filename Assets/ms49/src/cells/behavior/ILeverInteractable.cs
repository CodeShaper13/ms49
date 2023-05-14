/// <summary>
/// Cell Behaviors should implement this if they wish to recieve
/// notifications when an adjacent Lever is flipped.
/// </summary>
public interface ILeverReciever {

    void OnLeverFlip(CellBehavior lever);
}
