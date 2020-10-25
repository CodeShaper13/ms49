using fNbt;

public class WorkerInfo {

    public string firstName { get; private set; }
    public string lastName { get; private set; }
    public EnumGender gender { get; private set; }
    public int skinTone { get; private set; }
    public int hairStyle { get; private set; }
    public Personality personality { get; private set; }
    public int payShift { get; private set; }

    public int pay => Main.instance.workerFactory.getPayFromModifier(this.personality.payModifier) + this.payShift;
    public string fullName => this.firstName + " " + this.lastName;

    public WorkerInfo(string first, string last, EnumGender gender, int skinTone, int hairColor, int hairStyle, Personality personality, int payShift) {
        this.firstName = first;
        this.lastName = last;
        this.gender = gender;
        this.skinTone = skinTone;
        this.hairStyle = hairStyle;
        this.personality = personality;
        this.payShift = payShift;
    }

    public WorkerInfo(NbtCompound tag) {
        this.firstName = tag.getString("firstName");
        this.lastName = tag.getString("lastName");
        this.gender = (EnumGender)tag.getInt("gender");
        this.skinTone = tag.getInt("skinTone");
        this.hairStyle = tag.getInt("hairStyle");
        this.personality = Main.instance.personalityRegistry.getElement(tag.getInt(
            "personality",
            Main.instance.personalityRegistry.defaultPersonalityId));
        this.payShift = tag.getInt("payShift");
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.setTag("firstName", this.firstName);
        tag.setTag("lastName", this.lastName);
        tag.setTag("gender", (int)this.gender);
        tag.setTag("skinTone", this.skinTone);
        tag.setTag("hairStyle", this.hairStyle);
        tag.setTag("personality", Main.instance.personalityRegistry.getIdOfElement(this.personality));
        tag.setTag("payShift", this.payShift);

        return tag;
    }
}
