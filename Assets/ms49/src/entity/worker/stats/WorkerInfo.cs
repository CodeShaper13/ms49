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
    public string FullName => string.Format("{0} {1}", this.firstName, this.lastName);

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
        this.firstName = tag.GetString("firstName");
        this.lastName = tag.GetString("lastName");
        this.gender = (EnumGender)tag.GetInt("gender");
        this.skinTone = tag.GetInt("skinTone");
        this.hairStyle = tag.GetInt("hairStyle");
        this.personality = Main.instance.PersonalityRegistry[tag.GetInt(
            "personality",
            -1)];
        if(this.personality == null) {
            this.personality = Main.instance.PersonalityRegistry.GetDefaultPersonality();
        }
        this.payShift = tag.GetInt("payShift");
    }

    public NbtCompound writeToNbt() {
        NbtCompound tag = new NbtCompound();

        tag.SetTag("firstName", this.firstName);
        tag.SetTag("lastName", this.lastName);
        tag.SetTag("gender", (int)this.gender);
        tag.SetTag("skinTone", this.skinTone);
        tag.SetTag("hairStyle", this.hairStyle);
        tag.SetTag("personality", Main.instance.PersonalityRegistry.GetIdOfElement(this.personality));
        tag.SetTag("payShift", this.payShift);

        return tag;
    }
}
