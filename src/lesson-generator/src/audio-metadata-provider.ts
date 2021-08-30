import {Course, Speed} from "./types";

const voicesByLang = new Map<string, string>([['en', 'Matthew'], ['ru', 'Tatyana']]);

export default class AudioMetadataProvider {
    public getVoice(lang: string): string | undefined {
        return voicesByLang.get(lang);
    }

    public getTargetVoiceSpeed(): Speed {
        return 'slow';
    }

    public getTranslationVoiceSpeed(): Speed {
        return 'medium';
    }
}