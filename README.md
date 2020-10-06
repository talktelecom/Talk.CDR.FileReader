# Talk.CDR.FileReader

Requesito de execução do progreama

    - Sistema operacional Windows 7 ou superior, MacOS, Linux
    - Ter instalado o dotnetframework 3.1
        Referencia https://dotnet.microsoft.com/download/dotnet-core/3.1
        Windows - https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.6-windows-x64-installer
        Linux - https://docs.microsoft.com/dotnet/core/install/linux-package-managers
        MacOs - https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-aspnetcore-3.1.6-macos-x64-binaries

Exemplos de utilização

    - Extraia o zip do executavel compilado
    - Atraves do cmd acesse a pasta do mesmo
    - Execute o seguinte comando:
    ```
        $ dotnet Talk.CDR.FileReader.dll "ExemploClaro.txt"
    ```
    - Repare que esta sendo passado como argumento o nome do arquivo que deverá ser processado. O mesmo pode ser apenas o nome caso esteja na mesma pasta ou o caminho completo caso esteja em outro diretorio
    ```
    $ dotnet Talk.CDR.FileReader.dll "C:\CDR\Exemplos\claro.txt"
    ```

    Será solicitado o layout do arquivo informado. Deverá ser selecionado as opções entre 1-5.

    Ao final do processamento será gerado um arquivo csv com o resumo de informações do mesmo.
