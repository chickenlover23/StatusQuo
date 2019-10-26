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



    public void FillQuestionPopUp(Question temp)
    {
        openedFromNotificationsPanel = false;
        activeQuestion = temp;

        questionQuestion.text = activeQuestion.question;
        questionA.text = activeQuestion.a;
        questionB.text = activeQuestion.b;
        questionPopUp.SetActive(true);
    }

    public void FillQuestionPopUp()
    {
        openedFromNotificationsPanel = true;
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

            temp.GetComponent<Button>().onClick.AddListener(delegate { FillQuestionPopUp(); });
        }
        openedFromNotificationsPanel = false;
    }


    public void AnswerQuestion(int ans)
    {
        StartCoroutine(answerToQuestion(activeQuestion, ans));
    }


    IEnumerator answerToQuestion(Question q, int ans)
    {
        WWWForm form = new WWWForm();
        form.AddField("quest_id", q.questionId);
        form.AddField("ans", ans);

        UnityWebRequest webRequest = UnityWebRequest.Post(All_Urls.getUrl().answerQuestion, form);
        webRequest.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("access_token"));

        yield return webRequest.SendWebRequest();


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
                Destroy(q.gameObject);
            }
            questionPopUp.SetActive(false);
            GetComponent<Toast>().ShowToast(data["message"].ToString());
            openedFromNotificationsPanel = false;
        }
    }
}






