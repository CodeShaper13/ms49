using System.Collections.Generic;
using UnityEngine;

public class Names {

    private static Names maleNames;
    private static Names femaleNames;
    private static Names lastNames;

    private readonly TextAsset textAsset;
    private string[] names;

    public static void bootstrap() {
        Names.maleNames = new Names(References.list.maleNames);
        Names.femaleNames = new Names(References.list.femaleNames);
        Names.lastNames = new Names(References.list.lastNames);
    }

    public Names(TextAsset text) {
        this.textAsset = text;
        this.names = this.readTextAsset(this.textAsset, true).ToArray();
    }

    /// <summary>
    /// Returns a random name for the list.
    /// </summary>
    public string getRndName() {
        return this.names[Random.Range(0, this.names.Length)];
    }

    public static void getRandomName(EnumGender gender, out string firstName, out string lastName) {
        if(gender == EnumGender.MALE) {
            firstName = Names.maleNames.getRndName();
        }
        else {
            firstName = Names.femaleNames.getRndName();
        }
        lastName = Names.lastNames.getRndName();
    }

    /// <summary>
    /// Reads a Text Asset and returns the contents.  Empty
    /// line and lines starting with "#" are ignored.
    /// </summary>
    private List<string> readTextAsset(TextAsset textAsset, bool removeComments = true) {
        string[] strings = textAsset.text.Split(new string[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        List<string> list = new List<string>(strings);
        if(removeComments) {
            list.RemoveAll(i => i.StartsWith("#"));
        }
        return list;
    }
}