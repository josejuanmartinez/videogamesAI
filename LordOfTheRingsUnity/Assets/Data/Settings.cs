using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField] NationsEnum nation;
    [SerializeField] DifficultiesEnum difficulty;

    public void SetHumanPlayer(NationsEnum nation)
    {
        this.nation = nation;
    }

    public void SetDifficulty(DifficultiesEnum difficulty)
    {
        this.difficulty = difficulty;
    }

    public NationsEnum GetHumanPlayer()
    {
        return nation;
    }

    public DifficultiesEnum GetDifficulty()
    {
        return difficulty;
    }
}
