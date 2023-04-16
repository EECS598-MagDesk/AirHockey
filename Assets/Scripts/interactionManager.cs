using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class interactionManager : MonoBehaviour
{

    public string inputDir = "";

    public GameObject controller_prefab;
    public GameObject controller2_prefab;

    GameObject controllerOne;
    List<GameObject> controllerOneWall;
    GameObject controllerTwo;
    List<GameObject> controllerTwoWall;

    Vector3 OneTarget;
    Vector3 TwoTarget;

    public List<Vector3> testOnePos = new List<Vector3>();
    public List<Vector3> testTwoPos = new List<Vector3>();

    private float speed = 100000f;
    private float aiSpeed = 20f;

    private float originY = 5f;
    public float lengthMulti = 73f;
    public float widthMulti = 36f;
    private float heightMulti = 70f;
    //private float heightOffset = 1.8f;

    private float prevTime = 0f;
    private float timeOffset = 0.01f;

    public scoreManager score;

    public communicationManager commManager;

    public bool useTest = false;
    public bool useCommManager = false;
    public bool useAI = false;

    public aiManager AI;
    
    // Start is called before the first frame update
    void Start()
    {
        controllerOne = GameObject.Instantiate(controller_prefab, gameObject.transform);
        controllerOne.SetActive(false);
        controllerOne.GetComponent<sliderCheck>().score_manger = score;
        controllerTwo = GameObject.Instantiate(controller2_prefab, gameObject.transform);
        controllerTwo.SetActive(false);
        controllerTwo.GetComponent<sliderCheck>().score_manger = score;

    }

    // Update is called once per frame
    void Update()
    {
        if (useTest)
        {
            parseInput(testOnePos, testTwoPos);
        }
        else
        {
            if (Time.time > prevTime + timeOffset)
            {
                if (useCommManager)
                {
                    readCommManager();
                }
                else
                {
                    readInput(inputDir);
                }
                prevTime = Time.time;
            }
        }

        if (useAI)
        {
            Vector3 puckPos = score.puck.transform.GetChild(0).transform.position;
            puckPos.z = puckPos.z / widthMulti;
            puckPos.x = puckPos.x / lengthMulti;
            AI.TrackPos(puckPos);
            controllerTwo.SetActive(true);
            TwoTarget = AI.GetPosition();
            normalize(ref TwoTarget);
        }
    }

    private void FixedUpdate()
    {
        controllerOne.transform.position = Vector3.MoveTowards(controllerOne.transform.position, OneTarget, speed * Time.deltaTime);

        if (useAI)
        {
            controllerTwo.transform.position = Vector3.MoveTowards(controllerTwo.transform.position, TwoTarget, aiSpeed * Time.deltaTime);
        }
        else
        {
            controllerTwo.transform.position = Vector3.MoveTowards(controllerTwo.transform.position, TwoTarget, speed * Time.deltaTime);
        }
    }

    void distroyWall(int type)
    {
        if (type == 1)
        {
            foreach (GameObject obj in controllerOneWall)
            {
                Destroy(obj);
            }
            controllerOneWall.Clear();
        }
        else
        {
            foreach (GameObject obj in controllerTwoWall)
            {
                Destroy(obj);
            }
            controllerTwoWall.Clear();
        }
        
    }

    Vector3 SetYOffset(Vector3 pos)
    {
        return new Vector3(pos[0], pos[1], pos[2]);
    }

    void parseInput (List<Vector3> onePos, List<Vector3> twoPos)
    {
        if (!controllerOne.activeSelf)
        {
            controllerOne.transform.position = SetYOffset(onePos[0]);
            controllerOne.SetActive(true);
        }
        OneTarget = SetYOffset(onePos[0]);
       
        if (!controllerTwo.activeSelf)
        {
            controllerTwo.transform.position = SetYOffset(onePos[1]);
            controllerTwo.SetActive(true);
        }
        TwoTarget = SetYOffset(onePos[1]);
    }

    /*
    void parseInput (List<Vector3> onePos, List<Vector3> twoPos)
    {
        if (onePos.Count >= 5 && twoPos.Count >= 5)
        {
            score.ResetGame();
            controllerOne.SetActive(false);
            distroyWall(1);
            controllerTwo.SetActive(false);
            distroyWall(2);
        }
        if (onePos.Count == 1)
        {
            if (onePos.Count != controllerOneWall.Count)
            {
                distroyWall(1);
            }
            if (!controllerOne.activeSelf)
            {
                controllerOne.transform.position = SetYOffset(onePos[0]);
                controllerOne.SetActive(true);
            }
            OneTarget = SetYOffset(onePos[0]);
        }
        else if (onePos.Count > 1)
        {
            if (controllerOne.activeSelf)
            {
                controllerOne.SetActive(false);
            }
            if (onePos.Count != controllerOneWall.Count)
            {
                distroyWall(1);
                foreach (Vector3 obj_pos in onePos)
                {
                    controllerOneWall.Add(GameObject.Instantiate(controller_prefab, SetYOffset(obj_pos), new Quaternion(0, 0, 0, 0)));
                }
            }
        }
        else
        {
            controllerOne.SetActive(false);
            distroyWall(1);
        }
        if (twoPos.Count == 1)
        {
            if (twoPos.Count != controllerTwoWall.Count)
            {
                distroyWall(2);
            }
            if (!controllerTwo.activeSelf)
            {
                controllerTwo.transform.position = SetYOffset(twoPos[0]);
                controllerTwo.SetActive(true);
            }
            TwoTarget = SetYOffset(twoPos[0]);
        }
        else if (twoPos.Count > 1)
        {
            if (controllerTwo.activeSelf)
            {
                controllerTwo.SetActive(false);
            }
            if (twoPos.Count != controllerTwoWall.Count)
            {
                distroyWall(2);
                foreach (Vector3 obj_pos in twoPos)
                {
                    controllerTwoWall.Add(GameObject.Instantiate(controller2_prefab, SetYOffset(obj_pos), new Quaternion(0, 0, 0, 0)));
                }
            }
        }
        else
        {
            controllerTwo.SetActive(false);
            distroyWall(2);
        }
    }
    */

    void normalize(ref float x, ref float y, ref float z)
    {
        x = lengthMulti * x;
        y = originY + heightMulti * y;
        z = widthMulti * z;
    }

    void normalize(ref Vector3 v)
    {
        v[0] = lengthMulti * v[0];
        v[1] = originY + heightMulti * v[1];
        v[2] = widthMulti * v[2];
    }

    void readCommManager()
    {
        string data = commManager.Get();
        string pattern = @"\([^)]*\)";

        // Match the pattern in the input string from the end using RightToLeft option
        Match lastMatch = Regex.Match(data, pattern, RegexOptions.RightToLeft);

        // Check if a match was found
        if (lastMatch.Success)
        {
            Debug.Log(lastMatch.Value);
        }
        else
        {
            return;
        }

        string[] lines = lastMatch.Value.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );


        List<Vector3> onePos = new List<Vector3>();
        List<Vector3> twoPos = new List<Vector3>();

        for (int i = 1; i < lines.Length - 1; i++)
        {
            string line = lines[i];
            string[] parsedLine = line.Split(char.Parse(" "));
            float x = float.Parse(parsedLine[0]);
            float y = float.Parse(parsedLine[2]);
            float z = float.Parse(parsedLine[1]);
            normalize(ref x, ref y, ref z);
            if (x < 0)
            {
                onePos.Add(new Vector3(-x, y, -z));
            }
            else
            {
                twoPos.Add(new Vector3(-x, y, -z));
            }
        }
        parseInput(onePos, twoPos);
    }

    void readInput(string dir)
    {
        List<Vector3> onePos = new List<Vector3>();
        List<Vector3> twoPos = new List<Vector3>();

        /*
         * put file read and parse function here
         * 
         */

        StreamReader reader = new StreamReader(this.inputDir);

        List<string> lines = new List<string>();
        while(!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            lines.Add(line);
            
        }
        reader.Close();
        foreach (string line in lines) {
            string[] parsedLine = line.Split(char.Parse(" "));
            float x = float.Parse(parsedLine[0]);
            float y = float.Parse(parsedLine[2]);
            float z = float.Parse(parsedLine[1]);
            normalize(ref x, ref y, ref z);
            //if (x < 0)
            //{
                onePos.Add(new Vector3(-x, y, -z));
            //}
            //else
            //{
            //    twoPos.Add(new Vector3(-x, y, -z));
            //}
        }

        parseInput(onePos, twoPos);
        
    }
}
