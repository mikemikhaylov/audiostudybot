import FileProvider from "./file-provider";
import {Course, CourseLessons} from "./types";

const {resolve} = require('path');

const newCardsPerLesson = 5;
const reviewDays = [1, 2, 3, 5, 7, 10, 13];

export default class LessonGenerator {
    constructor(private readonly fileProvider: FileProvider) {
    }

    public async generate(course: Course, reversedCourse: Course | undefined, dir: string): Promise<void> {
        if (!await this.fileProvider.isEmpty(dir)) {
            console.log(`Dir ${dir} is not empty, skipping lessons generation`);
            return;
        }
        await this.createLessons(dir, course, 'lessons.json');
        if (reversedCourse) {
            await this.createLessons(dir, reversedCourse, 'lessons_reversed.json');
        }
    }

    private async createLessons(dir: string, course: Course, fileName: string): Promise<void> {
        const lessonCards = LessonGenerator.getLessonCards(course.cards.length);
        const courseLessons: CourseLessons = {
            courseId: course.id,
            courseVersion: course.version,
            lessons: lessonCards.map(x => {
                const cards = x.map(xx => course.cards[xx]);
                return {
                    fileId: undefined,
                    cards: cards.map((card, index) => {
                        return {
                            text: card.text,
                            transcription: card.transcription,
                            translation: card.translation,
                            usage: card.usage,
                            usageTranslation: card.usageTranslation,
                            isNew: index < newCardsPerLesson
                        };
                    })
                }
            })
        }
        console.log(`Generated ${courseLessons.lessons.length} for course ${course.id}`);
        await this.fileProvider.writeFile(resolve(dir, fileName), JSON.stringify(courseLessons, null, 2));
    }

    private static getLessonCards(numberOfCards: number): number[][] {
        const cardsByDay: number[][] = [];
        let day: number[] = [];
        for (let i = 0; i < numberOfCards; i++) {
            if (day.length < newCardsPerLesson) {
                day.push(i);
            } else {
                cardsByDay.push(day);
                day = [i];
            }
        }
        if (day.length) {
            cardsByDay.push(day);
        }
        const result: number[][] = [];
        for (let day = 0; day < cardsByDay.length; day++) {
            LessonGenerator.ensureData(result, day);
            result[day] = [...cardsByDay[day], ...result[day]];
            for (const reviewDay of reviewDays) {
                LessonGenerator.ensureData(result, day + reviewDay);
                result[day + reviewDay] = [...result[day + reviewDay], ...cardsByDay[day]];
            }
        }

        return result;
    }

    private static ensureData(days: number[][], day: number) {
        const targetLength = day + 1;
        while (days.length < targetLength) {
            days.push([]);
        }
    }
}