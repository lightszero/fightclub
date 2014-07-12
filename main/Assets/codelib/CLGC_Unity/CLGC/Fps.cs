using System;
using System.Collections.Generic;

using System.Text;
using UnityEngine;


class Fps : MonoBehaviour
	{
        float fps = 0;
        float fpstimer = 0;
        int lfc = 0;
        void OnGUI()
        {

            GUI.Label(new Rect(0, 0, 400, 400), "fps=" + fps);// Time.deltaTime.ToString());
        }
        void FixedUpdate()
        {
            fpstimer += Time.fixedDeltaTime;
            if (fpstimer > 1.0f)
            {
                int fc = Time.renderedFrameCount - lfc;
                fps = (float)fc / fpstimer;
                lfc = Time.renderedFrameCount;
                fpstimer -= 1.0f;
            }
        }
	}

