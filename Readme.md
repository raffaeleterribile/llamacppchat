# Semplici applicazioni per Llama.cpp

Queste sono alcune semplici applicazioni per Llama.cpp.

Scaricare Phi-3-mini-4k-instruct-q4.gguf da https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-gguf/tree/main e metterlo nella cartella principale.

Sono 4 applicazioni: 2 in Python e 2 in C#; 2 applicazioni usano le API delle librerie di Llama.cpp e 2 usano il server web REST Llama-Server.

- LlamaCppClientPy: Applicazione console che usa la libreria Python di OpenAI per comunicare con Llama-Server.
- LlamaCppClientCS: Applicazione Windows con una semplicissima interfaccia che usa la libreria C# di OpenAI per comunicare con Llama-Server.
- LlamaCppPy: Applicazione console che usa la libreria Python llama-cpp-python che si interfaccia a Llama.cpp tramite bindings.
- LlamaCppCS: Applicazione Windows con una semplicissima interfaccia che usa la libreria C# LlamaSharp che si interfaccia a Llama.cpp tramite bindings.

Per avviare il server Llama:

LlamaCppServer/llama-server -m Phi-3-mini-4k-instruct-q4.gguf -c 2048 --port 8080

Per installare la libreria Python, si può usare il seguente comando:
pip install llama-cpp-python --extra-index-url https://abetlen.github.io/llama-cpp-python/whl/cpu

Per installare la versione server in Python, si può usare il seguente comando:
pip install 'llama-cpp-python[server]'

Per avviare il server in Python, si può usare il seguente comando:
python -m llama_cpp.server --model PATH_TO_GGUF_MODEL --port 8080

Per avviarlo con un formato di prompt specifico:
python -m llama_cpp.server --model PATH_TO_GGUF_MODEL --chat_format chatml

# Progetto LlamaCppPy

