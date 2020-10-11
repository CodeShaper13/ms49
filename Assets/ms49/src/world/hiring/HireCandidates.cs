using fNbt;
using System.Collections.Generic;
using UnityEngine;

public class HireCandidates : MonoBehaviour, ISaveableState {

    [SerializeField]
    private World world = null;
    [SerializeField]
    private int startingCandidateCount = 2;
    [SerializeField]
    private WorkerType[] startingUnlockedTypes = null;
    [SerializeField]
    private float _workerTrainTime = 15f;
    [SerializeField]
    private int _hireCost = 100;
    [SerializeField, MinMaxSlider(1, 60), Tooltip("Time is in minutes")]
    private Vector2 candidateAvailabilityTimeRange = new Vector2(2, 8);

    private List<Candidate> candidates;
    private float traineeTrainingTime;
    private Trainee trainee;

    public float workerTrainTime => this._workerTrainTime;
    public int hireCost => this._hireCost;
    public int candaditeCount => this.candidates.Count;

    public string tagName => "hireCandidates";

    private void Awake() {
        this.candidates = new List<Candidate>();
    }

    private void Update() {
        if(this.isTraining()) {
            this.traineeTrainingTime += Time.deltaTime;
            if(this.traineeTrainingTime >= this.workerTrainTime || CameraController.instance.inCreativeMode) {
                // Create a new Worker
                Main.instance.workerFactory.spawnWorker(
                    this.world,
                    this.world.mapGenData.workerSpawnPoint,
                    trainee.info,
                    trainee.type);

                this.trainee = null;
            }
        }
    }

    /// <summary>
    /// Removes all candidates from the list.
    /// </summary>
    public void clearAllCandidates() {
        this.candidates.Clear();
    }

    public Candidate getCandidate(int index) {
        if(index < 0 || index >= this.candidates.Count) {
            return null;
        }

        return this.candidates[index];
    }

    /// <summary>
    /// Returns true if you are in the process of hiring someone new.
    /// </summary>
    public bool isTraining() {
        return this.trainee != null;
    }

    public float remainingTrainingTime() {
        return this.traineeTrainingTime;
    }

    public void hireAndTrain(int candidateIndex) {
        Candidate candidate = this.getCandidate(candidateIndex);
        this.trainee = new Trainee(candidate.info, candidate.type);
        this.traineeTrainingTime = 0;

        // Remove the Candidate from the list
        this.candidates.RemoveAt(candidateIndex);
    }

    public void refreshCanidates() {
        // Remove candidates that aren't available anymore
        for(int i = 0; i < this.candidates.Count; i++) {
            Candidate c = this.candidates[i];
            if(c != null) {
                if(this.world.time.time > c.endAvailabilityTime) {
                    // Worker is not longer available to be hired
                    this.candidates.RemoveAt(0);
                }
            }
        }

        // Find the number of candadites that should be shown.
        int candaditeCount = this.startingCandidateCount;
        foreach(MilestoneData milestone in this.world.milestones.milestones) {
            if(milestone == null) {
                continue;
            }

            if(CameraController.instance.inCreativeMode || milestone.isUnlocked) {
                candaditeCount += milestone.hireCandaditeIncrease;
            }
        }

        // Add new candidates
        int stop = candaditeCount - this.candidates.Count;
        for(int i = 0; i < stop; i++) {
            WorkerType type = this.getNewCandatiteType();

            if(type == null) {
                Debug.LogWarning("Error finding a type for a new candadite");
                continue;
            }

            this.candidates.Add(new Candidate(
                Main.instance.workerFactory.generateWorkerInfo(),
                type,
                this.world.time.time + Random.Range(
                    this.candidateAvailabilityTimeRange.x * 60,
                    this.candidateAvailabilityTimeRange.y * 60)));
        }
    }

    public void writeToNbt(NbtCompound tag) {
        NbtList list = new NbtList(NbtTagType.Compound);
        foreach(Candidate c in this.candidates) {
            if(c != null) {
                list.Add(c.writeToNbt());
            }
        }
        tag.setTag("candidates", list);

        if(this.trainee != null) {
            tag.setTag("trainee", this.trainee.writeToNbt());
            tag.setTag("trainingTime", this.traineeTrainingTime);
        }
    }

    public void readFromNbt(NbtCompound tag) {
        NbtList list = tag.getList("candidates");
        for(int i = 0; i < list.Count; i ++) {
            this.candidates.Add(new Candidate(list.Get<NbtCompound>(i)));
        }

        if(tag.hasKey("trainee")) {
            this.trainee = new Trainee(tag.getCompound("trainee"));
            this.traineeTrainingTime = tag.getFloat("trainingTime");
        }
    }

    /// <summary>
    /// Returns the WorkerType that a new candadite should have.
    /// Null is returned if there are no unlocked candadites.
    /// </summary>
    /// <returns></returns>
    private WorkerType getNewCandatiteType() {
        WorkerTypeRegistry reg = Main.instance.workerTypeRegistry;
        bool inCreative = CameraController.instance.inCreativeMode;

        // Get totals of all of the currently shown worker types.
        int[] counts = new int[reg.getRegistrySize()];
        foreach(Candidate c in this.candidates) {
            if(c != null) {
                int index = reg.getIdOfElement(c.type);
                counts[index]++;
            }
        }

        // Get a list of all of the unlocked worker types.  This may
        // contain duplicates, but that's fine.
        List<WorkerType> allUnlockedTypes = new List<WorkerType>();
        allUnlockedTypes.AddRange(this.startingUnlockedTypes);
        foreach(MilestoneData milestone in this.world.milestones.milestones) {
            if(milestone.isUnlocked) {
                allUnlockedTypes.AddRange(milestone.unlockedWorkerTypes);
            }
        }

        // Break early if nothing is unlocked.
        if(allUnlockedTypes.Count == 0) {
            return null;
        }

        // If there are none of a certain type, add them
        for(int i = 0; i < reg.getRegistrySize(); i++) {
            WorkerType t = reg.getElement(i);
            if(t != null && counts[i] == 0 && (allUnlockedTypes.Contains(t) || inCreative)) {
                return t; // There is none of this time currently hired.
            }
        }

        // There is at least one of every type, add the type that is
        // the least common (if there are multiple, pick a random).
        int lowestCount = int.MaxValue;
        for(int i = 0; i < counts.Length; i++) {
            if(reg.getElement(i) != null) {
                if(counts[i] < lowestCount) {
                    lowestCount = counts[i];
                }
            }
        }

        // Get a list of the Types that are the least shown.
        List<WorkerType> possibilites = new List<WorkerType>();
        for(int i = 0; i < reg.getRegistrySize(); i++) {
            WorkerType type = reg.getElement(i);
            if(type != null && counts[i] <= lowestCount && (allUnlockedTypes.Contains(type) || inCreative)) {
                possibilites.Add(type);
            }
        }
        
        if(possibilites.Count == 0) {
            // There is an equal number of all worker types, pick one at random
            return allUnlockedTypes[Random.Range(0, allUnlockedTypes.Count)];
        } else {
            return possibilites[Random.Range(0, possibilites.Count)];
        }
    }
}