# EJournal Automate

#### Для чего?
Программа предназначена для скачивания вложений из писем на цифровой образовательной платформе ЭлЖур.
#### Как работает?
Создаётся папка группы с названием группы, и в группе создаются папки на каждого студента с названием их ФИО. Каждое вложение из писем загружаются в папку студента, который его отправил. Если вложений несколько, то в папке студента создаётся папка с названием темы письма, в которую уже и будут загружаться вложения этого письма.
#### Для кого эта программа?
В первую очередь, для преподавателей, которые взаимодействуют со студентами через платформу ЭлЖур и им необходимо загружать разные вложения от студентов, например, практические работы.

## Использование
Скачайте [последнюю версию](https://github.com/Net2Fox/EJournalAutomate/releases), распакуйте и запустите EJournal Automate.

## Инструкция для разработчиков
Требования:
* Visual Studio 2022 v17.12 или новее.
* .NET 9.0.201 или новее.

Как собрать проект:

1. Клонируйте репозиторий:
   ```
   git clone https://github.com/Net2Fox/EJournalDesktop.git
   ```
2. Откройте проект в Visual Studio 2022.
3. Для корректной работы с API, необходимо получить ключ разработчика (DevKey). Получить его можно через тех. поддержку. Этот DevKey необходимо вставить в переменную "DevKey" в файле "EJournalAutomate/Services/API/ApiService.cs".
4. Соберите и запустите проект.

## Документация API

Подробную информацию о API можно найти на официальном сайте [ЭлЖура](https://eljur.ru/api/).

## Используемые библиотеки

Этот проект использует следующие библиотеки с открытым исходным кодом:

- **CommunityToolkit.Mvvm** - лицензируется под MIT License. См. [CommunityToolkit.Mvvm License](https://github.com/CommunityToolkit/dotnet?tab=License-1-ov-file#mit-license-mit).
- **Microsoft.Extensions.DependencyInjection** - лицензируется под MIT License. См. [Microsoft.Extensions.DependencyInjection License](https://github.com/dotnet/runtime?tab=MIT-1-ov-file).
- **Microsoft.Extensions.Logging** - лицензируется под MIT License. См. [Microsoft.Extensions.Logging License](https://github.com/dotnet/runtime?tab=MIT-1-ov-file).

## Лицензия
Этот проект находится под лицензией MIT. Подробности см. в файле [LICENSE](LICENSE).
