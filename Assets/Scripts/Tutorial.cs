using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class Tutorial : MonoBehaviour
{
    public GameObject astarPath;
    public GameObject mainCamera;
    public float flowCameraSpeed;
    public GameObject player;
    public GameObject daughter;
    public float daughterSpeed;
    public GameObject[] wayPoints;

    public GameObject canvas;
    public GameObject textBox;


    private AIDestinationSetter daughterAIDestinationSetter;

    [SerializeField] private bool isDaughterMoving;
    // Start is called before the first frame update
    void Start()
    {
        daughterAIDestinationSetter = daughter.GetComponent<AIDestinationSetter>();
        //fatherAIDestinationSetter = player.GetComponent<AIDestinationSetter>();
        daughter.GetComponent<AIPath>().maxSpeed = daughterSpeed;
        astarPath.GetComponent<AstarPath>().Scan();
        StartCoroutine(TellStory());
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TellStory()
    {
        
        yield return StartCoroutine(MoveDaughterTo(wayPoints[0].transform));// Слайд 1: дочь идет от точки 1 к точке 2

        StartCoroutine(Dialogue("...и я так ждала этого. Теперь я тоже в рядах культа!", Character.Daughter));

        yield return StartCoroutine(MoveFatherTo(wayPoints[7].transform));// Слайд 2: герой идет к точке 1

        yield return StartCoroutine(Dialogue("Да, дочь моя (уставшим голосом без энтузиазма)", Character.Father, 2f));

        //yield return new WaitForSeconds(2f);

        StartCoroutine(Dialogue("Старейшина был со мной очень добр (пытается зажечь фонарь*ЩЕЛК*) и вручил... (*ЩЕЛК* напряжение и нарастающее раздражение) ", Character.Daughter));

        StartCoroutine(SoftCameraFlow());// Плавное движение камеры к герою и передача управления игроку

        yield return StartCoroutine(MoveDaughterTo(wayPoints[1].transform));// Слайд 2: дочь идет от точки 1 к точке 2

        StartCoroutine(Dialogue("...разряженный фонарь! Отец!", Character.Daughter, 2f));

        yield return StartCoroutine(WaitForPlayer(wayPoints[1]));//Слайд 3: Ожидание игрока у точки 1

        StartCoroutine(Dialogue("Я вижу впереди слабый свет. Возможно, там колодец. Идем, заправим фонарь.", Character.Father));

        //yield return StartCoroutine(FatherSays());//Слайд 3: Герой произносит: Я вижу впереди слабый свет. Возможно, там колодец. Идем, заправим фонарь

        yield return StartCoroutine(MoveDaughterTo(wayPoints[2].transform));

        yield return StartCoroutine(WaitForPlayer(wayPoints[2]));//Слайд 3: Ожидание игрока у точки 2

        yield return StartCoroutine(ReloadLamp());//Слайд 4: Заполяем с дочуркой фонарь

        yield return StartCoroutine(Dialogue("Всё, готово, дочь моя. Заметь, колодец стал светить ярче.", Character.Father, 3f));

        yield return StartCoroutine(Dialogue("Он теперь не потухнет?", Character.Daughter, 2f));

        yield return StartCoroutine(Dialogue("Потухнет, но не скоро.", Character.Father, 2f));

        PlayerManager.Instance.playerCanMove = true;

        yield return StartCoroutine(MoveDaughterTo(wayPoints[3].transform));

        yield return StartCoroutine(MoveDaughterTo(wayPoints[4].transform));

        yield return StartCoroutine(MoveDaughterTo(wayPoints[5].transform));

        yield return StartCoroutine(MoveDaughterTo(wayPoints[6].transform));

    }

    IEnumerator ReloadLamp()
    {
        PlayerManager.Instance.playerCanMove = false;
        //Анимация наполнения фонаря и тд и тп
        yield return new WaitForSeconds(1f);
        
    }

    IEnumerator Dialogue(string text, Character character)
    {
        canvas.active = true;
        textBox.GetComponent<Text>().text = character + ": " + text;
        yield return null;
        //Добавить произношение реплики героем
    }

    IEnumerator Dialogue(string text, Character character, float delay)
    {
        canvas.active = true;
        StartCoroutine(Dialogue(text, character));
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
        PlayerManager.Instance.playerCanMove = true;
    }

    IEnumerator WaitForPlayer(GameObject waypoint)
    {
        while (!waypoint.GetComponent<WayPointScript>().isTriggered)
        {
            yield return null;
        }
        
    }
}
