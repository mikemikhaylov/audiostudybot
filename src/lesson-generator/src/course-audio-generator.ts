import {Course, Speed} from "./types";
import Synthesizer from "./synthesizer";
import AudioManager from "./audio-manager";
import AudioMetadataProvider from "./audio-metadata-provider";

export default class CourseAudioGenerator {
    constructor(private readonly synthesizer: Synthesizer,
                private readonly audioManager: AudioManager,
                private readonly audioMetadataProvider: AudioMetadataProvider) {
    }

    public async generate(course: Course, dir: string): Promise<void> {
        const targetLangSpeed = this.audioMetadataProvider.getTargetVoiceSpeed();
        const translationSpeed = this.audioMetadataProvider.getTranslationVoiceSpeed();
        const targetVoice = this.audioMetadataProvider.getVoice(course.language);
        if (!targetVoice) {
            throw new Error('No voice for target language');
        }
        const translationVoice = this.audioMetadataProvider.getVoice(course.translationLanguage);
        if (!translationVoice) {
            throw new Error('No voice for translation');
        }
        for (const card of course.cards) {
            await this.ensureAudio(dir, card.text, course.language, targetVoice, targetLangSpeed);
            await this.ensureAudio(dir, card.translation, course.translationLanguage, translationVoice, translationSpeed);
            await this.ensureAudio(dir, card.usage, course.language, targetVoice, targetLangSpeed);
            if (course.canBeReversed) {
                await this.ensureAudio(dir, card.translation, course.translationLanguage, translationVoice, targetLangSpeed);
                await this.ensureAudio(dir, card.text, course.language, targetVoice, translationSpeed);
                await this.ensureAudio(dir, card.usageTranslation, course.translationLanguage, translationVoice, targetLangSpeed);
            }
        }
    }

    private async ensureAudio(dir: string, text: string, language: string, voiceId: string, speed: Speed): Promise<void> {
        if (!text || !text.trim()) {
            return;
        }
        const logText = `(${language}/${voiceId}/${speed}) ${text}`;
        if (await this.audioManager.fileExists(dir, text, language, voiceId, speed)) {
            console.log(`Audio already exists ${logText}`);
            return;
        }
        console.log(`Generating audio for ${logText}`);
        const audio = await this.synthesizer.synthesize(text, voiceId, speed);
        await this.audioManager.saveFile(dir, text, language, voiceId, speed, audio);
    }
}