using BehaviorTree;

public class ZombieBT : Tree
{
    public UnityEngine.Transform[] waypoints;

    public static float speed = 5f;

    protected override Node SetupTree()
    {
        Node root = new Patrol_AI(transform, waypoints);
        return root;
    }
}
