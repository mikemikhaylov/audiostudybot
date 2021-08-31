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

    public async generate(options: { course: Course, lessonsDir: string, mediaDir: string, tmpDir: string, botToken: string, chatId: number, pauseFilePath: string, longPauseFilePath: string }): Promise<void> {
        const {course, lessonsDir, mediaDir, tmpDir, botToken, chatId, pauseFilePath, longPauseFilePath} = options;
        if (!await this.fileProvider.fileExists(pauseFilePath)) {
            throw new Error('Pause file does not exist');
        }
        const bot = new TelegramBot(botToken);
        for (const lessonPath of await this.fileProvider.getAllFilesInDirectory(lessonsDir, file => file.endsWith('.json'))) {
            console.log(`Working on audio for ${lessonPath}.`);
            const courseLessons: CourseLessons = JSON.parse(await this.fileProvider.readFile(lessonPath));
            let lessonNumber = 0;
            for (const lesson of courseLessons.lessons) {
                lessonNumber++;
                if (lesson.fileId) {
                    //continue;
                }
                const audioFiles: string[] = [];
                for (const card of lesson.cards) {
                    await this.addPath(audioFiles, mediaDir, card.text, courseLessons.reversed ? course.translationLanguage : course.language, true);
                    audioFiles.push(longPauseFilePath);
                    await this.addPath(audioFiles, mediaDir, card.translation, courseLessons.reversed ? course.language : course.translationLanguage, false);
                    if (card.usage) {
                        audioFiles.push(pauseFilePath);
                        await this.addPath(audioFiles, mediaDir, card.usage, courseLessons.reversed ? course.translationLanguage : course.language, true);
                    }
                    audioFiles.push(pauseFilePath);
                }
                audioFiles.pop();
                const output = resolve(tmpDir, `${Date.now()}_${uuidv4()}.mp3`);
                console.log(`Concatenating ${audioFiles.length} files for ${lessonPath}`);
                await concatFiles(audioFiles, output);
                const fileName = `${padBatchAudioNumber(lessonNumber)}. ${courseLessons.reversed && (course.nameTranslation && course.nameTranslation.trim()) ? course.nameTranslation : course.name}.mp3`
                console.log(`Sending file to Telegram for ${course.id}`);
                const sendingResult: { audio: { file_id: string } } = await bot.sendAudio(chatId, output, {}, {
                    filename: fileName
                });
                console.log(JSON.stringify(sendingResult));
                lesson.fileId = sendingResult.audio.file_id;
                await this.fileProvider.writeFile(lessonPath, JSON.stringify(courseLessons, null, 2));
                await sleep(getRandomArbitrary(1000, 4000));
            }
        }
    }

    private async addPath(files: string[], mediaDir: string, text: string, lang: string, isTarget: boolean) {
        files.push(await this.audioManager.getPath(mediaDir, text, lang, this.audioMetadataProvider.getVoice(lang)!,
            isTarget ? this.audioMetadataProvider.getTargetVoiceSpeed() : this.audioMetadataProvider.getTranslationVoiceSpeed()))
    }
}

function concatFiles(files: string[], output: string) {
    return new Promise<void>((resolve, reject) => {
        audioconcat(files).concat(output).on('error', function (err: Error) {
            reject(err)
        })
            .on('end', function () {
                resolve();
            });
    });
}

function sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function padBatchAudioNumber(n: number) {
    const nn = String(n);
    const width = 3;
    const z = '0';
    return nn.length >= width ? nn : new Array(width - nn.length + 1).join(z) + nn;
}

function getRandomArbitrary(min: number, max: number) {
    return Math.random() * (max - min) + min;
}