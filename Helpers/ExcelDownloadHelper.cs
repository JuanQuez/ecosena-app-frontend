#if ANDROID
using Android.Content;
using Android.Provider;
#endif

namespace EcosenaApp.Helpers;

public static class ExcelDownloadHelper
{
    private const string XlsxMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public static Task<bool> TrySaveToDownloadsAsync(byte[] bytes, string fileName)
    {
#if ANDROID
        return Task.FromResult(SaveToDownloadsAndroid(bytes, fileName));
#elif WINDOWS
        return Task.FromResult(SaveToDownloadsWindows(bytes, fileName));
#else
        return Task.FromResult(false);
#endif
    }

#if ANDROID
    private static bool SaveToDownloadsAndroid(byte[] bytes, string fileName)
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
            return false;

        var resolver = Android.App.Application.Context.ContentResolver;
        if (resolver is null)
            return false;

        var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, XlsxMimeType);
        values.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

        var uri = resolver.Insert(MediaStore.Downloads.ExternalContentUri!, values);
        if (uri is null)
            return false;

        using var stream = resolver.OpenOutputStream(uri);
        if (stream is null)
            return false;

        stream.Write(bytes, 0, bytes.Length);
        return true;
    }
#endif

#if WINDOWS
    private static bool SaveToDownloadsWindows(byte[] bytes, string fileName)
    {
        var downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        Directory.CreateDirectory(downloads);
        var path = Path.Combine(downloads, fileName);
        File.WriteAllBytes(path, bytes);
        return true;
    }
#endif
}