Nel file [README](https://github.com/abetlen/llama-cpp-python?tab=readme-ov-file) del progetto llama-cpp-python si dice che è possibile installare la libreria specificando il backend da usare.
Per esempio:
pip install llama-cpp-python -C cmake.args="-DGGML_BLAS=ON;-DGGML_BLAS_VENDOR=OpenBLAS"

In realtà, anche così, l'applicazione non parte, dando l'errore:
```
Traceback (most recent call last):
  File "PATH_TO_ENV\lib\site-packages\llama_cpp\_ctypes_extensions.py", line 67, in load_shared_library
    return ctypes.CDLL(str(lib_path), **cdll_args)  # type: ignore
  File "PATH_TO_ENV\lib\ctypes\__init__.py", line 374, in __init__
    self._handle = _dlopen(self._name, mode)
OSError: [WinError 193] %1 non è un'applicazione di Win32 valida

During handling of the above exception, another exception occurred:

Traceback (most recent call last):
  File "PATH_TO_ENV\lib\runpy.py", line 196, in _run_module_as_main
    return _run_code(code, main_globals, None,
  File "PATH_TO_ENV\lib\runpy.py", line 86, in _run_code
    exec(code, run_globals)
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy\__main__.py", line 39, in <module>
    cli.main()
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy/..\debugpy\server\cli.py", line 430, in main
    run()
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy/..\debugpy\server\cli.py", line 284, in run_file
    runpy.run_path(target, run_name="__main__")
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy\_vendored\pydevd\_pydevd_bundle\pydevd_runpy.py", line 321, in run_path
    return _run_module_code(code, init_globals, run_name,
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy\_vendored\pydevd\_pydevd_bundle\pydevd_runpy.py", line 135, in _run_module_code
    _run_code(code, mod_globals, init_globals,
  File "c:\program files\microsoft visual studio\2022\community\common7\ide\extensions\microsoft\python\core\debugpy\_vendored\pydevd\_pydevd_bundle\pydevd_runpy.py", line 124, in _run_code
    exec(code, run_globals)
  File "C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppPy\llamacpp.py", line 1, in <module>
    from llama_cpp import Llama
  File "PATH_TO_ENV\lib\site-packages\llama_cpp\__init__.py", line 1, in <module>
Il thread 'MainThread' (1) è terminato con il codice 0 (0x0).
    from .llama_cpp import *
  File "PATH_TO_ENV\lib\site-packages\llama_cpp\llama_cpp.py", line 38, in <module>
    _lib = load_shared_library(_lib_base_name, _base_path)
  File "PATH_TO_ENV\lib\site-packages\llama_cpp\_ctypes_extensions.py", line 69, in load_shared_library
    raise RuntimeError(f"Failed to load shared library '{lib_path}': {e}")
RuntimeError: Failed to load shared library 'PATH_TO_ENV\lib\site-packages\llama_cpp\lib\llama.dll': [WinError 193] %1 non è un'applicazione di Win32 valida
```

## Progetto LlamaCppCS

Installando sia il pacchetto "LLamaSharp" che il pacchetto "LLamaSharp.Backend.Cpu" si ha il seguente errore di compilazione:

NETSDK1152: Found multiple publish output files with the same relative path

È un problema di LlamaSharp, tracciato nella issue [382](https://github.com/SciSharp/LLamaSharp/issues/382) del loro repository.
Sembrerebbe risolta con la pull request [561](https://github.com/SciSharp/LLamaSharp/pull/561), ma a me si verifica ancora.
Sempre nella issue 382, c'è un commento che sembra essere utile: [https://github.com/SciSharp/LLamaSharp/issues/382#issuecomment-2155704269](https://github.com/SciSharp/LLamaSharp/issues/382#issuecomment-2155704269)
Il commento indica di aggiungere queste righe nel file .csproj (tenere presente che occorre sostituire la versione 0.12.0 di LlamaSharp indicata nel commento con la versione installata, che ora è la 0.18.0):
```xml
    <PropertyGroup>
        <ErrorOnDuplicatePublishOutputFiles>false</ErrorOnDuplicatePublishOutputFiles>
        <RestorePackagesPath>bin\$(Configuration)\.nuget\packages</RestorePackagesPath>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="LLamaSharp" Version="0.12.0" />
        <PackageReference Include="LLamaSharp.Backend.Cpu" Version="0.12.0" />
        <PackageReference Include="LLamaSharp.Backend.Cuda11" Version="0.12.0" />
        <PackageReference Include="LLamaSharp.Backend.Cuda12" Version="0.12.0" />
        <PackageReference Include="LLamaSharp.Backend.OpenCL" Version="0.12.0" />
    </ItemGroup>

    <ItemGroup>
        <LlamaSharpBackendCpu Include="$(RestorePackagesPath)\llamasharp.backend.cpu\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendCuda11 Include="$(RestorePackagesPath)\llamasharp.backend.cuda11\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendCuda12 Include="$(RestorePackagesPath)\llamasharp.backend.cuda12\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendOpenCL Include="$(RestorePackagesPath)\llamasharp.backend.opencl\0.12.0\runtimes\**\*.*" />
    </ItemGroup>

    <Target Name="CopyRuntimesFolderOnPublish" AfterTargets="Publish">
        <Delete Files="$(PublishDir)llama.dll" />
        <Delete Files="$(PublishDir)llava_shared.dll" />
        <Copy SourceFiles="@(LlamaSharpBackendCpu)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda11)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda12)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendOpenCL)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
    </Target>
```

Però nel [workaround](https://github.com/plastic-plant/llamasharp-issue-382/commit/a47684f07928003ec366dc1a5f3a4c933cce2948#diff-a7cc41d8cb5ad2283b1efa22a43a35152f6b8066c5fb1b6cc08791a18e24e9a2R56) 
indicato nel commento è indicata una soluzione leggermente diversa:
```xml
    <ItemGroup>
        <LlamaSharpBackendCpu Include="$(RestorePackagesPath)\llamasharp.backend.cpu\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendCuda11 Include="$(RestorePackagesPath)\llamasharp.backend.cuda11\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendCuda12 Include="$(RestorePackagesPath)\llamasharp.backend.cuda12\0.12.0\runtimes\**\*.*" />
        <LlamaSharpBackendOpenCL Include="$(RestorePackagesPath)\llamasharp.backend.opencl\0.12.0\runtimes\**\*.*" />
    </ItemGroup>
    <Target Name="CopyRuntimesFolderOnBuild" AfterTargets="Build">
        <Delete Files="$(OutDir)llama.dll" />
        <Delete Files="$(OutDir)llava_shared.dll" />
        <Copy SourceFiles="@(LlamaSharpBackendCpu)" DestinationFolder="$(OutputPath)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda11)" DestinationFolder="$(OutputPath)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda12)" DestinationFolder="$(OutputPath)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendOpenCL)" DestinationFolder="$(OutputPath)\runtimes\%(RecursiveDir)" />
    </Target>
    <Target Name="CopyRuntimesFolderOnPublish" AfterTargets="Publish">
        <Delete Files="$(PublishDir)llama.dll" />
        <Delete Files="$(PublishDir)llava_shared.dll" />
        <Copy SourceFiles="@(LlamaSharpBackendCpu)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda11)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendCuda12)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
        <Copy SourceFiles="@(LlamaSharpBackendOpenCL)" DestinationFolder="$(PublishDir)\runtimes\%(RecursiveDir)" />
    </Target>
    <!-- /Workaround -->
```
Applicando questo suggerimento, l'errore cambia in:
APPX1101	il payload contiene due o più file con lo stesso percorso di destinazione 'ggml.dll'. File di origine: 
C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppCS\bin\Debug\.nuget\packages\llamasharp.backend.cpu\0.18.0\runtimes\win-x64\native\avx\ggml.dll
C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppCS\bin\Debug\.nuget\packages\llamasharp.backend.cpu\0.18.0\runtimes\win-x64\native\avx2\ggml.dll
C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppCS\bin\Debug\.nuget\packages\llamasharp.backend.cpu\0.18.0\runtimes\win-x64\native\avx512\ggml.dll
C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppCS\bin\Debug\.nuget\packages\llamasharp.backend.cpu\0.18.0\runtimes\win-x64\native\ggml.dll	LlamaCppCS (net8.0-windows10.0.19041.0)	C:\Progetti\VisualStudio\LlamaCppChat\LlamaCppCS\bin\Debug\.nuget\packages\microsoft.windowsappsdk\1.5.240802000\buildTransitive\Microsoft.Build.Msix.Packaging.targets	1532		

Forse i file duplicati potrebbero essere rimossi così:
```xml
  <ItemGroup>
    <!-- Escludi i file duplicati -->
    <None Remove="runtimes\win-x64\native\avx\ggml.dll" />
    <None Remove="runtimes\win-x64\native\avx2\ggml.dll" />
    <None Remove="runtimes\win-x64\native\avx512\ggml.dll" />
    <None Remove="runtimes\win-x64\native\avx\llama.dll" />
    <None Remove="runtimes\win-x64\native\avx2\llama.dll" />
    <None Remove="runtimes\win-x64\native\avx512\llama.dll" />
    <None Remove="runtimes\win-x64\native\avx\llava_shared.dll" />
    <None Remove="runtimes\win-x64\native\avx2\llava_shared.dll" />
    <None Remove="runtimes\win-x64\native\avx512\llava_shared.dll" />
  </ItemGroup>
```
Ma non funziona...

Installando solo "LLamaSharp" si ha il seguente errore:

```
System.TypeInitializationException
  HResult=0x80131534
  Messaggio=The type initializer for 'LLama.Native.NativeApi' threw an exception.
  Origine=<Non è possibile valutare l'origine dell'eccezione>
  Analisi dello stack:
<Non è possibile valutare l'analisi dello stack dell'eccezione>

  Questa eccezione è stata generata in origine nello stack di chiamate seguente:
    [Codice esterno]

Eccezione interna 1:
RuntimeError: The native library cannot be correctly loaded. It could be one of the following reasons: 
1. No LLamaSharp backend was installed. Please search LLamaSharp.Backend and install one of them. 
2. You are using a device with only CPU but installed cuda backend. Please install cpu backend instead. 
3. One of the dependency of the native library is missed. Please use `ldd` on linux, `dumpbin` on windows and `otool`to check if all the dependency of the native library is satisfied. Generally you could find the libraries under your output folder.
4. Try to compile llama.cpp yourself to generate a libllama library, then use `LLama.Native.NativeLibraryConfig.WithLibrary` to specify it at the very beginning of your code. For more information about compilation, please refer to LLamaSharp repo on github.
```

Si possono prendere i file ggml.dll, llama.dll e llava_shared.dll 
dalla cartella `bin\Debug\.nuget\packages\llamasharp.backend.cpu\0.19.0\runtimes\win-x64\native` 
e metterli in una cartella del progetto (in questo caso in LlamaCppCPU) ed includerli nella progetto come "Contenuti".
È anche necessario specificare queste opzioni:

    NativeLibraryConfig.All.WithAvx(AvxLevel.None);
    NativeLibraryConfig.All.WithCuda(false);
    NativeLibraryConfig.All.WithVulkan(false);
    NativeLibraryConfig.All.WithLibrary(".\\LlamaCppCPU\\llama.dll", ".\\LlamaCppCPU\\llava.dll");

Queste opzioni indicano che non si vuole usare le istruzioni AVX, CUDA e Vulkan e che la libreria nativa è in una cartella specifica.
