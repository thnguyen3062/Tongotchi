using Core.Utils;
using Spine.Unity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    public static Action OnTutorialStarted;
    public static Action OnTutorialFinished;

    public class TutorialChildElement
    {
        public Transform transform;
        public Transform parent;
        public int childIndex;

        public TutorialChildElement(Transform transform)
        {
            this.transform = transform;
            this.parent = transform.parent;
            this.childIndex = transform.GetSiblingIndex();
        }

        public void SetBackToOriginParent()
        {
            transform.SetParent(parent);
            transform.SetSiblingIndex(childIndex);
        }
    }

    [SerializeField] private Transform[] m_TutorialChildTransform;
    [SerializeField] private Transform m_APoint;
    [SerializeField] private Transform m_BPoint;
    [SerializeField] private SkeletonGraphic m_SkeletonGraphic;
    [SerializeField] private TextMeshProUGUI m_WispText;
    [SerializeField] private GameObject m_Bubble;
    [SerializeField] private Transform[] m_Positions;
    [SerializeField] private GameObject m_TutorialContainer;
    [SerializeField] private Transform m_TutorialUpper;
    [SerializeField] private GameObject m_BlackBackground;
    [SerializeField] private Button m_NextButton;
    [SerializeField] private GameObject m_NextText;
    [SerializeField] private Button m_SkipButton;
    [SerializeField] private float m_TextSpeed;
    [SerializeField] private bool canNextSentence = false;

    private WaitForSeconds textSpeed;
    private bool canNext = false;
    private Transform previousParent;
    private int previousIndex;
    private ICallback.CallFunc onMisisonCallback;
    private ICallback.CallFunc onCompleted;

    private TutorialChildElement[] m_TutorialChildParent;
    public GameObject TutorialContainer => m_TutorialContainer;
    public GameObject BlackBackground => m_BlackBackground;

    private void Start()
    {
        m_NextButton.onClick.AddListener(OnNextTutorial);
        m_SkipButton.onClick.AddListener(OnSkipTutorial);
        textSpeed = new WaitForSeconds(m_TextSpeed);
        m_TutorialUpper.gameObject.SetActive(true);

        if (PlayerData.Instance.data.tutorialPhase != TutorialPhase.Opening)
        {
            m_BPoint.position = m_Positions[0].position;
            m_SkeletonGraphic.AnimationState.SetAnimation(0, "a_to_b", false).Complete += delegate
            {
                m_SkeletonGraphic.AnimationState.SetAnimation(0, "idle_b", true);
            };
        }

        m_TutorialChildParent = new TutorialChildElement[m_TutorialChildTransform.Length];
        for (int i = 0; i < m_TutorialChildParent.Length; i++)
        {
            m_TutorialChildParent[i] = new TutorialChildElement(m_TutorialChildTransform[i]);
        }
    }

    private void OnSkipTutorial()
    {
        StopAllCoroutines();
        m_TutorialUpper.gameObject.SetActive(false);
        PlayerData.Instance.data.isTutorialDone = true;
        m_BlackBackground.SetActive(false);
        gameObject.SetActive(false);
        m_TutorialChildTransform[4].gameObject.SetActive(false);
        ResetChildElements();
        OnTutorialFinished?.Invoke();
        PlayerData.Instance.SaveData(() => {
            GameManager.Instance.DailyRewardManager.InitRewardData();
        });
    }

    private void ResetChildElements()
    {
        for (int i = 0; i < m_TutorialChildParent.Length; i++)
        {
            m_TutorialChildParent[i].SetBackToOriginParent();
        }
    }

    public void InitTutorial(ICallback.CallFunc complete)
    {
        onMisisonCallback = complete;
        m_TutorialContainer.SetActive(true);
        SetTutorial(TutorialPhase.Opening);
        OnTutorialStarted?.Invoke();
    }

    public void LoadTutorialFromSave()
    {
        m_BPoint.position = m_Positions[0].position;
        m_SkeletonGraphic.AnimationState.SetAnimation(0, "a_to_b", false).Complete += delegate
        {
            m_SkeletonGraphic.AnimationState.SetAnimation(0, "idle_b", true);
        };
    }

    private void OnNextTutorial()
    {
        if (!canNext)
            return;

        var data = PlayerData.Instance.data;

        switch (data.tutorialPhase)
        {
            case TutorialPhase.Opening:
                data.completedPhase = data.tutorialPhase;
                SetTutorial(TutorialPhase.LookAtEgg, onMisisonCallback);
                break;

            case TutorialPhase.LookAtEgg:
                data.completedPhase = data.tutorialPhase;
                m_TutorialContainer.SetActive(false);
                m_BlackBackground.SetActive(false);
                onCompleted?.Invoke();
                break;

            case TutorialPhase.CongrateToHatchingEgg:
                data.completedPhase = data.tutorialPhase;
                m_TutorialContainer.SetActive(false);
                m_BlackBackground.SetActive(false);
                ResetChildElements();
                onCompleted?.Invoke();
                break;

            case TutorialPhase.HatchingCompleted:
                data.completedPhase = data.tutorialPhase;
                SetTutorial(TutorialPhase.Hunger);
                break;

            case TutorialPhase.Hunger:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Hygiene);
                break;

            case TutorialPhase.Hygiene:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Happy);
                break;

            case TutorialPhase.Happy:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Health);
                break;

            case TutorialPhase.Health:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.EXP);
                break;

            case TutorialPhase.EXP:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Ticket);
                break;

            case TutorialPhase.Ticket:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Diamond);
                break;

            case TutorialPhase.Diamond:
                data.completedPhase = data.tutorialPhase;
                ResetChildElements();
                SetTutorial(TutorialPhase.Twelth);
                break;

            case TutorialPhase.Twelth:
                data.completedPhase = data.tutorialPhase;
                SetTutorial(TutorialPhase.Thirteenth);
                break;

            case TutorialPhase.Thirteenth:
                data.completedPhase = data.tutorialPhase;
                SetTutorial(TutorialPhase.Fourteenth);
                break;

            case TutorialPhase.Fourteenth:
                data.completedPhase = data.tutorialPhase;
                data.isTutorialDone = true;
                m_TutorialUpper.gameObject.SetActive(false);
                gameObject.SetActive(false);
                m_BlackBackground.SetActive(false);
                break;
        }

        PlayerData.Instance.SaveData(() => {
            if (PlayerData.Instance.data.isTutorialDone)
            {
                GameManager.Instance.DailyRewardManager.InitRewardData();
                ResetChildElements();
                OnTutorialFinished?.Invoke();
            }
        });
    }

    public void SetTutorial(TutorialPhase phase, ICallback.CallFunc onComplete = null)
    {
        PlayerData.Instance.data.tutorialPhase = phase;

        switch (phase)
        {
            case TutorialPhase.Opening:
                canNext = false;
                m_BlackBackground.SetActive(true);
                m_BPoint.position = m_Positions[0].position;
                m_SkeletonGraphic.AnimationState.SetAnimation(0, "a_to_b", false).Complete += delegate
                {
                    m_SkeletonGraphic.AnimationState.SetAnimation(0, "idle_b", true);
                    string[] contents = new string[]
                    {
                        "Welcome, honored Guardian, to Shizen Mori, the forest of nature and the gods.",
                        "I am wisp, and I'm here to help guide you through the forest and through your journey being a Tongotchi Guardian!"
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                };
                break;

            case TutorialPhase.LookAtEgg:
                canNext = false;
                string[] lookAtEggContents = new string[]
                {
                    "Come!",
                    "It looks like the egg that was chosen for you is about to hatch, there is just one small thing left to do!"
                };
                StartCoroutine(TypeSentences(lookAtEggContents, () =>
                {
                    onCompleted = onComplete;
                    canNext = true;
                }));
                break;

            case TutorialPhase.CongrateToHatchingEgg:
                canNext = false;
                m_TutorialContainer.SetActive(true);
                m_BlackBackground.SetActive(true);
                m_TutorialChildTransform[0].SetParent(m_TutorialUpper);
                string[] congs = new string[]
                {
                    "You did it! Now get ready, your companion is hatching!"
                };
                StartCoroutine(TypeSentences(congs, () =>
                {
                    onCompleted = onComplete;
                    canNext = true;
                }));
                break;

            case TutorialPhase.HatchingCompleted:
                canNext = false;
                m_TutorialContainer.SetActive(true);
                m_BlackBackground.SetActive(true);
                string[] hatchComp = new string[]
                {
                    "Your Tongotchi is here!",
                    "Let us now turn to the daily care they require to thrive."
                };
                StartCoroutine(TypeSentences(hatchComp, () =>
                {
                    canNext = true;
                }));
                break;

            // Add additional cases if needed
            case TutorialPhase.Hunger:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[1].parent;
                    m_TutorialChildTransform[1].SetParent(m_TutorialUpper);
                    string[] contents = new string[1]
                    {
                        "Ensure your pet is regularly fed to keep them healthy."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Hygiene:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[2].parent;
                    m_TutorialChildTransform[2].SetParent(m_TutorialUpper);
                    string[] contents = new string[2]
                    {
                        "Maintain cleanliness to keep your pet happy and comfortable.",
                        "Keep their space tidy to make sure they're clean and comfy."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Happy:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[3].parent;
                    m_TutorialChildTransform[3].SetParent(m_TutorialUpper);
                    string[] contents = new string[1]
                    {
                        "Provide toys to keep your pet entertained."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Health:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[4].parent;
                    m_TutorialChildTransform[4].SetParent(m_TutorialUpper);
                    m_TutorialChildTransform[4].gameObject.SetActive(true);
                    string[] contents = new string[2]
                    {
                        "If your pet gets sick, don’t worry!",
                        "Just give them some medicine to help them feel better soon."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.EXP:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[5].parent;
                    m_TutorialChildTransform[5].SetParent(m_TutorialUpper);

                    m_TutorialChildTransform[4].gameObject.SetActive(false);

                    string[] contents = new string[2]
                    {
                        "By attending to these needs, you earn XP and Tickets.",
                        "Both are essential for your journey as a Guardian."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Ticket:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[6].parent;
                    m_TutorialChildTransform[6].SetParent(m_TutorialUpper);
                    string[] contents = new string[1]
                    {
                        "Tickets are earned through daily activities and can be used to buy food, medicine, and toys.",
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Diamond:
                {
                    canNext = false;
                    previousParent = m_TutorialChildTransform[7].parent;
                    m_TutorialChildTransform[7].SetParent(m_TutorialUpper);
                    string[] contents = new string[1]
                    {
                        "You may also encounter Diamonds, a rarer currency, which can be used for special purchases.",
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Twelth:
                {
                    canNext = false;
                    string[] contents = new string[1]
                    {
                        "The kami are always looking for guardians, so they will reward you with extra XP for every friend you bring into the forest.",
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Thirteenth:
                {
                    canNext = false;
                    string[] contents = new string[2]
                    {
                        "Remember, your Tongotchi follows a cycle of 6 hours awake and 2 hours asleep.",
                        "Keeping to this schedule will help maintain their well-being."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
            case TutorialPhase.Fourteenth:
                {
                    canNext = false;
                    string[] contents = new string[2]
                    {
                        "Now, with knowledge and spirit intertwined, embark on your journey.",
                        "Nurture your Tongotchi, explore the mysteries of Shizen Mori, and let the magic of the forest guide you."
                    };
                    StartCoroutine(TypeSentences(contents, () =>
                    {
                        canNext = true;
                    }));
                    break;
                }
        }
    }

    private IEnumerator TypeSentences(string[] contents, ICallback.CallFunc completed)
    {
        m_Bubble.SetActive(true);
        foreach (string st in contents)
        {
            m_WispText.text = string.Empty;

            foreach (char c in st.ToCharArray())
            {
                m_WispText.text += c;
                if (m_WispText.isTextOverflowing)
                    m_WispText.alignment = TextAlignmentOptions.BottomLeft;
                else
                    m_WispText.alignment = TextAlignmentOptions.TopLeft;

                yield return textSpeed;
            }
            yield return new WaitForSeconds(1f);
            canNextSentence = true;
            m_NextText.SetActive(true);
            yield return new WaitUntil(() => canNextSentence == true && Input.GetMouseButton(0));
            canNextSentence = false;
            m_NextText.SetActive(false);
        }
        canNext = true;
        m_Bubble.SetActive(false);
        completed?.Invoke();
    }
}
