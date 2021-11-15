using UnityEngine;
using UnityEngine.UI;

public class HitpointsView : MonoBehaviour
{
	public RectTransform Container;
	public RectTransform Line;
	public Text PlayerName;
	
	public void SetValue(float value)
	{
		Vector2 v = Container.sizeDelta;
		v.x = Mathf.Max(0.0f, v.x * value);
		Line.sizeDelta = v;
	}
}