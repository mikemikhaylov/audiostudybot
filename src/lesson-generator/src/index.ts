import Generator from "./generator";
import FileProvider from "./file-provider";
import LessonGenerator from "./lesson-generator";
import CourseAudioGenerator from "./course-audio-generator";
import AudioManager from "./audio-manager";
import Synthesizer from "./synthesizer";
import AudioMetadataProvider from "./audio-metadata-provider";
import LessonAudioGenerator from "./lesson-audio-generator";
import CourseReverser from "./course-reverser";

const fileProvider = new FileProvider();
const lessonGenerator = new LessonGenerator(fileProvider);
const audioManager = new AudioManager(fileProvider);
const audioMetadataProvider = new AudioMetadataProvider();
const courseAudioGenerator = new CourseAudioGenerator(new Synthesizer(), audioManager, audioMetadataProvider);
const lessonAudioGenerator = new LessonAudioGenerator(fileProvider, audioManager, audioMetadataProvider);
const courseReverser = new CourseReverser(fileProvider);
(async () => {
    await new Generator(
        lessonGenerator,
        fileProvider,
        courseAudioGenerator,
        lessonAudioGenerator,
        courseReverser
    ).generate({
        courseFilePath: '../AudioStudy.Bot/AudioStudy.Bot.Courses/courses/en-ru/irregular-verbs.json',
        //reversedCourseFilePath: '../AudioStudy.Bot/AudioStudy.Bot.Courses/courses/en-ru/top3000-reversed.json',
        courseLessonsDirectory: '../AudioStudy.Bot/AudioStudy.Bot.Courses/lessons/en-ru/irregular-verbs/1',
        mediaOutputDirectory: '../../media_output',
        tmpDir: '../../media_output/tmp',
        botToken: '',
        pauseFilePath: 'assets/1300_pause.mp3',
        longPauseFilePath: 'assets/2500_pause.mp3',
        chatId: -504745169,
        botName: "Prod"
    });
})();

