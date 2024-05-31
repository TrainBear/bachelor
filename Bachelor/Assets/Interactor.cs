using System;
using UnityEngine;

interface IInteractable{
	public string HoverText { get; }
}

interface IInteractableTap : IInteractable{
    public void InteractTap();
}

interface IInteractableHold : IInteractable{
	public void InteractHold();
}

interface IInteractableEnd : IInteractable{
	public void InteractEnd();
}

public class Interactor : MonoBehaviour{
	[SerializeField] private float interactRange;
    [SerializeField] private new Camera camera;
    private terminal myTerminal;
    private IInteractable _interactable;
    private IInteractable _oldInteractable;

    public string HoverText
    {
	    get
	    {
		    if (_interactable is null)
		    {
			    return "";
		    }

		    return _interactable.HoverText;
	    }
    }

    void Update()
    {
		_interactable = GetInteractableHit();

		// Looks away
		if ( && _oldInteractable != _interactable)
		{
			if (Input.GetKey(KeyCode.F) && _oldInteractable is IInteractableEnd oldEndInteractable)
			{
				oldEndInteractable.InteractEnd();
			}
			_oldInteractable = _interactable;
		}
		
	    if (_interactable == null)
	    {
		    return;
	    }

	    // Press down
	    if (Input.GetKeyDown(KeyCode.F) && _interactable is IInteractableTap tapInteractable)
	    {
		    tapInteractable.InteractTap();
	    }

	    // Holds
	    if (Input.GetKey(KeyCode.F) && _interactable is IInteractableHold holdInteractable)
	    {
		    holdInteractable.InteractHold();
	    }
	    
	    // Lets go of key
	    if (Input.GetKeyUp(KeyCode.F) && _interactable is IInteractableEnd endInteractable)
	    {
		    endInteractable.InteractEnd();
	    }
    }

    /// <summary>
    /// TODO: Create terminal interactable script.
    /// </summary>
    private void DoTerminalInteraction()
    {
	    if (Input.GetKey(KeyCode.F))
	    {
		    Ray r = new Ray(camera.transform.position, camera.transform.forward);
		    if (Physics.Raycast(r, out RaycastHit hitInfo, interactRange))
		    {
			    if (hitInfo.collider.gameObject.TryGetComponent(out IInteractableHold interactObj))
			    {
				    if (hitInfo.collider.gameObject.tag == "terminal")
				    {
					    myTerminal = hitInfo.collider.gameObject.GetComponent<terminal>();

					    if ((!playerManager.instance.isTyping) && (!myTerminal.isShowingMap))
					    {
						    playerManager.instance.isTyping = true;
						    myTerminal.startAnimation();
						    myTerminal.showMap();
					    }
				    }
			    }
		    }
		    else
		    {
			    playerManager.instance.isTyping = false;
			    if (myTerminal != null)
			    {
				    myTerminal.stopAnimation();
				    myTerminal.cancelMap();
			    }
		    }
	    }

	    if (Input.GetKeyUp(KeyCode.F))
	    {
		    playerManager.instance.isTyping = false;
		    if (myTerminal != null)
		    {
			    myTerminal.stopAnimation();
			    myTerminal.cancelMap();
		    }
	    }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The interactable that is being looked at, and is in range.</returns>
    private IInteractable GetInteractableHit()
    {
	    Transform cameraTransform = camera.transform;
	    Ray r = new Ray(cameraTransform.position, cameraTransform.forward);
	    bool didHit = Physics.Raycast(r, out RaycastHit hitInfo, interactRange);
	    if (!didHit)
	    {
		    return null;
	    }
	    hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactable);
	    return interactable;
    }
}
