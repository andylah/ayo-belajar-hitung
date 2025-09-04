using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverSkorText;
    public TextMeshProUGUI gameOverLevelText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI lblPertanyaan;
    public CanvasGroup BtnJawabanGroup;
    public Button homeButton;

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
    string templateDefault = "Berapa {0} + {2} = ?";
    private bool remAds = false;

    string[] templateTambah = {
        "Budi punya {0} apel, lalu diberi {1} apel lagi, berapa apel Budi?",
        "Di taman ada {0} burung, lalu datang {1} burung lagi, jadi ada berapa burung?",
        "Ada {0} mobil di parkiran, lalu masuk {1} mobil lagi, jadi ada berapa mobil?",
        "Ibu membeli {0} roti, lalu membeli lagi {1} roti, jadi berapa roti ibu?",
        "Di kelas ada {0} anak, lalu masuk lagi {1} anak, jadi ada berapa anak di kelas?",
        "Andi punya {0} kelereng, lalu mendapat {1} kelereng lagi, berapa total kelereng Andi?",
        "Ada {0} ikan di kolam, lalu ditambah {1} ikan lagi, jadi ada berapa ikan di kolam?",
        "Di kebun ada {0} bunga, lalu ditanam lagi {1} bunga, berapa bunga di kebun?",
        "Rani punya {0} balon, lalu dibelikan {1} balon lagi, total balon Rani ada berapa?",
        "Pak guru membawa {0} pensil, lalu ditambah {1} pensil lagi, berapa pensil pak guru?"
    };

    string[] templateKurang = {
        "Paman punya {0} jeruk, dimakan {1} jeruk, berapa jeruk Paman?",
        "Ada {0} balon, lalu pecah {1} balon, tinggal berapa balon?",
        "Di rak ada {0} buku, dipinjam {1} buku, total buku yang tersisa?",
        "Ada {0} ayam di kandang, lalu keluar {1} ayam, berapa sisa ayam di kandang?",
        "Di meja ada {0} kue, dimakan {1} kue, sisa kue berapa?",
        "Ada {0} ikan di akuarium, lalu mati {1} ikan, sisa berapa ikan?",
        "Di kelas ada {0} murid, lalu pulang {1} murid, tinggal berapa murid di kelas?",
        "Andi punya {0} permen, lalu dimakan {1} permen, sisa permen Andi berapa?",
        "Ada {0} mobil di parkiran, lalu keluar {1} mobil, berapa mobil yang tersisa?",
        "Ibu membeli {0} telur, lalu dipakai {1} telur, sisa telur berapa?"
    };

    string[] templateKali = {
        "Ada {0} kantong, tiap kantong ada {1} permen, berapa total permen?",
        "Ada {0} rak, tiap rak ada {1} buku, berapa total buku di rak?",
        "Ada {0} kandang ayam, tiap kandang berisi {1} ayam, berapa total ayam?",
        "Ada {0} kotak, tiap kotak berisi {1} bola, berapa total bola?",
        "Ada {0} kelas, tiap kelas ada {1} murid, berapa total murid di kelas?",
        "Ada {0} keranjang, tiap keranjang berisi {1} jeruk, berapa total jeruk?",
        "Ada {0} piring, tiap piring berisi {1} kue, total kue berapa?",
        "Ada {0} baris kursi, tiap baris ada {1} kursi, total kursi ada berapa?",
        "Ada {0} kandang sapi, tiap kandang ada {1} sapi, berapa total sapi?",
        "Ada {0} tenda, tiap tenda berisi {1} anak, berapa total anak?"
    };

    string[] templateBagi = {
        "Ada {0} kue, dibagi kepada {1} anak sama rata, tiap anak dapat berapa?",
        "Ada {0} jeruk, dibagi kepada {1} orang sama rata, tiap orang dapat berapa?",
        "Ada {0} balon, dibagi kepada {1} anak sama rata, tiap anak dapat berapa?",
        "Ada {0} pensil, dibagi ke {1} murid sama rata, tiap murid dapat berapa?",
        "Ada {0} permen, dibagi ke {1} teman sama rata, tiap teman dapat berapa?",
        "Ada {0} mainan, dibagi rata ke {1} anak, tiap anak dapat berapa mainan?",
        "Ada {0} ikan, dibagi ke {1} akuarium, tiap akuarium ada berapa ikan?",
        "Ada {0} roti, dibagi rata ke {1} orang, tiap orang dapat berapa roti?",
        "Ada {0} kursi, dibagi ke {1} kelas, tiap kelas dapat berapa kursi?",
        "Ada {0} bola, dibagi rata ke {1} tim, tiap tim dapat berapa bola?"
    };

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

        remAds = intToBool(PlayerPrefs.GetInt("RemoveAds"));

        username = PlayerPrefs.GetString("username", "Player"); // Load username from PlayerPrefs
        usernameText.text = username;

        Debug.Log("Google Mobile Ads SDK Version: " + MobileAds.GetVersion());
    }


    public bool intToBool(int val){
        if (val == 1){
            return true;
        }else {
            return false;
        }
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
            if (remAds)
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
            //AdsController.Instance.ShowInterstitialAd();

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
            //if (currentLevel <= 1)
            //    currentOperator = Operator.Tambah;
            //else if (currentLevel <= 2)
            //    currentOperator = (Operator)Random.Range(0, 2); // Tambah / Kurang
            //else if (currentLevel <= 5)
            //    currentOperator = (Operator)Random.Range(0, 3); // Tambah / Kurang / Kali
            //else
            //    currentOperator = (Operator)Random.Range(0, 4); // Semua termasuk Bagi (opsional)

            currentOperator = GetWeightedOperator(currentLevel); // Pilih operator sesuai level

            // Atur range angka dinamis
            int min = 1;
            int max = 9 + 1;

            switch (currentOperator)
            {
                case Operator.Tambah:
                    angka1 = Random.Range(min, max + 1);
                    angka2 = Random.Range(min, max + 1);
                    jawabanBenar = angka1 + angka2;
                    if (currentLevel >= 2)
                    {
                        lblPertanyaan.text = string.Format(templateTambah[Random.Range(0, templateTambah.Length)], angka1, angka2);
                    }
                    break;

                case Operator.Kurang:
                    angka1 = Random.Range(min, max + 1);
                    angka2 = Random.Range(min, angka1 + 1); // hindari negatif
                    jawabanBenar = angka1 - angka2;
                    if (currentLevel >= 2)
                    {
                        lblPertanyaan.text = string.Format(templateKurang[Random.Range(0, templateTambah.Length)], angka1, angka2);
                    }
                    break;

                case Operator.Kali:
                    angka1 = Random.Range(min, max);
                    angka2 = Random.Range(min, max);
                    jawabanBenar = angka1 * angka2;
                    if (currentLevel >= 2)
                    {
                        lblPertanyaan.text = string.Format(templateKali[Random.Range(0, templateTambah.Length)], angka1, angka2);
                    }
                    break;

                case Operator.Bagi:
                    angka2 = Random.Range(1, max);
                    jawabanBenar = Random.Range(1, max + 1);
                    angka1 = angka2 * jawabanBenar; // supaya hasil bagi bulat
                    if (currentLevel >= 2)
                    {
                        lblPertanyaan.text = string.Format(templateBagi[Random.Range(0, templateTambah.Length)], angka1, angka2);
                    }
                    break;
            }

        } while ((angka1 == prevAngka1 && angka2 == prevAngkas2 && currentOperator == prevOperator)
                  || jawabanBenar == prevJawaban);

        if (currentLevel < 2)
            lblPertanyaan.text = string.Format(templateDefault, angka1, currentOperator, angka2);

        prevAngka1 = angka1;
        prevAngkas2 = angka2;
        prevOperator = currentOperator;
        prevJawaban = jawabanBenar;

        // Tampilkan angka & operator pakai sprite
        //angka1sprite.sprite = angkaSprites[angka1];
        //angka2sprite.sprite = angkaSprites[angka2];
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
        if (currentLevel > 5)
        {
            star_1.enabled = true;
            star_2.enabled = true;
            star_3.enabled = true;
        }
        else if (currentLevel > 3)
        {
            star_1.enabled = true;
            star_2.enabled = true;
        }
        else if (currentLevel > 0)
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
        if (remAds)
            AdsController.Instance.ShowInterstitialAd(); // Show ad on restart
        skor = 0;
        currentLevel = 1;
        nyawa = 3; // Reset nyawa
        isGameOver = false; // Reset game over state
        bgmSource.Play(); // Restart background music
        ApplyMusicSetting(); // ⬅️ Apply ulang saat restart
        UpdateScoreUI();
        updateNyawa(); // Reset nyawa UI
        gameOverPanel.SetActive(false);
        GenerateSoal();
    }

    public void HomeButton()
    {
        // Logic to return to the main menu
        // For example, you can load the main menu scene
        //show interstitial ad every 3 levels
        if (remAds)
            AdsController.Instance.ShowInterstitialAd();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // Ganti dengan nama scene menu utama Anda
    }

    Operator GetWeightedOperator(int level)
    {
        List<Operator> pool = new List<Operator>();

        if (level <= 1)
        {
            pool.Add(Operator.Tambah);
        }
        else if (level <= 2)
        {
            pool.AddRange(new Operator[] { Operator.Tambah, Operator.Kurang });
        }
        else if (level > 2 && level <= 4)
        {
            // Lebih sering Kurang
            pool.AddRange(new Operator[] { Operator.Tambah, Operator.Tambah, Operator.Kurang, Operator.Kurang, Operator.Kurang, Operator.Kali });
        }
        else if (level > 4 && level < 6)
        {
            // Lebih sering Kali
            pool.AddRange(new Operator[] { Operator.Tambah, Operator.Kurang, Operator.Kali, Operator.Kali, Operator.Kali, Operator.Bagi });
        }
        else
        {
            // Level 6 ke atas → lebih sering Bagi
            pool.AddRange(new Operator[] { Operator.Tambah, Operator.Kurang, Operator.Kali, Operator.Bagi, Operator.Bagi, Operator.Bagi });
        }

        return pool[Random.Range(0, pool.Count)];
    }
}
