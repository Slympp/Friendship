using System.Collections.Generic;
using System.Linq;
using GameManager;
using SimpleFirebaseUnity;
using UnityEngine;

namespace Ranking {
    public class ScoreboardManager : MonoBehaviour {
    
        private Firebase m_Firebase;
        private Firebase m_ScoresBase;
        
        private const string Host = "friendship-75839.firebaseio.com/";
//        private const string Credential = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ2IjowLCJkIjp7InVpZCI6IkdhbWVDbGllbnQifSwiaWF0IjoxNTYwODg2NjUxfQ.Y8QRBKm7GhGjukgziOMb8XJtRnRDE2yuX6bsiRqV5zY";

        private List<Entry> m_Scoreboard;
    
        void Awake() {
            InitRanking();
            GetEntries();
        }

        private void InitRanking() {
            m_Firebase = Firebase.CreateNew(Host);
            
            m_ScoresBase = m_Firebase.Child("scores");
            
            m_ScoresBase.OnGetSuccess += OnGetEntriesSuccess;
            m_ScoresBase.OnGetFailed += OnGetEntriesFailed;

            m_ScoresBase.OnPushSuccess += OnAddSuccess;
            m_ScoresBase.OnPushFailed += OnAddFailed;
        }

        public void GetEntries() {
            m_ScoresBase.GetValue();
        }

        private void OnGetEntriesSuccess(Firebase b, DataSnapshot data) {

            List<Entry> updatedScoreboard = new List<Entry>();

            Dictionary<string, object> scores = data.Value<Dictionary<string, object>>();
            foreach (Dictionary<string, object> entryData in scores.Values) {
                string name = entryData.ContainsKey("name") ? entryData["name"].ToString() : "!ERROR";
                string score = entryData.ContainsKey("score") ? entryData["score"].ToString() : "0";
                string time = entryData.ContainsKey("time") ? entryData["time"].ToString() : "0";

                updatedScoreboard.Add(new Entry {
                    Name = name,
                    Score = int.Parse(score),
                    Time = int.Parse(time)
                });
            }

            if (updatedScoreboard.Any()) {
                updatedScoreboard.Sort((e1, e2) => e2.Score.CompareTo(e1.Score));
                m_Scoreboard = updatedScoreboard;
                GetComponent<UIManager>().UpdateScoreboard(m_Scoreboard);
            }
        }

        private void OnGetEntriesFailed(Firebase b, FirebaseError error) {
            Debug.LogError($"Failed to retrieve entries: {error.Message}");
        }
        
        public void AddEntry(string entryName, int score, int time) {

            Dictionary<string, object> entry = new Dictionary<string, object> {
                {"name", entryName},
                {"score", score},
                {"time", time}
            };
            m_ScoresBase.Push(entry);
        }

        private void OnAddSuccess(Firebase b, DataSnapshot data) {
            Debug.Log($"Entry successfully added {data.RawJson}");
        }

        private void OnAddFailed(Firebase b, FirebaseError error) {
            Debug.LogError($"Failed to add entry: {error.Message}");
        }

        private string GetRandomId() {
            const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
            const int length = 10;

            string id = "";
            for (int i = 0; i < length; i++) {
                id += glyphs[Random.Range(0, glyphs.Length)];
            }
            return id;
        }

        private void LogEntries() {
            int ranking = 1;
            foreach (Entry entry in m_Scoreboard) {
                Debug.Log($"#{ranking++} {entry.Name} - {entry.Score}");
            }
        }

        public struct Entry {
            public string Name;
            public int Score;
            public int Time;
        }
    }
}
