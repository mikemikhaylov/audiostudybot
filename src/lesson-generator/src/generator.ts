import {GeneratorConfig} from "./types";
import LessonGenerator from "./lesson-generator";
import FileProvider from "./file-provider";
import CourseAudioGenerator from "./course-audio-generator";
import LessonAudioGenerator from "./lesson-audio-generator";

export default class Generator {
    constructor(
        private readonly lessonGenerator: LessonGenerator,
        private readonly fileProvider: FileProvider,
        private readonly audioGenerator: CourseAudioGenerator,
        private readonly lessonAudioGenerator: LessonAudioGenerator) {
    }

    public async generate(config: GeneratorConfig): Promise<void> {
        console.log(`Generating for course file ${config.courseFilePath}`)
        const course = await this.getCourse(config.courseFilePath);
        await this.lessonGenerator.generate(course, config.courseLessonsDirectory);
        await this.audioGenerator.generate(course, config.mediaOutputDirectory);
        await this.lessonAudioGenerator.generate({
            course,
            lessonsDir: config.courseLessonsDirectory,
            mediaDir: config.mediaOutputDirectory,
            tmpDir: config.tmpDir,
            chatId: config.chatId,
            botToken: config.botToken,
            pauseFilePath: config.pauseFilePath
        });
    }

    private async getCourse(path: string) {
        return JSON.parse(await this.fileProvider.readFile(path));
    }
}