const fs = require("fs");

const setsToImport = [
    'food',
    'for-beginners',
    'irregular-verbs',
    'it',
    'most_frequent_words_1-200',
    'most_frequent_words_201-400',
    'most_frequent_words_401-600',
    'most_frequent_words_601-800',
    'most_frequent_words_801-1000',
    'most_frequent_words_1001-1200',
    'most_frequent_words_1201-1400',
    'most_frequent_words_1401-1600',
    'most_frequent_words_1601-1800',
    'most_frequent_words_1801-2000',
    'most_frequent_words_2001-2200',
    'most_frequent_words_2201-2400',
    'most_frequent_words_2401-2600',
    'most_frequent_words_2601-2800',
    'most_frequent_words_2801-3000',
    'phrasal-verbs',
    'travel'
]

for (const setToImport of setsToImport) {
    const set = JSON.parse(fs.readFileSync(`../src/AudioStudy.Bot/AudioStudy.Bot.Courses/courses/en-ru/${setToImport}.json`));
    const lines = [];
    for (const card of set.cards) {
        if(card.usage && card.usageTranslation) {
            lines.push(`${card.text};${card.translation};${card.usage};${card.usageTranslation}`)
        }
        else if(card.usage) {
            lines.push(`${card.text};${card.translation};${card.usage}`)
        }
        else {
            lines.push(`${card.text};${card.translation}`)
        }
    }
    fs.writeFileSync(`./csv/${setToImport}.csv`, lines.join('\n'));
    console.log('ok');
}
