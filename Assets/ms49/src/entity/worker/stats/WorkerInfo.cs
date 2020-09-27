using fNbt;

public class WorkerInfo {

    public string firstName { get; private set; }
    public string lastName { get; private set; }
    public EnumGender gender { get; private set; }
    public int skinTone { get; private set; }
    public int hairStyle { get; private set; }
    public int personality { get; private set; }
    public float workSpeed { get; private set; } // Unused
    public int pay { get; set; }

    public string fullName => this.firstName + " " + this.lastName;

    public WorkerInfo(string first, string last, EnumGender gender, int skinTone, int hairColor, int hairStyle, int personalityId, float v5, int pay) {
        this.firstName = first;
        this.lastName = last;
        this.gender = gender;
        this.skinTone = skinTone;
        this.hairStyle = hairStyle;
        this.personality = personalityId;
        this.workSpeed = v5;
        this.pay = pay;
    }

    public WorkerInfo(NbtCompound tag) {
        this.firstName = tag.getString("firstName");
        this.lastName = tag.getString("lastName");
        this.gender = (EnumGender)tag.getInt("gender");
        this.skinTone = tag.getInt("skinTone");
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
        tag.setTag("hairStyle", this.hairStyle);
        tag.setTag("personality", this.personality);
        tag.setTag("workSpeed", this.workSpeed);
        tag.setTag("pay", this.pay);

        return tag;
    }
}
