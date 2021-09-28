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

``id`` - can have any string value, but please specify GUID

``version`` - you can specify any integer but for a newly added course please specify 0, multiple versions of course can exist simultaniously. It will be useful for example in case if you add or delete cards, so that those who already started the cours will not be affected.

``weight`` - used solely for sorting purpuses, the lesses the higher.

Be aware of telegram message maximum length 

## Lesson Generator

## Telegram Bot