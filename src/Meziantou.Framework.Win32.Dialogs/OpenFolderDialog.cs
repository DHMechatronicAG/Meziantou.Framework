using System.ComponentModel;
using System.Runtime.Versioning;
using Meziantou.Framework.Win32.Natives;

namespace Meziantou.Framework.Win32;

[SupportedOSPlatform("windows")]
public sealed class OpenFolderDialog
{
    public DialogResult ShowDialog()
    {
        return ShowDialog(IntPtr.Zero);
    }

    public DialogResult ShowDialog(IntPtr owner) // IWin32Window
    {
        var hwndOwner = owner != IntPtr.Zero ? owner : NativeMethods.GetActiveWindow();
        var dialog = (IFileOpenDialog)new NativeFileOpenDialog();
        Configure(dialog);

        var hr = dialog.Show(hwndOwner);
        if (hr == NativeMethods.ERROR_CANCELLED)
            return DialogResult.Cancel;

        if (hr != NativeMethods.S_OK)
            return DialogResult.Abort;

        dialog.GetResult(out var item);
        item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var path);
        SelectedPath = path;
        return DialogResult.OK;
    }

    public string? Title { get; set; }
    public string? OkButtonLabel { get; set; }
    public string? InitialDirectory { get; set; }
    public string? SelectedPath { get; set; }
    public bool ChangeCurrentDirectory { get; set; }

    private void Configure(IFileOpenDialog dialog)
    {
        dialog.SetOptions(CreateOptions());

        if (!string.IsNullOrEmpty(InitialDirectory))
        {
            var result = NativeMethods.SHCreateItemFromParsingName(InitialDirectory, IntPtr.Zero, typeof(IShellItem).GUID, out var item);
            switch (result)
            {
                case NativeMethods.S_OK:
                    if (item is not null)
                    {
                        dialog.SetFolder(item);
                    }

                    break;
                case NativeMethods.FILE_NOT_FOUND:
                    break;
                default:
                    throw new Win32Exception(result);
            }
        }

        if (Title is not null)
        {
            dialog.SetTitle(Title);
        }

        if (OkButtonLabel is not null)
        {
            dialog.SetOkButtonLabel(OkButtonLabel);
        }
    }

    private FOS CreateOptions()
    {
        var result = FOS.FOS_FORCEFILESYSTEM | FOS.FOS_PICKFOLDERS;
        if (!ChangeCurrentDirectory)
        {
            result |= FOS.FOS_NOCHANGEDIR;
        }

        return result;
    }
}
