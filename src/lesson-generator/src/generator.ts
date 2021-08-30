import {GeneratorConfig} from "./types";
import LessonGenerator from "./lesson-generator";

export default class Generator {
    constructor(private readonly lessonGenerator: LessonGenerator) {
    }
    
    public async generate(config: GeneratorConfig): Promise<void> {
        await this.lessonGenerator.generate(config.courseFilePath, config.courseLessonsDirectory);
    }
    
}