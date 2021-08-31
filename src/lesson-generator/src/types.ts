export type GeneratorConfig = {
    mediaOutputDirectory: string;
    courseFilePath: string;
    reversedCourseFilePath?: string;
    courseLessonsDirectory: string;
    tmpDir: string;
    botToken: string;
    pauseFilePath: string;
    longPauseFilePath: string;
    chatId: number;
}

export type Course = {
    id: string;
    name: string;
    nameTranslation: string;
    cards: Card[];
    version: number;
    language: string;
    translationLanguage: string;
    description: string;
    descriptionTranslation: string;
}

export type LessonCard = {
    text: string;
    transcription: string;
    translation: string;
    usage: string;
    usageTranslation: string;
    isNew: boolean
}

export type Card = LessonCard & {
    translationTranscription: string;
}

export type CourseLessons = {
    courseId: string;
    courseVersion: number;
    lessons: Lesson[];
}

export type Lesson = {
    fileId?: string;
    cards: LessonCard[];
}

export type Speed = 'slow' | 'medium' | 'fast';