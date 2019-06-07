using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using TheBlock;


public class theBlockScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMBombModule module;
    public KMSelectable[] buttons;
    public Renderer[] discoButtons;
    public Material[] discoButtonMats;
    public Renderer[] cubeFaces; 
    public Material[] cubeFacesMats;
    public Material ambientLight;
    public Light[] AllLights;
    public int Rule;
    public Answers correctAnswer;
    public int counter;
    
    FaceColour[] cubeFaceColours;     // array to store the colours of the faces    
    int correctButton;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        float scalar = transform.lossyScale.x;

            foreach (KMSelectable button in buttons)
            {
                KMSelectable pressedButton = button;
                button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
            }

            foreach (Light light in AllLights)
            {
                light.range *= scalar;
            }
    }

    void Start()
    {
        SetDiscoLights();
        SetCubeFaces();
        DetermineAnswer();
    }

    void SetCubeFaces()
    {
        {
            // Each face gets a random color
            cubeFaceColours = Enumerable.Range(0, 6).Select(i => (FaceColour)UnityEngine.Random.Range(0, 4)).ToArray();

            // Assign the materials
            for (var i = 0; i < cubeFaces.Length; i++)
                cubeFaces[i].material = cubeFacesMats[(int)cubeFaceColours[i]];
        }
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[0]);
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[1]);
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[2]);
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[3]);
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[4]);
            Debug.LogFormat("[The Block #{0}] Face 1 Colour: {1}.", moduleId, cubeFaceColours[5]);
    }

    public void Strike()
    {//1, 3, 5, 6, 8, 9, 10, and 13
        module.HandleStrike();
        counter = 0;
        Debug.LogFormat("[The Block #{0}] Module Resetting.", moduleId);
        Debug.LogFormat("[The Block #{0}] Selecting New Colours.", moduleId);
        SetCubeFaces();
        DetermineAnswer();
    }

    public void ButtonPress(KMSelectable button)
    {
        button.AddInteractionPunch(0.2f);
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.BigButtonPress, transform);

        if (moduleSolved == true)
        {
            return;
        }

        else
        {
            switch (correctAnswer)
            {
                case Answers.Unicorn: //Rule 1
                    if (button == buttons[6] && counter == 0)
                    {
                        counter = 1;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[6] && counter == 1)
                    {
                        counter = 2;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[6] && counter == 2)
                    {
                        counter = 3;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name); 
                    }
                    else if (button == buttons[6] && counter == 3)
                    {
                        counter = 4;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[6] && counter == 4)
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.FirstGreen: //Rule 2
                    for (int i = 0; i < 6; i++)
                    {
                        if (cubeFaceColours[i] == FaceColour.Green)
                        {
                            correctButton = i;
                            break;
                        }
                    }
                    if (button == buttons[correctButton])
                    {
                        module.HandlePass();
                        moduleSolved = true;
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }                    

                    break;

                case Answers.AllReverse: //Rule 3
                    if (button == buttons[counter])
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                        if (counter == 0)
                        {
                            module.HandlePass();
                            moduleSolved = true;
                            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                            Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        }
                        else
                        {
                            counter--;
                        }
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.Side5: //Rule 4
                    if (button == buttons[4])
                    {
                        module.HandlePass();
                        moduleSolved = true;
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.Side2Side4: //Rule 5
                    if (button == buttons[1] && counter == 0)
                    {
                        counter = 1;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[3] && counter == 1)
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        moduleSolved = true;
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.AllNumerical: //Rule 6
                    if (button == buttons[counter])
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                        if (counter == 5)
                        {
                            module.HandlePass();
                            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                            moduleSolved = true;
                            Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.BlockRule7: //Rule 7
                    if (button == buttons[6])
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.LastBlue: //Rule 8
                    for (int i = 5; i > -1; i--)
                    {
                        if (cubeFaceColours[i] == FaceColour.Blue)
                        {
                            correctButton = i;
                            break;
                        }
                    }
                    if (button == buttons[correctButton])
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }

                    break;

                case Answers.EvenNumerical: //Rule 9
                    if (button == buttons[1] && counter == 0)
                    {
                        counter = 1;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[3] && counter == 1)
                    {
                        counter = 2;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[5] && counter == 2)
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        moduleSolved = true;
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.OddReverse: //Rule 10
                    if (button == buttons[4] && counter == 0)
                    {
                        counter = 1;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[2] && counter == 1)
                    {
                        counter = 2;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[0] && counter == 2)
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        moduleSolved = true;
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.FirstBlue: //Rule 11
                    for (int i = 0; i < 6; i++)
                    {
                        if (cubeFaceColours[i] == FaceColour.Blue)
                        {
                            correctButton = i;
                            break;
                        }
                    }
                    if (button == buttons[correctButton])
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }

                    break;

                case Answers.BlockRule12: //Rule 12
                    if (button == buttons[6])
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.Side1Side4: //Rule 13
                    if (button == buttons[0] && counter == 0)
                    {
                        counter = 1;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}.", moduleId, button.gameObject.name);
                    }
                    else if (button == buttons[3] && counter == 1)
                    {
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                        moduleSolved = true;
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;

                case Answers.Side4Rule14: //Rule 14
                    if (button == buttons[3])
                    {
                        module.HandlePass();
                        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        moduleSolved = true;
                        Debug.LogFormat("[The Block #{0}] The button you pressed was: {1}. Module Disarmed.", moduleId, button.gameObject.name);
                    }
                    else
                    {
                        Debug.LogFormat("[The Block #{0}] The Button you pressed was: {1}. This was incorrect - Strike Incurred", moduleId, button.gameObject.name);
                        Strike();
                    }
                    break;
            }
        }
    }

    public void DetermineAnswer()
        {
            var numRed = cubeFaceColours.Count(color => color == FaceColour.Red);
            var numBlue = cubeFaceColours.Count(color => color == FaceColour.Blue);
            var numGreen = cubeFaceColours.Count(color => color == FaceColour.Green);
            var numYellow = cubeFaceColours.Count(color => color == FaceColour.Yellow);

        //UNICORN - lit BOB, 3 batteries in 2 holders and an empty port plate - Press the Block 5 times
        if (Bomb.GetBatteryCount() == 3 && Bomb.GetBatteryHolderCount() == 2 && Bomb.GetPortPlates().Any(x => x.Length == 0) && Bomb.IsIndicatorOn(Indicator.BOB))
        {
            Debug.LogFormat("[The Block #{0}] Rule 1 Selected", moduleId);
            Rule = 1;
            correctAnswer = Answers.Unicorn;
        }

        //If there is a Parallel and a Serial Port present, and Side 4 is Green - Press the first green side in the NET.
        else if (Bomb.GetPortCount(Port.Parallel) > 0 && Bomb.GetPortCount(Port.Serial) > 0 && (cubeFaceColours[3] == FaceColour.Green))
        {
            Debug.LogFormat("[The Block #{0}] Rule 2 Selected", moduleId);
            Rule = 2;
            correctAnswer = Answers.FirstGreen;
        }

        //If the serial number contains a Vowel, and there is an unlit SIG indicator - Press all sides in reverse numerical order.
        else if (Bomb.GetSerialNumberLetters().Any(x => x == 'A' || x == 'E' || x == 'I' || x == 'O' || x == 'U') && Bomb.IsIndicatorOff(Indicator.SIG))
        {
            Debug.LogFormat("[The Block #{0}] Rule 3 Selected", moduleId);
            Rule = 3;
            correctAnswer = Answers.AllReverse;
            counter = 5;
        }

        //If there are 3+ Batteries, and Side 1 is Red - Press Side 5.
        else if (Bomb.GetBatteryCount() > 2 && (cubeFaceColours[0] == FaceColour.Red))
        {
            Debug.LogFormat("[The Block #{0}] Rule 4 Selected", moduleId);
            Rule = 4;
            correctAnswer = Answers.Side5;
        }

        //If there are more Blue sides than Red sides, and more Yellow sides than Green sides - Press Side 2, then Side 4.
        else if (numBlue > numRed && numYellow > numGreen)
        {
            Debug.LogFormat("[The Block #{0}] Rule 5 Selected", moduleId);
            Rule = 5;
            correctAnswer = Answers.Side2Side4;
        }

        //If there are no Yellow sides - press all sides in numerical order
        else if (cubeFaceColours[0] != FaceColour.Yellow && cubeFaceColours[1] != FaceColour.Yellow && cubeFaceColours[2] != FaceColour.Yellow && cubeFaceColours[3] != FaceColour.Yellow && cubeFaceColours[4] != FaceColour.Yellow && cubeFaceColours[5] != FaceColour.Yellow)
        {
            Debug.LogFormat("[The Block #{0}] Rule 6 Selected", moduleId);
            Rule = 6;
            correctAnswer = Answers.AllNumerical;
        }

        //If Side 2 is Yellow, and Side 3 is Blue - press on the Block.
        else if (cubeFaceColours[1] == FaceColour.Yellow && cubeFaceColours[2] == FaceColour.Blue)
        {
            Debug.LogFormat("[The Block #{0}] Rule 7 Selected", moduleId);
            Rule = 7;
            correctAnswer = Answers.BlockRule7;
        }

        //If there are no lit indicators, and Side 2 and Side 4 are Blue - Press on the last blue side in the NET.
        else if (cubeFaceColours[1] == FaceColour.Blue && cubeFaceColours[3] == FaceColour.Blue && Bomb.GetOnIndicators().Count() == 0)
        {
            Debug.LogFormat("[The Block #{0}] Rule 8 Selected", moduleId);
            Rule = 8;
            correctAnswer = Answers.LastBlue;
        }

        //If there are 2 port plates, and one is empty - Press all even sides, in numerical order
        else if (Bomb.GetPortPlateCount() == 2 && Bomb.GetPortPlates().Any(x => x.Length == 0))
         {
            Debug.LogFormat("[The Block #{0}] Rule 9 Selected", moduleId);
            Rule = 9;
            correctAnswer = Answers.EvenNumerical;
        }

        //If Side 5 is Blue, and Side 1 is Green - Press all odd sides, in reverse numerical order
        else if (cubeFaceColours[4] == FaceColour.Blue && cubeFaceColours[0] == FaceColour.Green)
        {
            Debug.LogFormat("[The Block #{0}] Rule 10 Selected", moduleId);
            Rule = 10;
            correctAnswer = Answers.OddReverse;
        }

        //If there are no batteries, and Side 3 is Blue - Press on the first blue side in the NET.
        else if (cubeFaceColours[2] == FaceColour.Blue && Bomb.GetBatteryCount() == 0)
        {
            Debug.LogFormat("[The Block #{0}] Rule 11 Selected", moduleId);
            Rule = 11;
            correctAnswer = Answers.FirstBlue;
        }

        // If there is at least 1 DVI-D Port, and exactly 1 Battery - Press on the Block
        else if (Bomb.GetPortCount(Port.DVI) > 0 && Bomb.GetBatteryCount() == 1)
        {
             Debug.LogFormat("[The Block #{0}] Rule 12 Selected", moduleId);
             Rule = 12;
            correctAnswer = Answers.BlockRule12;
        }

        //If there are more Red sides than Blue sides - Press Side 1, then Side 4.
        else if (numRed > numBlue)
        {
            Debug.LogFormat("[The Block #{0}] Rule 13 Selected", moduleId);
            Rule = 13;
            correctAnswer = Answers.Side1Side4;
        }

        //Otherwise Press Side 4
        else
        {
            Debug.LogFormat("[The Block #{0}] Rule 14 Selected", moduleId);
            Rule = 14;
            correctAnswer = Answers.Side4Rule14;
        }
    }

    void SetDiscoLights()
    {
        for (int i = 0; i < 6; i++)
        {
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[i].material = discoButtonMats[index];
        }
        StartCoroutine(disco1());
        StartCoroutine(disco2());
        StartCoroutine(disco3());
        StartCoroutine(disco4());
        StartCoroutine(disco5());
        StartCoroutine(disco6());
    }
    IEnumerator disco1()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[0].material = discoButtonMats[index];
        }
    }
    IEnumerator disco2()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[1].material = discoButtonMats[index];
        }
    }
    IEnumerator disco3()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[2].material = discoButtonMats[index];
        }
    }
    IEnumerator disco4()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[3].material = discoButtonMats[index];
        }
    }
    IEnumerator disco5()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[4].material = discoButtonMats[index];
        }
    }
    IEnumerator disco6()
    {
        while (!moduleSolved)
        {
            float delay = UnityEngine.Random.Range(0.2f, 1.8f);
            yield return new WaitForSeconds(delay);
            int index = UnityEngine.Random.Range(0, 11);
            discoButtons[5].material = discoButtonMats[index];
        }
    }

    
}
