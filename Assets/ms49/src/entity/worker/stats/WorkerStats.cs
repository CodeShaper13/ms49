using UnityEngine;

public class WorkerStats {

    private string firstName;
    private string lastName;
    private EnumGender gender;

    public WorkerStats() {
        int easterEggRnd = Random.Range(0, 100_000); // 100 thousand
        if(easterEggRnd == 1) {
            this.firstName = "Dalton";
            this.lastName = "Didelot";
            this.gender = EnumGender.MALE;
        }
        else if(easterEggRnd == 111599) {
            this.firstName = "PJ";
            this.lastName = "Didelot";
            this.gender = EnumGender.MALE;
        }
        else {
            this.gender = Random.Range(0, 2) == 0 ? EnumGender.MALE : EnumGender.FEMALE;
            Main.instance.names.getRandomName(this.gender, out this.firstName, out this.lastName);
        }
    }

    public string getFirstName() {
        return this.firstName;
    }

    public string getLastName() {
        return this.lastName;
    }

    public string getFullName() {
        return this.firstName + " " + this.lastName;
    }

    public EnumGender getGender() {
        return this.gender;
    }
}
