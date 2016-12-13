using UnityEngine;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class ThirdBaseModule : MonoBehaviour
{
	public KMSelectable[] buttons;
	public Transform[] LEDs;
	public TextMesh Display;

	string[] phrase = new string[28] {"NHXS", "IH6X", "XI8Z", "I8O9", "XOHZ", "H68S", "8OXN", "Z8IX", "SXHN", "6NZH", "H6SI", "6O8I", "NXO8", "66I8", "S89H", "SNZX", "9NZS", "8I99", "ZHOX", "SI9X", "SZN6", "ZSN8", "HZN9", "X9HI", "IS9H", "XZNS", "X6IS", "8NSZ"};


    int[] position = new int[28] {2, 1, 5, 1, 5, 2, 4, 3, 5, 2, 5, 3, 3, 4, 4, 5, 3, 5, 3, 3, 0, 5, 4, 3, 2, 5, 1, 5};
	int[,] wordList = new int[28, 14]  {{ 3, 10,  6, 11,  5,  4, 13,  2,  1,  0,  9,  7,  8, 12},
                                        { 6, 10,  3, 11,  0,  7,  8, 13,  9,  4, 12,  5,  1,  2},
                                        { 0,  3,  4, 11,  9, 13, 10,  7,  2,  8,  5,  6,  1, 12},
                                        {11,  8,  5,  0,  6,  1, 12,  3,  9,  2,  7, 13,  4, 10},
                                        { 6,  4,  9,  1,  2,  7, 11,  8,  3,  5, 12,  0, 13, 10},
                                        { 9,  3,  0, 11,  8, 10,  1,  6, 12,  2,  7,  4, 13,  5},
                                        { 2,  1,  9,  4,  3,  0, 10,  8, 13,  7,  6, 11, 12,  5},
                                        {12, 10,  3, 11,  7, 13,  2,  1,  8,  4,  9,  6,  0,  5},
                                        { 7,  6, 12,  5,  4,  2, 10,  0,  1,  9, 13,  3,  8, 11},
                                        {10,  9,  5,  8, 11,  0,  7,  4,  6, 12, 13,  2,  3,  1},
                                        { 0,  1,  2, 13,  8, 12,  4, 10, 11,  9,  6,  7,  3,  5},
                                        { 7,  2,  3,  4,  1, 13,  8, 12,  9, 11, 10,  5,  6,  0},
                                        { 6,  8,  7,  3,  0,  9,  5, 13,  4, 12,  1,  2, 10, 11},
                                        {10, 11,  0,  2, 13,  3,  1,  6,  7,  9,  5,  4,  8, 12},
                                        {15, 27, 24, 19, 22, 20, 21, 23, 14, 16, 26, 25, 17, 18},
                                        {15, 18, 17, 16, 23, 25, 21, 24, 27, 26, 22, 20, 14, 19},
                                        {27, 17, 18, 22, 24, 15, 20, 25, 19, 16, 21, 26, 23, 14},
                                        {18, 24, 26, 15, 19, 23, 21, 25, 16, 14, 22, 27, 20, 17},
                                        {21, 17, 15, 18, 24, 20, 27, 14, 22, 16, 19, 25, 26, 23},
                                        {16, 25, 22, 18, 14, 23, 21, 26, 17, 15, 20, 24, 19, 27},
                                        {23, 14, 20, 15, 19, 27, 18, 25, 22, 26, 24, 21, 17, 16},
                                        {20, 14, 17, 22, 24, 21, 23, 16, 15, 26, 18, 27, 25, 19},
                                        {16, 22, 20, 24, 21, 17, 14, 18, 19, 15, 27, 23, 26, 25},
                                        {27, 15, 24, 19, 18, 20, 22, 25, 26, 16, 14, 17, 21, 23},
                                        {19, 15, 21, 18, 25, 27, 24, 26, 23, 17, 20, 22, 14, 16},
                                        {17, 14, 23, 21, 16, 20, 27, 19, 22, 24, 25, 15, 18, 26},
                                        {22, 24, 14, 20, 25, 23, 21, 19, 15, 16, 26, 27, 17, 18},
                                        {17, 23, 26, 22, 16, 25, 15, 20, 27, 14, 19, 24, 18, 21}};

    int displayNum;
	int[] buttonNum = new int[6];
    int solution;
	int stage = 0;
	bool isActivated = false;
	bool isPassed = false;

	void Start()
	{
		Init();

		GetComponent<KMBombModule>().OnActivate += ActivateModule;
	}

	void Init()
	{
		for(int i = 0; i < buttons.Length; i++)
		{
			TextMesh buttonTextMesh = buttons[i].GetComponentInChildren<TextMesh>();
			buttonTextMesh.text = " ";
			int j = i;
			buttons[i].OnInteract += delegate () {buttons[j].AddInteractionPunch(0.3f); OnPress(j == solution); return false; };
		}

		for (int i = 0; i < LEDs.Length; i++)
		{
            foreach (MeshRenderer Render in LEDs[i].GetComponentsInChildren<MeshRenderer>())
			{
				if (Render.name == "Off")
				{
					Render.enabled = true;
				}
				else if (Render.name == "On")
                {
					Render.enabled = false;
				}
			}
		}
	}

	void ActivateModule()
	{
		isActivated = true;
		StartCoroutine(NewStage(true));
	}

	void OnPress(bool correctButton)
	{
		GetComponent<KMAudio> ().PlayGameSoundAtTransform (KMSoundOverride.SoundEffect.ButtonPress, transform);

		if (!isPassed)
		{
			if (isActivated)
			{
				if (correctButton)
				{
                    foreach (MeshRenderer Render in LEDs[stage].GetComponentsInChildren<MeshRenderer>())
                    {
                        if (Render.name == "On")
                        {
                            Render.enabled = true;
                        }
                        else if(Render.name == "Off")
                        {
                            Render.enabled = false;
                        }
                    }
                    stage++;
					if (stage == 3)
					{
						GetComponent<KMBombModule> ().HandlePass ();
						isPassed = true;
						stage = 0;
					}
					else
					{
						StartCoroutine(NewStage(false));
					}
				}
				else
				{
					GetComponent<KMBombModule> ().HandleStrike ();
					StartCoroutine(NewStage(false));
				}
			}
		}
	}

	protected IEnumerator NewStage(bool isFirstStage)
	{
		isActivated = false;
		if(!isFirstStage)
		{
			Display.text = " ";
			yield return new WaitForSeconds (1.1f);
			for (int i = 0; i < buttons.Length; i++) {
				TextMesh buttonTextMesh = buttons [i].GetComponentInChildren<TextMesh> ();
				buttonTextMesh.text = " ";
				foreach (MeshRenderer Render in buttons[i].GetComponentsInChildren<MeshRenderer>()) {
					if (Render.name == "Model") {
						Render.enabled = false;
					}
				}
				yield return new WaitForSeconds (0.05f);
			}

			yield return new WaitForSeconds (0.8f);
		}
		yield return new WaitForSeconds (0.3f);
		displayNum = Random.Range(0, 28);

		for(int i = 0; i < buttons.Length; i++)
		{
            bool chosen = false;
            while(!chosen)
            {
                buttonNum[i] = Random.Range(0, 28);
                chosen = true;
                for(int j = 0; j < i; j++)
                {
                    if(buttonNum[j] == buttonNum[i])
                    {
                        chosen = false;
                    }
                }
            }
			TextMesh buttonTextMesh = buttons[i].GetComponentInChildren<TextMesh>();
			buttonTextMesh.text = phrase[buttonNum[i]];
            foreach (MeshRenderer Render in buttons[i].GetComponentsInChildren<MeshRenderer>())
            {
                if (Render.name == "Model")
                {
                    Render.enabled = true;
                }
            }
            yield return new WaitForSeconds (0.05f);
		}

        for(int i = 0; i < 14; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                if(wordList[buttonNum[position[displayNum]], i] == buttonNum[j])
                {
                    solution = j;
                    i = 14;
                    j = 6;
                }
            }
        }
		Display.text = phrase[displayNum];
		isActivated = true;

		Debug.Log(phrase[displayNum]);
        Debug.Log(phrase[buttonNum[position[displayNum]]]);
        Debug.Log(phrase[buttonNum[solution]]);
    }
}
