﻿public abstract class AbstractDepositPoint : AbstractBehaviorContainer {

    /// <summary>
    /// Check if the deposit point is "open" and accepting items.  If
    /// the Deposit point is full or disabled, this should return
    /// false.
    public abstract bool isOpen();
}
