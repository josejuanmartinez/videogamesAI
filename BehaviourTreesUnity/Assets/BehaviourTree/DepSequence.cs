using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DepSequence : Node {
    BehaviourTree dependancy;
    NavMeshAgent agent;
    public DepSequence(string n, BehaviourTree d, NavMeshAgent a) {
        name = n;
        dependancy = d;
        agent = a;
    }

    public override Status Process() {
        if (dependancy.Process() == Status.FAILURE) {
            agent.ResetPath();
            // Reset all children
            foreach (Node n in children) {

                n.Reset();
            }
            return Status.FAILURE;
        }

        Status childstatus = children[currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE)
            return childstatus;

        currentChild++;
        if (currentChild >= children.Count) {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }


}
