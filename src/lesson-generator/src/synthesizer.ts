import {VoiceId } from "aws-sdk/clients/polly";

const AWS = require('aws-sdk')

const polly = new AWS.Polly({
    signatureVersion: 'v4',
    region: 'us-east-1'
})

const neuralVoicesSet = new Set();
//neuralVoicesSet.add('Matthew');

type speed = 'slow' | 'medium' | 'fast';
export default class Synthesizer {
    public async synthesize(text: string, voiceId: VoiceId, speed?: speed): Promise<Buffer> {
        if (!speed) {
            speed = 'medium';
        }
        text = `<prosody rate="${speed}">${text}</prosody><break time="1200ms"/>`;
        if (neuralVoicesSet.has(voiceId)) {
            text = `<amazon:domain name="conversational">${text}</amazon:domain>`
        }
        text = `<speak>${text}</speak>`
        return this.synthesizeSsml(text, voiceId);
    }

    async synthesizeSsml(ssml: string, voiceId: VoiceId): Promise<Buffer> {
        const result = await polly.synthesizeSpeech({
            'Text': ssml,
            'Engine': neuralVoicesSet.has(voiceId) ? 'neural' : 'standard',
            'TextType': 'ssml',
            'OutputFormat': 'mp3',
            'VoiceId': voiceId,
            'SampleRate': '24000'
        }).promise();
        return Synthesizer.processResult(result);
    }
    
    private static processResult(result: any): Buffer {
        if (result.AudioStream instanceof Buffer) {
            return result.AudioStream;
        } else {
            throw 'AudioStream is not a Buffer.'
        }
    }
}