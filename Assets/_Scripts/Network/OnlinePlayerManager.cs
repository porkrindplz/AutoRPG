using System.Collections;
using LootLocker.Requests;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;

namespace _Scripts.Network
{
    public class OnlinePlayerManager : MonoBehaviour
    {
    
        public Leaderboard leaderboard;
        private void Start()
        {
            StartCoroutine(SetupRoutine());
        }

        IEnumerator SetupRoutine()
        {
            yield return LoginRoutine();
            yield return leaderboard.FetchHighScoresRoutine();
            yield return leaderboard.FetchCurrentPlayerPlacement();
        }

        IEnumerator LoginRoutine()
        {
            bool done = false;
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                if (response.success)
                {
                    Logger.Log("Player was logged in");
                    PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                    done = true;
                }
                else
                {
                    Debug.LogError("Could not start session");
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
        }
    }
}
