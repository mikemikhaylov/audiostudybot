import Generator from "./generator";
import FileProvider from "./file-provider";
import LessonGenerator from "./lesson-generator";

const fileProvider = new FileProvider();
const lessonGenerator = new LessonGenerator(fileProvider);
(async () => {
    await new Generator(
        lessonGenerator
    ).generate({
        courseFilePath: '../AudioStudy.Bot/AudioStudy.Bot.Courses/courses/en-ru/top3000.json',
        courseLessonsDirectory: '../AudioStudy.Bot/AudioStudy.Bot.Courses/lessons/en-ru/top3000/1',
        mediaOutputDirectory: ''
    })
})();

