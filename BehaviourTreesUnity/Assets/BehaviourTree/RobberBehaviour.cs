//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent {
    public GameObject diamond;
    public GameObject painting;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    public GameObject cop;

    public GameObject[] art;

    GameObject pickup;

    [Range(0, 1000)]
    public int money = 800;

    Leaf goToBackDoor;
    Leaf goToFrontDoor;

    // Start is called before the first frame update
    public override void Start() {
        base.Start();

        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);
        Leaf hasGotMoney = new Leaf("Has Got Money", HasMoney);

        RSelector selectObject = new RSelector("Select Object to Steal");
        for (int i = 0; i < art.Length; i++) {
            Leaf gta = new Leaf("Go to " + art[i].name, i, GoToArt);
            selectObject.AddChild(gta);
        }

        goToBackDoor = new Leaf("Go To Backdoor", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To Frontdoor", GoToFrontDoor, 1);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PSelector opendoor = new PSelector("Open Door");

        Sequence runAway = new Sequence("Run Away");
        Leaf canSee = new Leaf("Can See Cop?", CanSeeCop);
        Leaf flee = new Leaf("Flee From Cop", FleeFromCop);

        Inverter invertMoney = new Inverter("Invert Money");
        invertMoney.AddChild(hasGotMoney);

        opendoor.AddChild(goToFrontDoor);
        opendoor.AddChild(goToBackDoor);

        runAway.AddChild(canSee);
        runAway.AddChild(flee);

        Inverter cantSeeCop = new Inverter("Cant See Cop");
        cantSeeCop.AddChild(canSee);

        Sequence s1 = new Sequence("s1");
        s1.AddChild(invertMoney);
        Sequence s2 = new Sequence("s2");
        s2.AddChild(cantSeeCop);
        s2.AddChild(opendoor);
        Sequence s3 = new Sequence("s3");
        s3.AddChild(cantSeeCop);
        s3.AddChild(selectObject);
        Sequence s4 = new Sequence("s4");
        s4.AddChild(cantSeeCop);
        s4.AddChild(goToVan);

        /*steal.AddChild(s1);
        steal.AddChild(s2);
        steal.AddChild(s3);
        steal.AddChild(s4);*/
        Leaf isOpen = new Leaf("Is Open", IsOpen);
        Inverter isClosed = new Inverter("Is Closed");
        isClosed.AddChild(isOpen);

        BehaviourTree stealConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Stealing Conditions");
        conditions.AddChild(isClosed);
        conditions.AddChild(cantSeeCop);
        conditions.AddChild(invertMoney);
        stealConditions.AddChild(conditions);
        DepSequence steal = new DepSequence("Steal Something", stealConditions, agent);
        //steal.AddChild(invertMoney);
        steal.AddChild(opendoor);
        steal.AddChild(selectObject);
        steal.AddChild(goToVan);

        Selector stealWithFallback = new Selector("Steal with Fallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);

        Selector beThief = new Selector("Be a thief");
        beThief.AddChild(stealWithFallback);
        beThief.AddChild(runAway);

        tree.AddChild(beThief);

        tree.PrintTree();

        StartCoroutine("DecreaseMoney");
    }

    IEnumerator DecreaseMoney() {
        while (true) {

            money = Mathf.Clamp(money - 50, 0, 1000);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }

    public Node.Status CanSeeCop() {

        return CanSee(cop.transform.position, "Cop", 10, 90);
    }

    public Node.Status FleeFromCop() {

        return Flee(cop.transform.position, 10);
    }

    public Node.Status HasMoney() {

        if (money < 500)
            return Node.Status.FAILURE;
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToDiamond() {
        if (!diamond.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(diamond.transform.position);
        if (s == Node.Status.SUCCESS) {
            diamond.transform.parent = this.gameObject.transform;
            pickup = diamond;
        }
        return s;
    }

    public Node.Status GoToPainting() {
        if (!painting.activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(painting.transform.position);
        if (s == Node.Status.SUCCESS) {
            painting.transform.parent = this.gameObject.transform;
            pickup = painting;
        }
        return s;
    }

    public Node.Status GoToArt(int i) {
        if (!art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(art[i].transform.position);
        if (s == Node.Status.SUCCESS) {
            art[i].transform.parent = this.gameObject.transform;
            pickup = art[i];
        }
        return s;
    }

    public Node.Status GoToBackDoor() {
        Node.Status s = GoToDoor(backdoor);
        if (s == Node.Status.FAILURE)
            goToBackDoor.sortOrder = 10;
        else
            goToBackDoor.sortOrder = 1;
        return s;
    }

    public Node.Status GoToFrontDoor() {
        Node.Status s = GoToDoor(frontdoor);
        if (s == Node.Status.FAILURE)
            goToFrontDoor.sortOrder = 10;
        else
            goToFrontDoor.sortOrder = 1;
        return s;
    }

    public Node.Status GoToVan() {
        Node.Status s = GoToLocation(van.transform.position);
        if (s == Node.Status.SUCCESS) {
            if (pickup != null) {
                money += 300;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return s;
    }



}
