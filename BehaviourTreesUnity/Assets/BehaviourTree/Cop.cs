using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cop : BTAgent {

    public GameObject[] patrolPoints;
    public GameObject robber;

    public override void Start() {
        base.Start();

        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for (int i = 0; i < patrolPoints.Length; ++i) {

            Leaf pp = new Leaf("Go to " + patrolPoints[i].name, i, GoToPoint);
            selectPatrolPoint.AddChild(pp);
        }

        Sequence chaseRobber = new Sequence("Chase");
        Leaf canSee = new Leaf("Can See Robber?", CanSeeRobber);
        Leaf chase = new Leaf("Chase Robber", ChaseRobber);

        chaseRobber.AddChild(canSee);
        chaseRobber.AddChild(chase);

        Inverter cantSeeRobber = new Inverter("Can't See Robber");
        cantSeeRobber.AddChild(canSee);

        BehaviourTree patrolConditions = new BehaviourTree();
        Sequence condition = new Sequence("Patrol Conditions");
        condition.AddChild(cantSeeRobber);
        patrolConditions.AddChild(condition);
        DepSequence patrol = new DepSequence("Patrol Until", patrolConditions, agent);
        patrol.AddChild(selectPatrolPoint);

        Selector beCop = new Selector("Be a cop");
        beCop.AddChild(patrol);
        beCop.AddChild(chaseRobber);

        tree.AddChild(beCop);
    }

    public Node.Status GoToPoint(int i) {

        Node.Status s = GoToLocation(patrolPoints[i].transform.position);
        return s;
    }

    public Node.Status CanSeeRobber() {

        return CanSee(robber.transform.position, "Robber", 5.0f, 60.0f);
    }

    Vector3 rl;

    public Node.Status ChaseRobber() {

        float chaseDistance = 10.0f;

        if (state == ActionState.IDLE) {

            rl = this.transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
        }

        return GoToLocation(rl);
    }
}
