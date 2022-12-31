﻿using NAPS2.Platform.Linux;

namespace NAPS2.Platform;

public class LinuxSystemCompat : ISystemCompat
{
    private const int RTLD_LAZY = 1;
    private const int RTLD_GLOBAL = 8;
    
    public bool IsWiaDriverSupported => false;

    public bool IsTwainDriverSupported => false;

    public bool IsAppleDriverSupported => false;

    public bool IsSaneDriverSupported => true;

    public bool CanUseWin32 => false;

    public bool CanEmail => false;

    public bool CanPrint => false;

    public bool ShouldRememberBackgroundOperations => false;

    public bool UseSystemTesseract => false;

    public bool RenderInWorker => false;

    public bool UseSeparateWorkerExe => false;

    public string[] ExeSearchPaths => LibrarySearchPaths;

    public string[] LibrarySearchPaths => new[] { "_linux" };

    public string TesseractExecutableName => "tesseract";

    public string PdfiumLibraryName => "libpdfium.so";

    public string[]? SaneLibraryDeps => null;

    public string SaneLibraryName => "libsane.so.1";
    
    public IntPtr LoadLibrary(string path) => LinuxInterop.dlopen(path, RTLD_LAZY | RTLD_GLOBAL);

    public IntPtr LoadSymbol(IntPtr libraryHandle, string symbol) => LinuxInterop.dlsym(libraryHandle, symbol);

    public string GetLoadError() => LinuxInterop.dlerror();

    public void SetEnv(string name, string value) => throw new NotSupportedException();

    public IDisposable FileReadLock(string path) => new FileStream(path, FileMode.Open, FileAccess.Read);

    public IDisposable FileWriteLock(string path) => new FileStream(path, FileMode.Open, FileAccess.Write);
}