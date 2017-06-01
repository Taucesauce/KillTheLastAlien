using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EventTest : MonoBehaviour {

    private UnityAction someListener;
	
    void Awake() {
        someListener = new UnityAction(SomeFunction);
    }

    void OnEnable() {
        EventManager.StartListening("Left", someListener);
        EventManager.StartListening("Middle", SomeOtherFunction);
        EventManager.StartListening("Right", SomeThirdFunction);
    }

    void OnDisable() {
        EventManager.StopListening("Left", someListener);
        EventManager.StopListening("Middle", SomeOtherFunction);
        EventManager.StopListening("Right", SomeThirdFunction);
    }

    void SomeFunction() {
        Debug.Log("Left");
    }

    void SomeOtherFunction() {
        Debug.Log("Middle");
    }

    void SomeThirdFunction() {
        Debug.Log("Right");
    }
}
