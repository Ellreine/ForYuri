using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SUPPORTED_KEYS = new KeyCode[] {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
    };

    private Row[] rows;

    private string[] solutions;
    private string[] validWords;
    private string word;

    private int rowIndex;
    private int columnIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public TextMeshProUGUI invalidWordText;
    public TextMeshProUGUI wordCountText;
    public Button newWordButton;
    public Button tryAgainButton;

    // Добавляем ссылки на CanvasMenu, основной Canvas и кнопки
    public GameObject canvasMenu;
    public GameObject mainCanvas;
    public Button newGameButton;
    public Button continueGameButton;
    public Button exitButton;

    public string Word => word; // Добавляем публичное свойство для доступа к загаданному слову

    private int wordCount = 0;

    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();
        canvasMenu.SetActive(false); // Скрываем меню при запуске игры
        mainCanvas.SetActive(true); // Включаем основной Canvas при запуске игры
    }

    private void Start()
    {
        LoadData();
        NewGame();

        // Привязываем методы к кнопкам меню
        newGameButton.onClick.AddListener(NewGame);
        continueGameButton.onClick.AddListener(ContinueGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void NewGame()
    {
        ClearBoard();
        SetRandomWord();
        wordCount = 0; // Сбрасываем счетчик слов
        UpdateWordCountText(); // Обновляем текстовое поле
        canvasMenu.SetActive(false); // Скрываем меню при начале новой игры
        mainCanvas.SetActive(true); // Включаем основной Canvas при начале новой игры
        enabled = true;
    }

    public void NewWord()
    {
        ClearBoard();
        SetRandomWord();
        // Не сбрасываем счетчик слов
        canvasMenu.SetActive(false); // Скрываем меню при начале новой игры
        mainCanvas.SetActive(true); // Включаем основной Canvas при начале новой игры
        enabled = true;
    }

    public void ContinueGame()
    {
        canvasMenu.SetActive(false); // Скрываем меню при продолжении игры
        mainCanvas.SetActive(true); // Включаем основной Canvas при продолжении игры
        enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit(); // Выход из игры
    }

    private void LoadDataFromFile(string path, out string[] words)
    {
        if (File.Exists(path))
        {
            words = File.ReadAllLines(path);
        }
        else
        {
            Debug.LogError("File not found: " + path);
            words = new string[0];
        }
    }

    private void LoadData()
    {
        string validWordsPath = Path.Combine(Application.streamingAssetsPath, "official_wordle_all.txt");
        string solutionsPath = Path.Combine(Application.streamingAssetsPath, "official_wordle_common.txt");

        LoadDataFromFile(validWordsPath, out validWords);
        LoadDataFromFile(solutionsPath, out solutions);
    }

    private void SetRandomWord()
    {
        word = solutions[Random.Range(0, solutions.Length)];
        word = word.ToLower().Trim();
    }

    private void Update()
    {
        // Обрабатываем нажатие клавиши ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
            return;
        }

        if (!enabled) return;

        Row currentRow = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            columnIndex = Mathf.Max(columnIndex - 1, 0);

            currentRow.tiles[columnIndex].SetLetter('\0');
            currentRow.tiles[columnIndex].SetState(emptyState);

            invalidWordText.gameObject.SetActive(false);
        }
        else if (columnIndex >= currentRow.tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }
        else
        {
            for (int i = 0; i < SUPPORTED_KEYS.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEYS[i]))
                {
                    currentRow.tiles[columnIndex].SetLetter((char)SUPPORTED_KEYS[i]);
                    currentRow.tiles[columnIndex].SetState(occupiedState);
                    columnIndex++;
                    break;
                }
            }
        }
    }

    private void ToggleMenu()
    {
        bool isActive = canvasMenu.activeSelf;
        canvasMenu.SetActive(!isActive);
        mainCanvas.SetActive(isActive); // Отключаем основной Canvas при открытии меню и включаем при закрытии
        enabled = isActive; // Если меню активно, то игра приостанавливается
    }

    private void SubmitRow(Row row)
    {
        if (!IsValidWord(row.word))
        {
            invalidWordText.gameObject.SetActive(true);
            StartCoroutine(HideInvalidWordText()); // Запускаем корутину для скрытия текста через 3 секунды
            ClearRow(row); // Очищаем строку, если слово недопустимо
            return;
        }

        string remaining = word;

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.letter == word[i])
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        }

        for (int i = 0; i < row.tiles.Length; i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }

        if (HasWon(row))
        {
            wordCount++;
            UpdateWordCountText();
            newWordButton.gameObject.SetActive(true);
            enabled = false;
        }
        else
        {
            rowIndex++;
            columnIndex = 0;

            if (rowIndex >= rows.Length)
            {
                tryAgainButton.gameObject.SetActive(true);
                newWordButton.gameObject.SetActive(true);
                enabled = false;
            }
        }
    }

    private IEnumerator HideInvalidWordText()
    {
        yield return new WaitForSeconds(3);
        invalidWordText.gameObject.SetActive(false);
    }

    private void ClearRow(Row row)
    {
        for (int col = 0; col < row.tiles.Length; col++)
        {
            row.tiles[col].SetLetter('\0');
            row.tiles[col].SetState(emptyState);
        }

        columnIndex = 0;
    }

    private void ClearBoard()
    {
        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row].tiles.Length; col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetState(emptyState);
            }
        }

        rowIndex = 0;
        columnIndex = 0;
    }

    private bool IsValidWord(string word)
    {
        for (int i = 0; i < validWords.Length; i++)
        {
            if (validWords[i] == word)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasWon(Row row)
    {
        for (int i = 0; i < row.tiles.Length; i++)
        {
            if (row.tiles[i].state != correctState)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateWordCountText()
    {
        wordCountText.text = "Words Guessed: " + wordCount;
    }

    private void OnEnable()
    {
        tryAgainButton.gameObject.SetActive(false);
        newWordButton.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (rowIndex > 0 && rowIndex <= rows.Length && !HasWon(rows[rowIndex - 1]))
        {
            tryAgainButton.gameObject.SetActive(true);
        }
    }
}
