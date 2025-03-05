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
            // id 조건 부적합.
            Debug.Log("Wrong ID Pattern");
            SetCloudWarningPanel(true, "유효하지 않는 ID");

            //return;
        }

        if (!Regex.IsMatch(pwd, pwdPattern))
        {
            // pwd 조건 부적합.
            Debug.Log("Wrong PWD Pattern");
            SetCloudWarningPanel(true, "유효하지 않는 비밀번호.");

            //return;
        }

        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(id, pwd);
            //SetCloudWarningPanel(true, "Success.\nPlease press the Save/Load button again.");
            //SetCloudWarningPanel(true, "Success.\nPlease do it again.");
            SetCloudWarningPanel(true, "성공.\n다시 시도해주세요.");

            logInPanel.SetActive(false);
            canclePanel.SetActive(false);
        }
        catch (AuthenticationException e)
        {
            Debug.Log($"{e.Message}");

            //if (AuthenticationErrorCodes.AccountAlreadyLinked == e.ErrorCode)
            //{
            //    // 이미 존재.

            //    Debug.Log("Already Exist");

            //    SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            //    return;
            //}

            //SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            SetCloudWarningPanel(true, "ID 혹은 비밀번호가 맞는지 확인.");

            return;
        }
        catch (RequestFailedException req)
        {
            if (req.ErrorCode == 400)
            {
                Debug.Log("없는 거야 400");
            }
            if (req.ErrorCode == 401)
            {
                Debug.Log("없는 거야 401");
            }
            if (req.ErrorCode == 402)
            {
                Debug.Log("없는 거야 402");
            }
            if (req.ErrorCode == 403)
            {
                Debug.Log("없는 거야 403");
            }

            // 올바른지 확인 부탁.
            //SetCloudWarningPanel(true, "Check if the ID or Password is correct");
            SetCloudWarningPanel(true, "ID 혹은 비밀번호가 맞는지 확인.");

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
            // id 조건 부적합.
            Debug.Log("Wrong ID Pattern");

            //SetCloudWarningPanel(true, "<< Invalid ID >>\n\nShould be 4 to 20 characters long.\nOnly the following special characters are allowed: ',', '-', '_', '@'.\nMust contain letters and/or numbers.");
            SetCloudWarningPanel(true, "<< 유효하지 않는 ID >>\n\n길이 : 4~20\n허용되는 특수문자 : ',', '-','_', '@'.\n문자, 숫자만 허용.");            

            return;
        }

        if (!Regex.IsMatch(pwd, pwdPattern))
        {
            // pwd 조건 부적합.
            Debug.Log("Wrong PWD Pattern");

            //SetCloudWarningPanel(true, "<< Invalid Password >>\n\nShould be 8 to 30 characters long.\nContain at least one lowercase and one uppercase letter.\nAnd one symbol");
            SetCloudWarningPanel(true, "<< 유효하지 않는 Password >>\n\n길이 : 8~30\n최소 하나 이상의 소문자와 대문자\n하나 이상의 특수문자.");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, pwd);
            await OnLoginButton();
            //SetCloudWarningPanel(true, "Success.\nPlease press the Save/Load button again.");
            //SetCloudWarningPanel(true, "Success.\nPlease do it again.");
            SetCloudWarningPanel(true, "성공.\n다시 시도해주세요.");

            await AuthenticationService.Instance.UpdatePlayerNameAsync(id);

            logInPanel.SetActive(false);
            canclePanel.SetActive(false);
        }
        catch (AuthenticationException e)
        {
            Debug.Log($"{e.Message}");

            if (AuthenticationErrorCodes.AccountAlreadyLinked == e.ErrorCode)
            {
                // 이미 존재.
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
