using fNbt;
using UnityEngine;

public class WorkerInfo {

    public string firstName { get; private set; }
    public string lastName { get; private set; }
    public EnumGender gender { get; private set; }
    public int skinTone { get; private set; }
    public int hairColor { get; private set; }
    public int hairStyle { get; private set; }
    public int personality { get; private set; }
    public float workSpeed { get; private set; }
    public int pay { get; private set; }

    public string fullName => this.firstName + " " + this.lastName;

    public WorkerInfo() {
        this.gender = EnumGender.MALE; // = Random.Range(0, 2) == 0 ? EnumGender.MALE : EnumGender.FEMALE;
        string first;
        string last;
        Main.instance.names.getRandomName(this.gender, out first, out last);
        this.firstName = first;
        this.lastName = last;
        this.skinTone = Random.Range(0, 3);
        this.hairColor = Random.Range(0, 5);
        this.hairStyle = 0; // TODO
        this.personality = Random.Range(0, Main.instance.personalities.getPersonalityCount() - 1);
        this.workSpeed = Random.Range(0.8f, 1.2f);
        this.pay = Random.Range(10, 20);

        if(Random.Range(0, 100_000) == 111599) { // 1 in 100 thousand
            this.firstName = "PJ";
            this.lastName = "Didelot";
            this.gender = EnumGender.MALE;
            this.skinTone = 0;
            // TODO hair
            this.workSpeed = 2f;
            this.pay = 50;
        }
    }

    public WorkerInfo(NbtCompound tag) {
        this.firstName = tag.getString("firstName");
        this.lastName = tag.getString("lastName");
        this.gender = (EnumGender)tag.getInt("gender");
        this.skinTone = tag.getInt("skinTone");
        this.hairColor = tag.getInt("hairColor");
        this.hairStyle = tag.getInt("hairStyle");
        this.personality = tag.getInt("personality");
        this.workSpeed = tag.getFloat("workSpeed");
        this.pay = tag.getInt("pay");
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("firstName", this.firstName);
        tag.setTag("lastName", this.lastName);
        tag.setTag("gender", (int)this.gender);
        tag.setTag("skinTone", this.skinTone);
        tag.setTag("hairColor", this.hairColor);
        tag.setTag("hairStyle", this.hairStyle);
        tag.setTag("personality", this.personality);
        tag.setTag("workSpeed", this.workSpeed);
        tag.setTag("pay", this.pay);

        return tag;
    }
}
