import Generator from "./generator";
import FileProvider from "./file-provider";
import LessonGenerator from "./lesson-generator";
import CourseAudioGenerator from "./course-audio-generator";
import AudioManager from "./audio-manager";
import Synthesizer from "./synthesizer";
import AudioMetadataProvider from "./audio-metadata-provider";
import LessonAudioGenerator from "./lesson-audio-generator";

const fileProvider = new FileProvider();
const lessonGenerator = new LessonGenerator(fileProvider);
const audioManager = new AudioManager(fileProvider);
const audioMetadataProvider = new AudioMetadataProvider();
const courseAudioGenerator = new CourseAudioGenerator(new Synthesizer(), audioManager, audioMetadataProvider);
const lessonAudioGenerator = new LessonAudioGenerator(fileProvider, audioManager, audioMetadataProvider);
(async () => {
    await new Generator(
        lessonGenerator,
        fileProvider,
        courseAudioGenerator,
        lessonAudioGenerator
    ).generate({
        courseFilePath: '../AudioStudy.Bot/AudioStudy.Bot.Courses/courses/en-ru/top3000.json',
        courseLessonsDirectory: '../AudioStudy.Bot/AudioStudy.Bot.Courses/lessons/en-ru/top3000/1',
        mediaOutputDirectory: '../../media_output',
        tmpDir: '../../media_output/tmp',
        botToken: '1',
        pauseFilePath: 'assets/pause.mp3',
        chatId: 1
    })
})();

