using UnityEngine;

[CreateAssetMenu(fileName = "NewChallenge", menuName = "Game/Challenge Data")]
public class ChallengeData : ScriptableObject
{
    [Header("Challenge Info")]
    public string challengeQuestion;
    public float timeLimit = 60f;
    
    [Header("Answers")]
    public ChestAnswer[] answers;
}

[System.Serializable]
public class ChestAnswer
{
    [TextArea(2, 4)]
    public string answerText;
    public bool isCorrect;
    public bool isWinningAnswer;
}