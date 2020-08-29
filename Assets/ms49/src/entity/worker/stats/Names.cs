using System.Collections.Generic;
using UnityEngine;

public class Names : MonoBehaviour {

    [SerializeField]
    private TextAsset maleNamesTextAsset = null;
    [SerializeField]
    private TextAsset femaleNamesTextAsset = null;
    [SerializeField]
    private TextAsset lastNamesTextAsset = null;

    private string[] maleNames;
    private string[] femaleNames;
    private string[] lastNames;

    private void Awake() {
        this.maleNames = this.readTextAsset(this.maleNamesTextAsset, true).ToArray();
        this.femaleNames = this.readTextAsset(this.femaleNamesTextAsset, true).ToArray();
        this.lastNames = this.readTextAsset(this.lastNamesTextAsset, true).ToArray();
    }

    /// <summary>
    /// Returns a random name for the list.
    /// </summary>
    public string rnd(string[] names) {
        return names[Random.Range(0, names.Length)];
    }

    public void getRandomName(EnumGender gender, out string firstName, out string lastName) {
        if(gender == EnumGender.MALE) {
            firstName = this.rnd(this.maleNames);
        }
        else {
            firstName = this.rnd(this.femaleNames);
        }
        lastName = this.rnd(this.lastNames);
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