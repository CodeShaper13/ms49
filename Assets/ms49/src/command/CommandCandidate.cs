using UnityEngine;
using System.Collections;

public class CommandCandidate : CommandBase {

    public override string runCommand(World world, string[] args) {
        world.hireCandidates.clearAllCandidates();
        return "All candidates have been cleared";
    }
}
