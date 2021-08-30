export type GeneratorConfig = {
    mediaOutputDirectory: string;
    courseFilePath: string;
    courseLessonsDirectory: string;
}

export type Course = {
    id: string;
    cards: Card[];
    canBeReversed: boolean;
    version: number;
}

export type LessonCard = {
    text: string;
    transcription: string;
    translation: string;
    usage: string;
    usageTranslation: string;
}

export type Card = LessonCard & {
    translationTranscription: string;
}

export type CourseLessons = {
    courseId: string;
    reversed: boolean;
    courseVersion: number;
    lessons: Lesson[];
}

export type Lesson = {
    fileId?: string;
    cards: LessonCard[];
}