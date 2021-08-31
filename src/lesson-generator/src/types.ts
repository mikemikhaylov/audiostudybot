export type GeneratorConfig = {
    mediaOutputDirectory: string;
    courseFilePath: string;
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
    canBeReversed: boolean;
    version: number;
    language: string;
    translationLanguage: string;
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

export type Speed = 'slow' | 'medium' | 'fast';