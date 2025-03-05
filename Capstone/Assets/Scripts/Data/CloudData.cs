using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class CloudData : MonoBehaviour
{
    public static bool isDone = false;

    private static CloudData instance;

    private void Initiazlize()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);
    }

    [Header("Cloud Inputs")]
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField pwdInput;

    [Space(10), Header("Cloud Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signupButton;

    [Space(10), Header("Cloud Objects")]
    [SerializeField] private GameObject cloudPanel;
    [SerializeField] private GameObject logInPanel;
    [SerializeField] private GameObject cloudWarningPanel;
    [SerializeField] private GameObject canclePanel;
    [SerializeField] private TextMeshProUGUI cloudWarningText;

    string idPattern;
    string pwdPattern;

    private async void Start()
    {
        isDone = false;

        Initiazlize();

        loginButton.onClick.AddListener(
            async () =>
            {
                await OnLoginButton();
            }
            );

        signupButton.onClick.AddListener(
            async () =>
            {
                await OnSignUpButton();
            }
            );

        idPattern = @"^[a-zA-Z0-9][a-zA-Z0-9.,\-_@]{3,20}$";
        //pwdPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,30}$";
        pwdPattern = @"^(?=.*[A - Z])(?=.*[a - z])(?=.*\d)(?=.*[\W_])[A - Za - z\d\W_]{ 8,30}$";

        await InitializeServices();

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    SetCloudWarningPanel(true, "The Internet is not connected");
        //    logInPanel.SetActive(false);
        //}
    }

    public static CloudData Instance()
    {
        return instance;
    }

    async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();
    }

    async Task OnLoginButton()
    {
        SoundManager.OnButtonUp.Invoke();

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    SetCloudWarningPanel(true, "The Internet is not connected");
        //    //logInPanel.SetActive(false);

        //    return;
        //}

        string id = idInput.text;
        string pwd = pwdInput.text;

        if (!Regex.IsMatch(id, idPattern))
        {
            // id ���� ������.
            Debug.Log("Wrong ID Pattern");
            SetCloudWarningPanel(true, "��ȿ���� �ʴ� ID");

            //return;
        }

        if (!Regex.IsMatch(pwd, pwdPattern))
        {
            // pwd ���� ������.
            Debug.Log("Wrong PWD Pattern");
            SetCloudWarningPanel(true, "��ȿ���� �ʴ� ��й�ȣ.");

            //return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(id, pwd);
            //SetCloudWarningPanel(true, "Success.\nPlease press the Save/Load button again.");
            //SetCloudWarningPanel(true, "Success.\nPlease do it again.");
            SetCloudWarningPanel(true, "����.\n�ٽ� �õ����ּ���.");

            logInPanel.SetActive(false);
            canclePanel.SetActive(false);
        }
        catch (AuthenticationException e)
        {
            Debug.Log($"{e.Message}");

            //if (AuthenticationErrorCodes.AccountAlreadyLinked == e.ErrorCode)
            //{
            //    // �̹� ����.

            //    Debug.Log("Already Exist");

            //    SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            //    return;
            //}

            //SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            SetCloudWarningPanel(true, "ID Ȥ�� ��й�ȣ�� �´��� Ȯ��.");

            return;
        }
        catch (RequestFailedException req)
        {
            if (req.ErrorCode == 400)
            {
                Debug.Log("���� �ž� 400");
            }
            if (req.ErrorCode == 401)
            {
                Debug.Log("���� �ž� 401");
            }
            if (req.ErrorCode == 402)
            {
                Debug.Log("���� �ž� 402");
            }
            if (req.ErrorCode == 403)
            {
                Debug.Log("���� �ž� 403");
            }

            // �ùٸ��� Ȯ�� ��Ź.
            //SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            SetCloudWarningPanel(true, "ID Ȥ�� ��й�ȣ�� �´��� Ȯ��.");

            return;
        }
    }

    async Task OnSignUpButton()
    {
        SoundManager.OnButtonUp.Invoke();

        //if (Application.internetReachability == NetworkReachability.NotReachable)
        //{
        //    SetCloudWarningPanel(true, "The Internet is not connected");
        //    logInPanel.SetActive(false);
        //}

        string id = idInput.text;
        string pwd = pwdInput.text;

        if (!Regex.IsMatch(id, idPattern))
        {
            // id ���� ������.
            Debug.Log("Wrong ID Pattern");

            //SetCloudWarningPanel(true, "<< Invalid ID >>\n\nShould be 4 to 20 characters long.\nOnly the following special characters are allowed: ',', '-', '_', '@'.\nMust contain letters and/or numbers.");
            SetCloudWarningPanel(true, "<< ��ȿ���� �ʴ� ID >>\n\n���� : 4~20\n���Ǵ� Ư������ : ',', '-','_', '@'.\n����, ���ڸ� ���.");            

            return;
        }

        if (!Regex.IsMatch(pwd, pwdPattern))
        {
            // pwd ���� ������.
            Debug.Log("Wrong PWD Pattern");

            //SetCloudWarningPanel(true, "<< Invalid Password >>\n\nShould be 8 to 30 characters long.\nContain at least one lowercase and one uppercase letter.\nAnd one symbol");
            SetCloudWarningPanel(true, "<< ��ȿ���� �ʴ� Password >>\n\n���� : 8~30\n�ּ� �ϳ� �̻��� �ҹ��ڿ� �빮��\n�ϳ� �̻��� Ư������.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, pwd);
            await OnLoginButton();
            //SetCloudWarningPanel(true, "Success.\nPlease press the Save/Load button again.");
            //SetCloudWarningPanel(true, "Success.\nPlease do it again.");
            SetCloudWarningPanel(true, "����.\n�ٽ� �õ����ּ���.");

            await AuthenticationService.Instance.UpdatePlayerNameAsync(id);

            logInPanel.SetActive(false);
            canclePanel.SetActive(false);
        }
        catch (AuthenticationException e)
        {
            Debug.Log($"{e.Message}");

            if (AuthenticationErrorCodes.AccountAlreadyLinked == e.ErrorCode)
            {
                // �̹� ����.
                SetCloudWarningPanel(true, "Already Exists");

                //Debug.Log("Already Exists");
                return;
            }
        }
    }

    public void SetLogInPanel(bool active)
    {
        logInPanel.SetActive(active);
    }

    public void SetCloudWarningPanel(bool active, string text)
    {
        cloudWarningText.text = text;

        cloudWarningPanel.SetActive(active);
    }

    public void DelayIsDone()
    {
        StartCoroutine(DelayIsDoneRoutine());
    }

    IEnumerator DelayIsDoneRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        isDone = false;
    }
}
