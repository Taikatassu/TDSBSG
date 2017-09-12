using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intractable_Door : Interactable {

    [SerializeField]
    Collider interactionTrigger;
    [SerializeField]
    GameObject doorObject;
    [SerializeField]
    Vector3 openPos;
    [SerializeField]
    Vector3 openRot;

    Vector3 closedPos;
    Vector3 closedRot;
    [SerializeField]
    float animationDuration = 1.0f;

    private void Start() {
        permissionList = new List<ERobotType>();
        permissionList.Add(ERobotType.SMALL);

        closedPos = doorObject.transform.position;
        closedRot = doorObject.transform.eulerAngles;
    }

    protected override float InteractionStartDuration() {
        return startDurationTime;
    }

    protected override float InteractionEndDuration() {
        return endDurationTime;
    }

    public override float StartInteraction(IPossessable user) {
        if (base.StartInteraction(user) == -1.0f) { return -1.0f; }

        if (user.GetGameObject().GetComponent<Poss_Mobile>()) {
            Poss_Mobile userMobile = user.GetGameObject().GetComponent<Poss_Mobile>();
            Vector3 userPostion = userMobile.transform.position;
            //userMobile.SetDestination()
        }
        return startDurationTime;
    }

    public override float EndInteraction(IPossessable user) {
        if (base.EndInteraction(user) == -1.0f) { return -1.0f; }

        return endDurationTime;
    }

    private void Open() {

    }

    private void Close() {

    }
}
