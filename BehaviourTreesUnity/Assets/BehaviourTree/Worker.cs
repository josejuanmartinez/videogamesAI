using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : BTAgent {


    GameObject patron;
    public GameObject office;

    public override void Start() {

        base.Start();

        Leaf patronStillWaiting = new Leaf("Is Patron Waiting?", PatronIsWaiting);
        Leaf allocatePatron = new Leaf("Allocate Patron", AllocatePatron);

        Leaf goToPatron = new Leaf("Go to Patron", GoToPatron);
        Leaf goToOffice = new Leaf("Go to Office", GoToOffice);

        Sequence getPatron = new Sequence("Find a Patron");
        getPatron.AddChild(allocatePatron);

        BehaviourTree waiting = new BehaviourTree();
        waiting.AddChild(patronStillWaiting);

        DepSequence moveToPatron = new DepSequence("Moving to Patron", waiting, agent);
        moveToPatron.AddChild(goToPatron);

        getPatron.AddChild(moveToPatron);

        Selector beWorker = new Selector("Be a worker");
        beWorker.AddChild(getPatron);
        beWorker.AddChild(goToOffice);

        tree.AddChild(beWorker);
    }

    public Node.Status PatronIsWaiting() {

        if (patron == null) return Node.Status.FAILURE;
        if (patron.GetComponent<PatronBehaviour>().isWaiting) return Node.Status.SUCCESS;

        return Node.Status.FAILURE;
    }

    public Node.Status AllocatePatron() {

        if (Blackboard.Instance.patrons.Count == 0) {

            return Node.Status.FAILURE;
        }

        patron = Blackboard.Instance.patrons.Pop();

        if (patron == null) {

            return Node.Status.FAILURE;
        }

        return Node.Status.SUCCESS;
    }

    public Node.Status GoToPatron() {

        if (patron == null) return Node.Status.FAILURE;

        Node.Status s = GoToLocation(patron.transform.position);

        if (s == Node.Status.SUCCESS) {

            patron.GetComponent<PatronBehaviour>().ticket = true;
            patron = null;
        }

        return s;
    }

    public Node.Status GoToOffice() {

        Node.Status s = GoToLocation(office.transform.position);
        patron = null;
        return s;
    }
}
