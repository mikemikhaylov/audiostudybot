import {Course} from "./types";
import FileProvider from "./file-provider";
import {v4 as uuidv4} from 'uuid';
import {resolve} from "path";

export default class CourseReverser {
    constructor(private readonly fileProvider: FileProvider) {
    }

    public async reverseCourse(course: Course, reversedCourseFile: string): Promise<Course> {
        if (await this.fileProvider.fileExists(reversedCourseFile)) {
            console.log('reversed course already exists');
            return JSON.parse(await this.fileProvider.readFile(reversedCourseFile));
        }
        const reversedCourse: Course = JSON.parse(JSON.stringify(course));
        reversedCourse.id = uuidv4();
        if (reversedCourse.nameTranslation) {
            const buffer = reversedCourse.name;
            reversedCourse.name = reversedCourse.nameTranslation;
            reversedCourse.nameTranslation = buffer;
        }
        if (reversedCourse.descriptionTranslation) {
            const buffer = reversedCourse.description;
            reversedCourse.description = reversedCourse.descriptionTranslation;
            reversedCourse.descriptionTranslation = buffer;
        }
        const lang = reversedCourse.language;
        reversedCourse.language = reversedCourse.translationLanguage;
        reversedCourse.translationLanguage = lang;
        for (const card of reversedCourse.cards) {
            let buffer = card.text;
            card.text = card.translation;
            card.translation = buffer;

            buffer = card.usage;
            card.usage = card.usageTranslation;
            card.usageTranslation = buffer;

            buffer = card.transcription;
            card.transcription = card.translationTranscription;
            card.translationTranscription = buffer;
        }
        await this.fileProvider.writeFile(reversedCourseFile, JSON.stringify(reversedCourse, null, 2));
        console.log(`Reversed course ${course.id} to ${reversedCourse.id}`);
        return reversedCourse;
    }
}