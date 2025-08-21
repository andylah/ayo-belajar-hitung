using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverSkorText;
    public TextMeshProUGUI gameOverLevelText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI lblUsername;
    public CanvasGroup BtnJawabanGroup;

    public GameObject gameOverPanel; // Panel to show when the game is over
    public Image star_1;
    public Image star_2;
    public Image star_3;

    private bool sudahJawab = false;
    bool isGameOver = false;
    bool isPaused = false;
    public Sprite pauseIcon;
    public Sprite playIcon;
    public Image pauseButtonImage;

    public Image timerImage;
    public Sprite[] angkaSprites;
    public Sprite[] operatorSprites;

    public Button[] jawabanButtons; // Array of buttons for answers
    public TextMeshProUGUI[] jawabanTexts; // Text components for the answer buttons

    public Image operatorImage;

    public Image angka1sprite;
    public Image angka2sprite;

    public Image[] nyawaSprite;
    public Sprite nyawaPenuhSprite;
    public Sprite nyawaHabisSprite;

    private float timer;
    private int skor = 0;
    private int angka1, angka2, jawabanBenar;
    private int nyawa = 3; // Starting lives

    private int prevAngka1 = -1, prevAngkas2 = -1;
    private int prevJawaban = int.MinValue;
    private int currentLevel = 1;
    //private int angkaKe = 1;
    enum Operator { Tambah, Kurang, Kali, Bagi }
    Operator currentOperator, prevOperator;

    public AudioSource bgmSource; // drag GameMusic di Inspector
    public AudioSource sfxSource; // drag SFXPlayer di Inspector

    public AudioClip correctSFX;
    public AudioClip levelUpSFX;
    public AudioClip wrongSFX;
    public AudioClip gameOverSFX;

    private CanvasGroup gameOverCanvasGroup;
    private int gameTimer;
    private string username;
    private void Start()
    {
        ApplyMusicSetting();
        GenerateSoal();
        UpdateScoreUI();

        updateNyawa(); // Update nyawa UI at start

        gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
        gameOverPanel.SetActive(false); // ⬅️ Nonaktifkan saat start
        star_1.enabled = false; // Sembunyikan bintang saat start
        star_2.enabled = false; // Sembunyikan bintang saat start
        star_3.enabled = false; // Sembunyikan bintang saat start

        gameTimer = PlayerPrefs.GetInt("time"); // Load saved timer setting

        timer = (float)gameTimer; // Set timer sesuai setting

        username = PlayerPrefs.GetString("username", "Player"); // Load username from PlayerPrefs
        lblUsername.text = "User : "+ username;

        //Debug.Log("Timer di GameScene: " + timer + " detik");

    }

    public void updateNyawa()
    {
        for (int i = 0; i < nyawaSprite.Length; i++)
        {
            if (i < nyawa)
            {
                nyawaSprite[i].sprite = nyawaPenuhSprite; // Set full heart sprite
            }
            else
            {
                nyawaSprite[i].sprite = nyawaHabisSprite; // Set empty heart sprite
            }
        }
    }

    public void TonggleBtnPause()
    {

        if (isPaused)
        {
            Time.timeScale = 1f;
            pauseButtonImage.sprite = pauseIcon;
            isPaused = false;
            BtnJawabanGroup.interactable = true; // Enable buttons when resumed
            BtnJawabanGroup.blocksRaycasts = true; // Enable raycasts for buttons
        }
        else
        {
            AdsController.Instance.ShowInterstitialAd();
            Time.timeScale = 0f;
            pauseButtonImage.sprite = playIcon;
            isPaused = true;
            BtnJawabanGroup.interactable = false; // Disable buttons when paused
            BtnJawabanGroup.blocksRaycasts = false; // Disable raycasts for buttons
        }
    }

    void ApplyMusicSetting()
    {
        bool isMusicOn = PlayerPrefs.GetInt("music", 1) == 1;

        if (bgmSource != null)
        {
            if (isMusicOn)
            {
                if (!bgmSource.isPlaying)
                    bgmSource.Play();
            }
            else
            {
                if (bgmSource.isPlaying)
                    bgmSource.Stop(); // Atau bgmSource.Pause();
            }
        }
    }

    void Update()
    {
       if (isGameOver || sudahJawab) return; // Skip update if game is over or already answered

        timer -= Time.deltaTime;
        int angkaInt = Mathf.CeilToInt(timer);
        angkaInt = Mathf.Clamp(angkaInt, 0, angkaSprites.Length - 1); // Ensure the index is within bounds

        // Update the timer display
        timerImage.sprite = angkaSprites[angkaInt];

        if (timer <= 0f)
        {
            jawabanSalahKarenaWaktuhabis();

        }

    }

    void updateLevel()
    {
        int newLevel = (skor / 10) + 1;

        if (newLevel != currentLevel)
        {
            //show interstitial ad every 3 levels
            AdsController.Instance.ShowInterstitialAd();

            currentLevel = newLevel;
            levelText.text = currentLevel.ToString();
            sfxSource.PlayOneShot(levelUpSFX); // Play level up sound
        }

    }

    void GenerateSoal()
    {
        if (isGameOver) return;

        do
        {
            // Pilih operator sesuai level
            if (currentLevel <= 2)
                currentOperator = Operator.Tambah;
            else if (currentLevel <= 4)
                currentOperator = (Operator)Random.Range(0, 2); // Tambah / Kurang
            else if (currentLevel <= 7)
                currentOperator = (Operator)Random.Range(0, 3); // Tambah / Kurang / Kali
            else
                currentOperator = (Operator)Random.Range(0, 4); // Semua termasuk Bagi (opsional)

            // Atur range angka dinamis
            int min = 1;
            int max = 9 + currentLevel * 2;

            switch (currentOperator)
            {
                case Operator.Tambah:
                    angka1 = Random.Range(min, max + 1);
                    angka2 = Random.Range(min, max + 1);
                    jawabanBenar = angka1 + angka2;
                    break;

                case Operator.Kurang:
                    angka1 = Random.Range(min, max + 1);
                    angka2 = Random.Range(min, angka1 + 1); // hindari negatif
                    jawabanBenar = angka1 - angka2;
                    break;

                case Operator.Kali:
                    angka1 = Random.Range(min, max + 1);
                    angka2 = Random.Range(min, max + 1);
                    jawabanBenar = angka1 * angka2;
                    break;

                case Operator.Bagi:
                    angka2 = Random.Range(1, max + 1);
                    jawabanBenar = Random.Range(1, max + 1);
                    angka1 = angka2 * jawabanBenar; // supaya hasil bagi bulat
                    break;
            }

        } while ((angka1 == prevAngka1 && angka2 == prevAngkas2 && currentOperator == prevOperator)
                  || jawabanBenar == prevJawaban);

        prevAngka1 = angka1;
        prevAngkas2 = angka2;
        prevOperator = currentOperator;
        prevJawaban = jawabanBenar;

        // Tampilkan angka & operator pakai sprite
        angka1sprite.sprite = angkaSprites[angka1];
        angka2sprite.sprite = angkaSprites[angka2];
        operatorImage.sprite = operatorSprites[(int)currentOperator];

        // Pilihan jawaban
        int jawabanBenarIndex = Random.Range(0, jawabanButtons.Length);
        for (int i = 0; i < jawabanButtons.Length; i++)
        {
            int pilihan;
            if (i == jawabanBenarIndex)
            {
                pilihan = jawabanBenar;
            }
            else
            {
                do
                {
                    int offset = Random.Range(-3, 4); // makin mirip
                    if (currentLevel > 5) offset = Random.Range(-7, 8); // level lebih tinggi, variasi lebih besar

                    pilihan = jawabanBenar + offset;
                } while (pilihan == jawabanBenar || pilihan < 0);
            }

            jawabanTexts[i].text = pilihan.ToString();
            jawabanButtons[i].onClick.RemoveAllListeners();
            Button tombol = jawabanButtons[i];
            jawabanButtons[i].onClick.AddListener(() => klikJawaban(tombol));
        }

        feedbackText.text = "";
        timer = (float)gameTimer;
        sudahJawab = false;
    }

    //void TampilkanAngka2Digit(int angka, Image digit1, Image digit2, int angkaKe)
    //{
    //    if (angka < 10)
    //    {
    //        if (angkaKe == 1)
    //        {
    //            digit2.sprite = angkaSprites[angka];
    //            digit1.enabled = false; // sembunyikan
    //            digit2.enabled = true; // sembunyikan

    //            angkaKe = 2; // Set angka ke 2 untuk angka kedua
    //        }
    //        else
    //        {
    //            digit1.sprite = angkaSprites[angka];
    //            digit1.enabled = true; // sembunyikan
    //            digit2.enabled = false; // sembunyikan

    //            angkaKe = 1;
    //        }

    //    }
    //    else
    //    {
    //        int puluhan = angka / 10;
    //        int satuan = angka % 10;

    //        digit1.sprite = angkaSprites[puluhan];
    //        digit2.sprite = angkaSprites[satuan];

    //        digit1.enabled = true;
    //        digit2.enabled = true;

    //    }

    //}

    public void klikJawaban(Button tombol)
    {
        if(isGameOver || sudahJawab) return; // Prevent further clicks if already answered
        sudahJawab = true; // Set the state to answered

        string textJawaban = tombol.GetComponentInChildren<TextMeshProUGUI>().text;
        int jawabanPemain = int.Parse(textJawaban);

        if (jawabanPemain == jawabanBenar)
        {
            feedbackText.text = "Benar !";
            skor += 2; // Increase score by 1 for correct answer
            sfxSource.PlayOneShot(correctSFX);
            UpdateScoreUI();
        }
        else
        {
            feedbackText.text = "Salah !";
            if (skor > 0)
                skor -= 1; // Decrease score by 1 for wrong answer
            sfxSource.PlayOneShot(wrongSFX); // Play game over sound
            UpdateScoreUI();
            nyawa--; // Decrease life
            if (nyawa > 0)
            {
                updateNyawa(); // Update nyawa UI
            }
            else
            {
                updateNyawa(); // Update nyawa UI
                gameOver(); // Call game over method
            }
        }

        Invoke("GenerateSoal", 1.5f); // Wait for 1.5 seconds before generating a new question
    }

    void jawabanSalahKarenaWaktuhabis()
    {
        sudahJawab = true;
        feedbackText.text = "Salah !";
        if (skor > 0)
            skor -= -1; // Decrease score by 1 for time out

        UpdateScoreUI();
        gameOver(); // Call game over method
        Invoke("GenerateSoal", 1.5f); // Wait for 1.5 seconds before generating a new question
    }

    void UpdateScoreUI()
    {
        scoreText.text = skor.ToString();
        updateLevel();

    }

    public void gameOver()
    {
        sudahJawab = true; // Prevent further answers
        StartCoroutine(DelayGameOverPanel());
    }

    IEnumerator DelayGameOverPanel()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeInPanel(gameOverCanvasGroup, 1f));
        gameOverPanel.SetActive(true); // Show the game over panel
        isGameOver = true; // Set game over state

        gameOverSkorText.text = skor.ToString();
        gameOverLevelText.text = currentLevel.ToString();

        // Set star images based on score
        if (skor > 40)
        {
            star_1.enabled = true;
            star_2.enabled = true;
            star_3.enabled = true;
        }
        else if (skor > 20)
        {
            star_1.enabled = true;
            star_2.enabled = true;
        }
        else if (skor > 0)
        {
            star_1.enabled = true;
        }

        bgmSource.Stop(); // Stop background music
        sfxSource.PlayOneShot(gameOverSFX); // Play game over sound

    }

    IEnumerator FadeInPanel(CanvasGroup canvasGroup, float duration)
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        gameOverPanel.SetActive(true);

        float timerFadeIn = 0f;
        while (timerFadeIn < duration)
        {
            timerFadeIn += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timerFadeIn / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void RestartGame()
    {
        skor = 0;
        currentLevel = 1;
        isGameOver = false; // Reset game over state
        bgmSource.Play(); // Restart background music
        ApplyMusicSetting(); // ⬅️ Apply ulang saat restart
        UpdateScoreUI();
        gameOverPanel.SetActive(false);
        GenerateSoal();
    }

    public void HomeButton()
    {
        // Logic to return to the main menu
        // For example, you can load the main menu scene
        //show interstitial ad every 3 levels
        AdsController.Instance.ShowInterstitialAd();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Ganti dengan nama scene menu utama Anda
    }
}
