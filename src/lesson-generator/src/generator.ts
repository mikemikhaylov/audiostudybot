import {GeneratorConfig} from "./types";
import LessonGenerator from "./lesson-generator";
import FileProvider from "./file-provider";
import CourseAudioGenerator from "./course-audio-generator";
import LessonAudioGenerator from "./lesson-audio-generator";
import CourseReverser from "./course-reverser";

export default class Generator {
    constructor(
        private readonly lessonGenerator: LessonGenerator,
        private readonly fileProvider: FileProvider,
        private readonly courseAudioGenerator: CourseAudioGenerator,
        private readonly lessonAudioGenerator: LessonAudioGenerator,
        private readonly courseReverser: CourseReverser) {
    }

    public async generate(config: GeneratorConfig): Promise<void> {
        console.log(`Generating for course file ${config.courseFilePath}`)
        const course = await this.getCourse(config.courseFilePath);
        const reversedCourse = config.reversedCourseFilePath ? await this.courseReverser.reverseCourse(course, config.reversedCourseFilePath) : undefined;

        await this.lessonGenerator.generate(course, reversedCourse, config.courseLessonsDirectory);

        await this.courseAudioGenerator.generate(course, config.mediaOutputDirectory);
        if (reversedCourse) {
            await this.courseAudioGenerator.generate(reversedCourse, config.mediaOutputDirectory);
        }
        await this.lessonAudioGenerator.generate({
            courses: reversedCourse ? [course, reversedCourse] : [course],
            lessonsDir: config.courseLessonsDirectory,
            mediaDir: config.mediaOutputDirectory,
            tmpDir: config.tmpDir,
            chatId: config.chatId,
            botToken: config.botToken,
            pauseFilePath: config.pauseFilePath,
            longPauseFilePath: config.longPauseFilePath,
            botName: config.botName
        });
    }

    private async getCourse(path: string) {
        return JSON.parse(await this.fileProvider.readFile(path));
    }
}
