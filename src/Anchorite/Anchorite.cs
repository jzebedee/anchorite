using SQLitePCL;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Anchorite;

public static class Anchorite
{
    private const string NativeLibraryName = "sqlite3_anchorite";

    sealed class NativeLibraryAdapter : IGetFunctionPointer
    {
        readonly IntPtr _library;

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string filename);

        [DllImport("kernel32")]
        static extern IntPtr GetProcAddress(IntPtr handle, string functionName);

        public NativeLibraryAdapter(string name)
            => _library = LoadLibrary(name);

        public IntPtr GetFunctionPointer(string name)
            => GetProcAddress(_library, name);
    }

    public static void Init(string assemblyLocation, bool freeze = false)
    {
        SQLite3Provider_dynamic_cdecl.Setup(NativeLibraryName, new NativeLibraryAdapter(Path.Combine(assemblyLocation, NativeLibraryName)));
        raw.SetProvider(new SQLite3Provider_dynamic_cdecl());
        if (freeze)
        {
            raw.FreezeProvider();
        }
    }
}
