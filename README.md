# AudioStudyBot - Telegram bot with audio-only language flashcards

The repository consists of 2 parts. The first one is a Node.js application written in TypeScript which can help you create lessons with audio-only flashcards and built-in spaced repetition.

The second one is the Telegram bot that you can use to deliver/distribute/consume the lessons.

If you are not interested in studying the source code or adding new courses you can go straight to the bot and check it out: 

👉👉👉 [https://t.me/audiostudybot](https://t.me/audiostudybot)

## Adding New Courses
You can find existing courses [here](src/AudioStudy.Bot/AudioStudy.Bot.Courses/courses). Basically, a course has the following JSON stricture.
```json
{
  "id": "13800754-f9d3-43cf-808e-a51cb4d16125",
  "version": 1,
  "weight": 0,
  "type": "words",
  "name": "For beginners",
  "nameTranslation": "Для начинающих",
  "language": "en",
  "translationLanguage": "ru",
  "description": "",
  "descriptionTranslation": "",
  "cards": [
    {
      "text": "listen",
      "translation": "cлушать",
      "transcription": "ˈlɪsn",
      "usage": "Listen, everybody!",
      "usageTranslation": "Слушайте, все!"
    }
  ]
}
```

``id`` - *String.* Can have any value, but please specify GUID.

``version`` - *Number.* You can specify any integer but for a newly added course please specify 0, multiple versions of a course can exist simultaneously. It will be useful for example in case if you add or delete cards so that those who already started the course will not be affected.

``weight`` - *Number.* Used solely for sorting purposes, the lesser the higher.

``type`` - *'words' | 'phrases'*. Only affects bot UI.

``name`` - *String.* Name of the course.

``nameTranslation`` - *String.* Used when ``translationLanguage`` coincides with user interface language, otherwise, the ``name`` is used. *Optional.*

``language`` - *String.* Target language. In usual flashcards, it would be the language of the front side. E.g. 'en', 'ru', 'es'.

``translationLanguage`` - *String.* Translation language. In usual flashcards, it would be the language of the back side. E.g. 'en', 'ru', 'es'.

``description`` - *String.* Description of the course. *Optional.*

``descriptionTranslation`` - *String.* Used when ``translationLanguage`` coincides with user interface language, otherwise, ``description`` is used. *Optional.*

``cards`` - *Array.* Array of cards.

``cards.text`` - *String.* A word or a phrase you want to learn. Frond side of a flashcard.

``cards.transcription`` - *String.* Transcription of ``text``. *Optional.*

``cards.translation`` - *String.* Translation of a word or a phrase you want to learn. Frond back of a flashcard. 

``cards.usage`` - *String.* Usage example. *Optional.*

``cards.usageTranslation`` - *String.* Translation of the usage example. *Optional.*

``cards.translationTranscription`` - *String.* Used when you later want to reverse a course. E.g. to make a French-English course from English-French. *Optional.*

Be aware that Telegram maximum length for the audio caption is 1024, so please don't use cards with a lot of text in them.

If you want to add a course to this repository then please just add it to this [directory](src/AudioStudy.Bot/AudioStudy.Bot.Courses/courses). I will do everything else myself. If you want to run your own instance of the bot, don't forget to add the course file as an embedded resourse in this  [project](src/AudioStudy.Bot/AudioStudy.Bot.Courses/AudioStudy.Bot.Courses.csproj).

## Lesson Generator
Node.js app so you have to have Node.js installed to run it.

The source code is [here](src/lesson-generator/).

Before running it you need to sign up for [Amazon Web Services](https://aws.amazon.com/) to be able to use Amazon Polly for text-to-speech functionality. It has a free tier for 1 year.

Then you have to get your [credentials](https://docs.aws.amazon.com/sdk-for-javascript/v2/developer-guide/getting-your-credentials.html), and set them locally for example using [AWS CLI](https://aws.amazon.com/en/cli/) and ``aws configure`` command.

Also before running it you have to set the necessary parameters in [index.ts](src/lesson-generator/src/index.ts).

```courseFilePath``` - *String.* 'path/to/course.json'.

```reversedCourseFilePath``` - *String.* If you want to generate reversed course (e.g. from English-Spanish course generate Spanish-English) from ```courseFilePath``` specify where to put the output. *Optional.*

```courseLessonsDirectory``` - *String.* Output directory for lessons JSON files. Should be empty.

```mediaOutputDirectory``` - *String.* Where to put generated audio files.

```tmpDir``` - *String.* Where to put temporary audio files.

```botToken``` - *String.* Telegram bot token.

```pauseFilePath``` - *String.* Path to mp3 file with a pause.

```longPauseFilePath``` - *String.* Path to mp3 file with a long pause.

```chatId``` - *String.* Chat ID where to upload audio files for later usage.

```botName``` - *String.* Arbitrary string. Should be the same as ```Lessons.BotName``` in the [appsettings.json](src/AudioStudy.Bot/AudioStudy.Bot.Host/appsettings.json).
       
To run it use the following command.

```js
npm run start
```

## Telegram Bot

.NET 5 app. You need to have [dotnet](https://dotnet.microsoft.com/download/dotnet) installed to run it.

The source code is [here](src/AudioStudy.Bot).

Configuration can be found in [appsettings.json](src/AudioStudy.Bot/AudioStudy.Bot.Host/appsettings.json).

## Credits

The 'Most frequent words...' English courses are based on free samples from [wordfrequency.info](https://www.wordfrequency.info/).

Some of the usage examples are from [tatoeba.org](https://tatoeba.org/). Tatoeba's data is released under CC-BY 2.0 FR. 

## Disclaimer
Not the prettiest code I've ever written.