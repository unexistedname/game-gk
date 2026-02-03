using UnityEngine;
using UnityEngine.UI;

public abstract class MachineBase : MonoBehaviour
{
    [Header("Machine UI")]
    public GameObject machineWindowUI;
    public Button closeButton;

    [Header("Camera Setup")]
    public Transform cameraPosition;

    protected InteractableObject interactableRef;
    protected CameraOrbit cameraOrbit;

    protected virtual void Start(){
        interactableRef = GetComponent<InteractableObject>();
        cameraOrbit = FindAnyObjectByType<CameraOrbit>();

        if (machineWindowUI) machineWindowUI.SetActive(false);
        if (closeButton) closeButton.onClick.AddListener(OnCloseButtonClicked);
    }
    public virtual void EnterInteraction(){
        if (cameraOrbit && cameraPosition != null){
            cameraOrbit.FocusOnTarget(cameraPosition);
        }
        else{
            Debug.LogWarning("Lupa assign 'Camera Position' di inspector mesin!");
        }
        if (machineWindowUI) machineWindowUI.SetActive(true);
        if (interactableRef) interactableRef.HideInfoUI();
    }
    public virtual void OnCloseButtonClicked(){
        if (cameraOrbit) cameraOrbit.ResetFocus();
        if (machineWindowUI) machineWindowUI.SetActive(false);
        if (interactableRef) interactableRef.ShowInfoUI();
    }
}
