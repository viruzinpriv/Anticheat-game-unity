using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

public class AntiCheatManager : MonoBehaviour
{
    private string originalHash = "ABCD1234...";
    private string[] appsProibidos = { "org.gameguardian", "com.cheatengine", "com.luckypatcher" };

    private int pontosReais = 100;
    private int pontosEspelhados = 100;
    private int score = 0;
    private int scoreFirebase = 0;

    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    void Start()
    {
        InitializeFirebase();

        VerificarAppsProibidos();
        if (!VerificarIntegridade())
            ReportCheat("APK Modificado");

        if (IsDeviceRooted())
            ReportCheat("Root Detectado");
    }

    void Update()
    {
        DetectarSpeedHack();
        DetectarTimeManipulation();
        DetectarMemoryHack();
        DetectarScoreHack();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;

                auth.SignInAnonymouslyAsync().ContinueWith(t =>
                {
                    if (t.IsCompleted && !t.IsFaulted)
                        Debug.Log("Login anônimo feito com sucesso");
                });
            }
            else
            {
                Debug.LogError("Firebase não disponível: " + task.Result);
            }
        });
    }

    void ReportCheat(string motivo)
    {
        Debug.LogWarning("TRAPAÇA DETECTADA: " + motivo);

        string userId = auth?.CurrentUser?.UserId ?? "desconhecido";
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        dbRef.Child("cheaters").Child(userId).Push().SetValueAsync(new
        {
            motivo = motivo,
            horario = timestamp,
            dispositivo = SystemInfo.deviceModel
        });

        AplicarPenalidade(motivo);
    }

    void AplicarPenalidade(string motivo)
    {
        Debug.LogError("Jogo bloqueado por trapaça: " + motivo);
#if UNITY_ANDROID
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
            .GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call("finish");
#else
        Application.Quit();
#endif
    }

    void DetectarSpeedHack()
    {
        if (Time.timeScale > 1.0f)
            ReportCheat("SpeedHack");
    }

    void DetectarTimeManipulation()
    {
        if (Time.deltaTime < 0.001f)
            ReportCheat("Time Manipulation");
    }

    void DetectarMemoryHack()
    {
        if (pontosReais != pontosEspelhados)
            ReportCheat("Memory Hack");
    }

    void DetectarScoreHack()
    {
        if (score > scoreFirebase)
            ReportCheat("Score Hack");
    }

    public bool VerificarIntegridade()
    {
        string currentHash = ObterHashAPK();
        return currentHash == originalHash;
    }

    public void VerificarAppsProibidos()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        foreach (string pacote in appsProibidos)
        {
            if (ApplicationHasPackage(pacote))
                ReportCheat("App de trapaça: " + pacote);
        }
#endif
    }

    public static bool IsDeviceRooted()
    {
        string[] paths = {
            "/system/app/Superuser.apk", "/sbin/su", "/system/bin/su",
            "/system/xbin/su", "/data/local/xbin/su", "/data/local/bin/su"
        };
        foreach (string path in paths)
        {
            if (File.Exists(path)) return true;
        }
        return false;
    }

    string ObterHashAPK()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var packageManager = activity.Call<AndroidJavaObject>("getPackageManager");
                var packageName = activity.Call<string>("getPackageName");
                var packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
                var applicationInfo = packageInfo.Get<AndroidJavaObject>("applicationInfo");
                string apkPath = applicationInfo.Get<string>("sourceDir");

                using (FileStream fs = new FileStream(apkPath, FileMode.Open, FileAccess.Read))
                {
                    SHA256 sha = SHA256.Create();
                    byte[] hash = sha.ComputeHash(fs);
                    return BytesToHex(hash);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao obter hash: " + e.Message);
        }
#endif
        return "";
    }

    bool ApplicationHasPackage(string packageName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                var packageManager = activity.Call<AndroidJavaObject>("getPackageManager");

                var packages = packageManager.Call<AndroidJavaObject[]>("getInstalledPackages", 0);
                foreach (var p in packages)
                {
                    string pkgName = p.Get<AndroidJavaObject>("applicationInfo").Get<string>("packageName");
                    if (pkgName.Equals(packageName)) return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Erro ao verificar pacote: " + e.Message);
        }
#endif
        return false;
    }

    string BytesToHex(byte[] bytes)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
