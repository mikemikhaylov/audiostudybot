using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioStudy.Bot.Domain.Model;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram.CallbackData;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.SharedUtils.Localization;

namespace AudioStudy.Bot.Domain.Services.Telegram.Helpers
{
    public class CourseHelper : ICourseHelper
    {
        private readonly IBotLocalization _botLocalization;
        private readonly ILessonProvider _lessonProvider;
        private readonly ICourseProvider _courseProvider;
        private readonly IUserService _userService;
        private readonly ILessonCardsPagingHelper _lessonCardsPagingHelper;

        public CourseHelper(IBotLocalization botLocalization,
            ILessonProvider lessonProvider,
            ICourseProvider courseProvider,
            IUserService userService,
            ILessonCardsPagingHelper lessonCardsPagingHelper)
        {
            _botLocalization = botLocalization;
            _lessonProvider = lessonProvider;
            _courseProvider = courseProvider;
            _userService = userService;
            _lessonCardsPagingHelper = lessonCardsPagingHelper;
        }

        public TelegramResponseMessage GetCoursePage(User user, OpenCourseCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course == null)
            {
                return new TelegramResponseMessage
                {
                    Text = _botLocalization.CourseNotFound(user.Language),
                    InlineButtons = new[]
                    {
                        new[]
                        {
                            new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                                new OpenPageCallbackData(data.Page, data.PageSize).ToString())
                        }
                    }
                };
            }

            var courseName = _courseProvider.GetCourseName(user.Language, course);
            var courseDescription = _courseProvider.GetCourseDescription(user.Language, course);
            var numberOfCards = course.Cards?.Length ?? 0;
            var userCourse = user.Courses?.FirstOrDefault(x => x.Id == data.CourseId);
            var numberOfLessons = _lessonProvider.GetCourseLessons(course.Id, userCourse?.Version ?? course.Version)
                .Length;
            var isUserCourse = userCourse != null;
            var inlineButtons = new List<TelegramInlineBtn[]>();
            inlineButtons.Add(new[]
            {
                new TelegramInlineBtn(_botLocalization.ShowCards(user.Language),
                    new OpenCourseCardsPageCallbackData(data.CourseId, 0, Consts.CardsPerPage, data.Page, data.PageSize).ToString())
            });
            if (userCourse == null)
            {
                inlineButtons.Add(new[]
                {
                    new TelegramInlineBtn(_botLocalization.StartCourseLearning(user.Language),
                        new StartLearningCallbackData(data.CourseId, data.Page, data.PageSize).ToString())
                });
            }
            else
            {
                inlineButtons.Add(new[]
                {
                    new TelegramInlineBtn(_botLocalization.GetNextLesson(user.Language),
                        new GetNextLessonCallbackData(data.CourseId).ToString())
                });
                inlineButtons.Add(new[]
                {
                    new TelegramInlineBtn(_botLocalization.StopCourseLearning(user.Language),
                        new StopLearningCallbackData(data.CourseId, data.Page, data.PageSize).ToString())
                });
            }

            if (userCourse?.LastLesson >= 0)
            {
                inlineButtons.Add(new[]
                {
                    new TelegramInlineBtn(_botLocalization.StartOverCourseLearning(user.Language),
                        new StartOverFromCoursePageCallbackData(data.CourseId, data.Page, data.PageSize).ToString())
                });
            }

            inlineButtons.Add(new[]
            {
                new TelegramInlineBtn(_botLocalization.InlineBackBtn(user.Language),
                    new OpenPageCallbackData(data.Page, data.PageSize).ToString())
            });

