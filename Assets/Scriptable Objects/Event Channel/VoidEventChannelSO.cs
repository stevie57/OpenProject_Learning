using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Rename this Void Event Channel")]
public class VoidEventChannelSO : EventChannelBaseSO
{
	public UnityAction OnEventRaised;

	public void RaiseEvent()
	{
		if (OnEventRaised != null)
			OnEventRaised.Invoke();
	}
}
