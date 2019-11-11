using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using UnityEngine.Networking;

public class QuestionManager : MonoBehaviour
{
    public Transform notificationPanelParent;
    public GameObject notificationPanelPrefab;

    public GameObject questionPopUp;
    public TMP_Text questionQuestion, questionA, questionB;
    public TMP_Text bronzeBar;


    Question activeQuestion = new Question();
    bool openedFromNotificationsPanel = false;

    bool answerToQuestionWorking;
    Dictionary<int, Question> questionsInNotificationPanel = new Dictionary<int, Question>();

    int counter = 0;


    public void FillQuestionPopUp(Question temp)
    {
        if (questionPopUp.activeSelf)
        {
            AddQuestionToNotificationPanel();
        }
        openedFromNotificationsPanel = false;
        activeQuestion = temp;

        questionQuestion.text = activeQuestion.question;
        questionA.text = activeQuestion.a;
        questionB.text = activeQuestion.b;
        questionPopUp.SetActive(true);
    }

    public void FillQuestionPopUp(int ind)
    {
        Debug.Log(ind);
        openedFromNotificationsPanel = true;
        activeQuestion = questionsInNotificationPanel[ind];
        questionQuestion.text = activeQuestion.question;
        questionA.text = activeQuestion.a;
        questionB.text = activeQuestion.b;
        questionPopUp.SetActive(true);
    }


    public void AddQuestionToNotificationPanel()
    {
        if (!activeQuestion.addedToNotificationPanel)
        {
            var temp = Instantiate(notificationPanelPrefab, notificationPanelParent);
            temp.transform.Find("Text_taskDescription").GetComponent<TMP_Text>().text = activeQuestion.question;
            temp.GetComponent<Question>().questionId = activeQuestion.questionId;
            temp.GetComponent<Question>().question = activeQuestion.question;
            temp.GetComponent<Question>().a = activeQuestion.a;
            temp.GetComponent<Question>().b = activeQuestion.b;
            temp.GetComponent<Question>().addedToNotificationPanel = true;
            activeQuestion = temp.GetComponent<Question>();
            activeQuestion.keyInNotificationPanelDictionary = counter;
            questionsInNotificationPanel[counter] = activeQuestion;

            int indd = counter;

            counter++;
            temp.GetComponent<Button>().onClick.AddListener(delegate { FillQuestionPopUp(indd); }); 
        }
        openedFromNotificationsPanel = false;
        StartCoroutine(GetComponent<Manager_Game>().changeMenuSprite());
    }


    public void AnswerQuestion(int ans)
    {
        if (!answerToQuestionWorking)
        {
            StartCoroutine(answerToQuestion(activeQuestion, ans));
        }
    }


    IEnumerator answerToQuestion(Question q, int ans)
    {
        answerToQuestionWorking = true;

        WWWForm form = new WWWForm();
        form.AddField("quest_id", q.questionId);
        form.AddField("ans", ans);

        UnityWebRequest webRequest = UnityWebRequest.Post(All_Urls.getUrl().answerQuestion, form);
        webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return webRequest.SendWebRequest();

        try
        {
            if (webRequest.error != null || webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(webRequest.downloadHandler.text);

                if (data["status"].ToString() == "success")
                {
                    GetComponent<Manager_Game>().AddToNumber(bronzeBar, int.Parse(data["bronze"].ToString()) - int.Parse(bronzeBar.text));
                }

                if (openedFromNotificationsPanel)
                {
                    //questionsInNotificationPanel.RemoveAt(q.keyInNotificationPanelDictionary);
                    questionsInNotificationPanel.Remove(q.keyInNotificationPanelDictionary);

                    Destroy(q.gameObject);
                    StartCoroutine(GetComponent<Manager_Game>().changeMenuSprite());
                }
                questionPopUp.SetActive(false);
                GetComponent<Toast>().ShowToast(data["message"].ToString());
                openedFromNotificationsPanel = false;
            }
            answerToQuestionWorking = false;
        }
        catch
        {
            answerToQuestionWorking = false;
        }
    }
}






