import {Course, CourseLessons, Speed} from "./types";
import FileProvider from "./file-provider";
import AudioManager from "./audio-manager";

const audioconcat = require('audioconcat');
import {v4 as uuidv4} from 'uuid';
import AudioMetadataProvider from "./audio-metadata-provider";

const {resolve} = require('path');

const TelegramBot = require('node-telegram-bot-api');

export default class LessonAudioGenerator {
    constructor(private readonly fileProvider: FileProvider,
                private readonly audioManager: AudioManager,
                private readonly audioMetadataProvider: AudioMetadataProvider) {
    }

    public async generate(options: { course: Course, lessonsDir: string, mediaDir: string, tmpDir: string, botToken: string, chatId: number, pauseFilePath: string }): Promise<void> {
        const {course, lessonsDir, mediaDir, tmpDir, botToken, chatId, pauseFilePath} = options;
        const bot = new TelegramBot(botToken);
        for (const lessonPath of await this.fileProvider.getAllFilesInDirectory(lessonsDir, file => file.endsWith('.json'))) {
            const courseLessons: CourseLessons = JSON.parse(await this.fileProvider.readFile(lessonPath));
            let lessonNumber = 0;
            for (const lesson of courseLessons.lessons) {
                const audioFiles: string[] = [];
                for (const card of lesson.cards) {
                    await this.addPath(audioFiles, mediaDir, card.text, courseLessons.reversed ? course.translationLanguage : course.language, true);
                    audioFiles.push(pauseFilePath);
                    await this.addPath(audioFiles, mediaDir, card.translation, courseLessons.reversed ? course.translationLanguage : course.language, false);
                    audioFiles.push(pauseFilePath);
                    await this.addPath(audioFiles, mediaDir, card.usage, courseLessons.reversed ? course.translationLanguage : course.language, true);
                    audioFiles.push(pauseFilePath);
                }
                audioFiles.pop();
                const output = resolve(tmpDir, `${uuidv4()}.mp3`);
                console.log(`Concatenating ${audioFiles.length} files for ${course.id}`);
                await concatFiles(audioFiles, output);
                const fileName = `${padBatchAudioNumber(++lessonNumber)}. ${course.name}.mp3`
                console.log(`Sending file to Telegram for ${course.id}`);
                const sendingResult: { fileId: string } = await bot.sendAudio(chatId, output, {}, {
                    filename: fileName
                });
                console.log(JSON.stringify(sendingResult));
                lesson.fileId = sendingResult.fileId;
                throw new Error();
            }
            await this.fileProvider.writeFile(lessonPath, JSON.stringify(courseLessons, null, 2));
        }
    }

    private async addPath(files: string[], mediaDir: string, text: string, lang: string, isTarget: boolean) {
        files.push(await this.audioManager.getPath(mediaDir, text, lang, this.audioMetadataProvider.getVoice(lang)!,
            isTarget ? this.audioMetadataProvider.getTargetVoiceSpeed() : this.audioMetadataProvider.getTranslationVoiceSpeed()))
    }
}

async function concatFiles(files: string[], output: string) {
    return new Promise<void>((resolve, reject) => {
        audioconcat(files).concat(output).on('error', function (err: Error) {
            reject(err)
        })
            .on('end', function () {
                resolve();
            });
    });
}

function padBatchAudioNumber(n: number) {
    const nn = String(n);
    const width = 4;
    const z = '0';
    return nn.length >= width ? nn : new Array(width - nn.length + 1).join(z) + nn;
}