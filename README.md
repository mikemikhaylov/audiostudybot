# AudioStudyBot - Telegram bot with audio only language flashcards

The repository consists of 2 parts. The first one is a Node.js application written in TypeScript which can help you create lessons with audio only flashcards and built-in spaced repetition.

The second one is Telegram bot that you can use to deliver/distribute/consume the lessons.

If you are not interested in studying the source code or adding new courses you can go straight to the bot and check it out: 

[https://t.me/audiostudybot](https://t.me/audiostudybot)

## Adding New Courses
You can find existing courses [here](src/AudioStudy.Bot/AudioStudy.Bot.Courses/courses). Basically a course have the following JSON stricture.
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

``version`` - *Number.* You can specify any integer but for a newly added course please specify 0, multiple versions of course can exist simultaniously. It will be useful for example in case if you add or delete cards, so that those who already started the cours will not be affected.

``weight`` - *Number.* Used solely for sorting purpuses, the lesses the higher.

``type`` - *'words' | 'phrases'*. Only affects bot UI.

``name`` - *String.* Name of the course.

``nameTranslation`` - *String.* Used when ``translationLanguage`` coincides user interface language, otherwise ``name`` is uded. *Optional.*

``language`` - *String.* Target language. In usual flashcards it would be the language of the front side. E.g. 'en', 'ru', 'es'.

``translationLanguage`` - *String.* Translation language. In usual flashcards it would be the language of the back side. E.g. 'en', 'ru', 'es'.

``description`` - *String.* Description of the course. *Optional.*

``descriptionTranslation`` - *String.* Used when ``translationLanguage`` coincides user interface language, otherwise ``description`` is uded. *Optional.*

``cards`` - *Array.* Array of cards.

``cards.text`` - *String.* A word or a phrase you want to learn. Frond side of a flashcard.

``cards.transcription`` - *String.* Transcription of ``text``. *Optional.*

``cards.translation`` - *String.* Translation of a word or a phrase you want to learn. Frond back of a flashcard. 

``cards.usage`` - *String.* Usage example. *Optional.*

``cards.usageTranslation`` - *String.* Translation of the usage example. *Optional.*

``cards.translationTranscription`` - *String.* Used when you later want to reverse a course. E.g. to make French-English course from English-French.*Optional.*

Be aware that Telegram maximum lenght for audio caption is 1024, so please don't use cards with a lot of text in them.

If you want add a course in this repository then please just add it to this [directory](src/AudioStudy.Bot/AudioStudy.Bot.Courses/courses). I will do everything else myself. If you want to run your own instance of the bot, don't forget to add course file as an embedded resourse in this  [project](src/AudioStudy.Bot/AudioStudy.Bot.Courses/AudioStudy.Bot.Courses.csproj).

## Lesson Generator

## Telegram Bot

## Disclaimer
Not the prettiest code I've ever wtitten.