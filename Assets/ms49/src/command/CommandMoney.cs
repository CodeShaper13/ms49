using UnityEngine;

public class CommandMoney : CommandBase {

    [SerializeField]
    private IntVariable money = null;

    public override string runCommand(World world, string[] args) {
        if(this.money != null) {
            int amount = this.parseInt(args, 0);
            this.money.value = amount;
            return "set money to " + amount;
        }

        return "money reference not set in inspector";
    }
}
