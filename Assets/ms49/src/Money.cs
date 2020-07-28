public static class Money {

    private static int amount = 8000;

    public static int get() {
        return Money.amount;
    }

    public static void setMoney(int amount) {
        Money.amount = amount;
    }

    public static void add(int amount) {
        Money.amount += amount;
    }

    public static void remove(int amount) {
        Money.amount -= amount;
    }

    public static bool inBlack() {
        return Money.amount >= 0;
    }
}
