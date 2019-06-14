using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    // AUDIO
    public AudioSource soundtrack;

    public AudioSource assDoughter;
    public AudioSource assFather;

    public AudioClip[] clipDoughter;
    public AudioClip[] clipFather;

    public GameObject loadingCanvas;

    public GameObject wellLight;

    public GameObject astarPath;
    public GameObject mainCamera;
    public float flowCameraSpeed;
    public GameObject player;
    public GameObject daughter;
    public float daughterSpeed;
    public GameObject[] wayPoints;
    public GameObject ghostStone;
    public GameObject lamp;
    public GameObject hiddenText;
    public GameObject enemy;
    public GameObject escapeObject;
    public GameObject exit;

    public GameObject canvas;
    public GameObject cameraCell;
    public GameObject textBox;

    public Cutscene startCutscene;

    public static Tutorial Instance { get; private set; }


    private AIDestinationSetter daughterAIDestinationSetter;
    private MoveController _keyManager;

    [SerializeField] private bool isDaughterMoving;

    public Tutorial() : base()
    {
        Instance = this;
    }

    void Start()
    {
        daughterAIDestinationSetter = daughter.GetComponent<AIDestinationSetter>();
        //fatherAIDestinationSetter = player.GetComponent<AIDestinationSetter>();
        daughter.GetComponent<AIPath>().maxSpeed = daughterSpeed;
        astarPath.GetComponent<AstarPath>().Scan();
        StartCoroutine(TellStory());
        _keyManager = MoveController.Instance;

    }

    void Update()
    {
        if (_keyManager.GetPauseButton())
        {
            if (!UIManager.Instance.transform.GetChild(0).gameObject.activeSelf)
            {
                UIManager.Instance.ShowPauseMenu();
            }
            else
            {
                UIManager.Instance.HidePauseMenu();
            }
        }
    }

    IEnumerator TellStory()
    {
//        yield return StartCoroutine(ShowStartCutscene());
        FreezePlayer();

//        yield return new WaitForSeconds(48f);

        soundtrack.Play();

        StartCoroutine(Dialogue("...и я так ждала этого. Теперь я тоже в рядах культа!", Character.Daughter, 3f, assDoughter, clipDoughter[0]));


        yield return StartCoroutine(MoveDaughterTo(wayPoints[0].transform));// Слайд 1: дочь идет от точки 1 к точке 2


        yield return StartCoroutine(MoveFatherTo(wayPoints[7].transform));// Слайд 2: герой идет к точке 1

        yield return StartCoroutine(Dialogue("Да, дочь моя.", Character.Father, 2f, assFather, clipFather[0]));

        //yield return new WaitForSeconds(2f);

        StartCoroutine(Dialogue("Старейшина был со мной очень добр ... и вручил...", Character.Daughter, 1, assDoughter, clipDoughter[1]));

        StartCoroutine(SoftCameraFlow());// Плавное движение камеры к герою и передача управления игроку

        yield return StartCoroutine(MoveDaughterTo(wayPoints[1].transform));// Слайд 2: дочь идет от точки 1 к точке 2

        StartCoroutine(Dialogue("...разряженный фонарь! Отец!", Character.Daughter, 2f, assDoughter, clipDoughter[2]));

        yield return StartCoroutine(WaitForPlayer(wayPoints[1]));//Слайд 3: Ожидание игрока у точки 1

        StartCoroutine(Dialogue("Я вижу впереди слабый свет. Возможно, там колодец. Идем, заправим фонарь.", Character.Father, 3f, assFather, clipFather[1]));

        //yield return StartCoroutine(FatherSays());//Слайд 3: Герой произносит: Я вижу впереди слабый свет. Возможно, там колодец. Идем, заправим фонарь

        yield return StartCoroutine(MoveDaughterTo(wayPoints[2].transform));

        yield return StartCoroutine(WaitForPlayer(wayPoints[2]));//Слайд 3: Ожидание игрока у точки 2

        yield return StartCoroutine(ReloadLamp());//Слайд 4: Заполяем с дочуркой фонарь

        yield return StartCoroutine(Dialogue("Всё, готово, дочь моя. Заметь, колодец стал светить ярче.", Character.Father, 3f, assFather, clipFather[2]));

        yield return StartCoroutine(Dialogue("Он теперь не потухнет?", Character.Daughter, 2f, assDoughter, clipDoughter[5]));

        yield return StartCoroutine(Dialogue("Потухнет, но не скоро.", Character.Father, 2f, assFather, clipFather[3]));



        yield return StartCoroutine(MoveDaughterTo(wayPoints[3].transform));

        yield return new WaitForSeconds(1f);

        UnfreezePlayer();
        //!!! 2,8
        yield return StartCoroutine(Dialogue("По - моему, здесь только что был проход...Откуда здесь стена ? ", Character.Daughter, 1.8f, assDoughter, clipDoughter[6]));

        yield return StartCoroutine(WaitForPlayer(wayPoints[3]));

        FreezePlayer();

        yield return StartCoroutine(Dialogue("Люди не способны целиком воспринять город Древних. Кажется, что Р'льех меняется, и особенно - в темноте, где разум теряет все ориентиры. Будь осторожна.", Character.Father, 11f, assFather, clipFather[4]));

        StartCoroutine(MoveDaughterTo(wayPoints[4].transform));
        // 3.5
        yield return new WaitForSeconds(2f);

        UnfreezePlayer();

        yield return StartCoroutine(Dialogue("Отец, а что это на фонаре...рычаг?", Character.Daughter, 4f, assDoughter, clipDoughter[7]));

        yield return StartCoroutine(Dialogue("Да. Сейчас покажу. Мне кажется, я кое-что заметил на той стене в глубине.", Character.Father, 5f, assFather, clipFather[5]));

        yield return StartCoroutine(WaitForPlayer(wayPoints[4]));

//        player.GetComponent<Energy>().SetEnergy(100);

        yield return StartCoroutine(Dialogue("Линзы в твоем фонаре обладают особыми свойствами. Красный спектр отпугивает существ, живущих во тьме, зелёный же - позволяет увидеть сокрытое.", Character.Father, 6f, assFather, clipFather[6]));

        yield return StartCoroutine(WaitForPlayersAction());

        yield return StartCoroutine(ShowHiddenText());

        yield return StartCoroutine(Dialogue("Ого, что это значит?", Character.Daughter, 2f, assDoughter, clipDoughter[9]));

        yield return StartCoroutine(Dialogue("Жаждущих прозреть настигнет тьма.....Нам пора возвращаться.", Character.Father, 3f, assFather, clipFather[7]));

        yield return StartCoroutine(ShowEnemy());

        yield return StartCoroutine(MoveDaughterTo(wayPoints[5].transform));

        StartCoroutine(Dialogue("Ты слышал? Что-то в реке.", Character.Daughter, 2f, assDoughter, clipDoughter[10]));

        yield return StartCoroutine(WaitForPlayer(wayPoints[5]));

        StartCoroutine(Dialogue("Отойди от воды! Оно боится красного спектра!", Character.Father, 3f, assFather, clipFather[8]));

        yield return StartCoroutine(MoveDaughterTo(wayPoints[6].transform));

        yield return new WaitForSeconds(1f);

        enemy.GetComponent<AIDestinationSetter>().target = player.transform;

        enemy.GetComponent<EnemyDeepWaterer>().wasAwaken = true;
        enemy.GetComponent<IEnemy>().SetState(State.Moving);

        yield return new WaitForSeconds(3f);

        StartCoroutine(Dialogue("Что это за тварь?!", Character.Daughter, 2f, assDoughter, clipDoughter[11]));
        yield return new WaitForSeconds(2f);
        StartCoroutine(Dialogue("Тебе предстоит еще многое узнать. Позже я всё тебе расскажу, сейчас меня ждет Старейшина.", Character.Father, 3.5f, assFather, clipFather[9]));

        exit.active = true;

        StartCoroutine(MoveDaughterTo(exit.transform));

        yield return StartCoroutine(WaitForPlayer(exit));

        SceneManager.LoadScene("ReleaseScene");

    }

    private string CharToRus(Character character)
    {
        if (character == Character.Father)
        {
            return "Отец";
        }
        else
        {
            return "Дочь";
        }
    }

    IEnumerator ShowStartCutscene()
    {
        StartCoroutine(CutscenesManager.Instance.ShowFrames(startCutscene));
        yield return null;
    }

    IEnumerator ShowExitCutscene()
    {

        yield return null;
    }

    IEnumerator ShowEnemy()
    {
        enemy.active = true;
        //enemy.GetComponent<EnemyDeepWaterer>().SetState(State.Waiting);
        yield return new WaitForSeconds(1f);
        //enemy.GetComponent<EnemyDeepWaterer>().SetState(State.Moving);
    }


    IEnumerator StoneOn()
    {
        for (int i = 0; i < 255; i++)
        {
            ghostStone.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, ghostStone.GetComponent<SpriteRenderer>().color.a + 0.01f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator ReloadLamp()
    {
        FreezePlayer();


        //Анимация наполнения фонаря и тд и тп
        ghostStone.active = true;

        StartCoroutine(StoneOn());

        wellLight.GetComponent<Light>().intensity += 75;

        yield return new WaitForSeconds(1f);
        player.GetComponent<Energy>().SetEnergy(100);
    }

    IEnumerator ShowHiddenText()
    {
        Debug.Log("ShowHiddenText()");
        Color hiddenTextColor = hiddenText.GetComponent<SpriteRenderer>().color;
        float aColor = hiddenTextColor.a;
        do {
            yield return new WaitForSeconds(0.2f);
            hiddenTextColor = hiddenText.GetComponent<SpriteRenderer>().color;
            aColor = aColor + 0.05f;
            hiddenText.GetComponent<SpriteRenderer>().color = new Color(hiddenTextColor.r, hiddenTextColor.g, hiddenTextColor.b, aColor);
            Debug.Log("aColor = " + aColor);
        }
        while (aColor < 1);
        Debug.Log("ShowHiddenText(+)");
    }

    IEnumerator WaitForPlayersAction()
    {
        cameraCell.active = true;
        cameraCell.transform.position = mainCamera.transform.position;
        mainCamera.GetComponent<CameraController>().enabled = false;
        while (lamp.GetComponentInChildren<FieldOfView>().СurrentMode != 2)
        {
            yield return null;
        }
        mainCamera.GetComponent<CameraController>().enabled = true;
        cameraCell.active = false;
    }

    IEnumerator Dialogue(string text, Character character)
    {
        Debug.Log("Dialogue");
        canvas.active = true;
        textBox.GetComponent<Text>().text = CharToRus(character) + ": " + text;
        yield return null;
        //Добавить произношение реплики героем
        Debug.Log("Dialogue +");
    }

    IEnumerator Dialogue(string text, Character character, float delay)
    {
        canvas.active = true;
        StartCoroutine(Dialogue(text, character));
        yield return new WaitForSeconds(delay);
        canvas.active = false;
    }

    IEnumerator Dialogue(string text, Character character, float delay, AudioSource ass, AudioClip clip)
    {
        canvas.active = true;
        StartCoroutine(Dialogue(text, character));
        ass.PlayOneShot(clip);
        yield return new WaitForSeconds(delay);
        canvas.active = false;
    }

    IEnumerator MoveDaughterTo(Transform target)
    {
        daughterAIDestinationSetter.target = target;
        while (Vector2.Distance(daughter.transform.position, target.position) > 1f)
        {
            isDaughterMoving = true;
            yield return null;

        }

        Debug.Log("Дошла");
        isDaughterMoving = false;

    }

    IEnumerator MoveFatherTo(Transform target)
    {
        while (Vector2.Distance(player.transform.position, target.position) > 1f)
        {
            //isDaughterMoving = true;
            player.transform.position = Vector2.MoveTowards(player.transform.position, target.position, 4f * Time.deltaTime);
            yield return null;

        }

        //Debug.Log("Дошла");
        //isDaughterMoving = false;

    }

    IEnumerator SoftCameraFlow()
    {
        while (Vector2.Distance(player.transform.position, mainCamera.transform.position) > 0.5f)
        {
            mainCamera.transform.position = Vector2.MoveTowards(mainCamera.transform.position, player.transform.position, flowCameraSpeed * Time.deltaTime);
            mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10f);
            yield return null;

        }
        mainCamera.GetComponent<CameraController>().enabled = true;
        PlayerManager.Instance.ResumeManager();
    }

    IEnumerator WaitForPlayer(GameObject waypoint)
    {
        while (!waypoint.GetComponent<WayPointScript>().isTriggered)
        {
            yield return null;
        }

    }

    public void FreezePlayer()
    {
        PlayerManager.Instance.PauseManager();

    }

    public void UnfreezePlayer()
    {
        PlayerManager.Instance.ResumeManager();
    }
}
