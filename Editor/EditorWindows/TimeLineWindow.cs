using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace PlaytestingTool
{
    public class TimeLineWindow : EditorWindow
    {
        List<string> choices = new List<string>();
        int dataFlags = 0;

        List<SessionData> ChoosenSessionDataSets = new List<SessionData>();
        Vector2 GUIOverflow;

        [MenuItem("Tools/PlayTesting Tool/Visualisers/Time Line")]
        static void Init() => GetWindow<TimeLineWindow>("Time Line Visualiser");

        private void OnEnable()
        {
            RefreshData();
        }
        void OnFocus()
        {
            RefreshData();
        }

        public void RefreshData()
        {
            choices = GetSessionDataLib.GetSessionDataChoices(false);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical("box", GUILayout.Width(300));


            if (choices.Count >= 1)
            {
                GUILayout.Label("Choose Session Data:");

                GUILayout.BeginHorizontal("box");
                dataFlags = EditorGUILayout.MaskField(dataFlags, choices.ToArray());

                if (GUILayout.Button("Select"))
                {
                    ChoosenSessionDataSets = GetSessionDataLib.GetChosenSessionData(choices, dataFlags, false);
                    //  Debug.Log(ChoosenSessionDataSets.Count);
                }

                GUILayout.EndHorizontal();
            }
            else
                GUILayout.Label("No Data Collected Yet");

            GUILayout.EndVertical();

            GUIOverflow = GUILayout.BeginScrollView(GUIOverflow, false, false);
            drawTimeline();
            GUILayout.EndScrollView();

        }

        private void drawTimeline()
        {
            GUILayout.BeginHorizontal("box");

            foreach (var sessionData in ChoosenSessionDataSets)
            {
                if (sessionData.trackedProgressions.Count <= 0)
                    continue;


                GUILayout.BeginVertical("box", GUILayout.Width(300));
                GUILayout.Label($"Session {sessionData.dateCreated}");

                List <TrackedProgressionEvent> list = sessionData.trackedProgressions.OrderBy(x => x.timeStamp).ToList();
                for (int i = 0; i < list.Count; i++)
                {
                    TrackedProgressionEvent progressEvent = list[i];

                    DrawLib.DrawUILine(Color.grey);
                    GUILayout.Label($"{FormatTime(progressEvent.timeStamp)} {progressEvent.eventName}", GUILayout.Height(20));
                }

                var q = from x in list
                        group x by x.eventName into g
                        let count = g.Count()
                        orderby count descending
                        select new { Value = g.Key, Count = count };
                foreach (var x in q)
                {
                    DrawLib.DrawUILine(Color.grey);
                    GUILayout.Label($"{x.Value} occurred {x.Count} times", GUILayout.Height(20));

                 
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }


        public string FormatTime(float time)
        {
            int minutes = (int)time / 60;
            int seconds = (int)time - 60 * minutes;
            int milliseconds = (int)(1000 * (time - minutes * 60 - seconds));
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }
}

