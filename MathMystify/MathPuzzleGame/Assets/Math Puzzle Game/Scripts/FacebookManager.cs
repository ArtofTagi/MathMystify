using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;
using System.Collections.Generic;

public class FacebookManager : MonoBehaviour {

    //   public GameObject leaderboardContainer;
    //   public GameObject leaderboardPrefab;
    //
    //   //Friends at current level
    //  public Image friend1;
    // public Image friend2;
    //   public Image friend3;
    // public Image friend4;

    public Image friend1;
    public Image friend2;
    public Image friend3;
    public Image friend4;

    public GameObject ScoreEntryPanel;
    public GameObject ScoreScrollList;

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            Debug.Log("INITIALIZING");
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            Debug.Log("ACTIVATING");
            FB.ActivateApp();
        }
    }
    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...

        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }
    private static readonly List<string> readPermissions = new List<string> { "public_profile", "user_friends" };
    private static readonly List<string> publishPermissions = new List<string> { "publish_actions" };

    // Prompt the player to authenticate Friend Smash! with Facebook Login
    //
    // By default FB.LogInWithReadPermissions will attempt to authenticate the user with only
    // the basic permissions. If you need one or more additional permissions, pass in a list
    // of the permissions you wish to request from the player.
    //
    // In Friend Smash, only user_friends is required to enable access to friends, so that the game can show friends's
    // profile picture to make the experience more personal and engaging.
    //
    public static void PromptForLogin()
    {
        // Login for read permissions
        // https://developers.facebook.com/docs/unity/reference/current/FB.LogInWithReadPermissions
        FB.LogInWithReadPermissions(readPermissions, delegate (ILoginResult result)
        {
            Debug.Log("LoginCallback");
            if (FB.IsLoggedIn)
            {
                Debug.Log("Logged in with ID: " + AccessToken.CurrentAccessToken.UserId +
                          "\nGranted Permissions: " + AccessToken.CurrentAccessToken.Permissions.ToCommaSeparateList());
            }
            else
            {
                if (result.Error != null)
                {
                    Debug.LogError(result.Error);
                }
                Debug.Log("Not Logged In");
            }
        });
    }

    // Prompt the player to grant publish permissions with Facebook Login
    // Publish permissions allow seemless publishing of content to Facebook on behalf of a player,
    // such as open graph stories, scores, and achievements.
    // https://developers.facebook.com/docs/facebook-login/permissions#reference-publish_actions
    // 
    // Access to the 'publish_actions' permissions requires that your app goes through a lightweight review with Facebook
    // See more about Login Review here: https://developers.facebook.com/docs/facebook-login/review/faqs#what_is_review
    //
    // Reminder: 'publish_actions' is not required for FB.ShareLink or FB.FeedShare, it is only required for API based sharing
    //
    public void PromptForPublish()
    {
        // Login for publish permissions
        // https://developers.facebook.com/docs/unity/reference/current/FB.LogInWithPublishPermissions
        FB.LogInWithPublishPermissions(publishPermissions, AuthCallback);
    }
    public void login()
    {
        PromptForLogin();
        PromptForPublish();
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            SetScore(PlayerPrefs.GetInt("BestLevel", 0));
            QueryScores();
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }

    // Use this for initialization
    void Start () {
	
	}

    private List<object> scoresList = null;

    private int popLvl = 0;
   
    // All Scores API related Things

    public void QueryScores()
	{
		FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, ScoresCallback);
	}

	private void ScoresCallback(IResult result)
	{
		Debug.Log ("Scores callback: " + result);


        var scoresList = new List<object>();

        object scoresh;
        if (result.ResultDictionary.TryGetValue("data", out scoresh))
        {
            scoresList = (List<object>)scoresh;
        }

		foreach (Transform child in ScoreScrollList.transform) 
		{
			GameObject.Destroy (child.gameObject);
		}

		foreach (object score in scoresList) 
		{

			var entry = (Dictionary<string,object>) score;
			var user = (Dictionary<string,object>) entry["user"];




			GameObject ScorePanel;
			ScorePanel = Instantiate (ScoreEntryPanel) as GameObject;
			ScorePanel.transform.parent = ScoreScrollList.transform;

			Transform ThisScoreName = ScorePanel.transform.Find ("FriendName");
			Transform ThisScoreScore = ScorePanel.transform.Find ("FriendScore");
			Text ScoreName = ThisScoreName.GetComponent<Text>();
			Text ScoreScore = ThisScoreScore.GetComponent<Text>();

			ScoreName.text = user["name"].ToString();
			ScoreScore.text = entry["score"].ToString();

			Transform TheUserAvatar = ScorePanel.transform.Find ("FriendAvatar");
			Image UserAvatar = TheUserAvatar.GetComponent<Image>();


			FB.API (Util.GetPictureURL(user["id"].ToString (), 128,128), HttpMethod.GET, delegate(IGraphResult pictureResult){

				if(pictureResult.Error != null) // if there was an error
				{
					Debug.Log (pictureResult.Error);
				}
				else // if everything was fine
				{
					UserAvatar.sprite = Sprite.Create (pictureResult.Texture, new Rect(0,0,128,128), new Vector2(0,0));
				}

			});
		}
	}

	public void SetScore(int score)
	{
        Debug.Log("SET SCORE CALLED " + score.ToString());
		var scoreData = new Dictionary<string,string> ();
		scoreData ["score"] = score.ToString();
		FB.API ("/me/scores", HttpMethod.POST, delegate(IGraphResult result) {
			Debug.Log ("Score submit result: " + result);
		}, scoreData);
	}
}