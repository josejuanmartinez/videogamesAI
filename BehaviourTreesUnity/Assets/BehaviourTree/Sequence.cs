using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node {
    public Sequence(string n) {
        name = n;
    }

    public override Status Process() {
        Status childstatus = children[currentChild].Process();
        if (childstatus == Status.RUNNING) return Status.RUNNING;
        if (childstatus == Status.FAILURE) {

            currentChild = 0;
            foreach (Node n in children) {

                n.Reset();
            }
            return childstatus;
        }

        currentChild++;
        if (currentChild >= children.Count) {
            currentChild = 0;
            return Status.SUCCESS;
        }

        return Status.RUNNING;
    }


}
