using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Audio & Panel")]
    public AudioSource bgmMusic;
    public GameObject settingPanel;
    public GameObject tentangPanel;

    [Header("Music Toggle")]
    public RectTransform musicHandleTransform;
    public Transform musicOnTarget;
    public Transform musicOffTarget;

    public Toggle timer5Toggle; // Toggle untuk timer 5 detik
    public Toggle timer8Toggle; // Toggle untuk timer 8 detik
    public Toggle timer10Toggle; // Toggle untuk timer 10 detik


    public bool isMusicOn = true; // Status musik, default ON
    public int selectedTimer; // Timer yang dipilih, default 8 detik

    private void Start()
    {
        settingPanel.SetActive(false); // Menyembunyikan panel pengaturan saat menu utama dimuat
        tentangPanel.SetActive(false); // Menyembunyikan panel tentang saat menu utama dimuat   

        isMusicOn = PlayerPrefs.GetInt("music", 1) == 1;
        UpdateToggleVisuals();

       
        // Memastikan BGM dimulai saat menu utama dimuat
        if (bgmMusic != null)
        {
            bgmMusic.mute = !isMusicOn;
            if (isMusicOn)
            {
                bgmMusic.Play();
            }
        }
        if (!Social.localUser.authenticated)
        {
            Social.localUser.Authenticate(success =>
            {
                if (success)
                {
                    // Jika login berhasil, tampilkan informasi pengguna
                    PlayerPrefs.SetString("username", Social.localUser.userName);
                    PlayerPrefs.SetString("user_id", Social.localUser.id);
                    //Debug.Log("Login Berhasil!");
                    //Debug.Log("Username: " + Social.localUser.userName);
                    //Debug.Log("User ID: " + Social.localUser.id);
                }
                else
                {
                    //Debug.Log("Login Gagal.");
                }
            });
        }
        else
        {
            Debug.Log("Sudah login sebelumnya sebagai: " + Social.localUser.userName);
        }

    }

    public void OnToggleChanged()
    {
        if (timer5Toggle.isOn)
        {
            selectedTimer = 5;

            //Debug.Log("Timer 5 detik dipilih");
        }
        else if (timer8Toggle.isOn)
        {
            selectedTimer = 8;
            //Debug.Log("Timer 8 detik dipilih");
        }
        else if (timer10Toggle.isOn)
        {
            selectedTimer = 10;

            //Debug.Log("Timer 10 detik dipilih");
        }

        PlayerPrefs.SetInt("time", selectedTimer); // Simpan pilihan timer ke PlayerPrefs
        PlayerPrefs.Save(); // Simpan perubahan

        //Debug.Log("Timer yang dipilih: " + selectedTimer + " detik");
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("music", isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        // Update tampilan dan audio
        UpdateToggleVisuals();

        if (bgmMusic != null)
        {
            if (isMusicOn) bgmMusic.Play();
            else bgmMusic.Pause();
        }
    }

    private void UpdateToggleVisuals()
    {
        if (musicHandleTransform != null)
        {
            StopCoroutine(nameof(MoveHandleSmoothly)); // pastikan tidak duplikat
            StartCoroutine(MoveHandleSmoothly(musicHandleTransform, isMusicOn ? musicOnTarget.position : musicOffTarget.position));
        }

    }

    private IEnumerator MoveHandleSmoothly(RectTransform handle, Vector3 targetPosition)
    {
        float duration = 0.2f; // durasi animasi
        float elapsed = 0f;
        Vector3 startingPos = handle.position;

        while (elapsed < duration)
        {
            handle.position = Vector3.Lerp(startingPos, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        handle.position = targetPosition; // pastikan tepat di posisi akhir
    }

    // Fungsi untuk tombol Mulai
    public void MulaiGame()
    {
        SceneManager.LoadScene("GameScene"); // ganti dengan nama scene gameplay Anda
    }

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true); // Menampilkan panel pengaturan

        //Debug.Log("Panel pengaturan dibuka");

        if (PlayerPrefs.HasKey("time"))
        {
            selectedTimer = PlayerPrefs.GetInt("time");
        }
        else
        {
            selectedTimer = 8; // Default timer jika belum ada yang dipilih
            PlayerPrefs.SetInt("time", selectedTimer); // Simpan default timer ke PlayerPrefs
            PlayerPrefs.Save(); // Simpan perubahan
        }

        selectedTimer = PlayerPrefs.GetInt("time"); // Memuat timer yang dipilih sebelumnya
        //Debug.Log("Timer yang dipilih: " + selectedTimer + " detik");

        if (selectedTimer == 5)
        {
            timer5Toggle.isOn = true;
            timer8Toggle.isOn = false;
            timer10Toggle.isOn = false;
        }
        else if (selectedTimer == 8)
        {
            timer5Toggle.isOn = false;
            timer8Toggle.isOn = true;
            timer10Toggle.isOn = false;
        }
        else if (selectedTimer == 10)
        {
            timer5Toggle.isOn = false;
            timer8Toggle.isOn = false;
            timer10Toggle.isOn = true;
        }
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false); // Menyembunyikan panel pengaturan
    }

    public void OpenTentangPanel()
    {
        tentangPanel.SetActive(true); // Menampilkan panel tentang
    }

    public void CloseTentangPanel()
    {
        tentangPanel.SetActive(false); // Menyembunyikan panel tentang
    }

    
}