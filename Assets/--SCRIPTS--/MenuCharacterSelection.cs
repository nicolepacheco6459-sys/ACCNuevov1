using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuCharacterSelection : MonoBehaviour
{
    private int index;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private TextMeshProUGUI nameText;

    private CharacterSelection characterSelection;

    private void Start()
    {
        characterSelection = CharacterSelection.Instance;

        if (characterSelection == null)
        {
            Debug.LogError("CharacterSelection instance not found. Make sure CharacterSelection is in the scene.");
            return;
        }

        index = PlayerPrefs.GetInt("PlayerIndex");

        if (index > characterSelection.characters.Count - 1)
        {
            index = 0;
        }

        ChangeScreen();
    }

    private void ChangeScreen()
    {
        PlayerPrefs.SetInt("PlayerIndex", index);
        image.sprite = characterSelection.characters[index].image;
        nameText.text = characterSelection.characters[index].characterName;
    }

    public void NextCharacter()
    {
        if (index == characterSelection.characters.Count - 1)
        {
            index = 0;
        }
        else
        {
            index += 1;
        }

        ChangeScreen();
    }

    public void PrevCharacter()
    {
        if (index == 0)
        {
            index = characterSelection.characters.Count - 1;
        }
        else
        {
            index -= 1;
        }

        ChangeScreen();
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
}
