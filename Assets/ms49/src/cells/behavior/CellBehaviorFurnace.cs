using fNbt;
using UnityEngine;

// TODO fuel suffers from rounding errors and the like.
public class CellBehaviorFurnace : CellBehaviorContainer {

    [SerializeField]
    private FloatVariable _smeltSpeedMultiplyer = null;

    private float smeltProgress;
    private float fuel;
    private float fuelValue;

    public Item ItemInput {
        get => this.inventory[0];
        set => this.inventory[0] = value;
    }
    public Item ItemOutput {
        get => this.inventory[1];
        set => this.inventory[1] = value;
    }
    public Item ItemFuel {
        get => this.inventory[2];
        set => this.inventory[2] = value;
    }

    public float NormalizedSmeltProgress => this.ItemInput is ItemSmeltable smeltable ? this.smeltProgress / smeltable.SmeltingTime : 0f;
    public float NormalizedFuelValue => this.fuel / this.fuelValue;

    private void Update() {
        if(this.ItemInput is ItemSmeltable smeltable1) {
            if(this.fuel > 0 && this.smeltProgress < smeltable1.SmeltingTime) {
                // Keep cooking.
                this.smeltProgress += Time.deltaTime * (this._smeltSpeedMultiplyer != null ? this._smeltSpeedMultiplyer.value : 1f);
                this.fuel -= Time.deltaTime;
            }

            if(this.smeltProgress >= smeltable1.SmeltingTime && this.ItemOutput == null) {
                // Finish smelting item.
                this.ItemOutput = smeltable1.SmeltingResult;
                this.ItemInput = null;
                this.smeltProgress = 0f;
            }
        }

        if(this.fuel <= 0 && this.ItemFuel != null && this.ItemFuel.IsFuel) {
            // Consume fuel.
            this.fuel = this.ItemFuel.FuelValue;
            this.fuelValue = this.ItemFuel.FuelValue;
            this.ItemFuel = null;
        }
    }

    public override void onRightClick() {
        base.onRightClick();

        PopupFurnace popup = Main.instance.findPopup<PopupFurnace>();
        if(popup != null) {
            popup.open();
            popup.SetInventory(this.inventory);
            popup.furnace = this;
        }
    }

    public override string getTooltipText() {
        return "[RMB] Open Furnace";
    }

    public override void ReadFromNbt(NbtCompound tag) {
        base.ReadFromNbt(tag);

        this.smeltProgress = tag.getFloat("smeltTimer");
        this.fuel = tag.getFloat("fuel");
        this.fuelValue = tag.getFloat("fuelValue");
    }

    public override void WriteToNbt(NbtCompound tag) {
        base.WriteToNbt(tag);

        tag.setTag("smeltTimer", this.smeltProgress);
        tag.setTag("fuel", this.fuel);
        tag.setTag("fuelValue", this.fuelValue);
    }

    public override bool Deposit(Item item) {
        if(item == null) {
            return false;
        }

        // Only accept fuel, or a smeltable item, if the respective
        // slot is empty.
        if(item.IsFuel) {
            if(this.ItemFuel == null) {
                this.ItemFuel = item;
                return true;
            }
        }
        if(item is ItemSmeltable) {
            if(this.ItemInput == null) {
                this.ItemInput = item;
                return true;
            }
        }

        return false;
    }
}