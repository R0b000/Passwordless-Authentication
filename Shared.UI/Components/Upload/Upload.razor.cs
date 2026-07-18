using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Collections.Generic;

namespace Shared.UI.Components.Upload
{
    public partial class Upload : ComponentBase
    {
        [Parameter] public bool Multiple { get; set; } = false;
        [Parameter] public string? Accept { get; set; }
        [Parameter] public long MaxSize { get; set; } = 10 * 1024 * 1024;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public EventCallback<IBrowserFile> OnFileSelected { get; set; }
        [Parameter] public EventCallback<IBrowserFile> OnFileRemoved { get; set; }

        private bool _dragover;
        private List<UploadFile> Files { get; set; } = new();

        private async Task OnInputChange(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(Multiple ? int.MaxValue : 1))
            {
                await AddFile(file);
            }
        }

        private async Task OnDrop()
        {
            _dragover = false;
        }

        private async Task AddFile(IBrowserFile file)
        {
            if (file.Size > MaxSize) return;
            var entry = new UploadFile { Name = file.Name, Size = file.Size, Progress = 0, File = file };
            Files.Add(entry);
            await OnFileSelected.InvokeAsync(file);
            await SimulateProgress(entry);
        }

        private async Task RemoveFile(UploadFile file)
        {
            Files.Remove(file);
            await OnFileRemoved.InvokeAsync(file.File);
        }

        private async Task SimulateProgress(UploadFile file)
        {
            while (file.Progress < 100)
            {
                await Task.Delay(120);
                file.Progress = Math.Min(100, file.Progress + 10);
                StateHasChanged();
            }
        }

        private string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }

        private class UploadFile
        {
            public string Name { get; set; } = string.Empty;
            public long Size { get; set; }
            public int Progress { get; set; }
            public IBrowserFile File { get; set; } = default!;
        }
    }
}
