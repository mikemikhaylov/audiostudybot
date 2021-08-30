import FileProvider from "./file-provider";
import {Speed} from "./types";
const filenamify = require('filenamify');
const {resolve} = require('path');

import crypto from 'crypto';

export default class AudioManager {
    constructor(private readonly fileProvider: FileProvider) {
    }

    public async fileExists(dir: string, text: string, language: string, voiceId: string, speed: Speed): Promise<boolean> {
        const path = resolve(dir, language, voiceId, speed, AudioManager.getFileName(text));
        return await this.fileProvider.fileExists(path);
    }

    public async saveFile(dir: string, text: string, language: string, voiceId: string, speed: Speed, data: Buffer): Promise<void> {
        const fileDir = resolve(dir, language, voiceId, speed);
        await this.fileProvider.ensureDirExists(fileDir);
        const path = resolve(fileDir, AudioManager.getFileName(text));
        await this.fileProvider.writeBuffer(path, data);
    }

    public async getPath(dir: string, text: string, language: string, voiceId: string, speed: Speed): Promise<string> {
        return  resolve(dir, language, voiceId, speed, AudioManager.getFileName(text));
    }

    private static getFileName(text: string) {
        const hash = crypto
            .createHash("sha256")
            .update(text)
            .digest("hex");
        return `${filenamify(text, {replacement: '_', maxLength: 30})}${hash}.mp3`;
    }
}