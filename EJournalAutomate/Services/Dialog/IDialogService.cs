namespace EJournalAutomate.Services.Dialog;

public interface IDialogService
{
    void ShowError(string message);
    void ShowInfo(string message);
    bool ShowConfirmation(string message);
}