﻿using UnityEngine;

public class WorkerStats {

    private string firstName;
    private string lastName;
    private EnumGender gender;

    public WorkerStats() {
        int easterEggRnd = Random.Range(0, 100000); // 100 thousand
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
            Names.getRandomName(this.gender, out this.firstName, out this.lastName);
            this.gender = Random.Range(0, 1) == 0 ? EnumGender.MALE : EnumGender.FEMALE;
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
