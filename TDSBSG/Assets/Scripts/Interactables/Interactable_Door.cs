using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Door : Interactable {

	[SerializeField]
	Collider interactionTrigger;
	[SerializeField]
	GameObject doorObject;
	[SerializeField]
	GameObject doorObject_2;
	[SerializeField]
	Vector3 openPos;
	[SerializeField]
	Vector3 openRot;
	[SerializeField]
	Vector3 openPos_2;
	[SerializeField]
	Vector3 openRot_2;

	Vector3 closedPos;
	Vector3 closedRot;
	Vector3 closedPos_2;
	Vector3 closedRot_2;
	[SerializeField]
	float animationDuration = 1.0f;
    [SerializeField]
    bool autoClose = true;

    EDoorState state;

	ParticleSystem lightEffect;

	private enum EDoorState {
		IS_WAITING,
		IS_OPENING,
		IS_CLOSING
	}

	private void Start() {
		//permissionList = new List<ERobotType>();

        if(doorObject != null)
        {
            closedPos = doorObject.transform.localPosition;
            closedRot = doorObject.transform.eulerAngles;
        }

        if (doorObject_2 != null)
        {
            closedPos_2 = doorObject_2.transform.localPosition;
            closedRot_2 = doorObject_2.transform.eulerAngles;
        }

		lightEffect = null;
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
		if (startDurationTime >= animationDuration) {
			startDurationTime = animationDuration;
		}
		endDurationTime = animationDuration - startDurationTime;
        if(doorObject != null)
        {
            doorObject.transform.localPosition = Vector3.Lerp(closedPos, openPos, startDurationTime);
            doorObject.transform.eulerAngles = Vector3.Lerp(closedRot, openRot, startDurationTime);
        }

        if (doorObject_2 != null)
        {
            doorObject_2.transform.localPosition = Vector3.Lerp(closedPos_2, openPos_2, startDurationTime);
            doorObject_2.transform.eulerAngles = Vector3.Lerp(closedRot_2, openRot_2, startDurationTime);
        }
	}

	private void Close() {
		endDurationTime += Time.deltaTime;
		if (endDurationTime >= animationDuration) {
			endDurationTime = animationDuration;
		}
		startDurationTime = animationDuration - endDurationTime;
        if(doorObject != null)
        {
            doorObject.transform.localPosition = Vector3.Lerp(openPos, closedPos, endDurationTime);
            doorObject.transform.eulerAngles = Vector3.Lerp(openRot, closedRot, endDurationTime);
        }

        if(doorObject_2 != null)
        {
            doorObject_2.transform.localPosition = Vector3.Lerp(openPos_2, closedPos_2, endDurationTime);
            doorObject_2.transform.eulerAngles = Vector3.Lerp(openRot_2, closedRot_2, endDurationTime);
        }
	}

	private void OnTriggerEnter(Collider other) {
		Debug.Log("enter door");
		if (other.GetComponent(typeof(IPossessable))) {
			IPossessable hitUser = other.GetComponent<IPossessable>();
			if (hitUser.GetIsPossessed()) {
				if (ContainPermissionList(hitUser.GetRobotType())) {
					state = EDoorState.IS_OPENING;
					CreateGreenLightEffect();
					return;
				} else {
					CreateRedLightEffect();
					return;
				}
			}
		}
		else if (other.GetComponent<EnemyBase>()) {
			state = EDoorState.IS_OPENING;
			CreateGreenLightEffect();
			return;
		}

	}

	private void OnTriggerExit(Collider other) {

        if (autoClose)
        {
            if (other.GetComponent(typeof(IPossessable)))
            {
                IPossessable hitUser = other.GetComponent<IPossessable>();
                if (hitUser.GetIsPossessed())
                {
                    if (ContainPermissionList(hitUser.GetRobotType()))
                    {
                        state = EDoorState.IS_CLOSING;
                    }
                }
                OffLightEffect();
            }

            if (other.GetComponent<EnemyBase>())
            {
                state = EDoorState.IS_CLOSING;
                OffLightEffect();
            }
        }
	}

	void CreateGreenLightEffect() {
		if (lightEffect != null) { OffLightEffect(); }
		lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/GreanLight"),
			gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
		lightEffect.Play();
	}
	void CreateRedLightEffect() {
		if (lightEffect != null) { OffLightEffect(); }
		lightEffect = Instantiate(Resources.Load<ParticleSystem>("ParticleEffect/RedLight"),
			gameObject.transform.position + new Vector3(0, 3, 0), Quaternion.identity);
		lightEffect.Play();
	}
	void OffLightEffect() {
        if(lightEffect != null)
        {
            Destroy(lightEffect.gameObject);
        }
	}
}
