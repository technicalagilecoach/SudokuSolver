using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SudokuSolver.GUI.Services;

public interface IFileDialogService
{
    Task<string?> OpenFileAsync(string title, params FilePickerFileType[] fileTypes);
    Task<string?> SaveFileAsync(string title, params FilePickerFileType[] fileTypes);
}

public class FileDialogService : IFileDialogService
{
    private readonly TopLevel _topLevel;

    public FileDialogService(TopLevel topLevel)
    {
        _topLevel = topLevel;
    }

    public async Task<string?> OpenFileAsync(string title, params FilePickerFileType[] fileTypes)
    {
        var files = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes.Length > 0 ? fileTypes : new[] { 
                new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
            }
        });

        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    public async Task<string?> SaveFileAsync(string title, params FilePickerFileType[] fileTypes)
    {
        var file = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = title,
            FileTypeChoices = fileTypes.Length > 0 ? fileTypes : new[] { 
                new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
            }
        });

        return file?.Path.LocalPath;
    }
}