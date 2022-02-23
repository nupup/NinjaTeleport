using UnityEngine;
using Cinemachine;


public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;
	public float slowdownFactor = 0.05f;
	public float slowdownLength = 2f;

    private void Start()
    {
		Instance = this;
	}
    public void DoSlowmotion()
	{
		Time.timeScale = slowdownFactor;
		Time.fixedDeltaTime = Time.timeScale * .02f;
		//make sure the players camera movement does not slow
	}

	public void RemoveSlowMotion()
    {
		Time.timeScale = 1;

	}

}