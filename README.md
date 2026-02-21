# STRG Editor

Ferramenta desktop (Windows Forms) para abrir, visualizar, editar e salvar arquivos de texto `.STRG` usados em jogos como **DKCR** e **Metroid Prime 3** de Nintendo Wii.

## O que a ferramenta faz

- Abre **um ou vários arquivos STRG** ao mesmo tempo.
- Exibe as **linguagens disponíveis** no arquivo.
- Mostra e permite editar os textos em uma grade com suporte a múltiplas linhas.
- Salva alterações no arquivo original (**Save**) ou em um novo arquivo (**Save As**).
- Faz **busca de texto** com navegação entre resultados.
- Exporta textos para `.txt` e importa textos a partir de `.txt`.
- Permite fechar um arquivo específico ou todos os arquivos carregados.

## Recursos principais

- **Edição por idioma:** selecione um idioma e edite somente aquele conjunto de textos.
- **Busca inteligente:** encontra ocorrências no idioma selecionado e permite navegar entre elas.
- **Controle de alterações:** identifica arquivos modificados e avisa antes de fechar/importar.
- **Importação/Exportação TXT:** útil para tradução e revisão fora do programa.
- **Tratamento de quebras de linha:** converte `\n` corretamente entre visualização e armazenamento.

## Requisitos

- **Windows**
- **.NET Framework 4.8**
- Visual Studio (recomendado para compilar e executar)

## Como executar

1. Abra a solução `STRGeditor.sln` no Visual Studio.
2. Compile o projeto `STRGeditor`.
3. Execute a aplicação (`F5` ou "Start").

## Fluxo básico de uso

1. `File > Open` para carregar um ou mais arquivos `.STRG`.
2. Selecione o arquivo e o idioma desejado.
3. Edite os textos na tabela.
4. Use a barra de busca para localizar textos específicos.
5. Salve com `Save`, `Save All` ou `Save As`.
6. Opcionalmente, exporte/importa textos via `TXT`.
