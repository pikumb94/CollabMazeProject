using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// ErrorManager allows to menage errors.
/// </summary>
public static class ErrorManager {

    public enum Error { SOFT_ERROR, HARD_ERROR, RETRY_ERROR };

    public static void ManageError(Error error, int errorCode)
    {
        switch (error)
        {
            case Error.SOFT_ERROR:
                Debug.LogError("Unexpected soft error with code " + errorCode + ".");
                break;
            case Error.HARD_ERROR:
                ErrorManager.ErrorBackToMenu(errorCode);
                break;
            case Error.RETRY_ERROR:
                Debug.LogError("Unexpected error with code " + errorCode + ". The scene will be" +
                    " reloaded.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

    // Menages errors going back to the main menu.
    public static void ManageError(Error error, string errorMessage)
    {
        switch (error)
        {
            case Error.SOFT_ERROR:
                Debug.LogError("Unexpected soft error. " + errorMessage);
                break;
            case Error.HARD_ERROR:
                ErrorManager.ErrorBackToMenu(errorMessage);
                break;
            case Error.RETRY_ERROR:
                Debug.LogError("Unexpected  error. " + errorMessage + " The scene will be " +
                    "reloaded.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                break;
        }
    }

    // Menages errors going back to the main menu.
    public static void ErrorBackToMenu(int errorCode) {
        ParameterManager.Instance.ErrorCode = errorCode;
        SceneManager.LoadScene("Error");
    }

    // Menages errors going back to the main menu.
    public static void ErrorBackToMenu(string errorMessage) {
        ParameterManager.Instance.ErrorCode = 1;
        ParameterManager.Instance.ErrorMessage = errorMessage;
        SceneManager.LoadScene("Error");
    }

}
