using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public class LSystemGenerator : MonoBehaviour
{
    public static int NUM_OF_TREES = 15;
    public static int MAX_ITERATIONS = 7;

    public int title = 1;
    public int iterations = 4;
    public float angle = 30f;
    public float width = 1f;
    public float length = 2f;
    public float variance = 0f;
    public bool hasTreeChanged = false;
    public GameObject Tree = null;
    

    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;
    [SerializeField] private HUDScript HUD;

    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private int titleLastFrame;
    private int iterationsLastFrame;
    private float angleLastFrame;
    private float widthLastFrame;
    private float lengthLastFrame;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];

    // Start is called before the first frame update
    void Start()
    {
        titleLastFrame = title;
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;

        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            {'F',"F[+F]F[-F]F"}
        };

        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if(HUD.hasGenerateBeenPressed||Input.GetKeyDown(KeyCode.G))
        {
            ResetRandomValues();
            HUD.hasGenerateBeenPressed = false;
            Generate();
        }

        if(HUD.hasResetBeenPressed||Input.GetKeyDown(KeyCode.R))
        {
            ResetTreeValues();
            HUD.hasResetBeenPressed = false;
            HUD.Start();
            Generate();
        }

        if(titleLastFrame!=title)
        {
            if(title>=11)
            {
                HUD.rotation.gameObject.SetActive(true);
            }
            else
            {
                HUD.rotation.value = 0;
                HUD.rotation.gameObject.SetActive(false);
            }

            switch (title)
            {
                case 1:

                    SelectTreeOne();
                    break;

                case 2:
                    SelectTreeTwo();
                    break;

                case 3:
                    SelectTreeThree();
                    break;

                case 4:
                    SelectTreeFour();
                    break;

                case 5:
                    SelectTreeFive();
                    break;

                case 6:
                    SelectTreeSix();
                    break;

                case 7:
                    SelectTreeSeven();
                    break;

                case 8:
                    SelectTreeEight();
                    break;

                case 9:
                    SelectTreeNine();
                    break;

                case 10:
                    SelectTreeTen();
                    break;

                case 11:
                    SelectTreeEleven();
                    break;

                case 12:
                    SelectTreeTwelve();
                    break;
                
                case 13:
                    SelectTreeThirteen();
                    break;

                case 14:
                    SelectTreeFourteen();
                    break;

                case 15:
                    SelectTreeFifteen();
                    break;

                default:
                    SelectTreeOne();
                    break;
            }

            titleLastFrame = title;
        }

        if(iterationsLastFrame!=iterations)
        {
            if(iterations>=6)
            {
                HUD.warning.gameObject.SetActive(true);
                // StopCoroutine("TextFade");
                // StartCoroutine("TextFade");
            }
            else
            {
                HUD.warning.gameObject.SetActive(false);
            }
        }

        if(iterationsLastFrame!=iterations||
                angleLastFrame!=angle||
                widthLastFrame!=width||
                lengthLastFrame!=length)
        {
            ResetFlags();
            Generate();
        }
    }
    
    void Generate()
    {
        Destroy(Tree);

        Tree = Instantiate(treeParent);
        if(title<=3)
        {
            currentString = "F";
        }
        else
        {
            currentString = axiom;
        }
        
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }
            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        Debug.Log(currentString);
        
        for (int i = 0; i < currentString.Length; i++)
        {
            switch (currentString[i])
            {
                case 'F':
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * 2 * length);

                    GameObject treeLine = currentString[(i + 1) % currentString.Length] == 'X' 
                    || currentString[(i + 3) % currentString.Length] == 'F' 
                    && currentString[(i + 4) % currentString.Length] == 'X' 
                    ? Instantiate(leaf) : Instantiate(branch);


                    treeLine.transform.SetParent(Tree.transform);
                    treeLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    treeLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    treeLine.GetComponent<LineRenderer>().startWidth = width;
                    treeLine.GetComponent<LineRenderer>().endWidth = width;
                    break;
                
                case 'X':
                    break;

                case '+':
                    transform.Rotate(Vector3.back * angle* (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length])) ;
                    break;

                case '-':
                    transform.Rotate(Vector3.forward * angle* (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '*':
                    transform.Rotate(Vector3.up * 120* (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;

                case '/':
                    transform.Rotate(Vector3.down * 120* (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '[':
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;
                
                case ']':
                    TransformInfo origin = transformStack.Pop();
                    transform.position = origin.position;
                    transform.rotation = origin.rotation;
                    break;


                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 180, 0);
        Tree.transform.rotation = Quaternion.Euler(0, HUD.rotation.value, 0);
        Tree.transform.localScale=new Vector3(
            transform.localScale.x *HUD.scale.value / 5.0f, transform.localScale.y * HUD.scale.value / 5.0f, transform.localScale.z * HUD.scale.value / 5.0f);
    }

    private void SelectTreeOne()
    {
        rules = new Dictionary<char, string>
        {
            {'F',"F[+F]F[-F]F"}
        };
        HUD.iterations.text = "5";
        HUD.angle.text = "25.7";
        iterations = 5;
        angle = 25.7f;
        Generate();
    }

    private void SelectTreeTwo()
    {
        rules = new Dictionary<char, string>
        {
            {'F',"F[+F]F[-F][F]"}
        };
        HUD.iterations.text = "5";
        HUD.angle.text = "20";
        iterations = 5;
        angle = 20f;
        Generate();
    }

    private void SelectTreeThree()
    {
        rules = new Dictionary<char, string>
        {
            { 'F', "FF-[-F+F+F]+[+F-F-F]" }
        };
        HUD.iterations.text = "4";
        HUD.angle.text = "22.5";
        iterations = 4;
        angle = 22.5f;
        Generate();
    }

    private void SelectTreeFour()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+X]F[-X]+X]" },
            { 'F', "FF" }
        };
        HUD.iterations.text = "7";
        HUD.angle.text = "20";
        HUD.length.text = "1.0";
        iterations = 7;
        angle = 20f;
        length = 1.0f;
        Generate();
    }

    private void SelectTreeFive()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+X][-X]FX]" },
            { 'F', "FF" }
        };
        HUD.iterations.text = "7";
        HUD.angle.text = "25.7";
        HUD.length.text = "1.0";
        iterations = 7;
        angle = 20f;
        length = 1.0f;
        Generate();
    }

    private void SelectTreeSix()
    {
        

        Generate();
        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };
        HUD.iterations.text = "5";
        HUD.angle.text = "22.5";
        HUD.length.text = "2.0";
        iterations = 5;
        angle = 22.5f;
        length = 2.0f;
        Generate();
    }

    private void SelectTreeSeven()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };
        HUD.iterations.text = "4";
        HUD.angle.text = "20";
        iterations = 4;
        angle = 20f;
        Generate();
    }

    private void SelectTreeEight()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeNine()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeTen()
    {
        rules = new Dictionary<char, string>
        {
             { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeEleven()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeTwelve()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeThirteen()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFourteen()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[[*-FX][**FX-FX][***-FX]X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFifteen()
    {
        rules = new Dictionary<char, string>
        {
            {'X',"[F-[*-FX[*+FX][/+FX]]+[*-FX[*+FX][/+FX]]-[/-FX]X]"},
            { 'F', "FF" }
        };

        Generate();
    }

    private void ResetRandomValues()
    {
        for (int i = 0; i < randomRotationValues.Length;i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetFlags()
    {
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
    }

    private void ResetTreeValues()
    {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 0f;
    }

}
