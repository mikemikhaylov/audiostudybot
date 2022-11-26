const users = require('./users');
const {Telegraf} = require('telegraf');
const bot = new Telegraf('');

console.log('starting');

const text = '<b>Запуск приложения</b>\n\nПривет! Рады сообщить, что запустили мобильное приложение <b>AudioStudy</b>. Теперь учить английский через аудио флэш-карточки стало ещё удобнее.\n\nПодробности в <a href=\'https://vc.ru/tribuna/548023-prilozhenie-dlya-passivnogo-izucheniya-yazykov-ili-podgotovki-k-ekzamenam-audiostudy\'>статье на vc.ru.</a>\n\nБудем благодарны за поддержку статьи! Приложение пока для iOS, запуск на Android будет зависеть от вашей обратной связи. Так что напишите в комментариях, нужна ли версия для Android!';

async function start() {
    console.log('inside start, users count  ' + users.length)
    for (let i = 0; i < users.length; i++) {
        console.log(`Sending notification for user ${i} with chat id ${users[i].ChatId}`);
        try {
            await bot.telegram.sendMessage(users[i].ChatId,text, {parse_mode: 'HTML'});
            console.log(`Sent notification for user ${i} from ${users.length} with chat id ${users[i].ChatId}`);
            console.log('Sleeping');
            await sleep(1500);
        }
        catch(ex) {
            console.log(ex);
            continue;
        }
    }
    console.log('finished start')
}

start().catch(reason => console.log(reason));

async function sleep(msec) {
    return new Promise(resolve => setTimeout(resolve, msec));
}
