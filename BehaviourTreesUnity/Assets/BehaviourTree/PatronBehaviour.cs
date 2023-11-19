using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatronBehaviour : BTAgent {

    public GameObject[] art;
    public GameObject frontdoor;
    public GameObject homeBase;

    [Range(0, 1000)]
    public int boredom = 0;

    public bool ticket = false;
    public bool isWaiting = false;

    public override void Start() {

        base.Start();
        this.gameObject.GetComponent<NavMeshAgent>().speed = Random.Range(8, 12);
        boredom = Random.Range(20, 80);
        RSelector selectObject = new RSelector("Select Art to View");
        for (int i = 0; i < art.Length; i++) {
            Leaf gta = new Leaf("Go to " + art[i].name, i, GoToArt);
            selectObject.AddChild(gta);
        }

        Leaf goToFrontDoor = new Leaf("Go to Frontdoor", GoToFrontDoor);
        Leaf goToHome = new Leaf("Go Home", GoToHome);
        Leaf isBored = new Leaf("Is Bored?", IsBored);
        Leaf isOpen = new Leaf("Is Open?", IsOpen);

        Sequence viewArt = new Sequence("View Art");
        viewArt.AddChild(isOpen);
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        Leaf noTicket = new Leaf("Wait for Ticket", NoTicket);
        Leaf isWaiting = new Leaf("Waiting for Ticket", IsWaiting);

        BehaviourTree waitForTicket = new BehaviourTree();
        waitForTicket.AddChild(noTicket);

        Loop getTicket = new Loop("Ticket", waitForTicket);
        getTicket.AddChild(isWaiting);

        viewArt.AddChild(getTicket);

        BehaviourTree whileBored = new BehaviourTree();
        whileBored.AddChild(isBored);

        Loop lookAtPaintings = new Loop("Look", whileBored);
        lookAtPaintings.AddChild(selectObject);

        viewArt.AddChild(lookAtPaintings);


        viewArt.AddChild(goToHome);

        BehaviourTree galleryOpenCondition = new BehaviourTree();
        galleryOpenCondition.AddChild(isOpen);
        DepSequence bePatron = new DepSequence("Be an Art Patron", galleryOpenCondition, agent);
        bePatron.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector("View Art with Fallback");
        viewArtWithFallback.AddChild(bePatron);
        viewArtWithFallback.AddChild(goToHome);

        tree.AddChild(viewArtWithFallback);

        StartCoroutine("IncreaseBoredom");
    }

    public Node.Status GoToFrontDoor() {

        Node.Status s = GoToDoor(frontdoor);
        return s;
    }

    IEnumerator IncreaseBoredom() {

        //Debug.Log("Boredom coroutine started");
        while (true) {

            boredom = Mathf.Clamp(boredom + 20, 0, 1000);
            //Debug.Log("Adding to boredom");
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }

    public Node.Status GoToArt(int i) {
        if (!art[i].activeSelf) return Node.Status.FAILURE;
        Node.Status s = GoToLocation(art[i].transform.position);
        if (s == Node.Status.SUCCESS) {

            boredom = Mathf.Clamp(boredom - Random.Range(100, 200), 0, 1000);
        }

        return s;
    }

    public Node.Status GoToHome() {

        Node.Status s = GoToLocation(homeBase.transform.position);
        isWaiting = false;
        return s;
    }

    public Node.Status IsBored() {

        if (boredom < 100) {

            return Node.Status.FAILURE;
        }

        return Node.Status.SUCCESS;
    }

    public Node.Status NoTicket() {

        if (ticket || IsOpen() == Node.Status.FAILURE) {

            return Node.Status.FAILURE;
        }

        return Node.Status.SUCCESS;

    }

    public Node.Status IsWaiting() {

        if (Blackboard.Instance.RegisterPatron(this.gameObject)) {

            isWaiting = true;
            return Node.Status.SUCCESS;
        }

        return Node.Status.FAILURE;
    }
}