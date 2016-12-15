using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PreTouch : MonoBehaviour {

	public Image fullScreen;
	public RawImage cursor, pic;
	public Button prevButton, nextButton, showButton;
	public GameObject buttons;

	bool showCursor = true;
	float screenWidth, screenHeight, ratio_x, ratio_y, size;
	int picID = 0;

	const float PHONE_HEIGHT = 11.3f;
	const float PHONE_WIDTH = 6.4f;
	const float PHONE_DELTA = 5.0f;
	const float TO_SCREEN_DIST = 2.0f;
	const float AWAY_SCREEN_DIST = 8.0f;

	const float SUSPEND_TIME = 0.2f;
	const float DISPLAY_TIME = 0.5f;
	const float SUSPEND_DIST = 2.0f;

	public enum Phase
	{
		None = 0,
		Suspend = 1,
		Display = 2,
	};

	Vector2 now, pre;
	float t;
	Phase phase = Phase.None;

	// Use this for initialization
	void Start() {
		screenWidth = fullScreen.rectTransform.rect.width;
		screenHeight = fullScreen.rectTransform.rect.height;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		prevButton.onClick.AddListener(PrevPic);
		nextButton.onClick.AddListener(NextPic);
		showButton.onClick.AddListener(ChangeShowCursor);
	}
	
	public void Move(Vector3 dot) {
		dot.x -= PHONE_DELTA;
		now.x = dot.x; now.y = dot.y;
		ratio_y = -(dot.x / PHONE_HEIGHT - 0.5f);
		ratio_x = dot.y / PHONE_WIDTH - 0.5f;
		if (dot.z < 0 || dot.z >= AWAY_SCREEN_DIST) 
			size = 0;
		else if (dot.z <= TO_SCREEN_DIST) 
			size = 1;
		else
			size = (AWAY_SCREEN_DIST - dot.z) / (AWAY_SCREEN_DIST - TO_SCREEN_DIST);
	}

	void ChangeShowCursor() {
		showCursor ^= true;
	}

	void PrevPic() {
		picID = (picID + 7) % 8;
		pic.texture = (Texture)Resources.Load("Pictures/" + picID.ToString(), typeof(Texture));
	}

	void NextPic() {
		picID = (picID + 1) % 8;
		pic.texture = (Texture)Resources.Load("Pictures/" + picID.ToString(), typeof(Texture));
	}

	void Update() {
		if (showCursor)
			cursor.transform.localPosition = new Vector3(ratio_x * screenWidth, ratio_y * screenHeight, 0f);
		else
			cursor.transform.localPosition = new Vector3(-1.0f * screenWidth, -1.0f * screenHeight, 0f);
		cursor.transform.localScale = new Vector3(size, size, size);
		if (phase == Phase.None) {
			if (0.4f <= ratio_x && ratio_x <= 0.6f && -0.5f <= ratio_y && ratio_y <= 0.5f && size > 0) {
				phase = Phase.Suspend;
				t = Time.time;
				pre = now;
			}
		} else if (phase == Phase.Suspend) {
			if (size > 0 && Vector2.Distance(now, pre) <= SUSPEND_DIST) {
				if (Time.time - t >= SUSPEND_TIME) {
					phase = Phase.Display;
					buttons.transform.localPosition = new Vector3(0.44f * screenWidth, ratio_y * screenHeight, 0f);
					pre = now;
					t = Time.time;
				}
			}
			else phase = Phase.None;
		} else if (phase == Phase.Display) {
			if (size > 0 && Vector2.Distance(now, pre) <= SUSPEND_DIST) {
				t = Time.time;
			}
			else if (Time.time - t >= DISPLAY_TIME) {
				phase = Phase.None;
				buttons.transform.localPosition = new Vector3(-1.0f * screenWidth, -1.0f * screenHeight, 0f);
			}

		}

	}
}
