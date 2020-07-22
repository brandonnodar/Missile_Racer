using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using System.Collections.Generic;
using PlayFab.Json;
using TMPro;

public class Playfab_TrackController : MonoBehaviour
{
    public OS_TrackController osTrackController;

    string userEmail;
    string userPassword;
    string username;
    string myID;
    string nameOfStatistic;

    #region PLAYER LOGIN
    public void TryAutoLoginPlayer()
    {
        //Note: Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = "YOUR_GAME_TITLE_ID"; // Please change this value to your own titleId from PlayFab Game Manager
        }

        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");

            var request = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
            osTrackController.RecieveLoginResult(false);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        osTrackController.RecieveLoginResult(true);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        osTrackController.RecieveLoginResult(false);
    }
    #endregion PLAYER LOGIN

    #region UPLOAD STATS
    public void UploadStat(string statisticName, int statValue)
    {
        nameOfStatistic = statisticName;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { Stat = statValue, StatName = statisticName }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudUpdateStats, OnErrorShared);
    }

    public void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {
        // Cloud Script returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // Note how "messageValue" directly corresponds to the JSON values set in Cloud Script

        osTrackController.UploadStatsComplete();
    }
    #endregion UPLOAD STATS

    #region RANDOM TRACK INDEX
    public void GetTrackIndex(string statName)
    {
        string trackIndex = string.Empty;
        PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest()
        {
            PlayFabId = "DATABASE_ID",
        },
        result => {
            if (result.Data.ContainsKey(statName))
                trackIndex = result.Data[statName].Value;

            osTrackController.RecieveCloudTrackIndex(trackIndex);
        },
        error => {
            Debug.Log("Got error getting read-only user data:");
            Debug.Log(error.GenerateErrorReport());
            osTrackController.ConnectToServerFailed();
            print("errorrr!");
        });
    }
    #endregion RANDOM TRACK INDEXS

    #region PLAYER LEADERBOARD POSITION
    public void GetPlayerLeaderboardPosition(string statisticName)
    {
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest()
        {
            MaxResultsCount = 1,
            StatisticName = statisticName,
            PlayFabId = myID
        }, result => ResultPlayerLeaderboardPosition(result), OnPlayFabError);
    }

    void ResultPlayerLeaderboardPosition(GetLeaderboardAroundPlayerResult result)
    {
        osTrackController.RecievePlayerLeaderboardPosition(result.Leaderboard[0].Position);
    }
    #endregion PLAYER LEADERBOARD POSITION

    #region MISSILE DATA
    /// <summary>
    /// Initiates collecting top 5 players, and their missile info.
    /// </summary>
    /// <param name="statisticName">Statistic name.</param>
    public void GetLeaderboardTop5Info(string statisticName)
    {
        switch (statisticName)
        {
            case "TimeTrials":
                var requestTimeTrials = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "TimeTrials", MaxResultsCount = 5 };
                PlayFabClientAPI.GetLeaderboard(requestTimeTrials, ResultTimeTrialsTop5, OnErrorLeaderboard);
                break;
            case "Slalom":
                var requestSlalom = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "Slalom", MaxResultsCount = 5 };
                PlayFabClientAPI.GetLeaderboard(requestSlalom, ResultSlalomTop5, OnErrorLeaderboard);
                break;
        }
    }

    void ResultTimeTrialsTop5(GetLeaderboardResult result)
    {
        List<int> playerStats = new List<int>(0);
        List<string> playerNames = new List<string>(0);
        List<string> playfabIDs = new List<string>(0);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            playerStats.Add(player.StatValue);
            playerNames.Add(player.DisplayName);
            playfabIDs.Add(player.PlayFabId);
        }
        osTrackController.RecieveTimeTrialsTop5Players(playerStats, playerNames, playfabIDs);

        // Collect top 5 missile info (missile body, body color, and thruster).
        GetTimeTrialsTop5Missiles(playfabIDs);
    }

    void ResultSlalomTop5(GetLeaderboardResult result)
    {
        List<int> playerStats = new List<int>(0);
        List<string> playerNames = new List<string>(0);
        List<string> playfabIDs = new List<string>(0);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            playerStats.Add(player.StatValue);
            playerNames.Add(player.DisplayName);
            playfabIDs.Add(player.PlayFabId);
        }
        osTrackController.RecieveSlalomTop5Players(playerStats, playerNames, playfabIDs);

        // Collect top 5 missile info (missile body, body color, and thruster).
        GetSlalomTop5Missiles(playfabIDs);
    }

    void GetTimeTrialsTop5Missiles(List<string> playfabIDs)
    {
        for (int i = 0; i < playfabIDs.Count; i++)
        {
            if (playfabIDs[i] != "D3D3E34579194A6")
            {
                string id = playfabIDs[i];
                PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = playfabIDs[i],
                },
                 result => {
                     if (result.Data.ContainsKey("TimeTrials_MissileInfo")) // Pass data to Class: OS_Controller to store locally.
                     {
                         print("sending to os controller.. playfab id = " + i);
                         osTrackController.RecieveTimeTrialsTop5Missiles(result.Data["TimeTrials_MissileInfo"].Value, id);
                     }

                     else
                         osTrackController.RecieveTimeTrialsTop5Missiles("0-0-0", string.Empty);
                 },
                 error => {
                     Debug.Log("Got error getting read-only user data:");
                     Debug.Log(error.GenerateErrorReport());
                 });
            }
        }
    }

    void GetSlalomTop5Missiles(List<string> playfabIDs)
    {
        for (int i = 0; i < playfabIDs.Count; i++)
        {
            if (playfabIDs[i] != "D3D3E34579194A6")
            {
                string id = playfabIDs[i];
                PlayFabClientAPI.GetUserData(new GetUserDataRequest()
                {
                    PlayFabId = playfabIDs[i],
                },
                result => {
                    if (result.Data.ContainsKey("Slalom_MissileInfo")) // Pass data to Class: OS_Controller to store locally.
                        osTrackController.RecieveSlalomTop5Missile(result.Data["Slalom_MissileInfo"].Value, id);
                    else
                        osTrackController.RecieveSlalomTop5Missile("0-0-0", string.Empty);
                },
                error => {
                    Debug.Log("Got error getting read-only user data:");
                    Debug.Log(error.GenerateErrorReport());
                });
            }
        }
    }
    #endregion MISSILE DATA

    #region UPLOAD PLAYER MISSILE DATA
    public void UploadPlayerMissileData(string statisticName, int missileBodyIndex, int bodyColorIndex, int thrusterIndex)
    {
        string data = missileBodyIndex + "-" + bodyColorIndex + "-" + thrusterIndex;
        bool ready = false;
        string dictName = string.Empty;
        switch (statisticName)
        {
            case "TimeTrials":
                dictName = "TimeTrials_MissileInfo";
                ready = true;
                break;
            case "Slalom":
                dictName = "Slalom_MissileInfo";
                ready = true;
                break;
            default:
                ready = false;
                break;
        }

        if (ready)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
            {
                {dictName, data}
            },
                Permission = UserDataPermission.Public
            }, PlayerMissileDataUploadSuccess, OnErrorLeaderboard);
        }
    }

    void PlayerMissileDataUploadSuccess(UpdateUserDataResult result)
    {
        osTrackController.UploadPlayerMissileDataComplete();
    }
    #endregion UPLOAD PLAYER MISSILE DATA

    #region SUCCESS/ERROR
    public void OnPlayFabError(PlayFabError obj)
    {
        Debug.Log(obj.GenerateErrorReport());
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private static void OnErrorShared(PlayFabError error)
    {
        print(error.GenerateErrorReport());
    }
    #endregion SUCCESS/ERROR

    #region SERVER
    public void GetCurrentServerTime()
    {
        var time = new GetTimeRequest();
        PlayFabClientAPI.GetTime(time, result =>
        {
            //Debug.Log("server time = " + result.Time);
            osTrackController.RecieveCurrentServerTime(result.Time);
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
    }
    #endregion SERVER
