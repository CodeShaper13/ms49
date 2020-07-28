using System;

public static class MathHelper {

    public static int toNearestPowerOf2(int x) {
        return (int) Math.Pow(2, Math.Round(Math.Log(x) / Math.Log(2)));
    }
}
