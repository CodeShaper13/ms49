public class CommandMilestone : CommandBase {

    public override string runCommand(World world, string[] args) {
        if(args.Length > 0) {
            string s = args[0];
            if(s == "all") {
                MilestoneData m;
                do {
                    m = world.milestones.getCurrent();
                    world.milestones.unlock(m, true);
                } while(m != null);

            } else if(s == "next") {
                world.milestones.unlock(world.milestones.getCurrent(), true);
            }
            else {
                throw new WrongSyntaxException();
            }
        } else {
            throw new WrongSyntaxException();
        }

        return "unlocking milestone(s)";
    }
}
