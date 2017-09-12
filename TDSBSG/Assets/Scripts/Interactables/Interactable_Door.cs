using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Door : Interactable {

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

    EDoorState state;

    private enum EDoorState {
        IS_WAITING,
        IS_OPENING,
        IS_CLOSING
    }

    private void Start() {
        permissionList = new List<ERobotType>();
        permissionList.Add(ERobotType.DEFAULT);
        permissionList.Add(ERobotType.SMALL);

        closedPos = doorObject.transform.position;
        closedRot = doorObject.transform.eulerAngles;
    }

    private void Update() {
        if (state == EDoorState.IS_OPENING) {
            Open();
        } else if (state == EDoorState.IS_CLOSING) {
            Close();
        }
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
        }
        return startDurationTime;
    }

    public override float EndInteraction(IPossessable user) {
        if (base.EndInteraction(user) == -1.0f) { return -1.0f; }

        return endDurationTime;
    }

    private void Open() {

        startDurationTime += Time.deltaTime;
        if(startDurationTime >= animationDuration) {
            startDurationTime = animationDuration;
        }
        endDurationTime = animationDuration - startDurationTime;
        doorObject.transform.position = Vector3.Lerp(closedPos, openPos, startDurationTime);
        doorObject.transform.eulerAngles = Vector3.Lerp(closedRot, openRot, startDurationTime);
    }

    private void Close() {
        endDurationTime += Time.deltaTime;
        if (endDurationTime >= animationDuration) {
            endDurationTime = animationDuration;
        }
        startDurationTime = animationDuration - endDurationTime;
        doorObject.transform.position = Vector3.Lerp(openPos, closedPos, endDurationTime);
        doorObject.transform.eulerAngles = Vector3.Lerp(openRot, closedRot, endDurationTime);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("enter door");
        if (!other.GetComponent(typeof(IPossessable))) {
            return;
        }
        IPossessable user = other.GetComponent<IPossessable>();
        if(!user.GetIsPossessed()) {
            return;
        }
        if (ContainPermissionList(user.GetRobotType())) {
            state = EDoorState.IS_OPENING;
        }
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("exit door");
        if (!other.GetComponent(typeof(IPossessable))) {
            return;
        }
        IPossessable user = other.GetComponent<IPossessable>();
        if (!user.GetIsPossessed()) {
            return;
        }
        if (ContainPermissionList(user.GetRobotType())) {
            state = EDoorState.IS_CLOSING;
        }
    }
}
