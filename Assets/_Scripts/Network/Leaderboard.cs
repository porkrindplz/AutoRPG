using System.Collections;
using __Scripts.Systems;
using LootLocker.Requests;
using UnityEngine;
using Logger = _Scripts.Utilities.Logger;
using TMPro;
using UnityEngine.UI;

namespace _Scripts.Network
{
    public class Leaderboard : MonoBehaviour
    {
        private string leaderboardID = "acornsAtEnding";
 
        [SerializeField] TextMeshProUGUI playerNames;
        [SerializeField] TextMeshProUGUI playerScores;
        [SerializeField] private TextMeshProUGUI currentPlayerPlacement;
 
        public IEnumerator SubmitAndFetchRoutine(int score)
        {
            yield return SubmitScoreRoutine(score);
            yield return FetchHighScoresRoutine();
            yield return FetchCurrentPlayerPlacement();
        }
        public IEnumerator SubmitScoreRoutine(int score)
        {
            bool done = false;
            string playerID = PlayerPrefs.GetString("PlayerID");
            LootLockerSDKManager.SubmitScore(playerID,score,leaderboardID, (response) =>
            {
                if (response.success)
                {
                    Logger.Log("Score submitted");
                    done = true;
                }
                else
                {
                    Debug.LogWarning($"Failed. + {response.errorData}");
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
        }

        public IEnumerator FetchHighScoresRoutine()
        {
            bool done = false;
            LootLockerSDKManager.GetScoreList(leaderboardID, 10, 0, (response) =>
            {
                if (response.success)
                {
                    string tempPlayerNames = "PLAYER ID\n";
                    string tempPlayerScores = "ACORNS\n";

                    LootLockerLeaderboardMember[] members = response.items;
                    for (int i = 0; i < members.Length; i++)
                    {
                        if (members[i].player.name != "")
                        {
                            tempPlayerNames += members[i].rank + ". " + members[i].player.name;
                        }
                        else
                        {
                            tempPlayerNames += members[i].rank + ". " + members[i].player.id;
                        }

                        tempPlayerScores += members[i].score + " \n";
                        tempPlayerNames += "\n";
                    }

                    done = true;
                    playerNames.text = tempPlayerNames;
                    playerScores.text = tempPlayerScores;
                }
                else
                {
                    Logger.Log($"Failed. + {response.errorData}");
                    done = true;
                }

            });
            yield return new WaitWhile(() => done == false);
        }

        public IEnumerator FetchCurrentPlayerPlacement()
        {
            bool done = false;
            string playerID = PlayerPrefs.GetString("PlayerID");
            LootLockerSDKManager.GetMemberRank(leaderboardID, playerID, (response) =>
            {
                if (response.success)
                {
                    currentPlayerPlacement.text = $"Your top placement is {response.rank.ToString()} with {response.score.ToString()} acorns.";
                    done = true;
                }
                else
                {
                    Logger.Log($"Failed. + {response.errorData}");
                    currentPlayerPlacement.text ="No personal score ranking found.";
                    done = true;
                }
            });
            yield return new WaitWhile(() => done == false);
        }
        public void CloseLeaderboard()
        {
            AudioSystem.Instance.PlayMenuSelectSound();
            gameObject.SetActive(false);
        }
    }
}