            return new TelegramResponseMessage
            {
                Text = _botLocalization.Course(user.Language, courseName, courseDescription, numberOfCards,
                    numberOfLessons, isUserCourse, userCourse?.LastLesson),
                Html = true,
                InlineButtons = inlineButtons.ToArray()
            };
        }

        public async Task<TelegramResponseMessage> StartCourse(User user, StartLearningCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course != null)
            {
                await _userService.StartLearningCourse(user, course);
            }

            return GetCoursePage(user, new OpenCourseCallbackData(data.CourseId, data.Page, data.PageSize));
        }

        public async Task<TelegramResponseMessage> StopCourse(User user, StopLearningCallbackData data)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.AreYouSureStartOrStartOver(user.Language),
                InlineButtons = new[]
                {
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.Yes(user.Language),
                            new ConfirmStopLearningCallbackData(data.CourseId, data.Page, data.PageSize).ToString()),
                        new TelegramInlineBtn(_botLocalization.No(user.Language),
                            new OpenCourseCallbackData(data.CourseId, data.Page, data.PageSize).ToString())
                    }
                }
            };
        }

        public async Task<TelegramResponseMessage> StartOverFromPageCourse(User user,
            StartOverFromCoursePageCallbackData data)
        {
            return new TelegramResponseMessage
            {
                Text = _botLocalization.AreYouSureStartOrStartOver(user.Language),
                InlineButtons = new[]
                {
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.Yes(user.Language),
                            new ConfirmStartOverFromCoursePageCallbackData(data.CourseId, data.Page, data.PageSize)
                                .ToString()),
                        new TelegramInlineBtn(_botLocalization.No(user.Language),
                            new OpenCourseCallbackData(data.CourseId, data.Page, data.PageSize).ToString())
                    }
                }
            };
        }

        public async Task<TelegramResponseMessage> ConfirmStopCourse(User user, ConfirmStopLearningCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course != null)
            {
                await _userService.StopLearningCourse(user, course);
            }

            return GetCoursePage(user, new OpenCourseCallbackData(data.CourseId, data.Page, data.PageSize));
        }

        public async Task<TelegramResponseMessage> ConfirmStartOverCourse(User user,
            ConfirmStartOverFromCoursePageCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course != null)
            {
                await _userService.StartOverCourse(user, course);
            }

            return GetCoursePage(user, new OpenCourseCallbackData(data.CourseId, data.Page, data.PageSize));
        }

        public async Task<TelegramResponseMessage> GetNextLessonAsync(User user, GetNextLessonCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course == null)
            {
                return GetCoursePage(user, new OpenCourseCallbackData(data.CourseId, 0, Consts.CoursePerPage));
            }

            if (user.Courses?.Any(x => x.Id == data.CourseId) != true)
            {
                await _userService.StartLearningCourse(user, course);
            }

            var userCourse = user.Courses!.Single(x => x.Id == course.Id);
            if (_lessonProvider.TryGetNextLesson(course.Id, userCourse.Version, userCourse.LastLesson, out var lesson,
                out var lessonNumber))
            {
                await _userService.SetCurrentLesson(user, course, lessonNumber);
                return await _lessonCardsPagingHelper.GetPageAsync(user,
                    new OpenLessonCardsPageCallbackData(course.Id, userCourse.Version, lessonNumber, 0,
                        Consts.CardsPerPage));
            }

            return new TelegramResponseMessage
            {
                Text = _botLocalization.YouFinishedTheCourse(user.Language),
                InlineButtons = new[]
                {
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.StartOverCourseLearning(user.Language),
                            new StartOverCallbackData(course.Id).ToString()),
                        new TelegramInlineBtn(_botLocalization.ToTheCoursesList(user.Language),
                            new OpenPageCallbackData(0, Consts.CoursePerPage).ToString())
                    }
                }
            };
        }

        public async Task<TelegramResponseMessage> StartOver(User user, StartOverCallbackData data)
        {
            var course = _courseProvider.GetCourse(data.CourseId);
            if (course == null)
            {
                return GetCoursePage(user, new OpenCourseCallbackData(data.CourseId, 0, Consts.CoursePerPage));
            }

            await _userService.StartOverCourse(user, course);
            return new TelegramResponseMessage
            {
                Text = _botLocalization.CourseStartedOver(user.Language),
                InlineButtons = new[]
                {
                    new[]
                    {
                        new TelegramInlineBtn(_botLocalization.GetNextLesson(user.Language),
                            new GetNextLessonCallbackData(course.Id).ToString())
                    }
                }
            };
        }
    }
}