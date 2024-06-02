using UnityEngine;

internal sealed class Interactor : MonoBehaviour{
	/// <summary>
	/// How far away can game objects be interacted with. 
	/// </summary>
	[SerializeField, Range(0, 10)] private float interactRange = 2.5f;
	/// <summary>
	/// Reference to camera for ray-casting.
	/// </summary>
    [SerializeField] private new Camera camera;
	/// <summary>
	/// The key to trigger interaction.
	/// </summary>
	[SerializeField] private KeyCode interactKey = KeyCode.F;
	/// <summary>
	/// Delete me :)
	/// </summary>
    private terminal myTerminal;
	/// <summary>
	/// The current object that is able to be interacted with in this frame.
	/// </summary>
    private IInteractable _interactable;
	/// <summary>
	/// The object that was interactable the last frame.
	/// </summary>
    private IInteractable _oldInteractable;

	/// <summary>
	/// The current hover text that should be displayed this frame.
	/// </summary>
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

		// Interaction end (Player looks away)
		if (_oldInteractable != _interactable)
		{
			// Player is interacting
			if (Input.GetKey(interactKey) && _oldInteractable is IInteractableEnd oldEndInteractable)
			{
				oldEndInteractable.InteractEnd();
			}
			_oldInteractable = _interactable;
		}
		
	    if (_interactable == null)
	    {
		    return;
	    }

	    // Interaction start
	    if (Input.GetKeyDown(interactKey) && _interactable is IInteractableStart tapInteractable)
	    {
		    tapInteractable.InteractStart();
	    }

	    // Interaction hold
	    if (Input.GetKey(interactKey) && _interactable is IInteractableHold holdInteractable)
	    {
		    holdInteractable.InteractHold();
	    }
	    
	    // Interaction end (Player releases key)
	    if (Input.GetKeyUp(interactKey) && _interactable is IInteractableEnd endInteractable)
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
